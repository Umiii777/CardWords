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
    /// TODO: 描述LoadUI的args参数
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

public class SystemUIManager : MonoBehaviour
{
#region 弹出提示内容常量
    private const string TIPS_ASKING_ENERGY = "体力不足，请补充体力或等待回复";
    private const string TIPS_COIN_LACK = "金币不足";
    private const string TIPS_SUCCESSFUL_REDEEM = "兑换成功！";
    private const string TIPS_SUCCESSFUL_RECEIVING = "领取成功！";
#endregion

    public static SystemUIManager Instance;
    /// <summary>
    /// 加载关卡
    /// </summary>
    public static Func<Task> loadingLevel;

#region public 游戏内所有道具的信息和图标
    public string[] itemInfos;
    public Sprite[] itemIcons;
    private Dictionary<ItemType, KeyValuePair<string, Sprite>> dictItemInfoIcons;
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
    private Dictionary<UIType, KeyValuePair<MonoBehaviour, Action>> dictPrefabInitings;

#region 静态方法
    /// <summary>
    /// 生成指定类型的界面
    /// <br/><br/>
    /// SystemUIManager 会自动初始化和管理生成的界面
    /// </summary>
    /// <param name="type">要生成的界面类型</param>
    /// <param name="args">生成此界面时需要的数据。确定好第一个实参后，参考第一个实参的注释来传入</param>
    /// <returns></returns>
    public static async Task LoadUI(UIType type, params object[] args)
    {
        if (Instance == null)
            await Task.FromException(new InvalidOperationException("SystemUIManager.Instance 还未初始化，无法调用 LoadUI"));
        switch (type)
        {
            case UIType.Defeat:
                if (args.Length > 0 && float.TryParse(args[0].ToString(), out float progress))
                    Instance.uiInstances[(uint)UIType.Defeat] = Instance.CreateUI(
                        Instance.uiInstances[(uint)UIType.Defeat] as DefeatUIController,
                        Instance.defeatUIPrefab,
                        () => Instance.InitDefeatUI(progress)
                    );
                break;
            case UIType.Item:
                if (args.Length > 0 && Enum.IsDefined(typeof(ItemType), args[0]))
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
                break;
            default:
                Instance.uiInstances[(uint)UIType.Item] = Instance.CreateUI(
                    Instance.uiInstances[(uint)UIType.Item] as MonoBehaviour,
                    Instance.dictPrefabInitings[UIType.Item].Key,
                    Instance.dictPrefabInitings[UIType.Item].Value
                );
                break;
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

    /// <summary>
    /// 开始处理广告，同时禁用看广告领东西按钮的响应，直到处理完广告
    /// </summary>
    /// <param name="uiInstance">包含看广告领东西按钮的界面实例</param>
    /// <returns>须代入按钮回调的新委托</returns>
    private static Func<Task> StartAdAndBlockClicking(NonSingletonAdProcessor uiInstance)
    {
        Func<Task> clicking = uiInstance.clickingWatchAd;
        async Task startAdAndBlockClicking()
        {
            uiInstance.clicking = null;
            await clicking();
            uiInstance.clicking = clicking + startAdAndBlockClicking;
        }
        ;
        return startAdAndBlockClicking;
    }

    //TODO: 写方法注释
    //TODO: ShopUIController.clickingReceive 和 VictoryUIController.clickingReceiveMore 都改成 Func<string, Task>
    private static Func<T, Task> StartAdAndBlockClicking<T>(Type typeOfUIController, Func<T, Task> clickingFunc, string nameOfClickingFunc)
    {
      FieldInfo clickingField = typeOfUIController.GetField(nameOfClickingFunc, BindingFlags.Public | BindingFlags.Static);
      Func<T, Task> clicking = clickingField?.GetValue(null) as Func<T, Task>;
			if (clicking is null)
				throw new ArgumentException($"类型 {typeOfUIController.Name} 中没有名为 \"{nameOfClickingFunc}\" 且类型为 Func<{typeof(T).Name}, Task> 的静态公开字段");

			async Task startAdAndBlockClicking(T arg)
			{
				clickingField.SetValue(null, null);
				await (clicking.Method.Invoke(clicking.Target, new object[] { arg }) as Task);
				clickingField.SetValue(null, clicking + (async arg => await startAdAndBlockClicking(arg)));
			}
			return startAdAndBlockClicking;
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
#endregion

    void Awake()
    {
        InitUIManager();
        uiInstances[(uint)UIType.Home] = CreateUI(null, homeUIPrefab, InitHomeUI);
        //_ = LoadUI(UIType.Defeat, 0.6f);
        _= LoadUI(UIType.Item, ItemType.Hint);

#if UNITY_EDITOR
        Debug.Log("Coin:" + PlayerCoin.GetCoin() + " Energy:" + PlayerEnergy.GetEnergy());
        Debug.Log("Hint:" + PlayerItem.GetItem(ItemType.Hint) + " Shuffle:" + PlayerItem.GetItem(ItemType.Shuffle));
#endif
    }

    private async void InitUIManager()
    {
        Instance = this;
        uiInstances = new object[Enum.GetNames(typeof(UIType)).Length];
        uiInstances[(uint)UIType.Item] = new LinkedList<ItemUIController>();
        dictItemInfoIcons = itemInfos.ToDictionary(
            s => (ItemType)uint.Parse(s.Split('`', 2)[0]),
            s => new KeyValuePair<string, Sprite>
            (
                s.Split('`', 2)[1].Replace('，', ','),
                itemIcons.First(i => i.name.Split('_', 2)[0] == s.Split('`', 2)[0])
            )
        );
        dictPrefabInitings = new()
        {
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

    private void OnSpendCoin()
    {
        //TODO: 更新各界面金币相关显示
    }

    private void OnSpendEnergy()
    {
        //TODO: 更新各界面体力值显示
    }

    /// <summary>
    /// 失败界面初始化
    /// </summary>
    /// <param name="progress">玩家达成的游戏进度（0到1之间）</param>
    private void InitDefeatUI(float progress)
    {
        void destroy() => Destroy((uiInstances[(uint)UIType.Defeat] as MonoBehaviour).gameObject);
        DefeatUIController.progress = progress;
        DefeatUIController.clickingHome += () =>
        {
            uiInstances[(uint)UIType.Home] = CreateUI(
                uiInstances[(uint)UIType.Home] as MonoBehaviour,
                homeUIPrefab,
                InitHomeUI
            );
            destroy();
        };
        DefeatUIController.clickingContinue += async () =>
        {
            await Task.Delay(3000); //假装播放3秒广告
            if (loadingLevel is not null)
                await loadingLevel.Invoke(); //TODO: 这一行加载关卡
            destroy();
        };
        DefeatUIController.clickingReplay += async () =>
        {
            if (PlayerEnergy.TrySpendEnergy(1))
            {
                OnSpendEnergy();
                if (loadingLevel is not null)
                    await loadingLevel.Invoke(); //TODO: 这一行加载关卡
                destroy();
                return;
            }
            //TODO: 显示体力补充界面
            _= PopUpTips(TIPS_ASKING_ENERGY);
        };
    }

    /// <summary>
    /// 大厅界面初始化
    /// </summary>
    private void InitHomeUI()
    {
        //TODO: 把各个UI类与Player数据类之间的耦合转移到 SystemUIManager 里（包括大厅界面）
    }

    /// <summary>
    /// 道具界面初始化
    /// </summary>
    /// <param name="id">道具ID（与 ItemType 枚举一致）</param>
    /// <param name="ui">要初始化的道具界面实例</param>
    private void InitItemUI(ItemUIController itemUI, ItemType itemType)
    {
        KeyValuePair<string, Sprite> itemInfoIcon = dictItemInfoIcons[itemType];
        string[] infos = itemInfoIcon.Key.Split('`');

        itemUI.itemType = itemType;
        itemUI.nameText.text = infos[0];
        itemUI.price = int.Parse(infos[1]);
        itemUI.descriptionText.text = infos[2];
        itemUI.iconImage.sprite = itemInfoIcon.Value;
        
        itemUI.clickingClose += () =>
        {
            (uiInstances[(uint)UIType.Item] as LinkedList<ItemUIController>).Remove(itemUI);
            Destroy(itemUI.gameObject);
        };
        itemUI.clickingBuy += price =>
        {
            if (PlayerCoin.TrySpendCoin(price))
            {
                PlayerItem.AddItem(itemType, 1);
                _= PopUpTips(TIPS_SUCCESSFUL_REDEEM);
                return;
            }
            _= PopUpTips(TIPS_COIN_LACK);
        };
        itemUI.clickingWatchAd += async () =>
        {
            await Task.Delay(3000); //假装播放3秒广告
            PlayerItem.AddItem(itemType, 1);
            _= PopUpTips(TIPS_SUCCESSFUL_RECEIVING);
        };
        itemUI.clickingWatchAd = StartAdAndBlockClicking(itemUI);
    }

    private void InitShopUI()
    {
        //TODO: 通过 StartAdAndBlockClicking 禁用看广告领金币按钮响应
    }

    private void InitVictoryUI(int numRewardCoins)
    {
        //TODO: 通过 StartAdAndBlockClicking 禁用看广告领金币按钮响应
    }
}
