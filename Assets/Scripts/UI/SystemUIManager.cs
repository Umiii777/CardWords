using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏中的界面类型
/// </summary>
public enum UIType
{
    /// <summary>
    /// 关卡失败界面
    /// <br/><br/>
    /// 生成时须再传一个表示玩家游戏完成度的 float 值，最小为0，最大为1
    /// </summary>
    Defeat,
    /// <summary>
    /// 体力补充界面
    /// <br/><br/>
    /// 生成时不传其他参数
    /// </summary>
    Energy,
    /// <summary>
    /// 大厅界面
    /// <br/><br/>
    /// 生成时不传其他参数
    /// </summary>
    Home,
    /// <summary>
    /// 道具获取界面
    /// <br/><br/>
    /// 生成时须再传一个表示道具类型的 ItemType 枚举
    /// </summary>
    Item,
    /// <summary>
    /// 退出游戏确认界面
    /// <br/><br/>
    /// 生成时可再传一个表示退出游戏扣几点体力的 int 值，若不传则使用 QuitUIController.numEnergy 的值
    /// </summary>
    Quit,
    /// <summary>
    /// 商店界面
    /// <br/><br/>
    /// 生成时不传其他参数
    /// </summary>
    Shop,
    /// <summary>
    /// 关卡胜利界面
    /// <br/><br/>
    /// 生成时可再传一个表示奖励金币数量的 int 值，若不传则使用 VictoryUIController.numCoinsToReceive 的值
    /// </summary>
    Victory
}

/// <summary>
/// 加入新界面时须向该类添加 InitXXX 方法，并补充 InitUICallbacks 和 LoadUI 方法的switch分支
/// </summary>
public class SystemUIManager : MonoBehaviour
{
#region 异常消息内容常量
    private const string EXCEPITON_ILLEGAL_ENUM = "参数 type 的值为 @，不在枚举 # 之中";
    private const string EXCEPITON_STATIC_FIELD_NOT_FOUND = "类型 @ 中没有名为 \"#\" 且类型为 $ 的静态公开字段。实际参数可能不符合方法要求！";
    private const string EXCEPTION_MANAGER_UNINITIALIZED = "SystemUIManager.Instance 还未初始化，无法调用 @";
#endregion
#region 弹出提示内容常量
    private const string TIPS_ASKING_ENERGY = "体力不足，请补充体力或等待回复";
    private const string TIPS_COIN_LACK = "金币不足";
    private const string TIPS_SUCCESSFUL_REDEEM = "兑换成功！";
    private const string TIPS_SUCCESSFUL_RECEIVING = "领取成功！";
    private const string TIPS_ENERGY_ADDED = "体力 + @";
    private const string TIPS_ENERGY_IS_FULL = "兑换失败，体力已满";
#endregion

    public static SystemUIManager Instance;
    /// <summary>
    /// 关卡加载委托
    /// </summary>
    public static Func<string, bool, Task> loadingLevel;

#region public 游戏内所有道具的信息和图标
    public string[] itemInfos;
    public Sprite[] itemIcons;
    private Dictionary<ItemType, ValueTuple<string, Sprite>> dictItemInfoIcons;
#endregion

    [SerializeField]
    private RectTransform tipsPrefab;

#region 各界面预制体
    [SerializeField]
    private DefeatUIController defeatUIPrefab;
    [SerializeField]
    private EnergyUIController energyUIPrefab;
    [SerializeField]
    private HomeUIController homeUIPrefab;
    [SerializeField]
    private ItemUIController itemUIPrefab;
    [SerializeField]
    private QuitUIController quitUIPrefab;
    [SerializeField]
    private ShopUIController shopUIPrefab;
    [SerializeField]
    private VictoryUIController victoryUIPrefab;
#endregion

    private object[] uiInstances;
    private Dictionary<UIType, ValueTuple<MonoBehaviour, Action>> dictPrefabInitings;

#region 静态方法
    public static void InitUICallbacks(UIType type)
    {
        static GameObject uiGameObj(UIType type)
        {
            var uiObj = Instance.uiInstances[(uint)type] as MonoBehaviour;
            return uiObj == null ? null : uiObj.gameObject;
        }
        static async Task clickingContinue(UIType type, bool hasAd = true, string popUpMessage = null)
        {
            if (uiGameObj(type) == null)
                return;
            await (hasAd
                ? Task.Delay(3000) //假装播放3秒广告
                : Task.CompletedTask
            );
            await Task.WhenAll(new Task[]
            {
                //TODO: 下面这行加载关卡（loadingLevel(PlayerLevel.GetNextLevel(), bool)第一个参数表示玩家当前所在关卡，第二个参数表示重玩本关还是进下一关）
                loadingLevel is null ? Task.CompletedTask : loadingLevel("", true),
                popUpMessage is null ? Task.CompletedTask : PopUpTips(popUpMessage)
            });
            Destroy(uiGameObj(type));
            Instance.uiInstances[(uint)type] = null;
        }
        switch (type)
        {
            case UIType.Defeat:
                DefeatUIController.clickingHome = () =>
                {
                    _= LoadUI(UIType.Home);
                    Destroy(uiGameObj(UIType.Defeat));
                    Instance.uiInstances[(uint)UIType.Defeat] = null;
                };
                DefeatUIController.clickingContinue = async () => await clickingContinue(UIType.Defeat);
                DefeatUIController.clickingReplay = async () =>
                {
                    if (uiGameObj(UIType.Defeat) == null)
                        return;
                    if (PlayerEnergy.TrySpendEnergy(1))
                    {
                        Instance.OnUpdateEnergy();
                        //TODO: 下面这行加载关卡（loadingLevel(PlayerLevel.GetNextLevel(), bool)第一个参数表示玩家当前所在关卡，第二个参数表示重玩本关还是进下一关）
                        await (loadingLevel is null ? Task.CompletedTask : loadingLevel("", false));
                        Destroy(uiGameObj(UIType.Defeat));
                        Instance.uiInstances[(uint)UIType.Defeat] = null;
                        return;
                    }
                    await LoadUI(UIType.Energy);
                    _= PopUpTips(TIPS_ASKING_ENERGY);
                };
                return;
            case UIType.Energy:
                static void addEnergy()
                {
                    Instance.OnUpdateEnergy();
                    _= PopUpTips(TIPS_SUCCESSFUL_REDEEM + TIPS_ENERGY_ADDED.Replace("@", $"{1}"));
                }
                EnergyUIController.clickingClose = () => { Destroy(uiGameObj(UIType.Energy)); Instance.uiInstances[(uint)UIType.Energy] = null; };
                EnergyUIController.clickingBuy = price =>
                {
                    if (PlayerEnergy.GetEnergy() >= PlayerEnergy.MaxEnergy)
                    {
                        _= PopUpTips(TIPS_ENERGY_IS_FULL);
                        return;
                    }
                    if (PlayerCoin.TrySpendCoin(price))
                    {
                        PlayerEnergy.AddEnergy(1);
                        Instance.OnUpdateCoin();
                        addEnergy();
                        return;
                    }
                    _= PopUpTips(TIPS_COIN_LACK);
                };
                EnergyUIController.clickingWatchAd = async _=>
                {
                    await Task.Delay(3000); //假装播放3秒广告
                    if (Instance.uiInstances[(uint)UIType.Energy] == null)
                        return;
                    if (PlayerEnergy.TryAddEnergy(1))
                    {
                        addEnergy();
                        return;
                    }
                    _= PopUpTips(TIPS_ENERGY_IS_FULL);
                };
                EnergyUIController.clickingWatchAd = StartAdAndBlockClicking(typeof(EnergyUIController), EnergyUIController.clickingWatchAd);
                return;
            case UIType.Home: //TODO: 接着写大厅，然后写 OnUpdateCoin 和 OnUpdateEnergy
            case UIType.Item: return;
            case UIType.Quit: //TODO: 问海一退出确认从哪里触发，能不能不做
            case UIType.Shop: //TODO: 明天先写商店
            case UIType.Victory:
                VictoryUIController.clickingHome = () =>
                {
                    _= LoadUI(UIType.Home);
                    Destroy(uiGameObj(UIType.Victory));
                    Instance.uiInstances[(uint)UIType.Victory] = null;
                };
                VictoryUIController.clickingReceive = async () =>
                {
                    await clickingContinue(UIType.Victory, false, TIPS_SUCCESSFUL_RECEIVING);
                    PlayerCoin.AddCoin(VictoryUIController.numCoinsToReceive);
                    Instance.OnUpdateCoin();
                };
                VictoryUIController.clickingWatchAd = async _=> {
                    await clickingContinue(UIType.Victory, true, TIPS_SUCCESSFUL_RECEIVING);
                    PlayerCoin.AddCoin(VictoryUIController.numCoinsToReceive);
                    Instance.OnUpdateCoin();
                };
                VictoryUIController.clickingWatchAd = StartAdAndBlockClicking(typeof(VictoryUIController), VictoryUIController.clickingWatchAd);
                return;
            default:
                throw new ArgumentException(EXCEPITON_ILLEGAL_ENUM
                    .Replace("@", type.ToString())
                    .Replace("#", nameof(UIType))
                );
        }
    }

    /// <summary>
    /// 生成指定类型的界面
    /// <br/><br/>
    /// SystemUIManager 会自动初始化和管理生成的界面
    /// </summary>
    /// <param name="type">要生成的界面类型</param>
    /// <param name="args">生成此界面时需要的数据。确定好第一个实参后，参考第一个实参的注释来传入</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task LoadUI(UIType type, params object[] args)
    {
        if (Instance == null)
            await Task.FromException(new InvalidOperationException(EXCEPTION_MANAGER_UNINITIALIZED.Replace("@", nameof(LoadUI))));
        if (args.Length > 0)
            switch (type)
            {
                case UIType.Defeat:
                    if (float.TryParse(args[0].ToString(), out float progress))
                        Instance.uiInstances[(uint)UIType.Defeat] = Instance.CreateUI(
                            Instance.uiInstances[(uint)UIType.Defeat] as DefeatUIController,
                            Instance.defeatUIPrefab,
                            () => Instance.InitDefeatUI(progress)
                        );
                    return;
                case UIType.Item:
                    if (Enum.IsDefined(typeof(ItemType), args[0]))
                    {
                        var itemUIs = Instance.uiInstances[(uint)UIType.Item] as LinkedList<ItemUIController>;
                        ItemUIController itemUI = Instance.CreateUI(
                            itemUIs.Count > 0 ? itemUIs.First() : null,
                            Instance.itemUIPrefab
                        );
                        Instance.InitItemUI(itemUI, (ItemType)args[0]);
                        itemUIs.RemoveFirst();
                        itemUIs.AddLast(itemUI);
                    }
                    return;
            }
        switch (type)
        {
            case UIType.Victory:
                int numCoinsToReceive = args.Length == 0
                    ? VictoryUIController.numCoinsToReceive
                    : int.TryParse(args[0].ToString(), out int numCoins)
                        ? numCoins
                        : 0;
                Instance.uiInstances[(uint)UIType.Victory] = Instance.CreateUI(
                    Instance.uiInstances[(uint)UIType.Victory] as VictoryUIController,
                    Instance.victoryUIPrefab,
                    () => Instance.InitVictoryUI(numCoinsToReceive)
                );
                return;
            default:
                if (Enum.IsDefined(typeof(UIType), type))
                {
                    Instance.uiInstances[(uint)type] = Instance.CreateUI(
                        Instance.uiInstances[(uint)type] as MonoBehaviour,
                        Instance.dictPrefabInitings[type].Item1,
                        Instance.dictPrefabInitings[type].Item2
                    );
                    return;
                }
                throw new ArgumentException(EXCEPITON_ILLEGAL_ENUM
                    .Replace("@", $"{(uint)type}")
                    .Replace("#", nameof(UIType))
                );
        }
    }

    public static async Task PopUpTips(string tipsMessage, Transform parentTransform = null)
    {
        const int movingDuration = 80, stayingDuration = 1000, fadingDelay = 25;
        Transform parent = parentTransform == null
            ? Instance.transform.GetChild(Instance.transform.childCount - 1)
            : parentTransform;
        RectTransform rt = Instantiate(Instance.tipsPrefab, parent);
        CanvasGroup tipsBackground = rt.GetComponent<CanvasGroup>();
        int targetPosY = (int)rt.localPosition.y + 250;

        rt.GetChild(1).GetComponent<Text>().text = tipsMessage;
        if (parentTransform == null)
            while (rt.localPosition.y < targetPosY)
            {
                rt.localPosition += Vector3.up * 25;
                await Task.Delay(movingDuration / 10);
            }
        else
            await Task.Delay(movingDuration);
        await Task.Delay(stayingDuration);
        while (tipsBackground.alpha > 0f)
        {
            tipsBackground.alpha -= 0.1f;
            await Task.Delay(fadingDelay);
        }
        Destroy(rt.gameObject);
    }

    public static async void ProcessAd<T>(T toWait, Action afterWait = null, params object[] toWaitArgs) where T : Delegate
    {
        if (toWait is not null)
            await Task.WhenAll(toWait.GetInvocationList()
                .Select(d =>
                    d.Method.ReturnType == typeof(Task)
                    ? d.Method.Invoke(d.Target, toWaitArgs) as Task
                    : Task.FromException(new ArgumentException("SystemUIManager.ProcessAd的第一个参数只能是 Func<..., Task> 类型！"))
                )
            );
        afterWait?.Invoke();
    }

    /// <summary>
    /// 开始处理广告，同时禁用看广告领东西按钮的响应，直到处理完广告
    /// <br/><br/>
    /// 用于派生自 NonSingletonAdProcessor 的界面类型
    /// </summary>
    /// <param name="uiInstance">包含看广告领东西按钮的界面实例</param>
    /// <returns>须代入按钮回调的新委托</returns>
    private static Func<Task> StartAdAndBlockClicking(AdProcessor uiInstance)
    {
        Func<Task> clicking = uiInstance.clickingWatchAd;
        async Task startAdAndBlockClicking()
        {
            uiInstance.clickingWatchAd = null;
            await Task.WhenAll(clicking.GetInvocationList()
                .Cast<Func<Task>>()
                .Select(async f => await f())
            );
            uiInstance.clickingWatchAd = clicking + startAdAndBlockClicking;
        }
        ;
        return startAdAndBlockClicking;
    }

    /// <summary>
    /// 开始处理广告，同时禁用看广告领东西按钮的响应，直到处理完广告
    /// <br/><br/>
    /// 用于派生自 SingletonAdProcessor 的界面类型
    /// </summary>
    /// <typeparam name="T">看广告领东西按钮回调委托的返回值类型</typeparam>
    /// <param name="typeOfUIController">要禁用按钮响应的界面类型</param>
    /// <param name="clickingFunc">看广告领东西按钮的回调委托</param>
    /// <param name="nameOfClickingFunc">看广告领东西按钮回调委托的字段名称</param>
    /// <returns>须代入按钮回调的新委托</returns>
    /// <exception cref="ArgumentException"></exception>
    private static Func<T, Task> StartAdAndBlockClicking<T>(
        Type typeOfUIController,
        Func<T, Task> clickingFunc,
        string nameOfClickingFunc = nameof(StaticAdProcessor<MonoBehaviour, T>.clickingWatchAd)
    )
    {
        FieldInfo clickingField = typeOfUIController.GetField(
            nameOfClickingFunc,
            BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public
        );
        if (clickingField?.GetValue(null) is not Func<T, Task> clicking)
            throw new ArgumentException(EXCEPITON_STATIC_FIELD_NOT_FOUND
                .Replace("@", typeOfUIController.ToString())
                .Replace("#", nameOfClickingFunc)
                .Replace("$", $"Func<{typeof(T)}, Task>")
            );

        async Task startAdAndBlockClicking(T arg)
        {
            clickingField.SetValue(null, null);
            await Task.WhenAll(clicking.GetInvocationList()
                .Cast<Func<T, Task>>()
                .Select(async f => await f(arg))
            );
            clickingField.SetValue(null, clicking + (async arg => await startAdAndBlockClicking(arg)));
        }
        return startAdAndBlockClicking;
    }
#endregion

    void Awake()
    {
        InitUIManager();
        for (uint u = 0; u < Enum.GetNames(typeof(UIType)).Length; u++)
            InitUICallbacks((UIType)u);
        uiInstances[(uint)UIType.Home] = CreateUI(null, homeUIPrefab, InitHomeUI);

#if UNITY_EDITOR
        //PlayerPrefs.DeleteAll();
        //PlayerEnergy.SetEnergy(0);
        _= LoadUI(UIType.Energy);
        //_= LoadUI(UIType.Victory,100);
        //_= LoadUI(UIType.Defeat, 0.6f);
        //_= LoadUI(UIType.Item, ItemType.Hint);
        Debug.Log("Coin:" + PlayerCoin.GetCoin() + " Energy:" + PlayerEnergy.GetEnergy());
        Debug.Log("Hint:" + PlayerItem.GetItem(ItemType.Hint) + " Shuffle:" + PlayerItem.GetItem(ItemType.Shuffle));
#endif
    }

#region 各界面初始化方法
    private void InitDefeatUI(float progress) => DefeatUIController.progress = progress;
    private void InitEnergy() => _= 0; //TODO: 把各个UI类与Player数据类之间的耦合转移到 SystemUIManager 里（包括体力补充界面）
    private void InitHomeUI() => _= 0; //TODO: 把各个UI类与Player数据类之间的耦合转移到 SystemUIManager 里（包括大厅界面）
    private void InitItemUI(ItemUIController itemUI, ItemType itemType)
    {
        ValueTuple<string, Sprite> itemInfoIcon = dictItemInfoIcons[itemType];
        string[] infos = itemInfoIcon.Item1.Split('`');

        itemUI.itemType = itemType;
        itemUI.nameText.text = infos[0];
        itemUI.price = int.Parse(infos[1]);
        itemUI.descriptionText.text = infos[2];
        itemUI.iconImage.sprite = itemInfoIcon.Item2;

        ValueTuple<Action, Action<int>, Func<Task>> clickings;
        if (!ItemUIController.dictCachedClickings.TryGetValue(itemType, out clickings))
        {
            clickings = (
            () =>
            {
                (uiInstances[(uint)UIType.Item] as LinkedList<ItemUIController>).Remove(itemUI);
                Destroy(itemUI.gameObject);
            },
            price =>
            {
                if (PlayerCoin.TrySpendCoin(price))
                {
                    PlayerItem.AddItem(itemType, 1);
                    _= PopUpTips(TIPS_SUCCESSFUL_REDEEM);
                    return;
                }
                _= PopUpTips(TIPS_COIN_LACK);
            },
            async () =>
            {
                await Task.Delay(3000); //假装播放3秒广告
                PlayerItem.AddItem(itemType, 1);
                _= PopUpTips(TIPS_SUCCESSFUL_RECEIVING);
            }
            );
            ItemUIController.dictCachedClickings.Add(itemType, clickings);
        }
        (itemUI.clickingClose, itemUI.clickingBuy, itemUI.clickingWatchAd) = clickings;
        itemUI.clickingWatchAd = StartAdAndBlockClicking(itemUI);
    }
    private void InitShopUI() => _= 0;  //TODO: 通过 StartAdAndBlockClicking 禁用看广告领金币按钮响应
    private void InitVictoryUI(int numRewardCoins) => VictoryUIController.numCoinsToReceive = numRewardCoins;
#endregion

    private async void InitUIManager()
    {
        Instance = this;
        uiInstances = new object[Enum.GetNames(typeof(UIType)).Length];
        uiInstances[(uint)UIType.Item] = new();
        dictItemInfoIcons = itemInfos.ToDictionary(
            s => (ItemType)uint.Parse(s.Split('`', 2)[0]),
            s => (
                s.Split('`', 2)[1].Replace('，', ','),
                itemIcons.First(i => i.name.Split('_', 2)[0] == s.Split('`', 2)[0])
            )
        );
        dictPrefabInitings = new()
        {
            { UIType.Energy, new(energyUIPrefab, InitEnergy) },
            { UIType.Home, new(homeUIPrefab, InitHomeUI) }
        };
    }

    private T CreateUI<T>(T ui, T uiPrefab, Action initing = null) where T : MonoBehaviour
    {
        if (ui != null)
            Destroy(ui.gameObject);
        T uiNew = Instantiate(uiPrefab, transform);
        uiNew.name = uiNew.name[..^7]; // 去掉物体名字后面的"(Clone)"
        initing?.Invoke();
        return uiNew;
    }

    private void OnUpdateCoin()
    {
        //TODO: 更新各界面金币相关显示
    }

    private void OnUpdateEnergy()
    {
        //TODO: 更新各界面体力值显示
    }
}
