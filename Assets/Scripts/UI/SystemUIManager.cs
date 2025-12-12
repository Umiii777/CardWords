using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static AudioManager;

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
    /// 设置界面
    /// <br/><br/>
    /// 生成时须再传一个表示玩家当前是否处于关卡内的 bool 值
    /// </summary>
    Settings,
    /// <summary>
    /// 商店界面
    /// <br/><br/>
    /// 生成时不传其他参数
    /// </summary>
    Shop,
    /// <summary>
    /// 开启槽位界面
    /// <br/><br/>
    /// 生成时须再传一个要开启的槽位所在的 Row 对象
    /// </summary>
    UnlockSlot,
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
[RequireComponent(typeof(Canvas))]
public class SystemUIManager : MonoBehaviour
{
    private const int MAX_LEVEL = 110; //TODO: 该常量应在 PlayerProgress 类中定义

#region 异常消息内容常量
    private const string EXCEPITON_ILLEGAL_LOADUI_ARG = "未传入生成界面所必需的参数，将鼠标指针放在 @.# 上以查看参数说明";
    private const string EXCEPITON_ILLEGAL_ENUM_ARG = "参数 @ 的值为 #，不在枚举 $ 之中";
    private const string EXCEPITON_STATIC_FIELD_NOT_FOUND = "类型 @ 中没有名为 \"#\" 且类型为 $ 的静态公开字段。可能传入了错误的参数";
    private const string EXCEPTION_MANAGER_UNINITIALIZED = "SystemUIManager.Instance 还未初始化，无法调用 @";
#endregion
#region 弹出提示内容常量
    private const string TIPS_ASKING_ENERGY = "体力不足，请补充体力或等待回复";
    private const string TIPS_COIN_LACK = "金币不足";
    private const string TIPS_SUCCESSFUL_REDEEM = "兑换成功！";
    private const string TIPS_SUCCESSFUL_RECEIVING = "领取成功！";
    private const string TIPS_ENERGY_ADDED = "体力 + @";
    private const string TIPS_ENERGY_IS_FULL = "兑换失败，体力已满";
    private const string TIPS_SUCCESSFL_UNLOCKING = "槽位已开启！";
    private const string TIPS_WAIT_FOR_MORE_LEVELS = "更多关卡，敬请期待！";
    private const string TIPS_ASKING_ITEM = "道具数量不足，请兑换";
#endregion

    public static SystemUIManager Instance;

#region 静态委托
    /// <summary>
    /// 进入大厅委托
    /// </summary>
    public static Action initingHome;
    /// <summary>
    /// 关卡加载委托
    /// <br/><br/>
    /// 第一个参数为要加载的关卡编号，第二个参数为额外信息
    /// </summary>
    public static Func<int, object, Task> loadingLevel;
    /// <summary>
    /// 增加步数委托
    /// </summary>
    public static Func<Task> addingSteps;
    /// <summary>
    /// 开启槽位委托
    /// </summary>
    public static Func<Row, Task> unlockingSlot;
#endregion

#region 游戏内所有道具的信息和图标
    public string[] itemInfos;
    public Sprite[] itemIcons;
    private Dictionary<ItemType, ValueTuple<string, Sprite>> dictItemInfoIcons;
#endregion

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
    private SettingsUIController settingsUIPrefab;
    [SerializeField]
    private ShopUIController shopUIPrefab;
    [SerializeField]
    private UnlockSlotUIController unlockSlotUIPrefab;
    [SerializeField]
    private VictoryUIController victoryUIPrefab;
#endregion

    [SerializeField]
    private RectTransform tipsPrefab;

#region 关卡内UI相关
    private const string IN_LEVEL_UI_TAG = "InLevelUI";

    /// <summary>
    /// 关卡内显示金币数量的预制体名称
    /// </summary>
    [SerializeField]
    private string inLevelCoinsName;
    /// <summary>
    /// 关卡内内显示金币数量的文本
    /// </summary>
    private TextMeshProUGUI[] inLevelCoinsTexts;
    /// <summary>
    /// 使用道具按钮
    /// </summary>
    private Button[] inLevelItemButtons;
#endregion

    private object[] uiInstances;
    /// <summary>
    /// Keys: 所有生成时不传其他参数的 UIType 枚举值
    /// <br/><br/>
    /// Values: { Item1: UIType 对应的预制体; Item2: UIType 对应的初始化方法（即 SystemUIManager.InitXXX）}
    /// </summary>
    private Dictionary<UIType, ValueTuple<MonoBehaviour, Action>> dictPrefabInitings;

#region 静态方法
    /// <summary>
    /// 生成指定类型的界面
    /// <br/><br/>
    /// SystemUIManager 会自动初始化和管理生成的界面
    /// </summary>
    /// <param name="type">要生成的界面类型</param>
    /// <param name="args">生成此界面时需要的数据。确定好第一个实参后，参考第一个实参的注释来传入</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static async Task LoadUI(UIType type, params object[] args)
    {
        if (Instance == null)
            await Task.FromException(new InvalidOperationException(EXCEPTION_MANAGER_UNINITIALIZED.Replace("@", nameof(LoadUI))));
        static void playLoadingAudio(UISFXtype audioType) => AudioManager.Instance.PlayUISFX(audioType);
        if (args is { Length: 1 })
            switch (type)
            {
                case UIType.Defeat:
                    if (args[0] is float or int or double)
                        Instance.uiInstances[(uint)type] = Instance.CreateUI(
                            Instance.uiInstances[(uint)type] as DefeatUIController,
                            Instance.defeatUIPrefab,
                            () => { Instance.InitDefeatUI(Convert.ToSingle(args[0])); playLoadingAudio(UISFXtype.Defeat); }
                        );
                    return;
                case UIType.Item:
                    if (Enum.IsDefined(typeof(ItemType), args[0]))
                    {
                        var itemUIs = Instance.uiInstances[(uint)type] as LinkedList<ItemUIController>;
                        ItemUIController itemUI = Instance.CreateUI(
                            itemUIs.Count > 0 ? itemUIs.First() : null,
                            Instance.itemUIPrefab
                        );
                        Instance.InitItemUI(itemUI, (ItemType)args[0]);
                        if (itemUIs.Count > 0)
                            itemUIs.RemoveFirst();
                        itemUIs.AddLast(itemUI);
                    }
                    return;
                case UIType.Settings:
                    if (args[0] is bool isInLevel)
                        Instance.uiInstances[(uint)type] = Instance.CreateUI(
                            Instance.uiInstances[(uint)type] as SettingsUIController,
                            Instance.settingsUIPrefab,
                            () => Instance.InitSettingsUI(isInLevel)
                        );
                    return;
                case UIType.UnlockSlot:
                    if (args[0] is Row row)
                        Instance.uiInstances[(uint)type] = Instance.CreateUI(
                            Instance.uiInstances[(uint)type] as UnlockSlotUIController,
                            Instance.unlockSlotUIPrefab,
                            () => Instance.InitUnlckSlotUI(row)
                        );
                    return;
            }
        switch (type)
        {
            case UIType.Victory:
                Instance.uiInstances[(uint)type] = Instance.CreateUI(
                    Instance.uiInstances[(uint)type] as VictoryUIController,
                    Instance.victoryUIPrefab,
                    args is not null and { Length: 1 } && args[0] is int numCoins
                        ? () => { Instance.InitVictoryUI(numCoins); playLoadingAudio(UISFXtype.LevelComplete); }
                        : () => { Instance.InitVictoryUI(); playLoadingAudio(UISFXtype.LevelComplete); }
                );
                return;
            default:
                if (Enum.IsDefined(typeof(UIType), type))
                {
                    if (!Instance.dictPrefabInitings.ContainsKey(type))
                        await Task.FromException(new ArgumentException(EXCEPITON_ILLEGAL_LOADUI_ARG
                            .Replace("@", nameof(UIType))
                            .Replace("#", $"{type}")
                        ));
                    Instance.uiInstances[(uint)type] = Instance.CreateUI(
                        Instance.uiInstances[(uint)type] as MonoBehaviour,
                        Instance.dictPrefabInitings[type].Item1,
                        Instance.dictPrefabInitings[type].Item2
                    );
                    return;
                }
                await Task.FromException(new ArgumentException(EXCEPITON_ILLEGAL_ENUM_ARG
                    .Replace("@", nameof(type))
                    .Replace("#", $"{type}")
                    .Replace("$", nameof(UIType))
                ));
                return;
        }
    }

    public static async Task PopUpTips(string tipsMessage, Transform parentTransform = null, bool isToMoveUp = true)
    {
        const int movingDuration = 80, stayingDuration = 1000, fadingDelay = 25;
        Transform parent = parentTransform == null
            ? Instance.transform.GetChild(Instance.transform.childCount - 1)
            : parentTransform;
        RectTransform rt = Instantiate(Instance.tipsPrefab, parent);
        CanvasGroup tipsBackground = rt.GetComponent<CanvasGroup>();
        int targetPosY = (int)rt.localPosition.y + 250;

        rt.GetChild(1).GetComponent<Text>().text = tipsMessage;
        if (parentTransform == null || isToMoveUp)
            while (rt.localPosition.y < targetPosY)
            {
                rt.localPosition += Vector3.up * 25;
                await Task.Delay(movingDuration / 10);
            }
        else
            await Task.Delay(movingDuration);
        await Task.Delay(stayingDuration);
        while (tipsBackground != null && tipsBackground.alpha > 0f)
        {
            tipsBackground.alpha -= 0.1f;
            await Task.Delay(fadingDelay);
        }
        if (tipsBackground != null)
            Destroy(rt.gameObject);
    }

    public static void InitUICallbacks(UIType type)
    {
        static GameObject uiGameObj(UIType type)
        {
            var uiObj = Instance.uiInstances[(uint)type] as MonoBehaviour;
            return uiObj == null ? null : uiObj.gameObject;
        }
        static void destroyUI(UIType type)
        {
            Destroy(uiGameObj(type));
            Instance.uiInstances[(uint)type] = null;
        }
        switch (type)
        {
            case UIType.Defeat:
                DefeatUIController.clickingHome = async () => { await LoadUI(UIType.Home); destroyUI(type); };
                DefeatUIController.clickingWatchAd = async _=>
                {
                    await Task.Delay(1000); //假装播放1秒广告
                    await (addingSteps is null ? Task.CompletedTask : addingSteps()); // 增加步数
                    destroyUI(type);
                };
                DefeatUIController.clickingWatchAd = FireAndBan(typeof(DefeatUIController), DefeatUIController.clickingWatchAd);
                DefeatUIController.clickingReplay = async () =>
                {
                    if (uiGameObj(type) == null)
                        return;
                    if (PlayerEnergy.TrySpendEnergy(1))
                    {
                        //Instance.OnUpdateEnergy(); // 目前没必要调 OnUpdateEnergy
                        await (loadingLevel is null ? Task.CompletedTask : loadingLevel(PlayerProgress.GetCurrentLevel(), null)); // 重新加载当前关卡
                        destroyUI(type);
                        return;
                    }
                    await LoadUI(UIType.Energy);
                    await PopUpTips(TIPS_ASKING_ENERGY);
                };
                return;
            case UIType.Energy:
                static async Task onAddEnergy()
                {
                    Instance.OnUpdateEnergy();
                    await PopUpTips(TIPS_SUCCESSFUL_REDEEM + TIPS_ENERGY_ADDED.Replace("@", $"{1}"));
                }
                PlayerEnergy.timing = () =>
                {
                    int secondsToRecover = PlayerEnergy.SecondsToRecover;
                    if (secondsToRecover == 0)
                        Instance.OnUpdateEnergy();
                    EnergyUIController.SecondsToRecover = secondsToRecover;
                };
                EnergyUIController.clickingClose = () => destroyUI(type);
                EnergyUIController.clickingBuy = async price =>
                {
                    if (PlayerEnergy.GetEnergy() >= PlayerEnergy.MaxEnergy)
                    {
                        await PopUpTips(TIPS_ENERGY_IS_FULL);
                        return;
                    }
                    if (PlayerCoin.TrySpendCoin(price))
                    {
                        PlayerEnergy.AddEnergy(1);
                        Instance.OnUpdateCoin(-price);
                        await onAddEnergy();
                        return;
                    }
                    await PopUpTips(TIPS_COIN_LACK);
                };
                EnergyUIController.clickingWatchAd = async _=>
                {
                    await Task.Delay(1000); //假装播放1秒广告
                    if (Instance.uiInstances[(uint)type] == null)
                        return;
                    if (PlayerEnergy.TryAddEnergy(1))
                    {
                        await onAddEnergy();
                        return;
                    }
                    await PopUpTips(TIPS_ENERGY_IS_FULL);
                };
                EnergyUIController.clickingWatchAd = FireAndBan(typeof(EnergyUIController), EnergyUIController.clickingWatchAd);
                return;
            case UIType.Home:
                HomeUIController.clickingSettings = async () => await LoadUI(UIType.Settings, false);
                HomeUIController.clickingAddEnergy = async () => await LoadUI(UIType.Energy);
                HomeUIController.clickingShop = async () => await LoadUI(UIType.Shop);
                HomeUIController.clickingStart = async () =>
                {
                    if (loadingLevel is null)
                        return;
                    int level = PlayerProgress.GetCurrentLevel();
                    if (level > MAX_LEVEL)
                    {
                        await PopUpTips(TIPS_WAIT_FOR_MORE_LEVELS);
                        return;
                    }
                    if (PlayerEnergy.TrySpendEnergy(HomeUIController.NumEnergyToPlay))
                    {
                        //Instance.OnUpdateEnergy(); // 进关卡后不会立即显示体力值，目前没必要调 OnUpdateEnergy
                        await loadingLevel(level, default); // 加载玩家到达的最后一个关卡
                        destroyUI(type);
                        return;
                    }
                    await LoadUI(UIType.Energy);
                    await PopUpTips(TIPS_ASKING_ENERGY);
                };
                return;
            case UIType.Item: return;
            case UIType.Settings:
                SettingsUIController.clickingClose = () => destroyUI(type);
                SettingsUIController.clickingConfirm = async () => { await LoadUI(UIType.Home); destroyUI(type); };
                SettingsUIController.clickingReplay = async () =>
                {
                    if (uiGameObj(type) == null)
                        return;
                    await (loadingLevel is null ? Task.CompletedTask : loadingLevel(PlayerProgress.GetCurrentLevel(), null)); // 重新加载当前关卡
                    destroyUI(type);
                };
                return;
            case UIType.Shop:
                ShopUIController.clickingSettings = async () => await LoadUI(UIType.Settings, false);
                ShopUIController.clickingClose = () =>
                {
                    PlayerAd.SetWatchedAd(0);
                    //Instance.OnWatchAd(); // 目前没必要调 OnWatchAd
                    destroyUI(type);
                };
                ShopUIController.clickingBuy = async (ItemType, count, price) =>
                {
                    if (PlayerCoin.TrySpendCoin(price))
                    {
                        PlayerItem.AddItem(ItemType, count);
                        Instance.OnUpdateCoin(-price);
                        await PopUpTips(TIPS_SUCCESSFUL_REDEEM);
                        return;
                    }
                    await PopUpTips(TIPS_COIN_LACK);
                };
                ShopUIController.clickingWatchAd = async configs =>
                {
                    if (PlayerAd.GetWatchedAd() < configs[1])
                    {
                        await Task.Delay(1000); //假装播放1秒广告
                        PlayerAd.AddWatchedAdd();
                        Instance.OnWatchAd();
                    }
                    if (PlayerAd.GetWatchedAd() >= configs[1])
                    {
                        PlayerAd.AddWatchedAdd(-configs[1]);
                        PlayerCoin.AddCoin(configs[0]);
                        Instance.OnWatchAd();
                        Instance.OnUpdateCoin(configs[0]);
                        await PopUpTips(TIPS_SUCCESSFUL_RECEIVING);
                    }
                };
                ShopUIController.clickingWatchAd = FireAndBan(typeof(ShopUIController), ShopUIController.clickingWatchAd);
                return;
            case UIType.UnlockSlot:
                UnlockSlotUIController.clickingClose = () => destroyUI(type);
                UnlockSlotUIController.clickingWatchAd = async row =>
                {
                    await Task.Delay(1000); //假装播放1秒广告
                    await PopUpTips(TIPS_SUCCESSFL_UNLOCKING);
                    await (unlockingSlot is null ? Task.CompletedTask : unlockingSlot(row));
                    destroyUI(type);
                };
                UnlockSlotUIController.clickingWatchAd = FireAndBan(typeof(UnlockSlotUIController), UnlockSlotUIController.clickingWatchAd);
                return;
            case UIType.Victory:
                VictoryUIController.clickingReceive = async _=>
                {
                    int numCoins = VictoryUIController.numCoinsToReceive;
                    PlayerCoin.AddCoin(numCoins);
                    Instance.OnUpdateCoin(numCoins, true);
                    await PopUpTips(TIPS_SUCCESSFUL_RECEIVING);
                    await LoadUI(UIType.Home);
                    destroyUI(type);
                };
                VictoryUIController.clickingReceive = FireAndBan(
                    typeof(VictoryUIController),
                    VictoryUIController.clickingReceive,
                    nameof(VictoryUIController.clickingReceive)
                );
                VictoryUIController.clickingWatchAd = async _=>
                {
                    int numCoins = VictoryUIController.numCoinsToReceive * 10;
                    await Task.Delay(1000); //假装播放1秒广告
                    PlayerCoin.AddCoin(numCoins);
                    Instance.OnUpdateCoin(numCoins, true);
                    await PopUpTips(TIPS_SUCCESSFUL_RECEIVING);
                    await LoadUI(UIType.Home);
                    destroyUI(type);
                };
                VictoryUIController.clickingWatchAd = FireAndBan(typeof(VictoryUIController), VictoryUIController.clickingWatchAd);
                return;
            default:
                throw new ArgumentException(EXCEPITON_ILLEGAL_ENUM_ARG
                    .Replace("@", nameof(type))
                    .Replace("#", $"{type}")
                    .Replace("$", nameof(UIType))
                );
        }
    }

    //TODO: 删除两个 FireAndBan 方法，改为为各UI类添加 bool 字段用于判断是否正在处理广告
    /// <summary>
    /// 开始处理广告，同时禁用看广告领东西按钮的响应，直到处理完广告
    /// <br/><br/>
    /// 用于派生自 AdProcessor 的界面类型
    /// </summary>
    /// <param name="uiInstance">包含看广告领东西按钮的界面实例</param>
    /// <returns>须代入按钮回调的新委托</returns>
    private static Func<Task> FireAndBan<T>(AdProcessor<T> uiInstance) where T : MonoBehaviour
    {
        Func<Task> clicking = uiInstance.clickingWatchAd;
        async Task fireAndBan()
        {
            uiInstance.clickingWatchAd = null;
            await Task.WhenAll(clicking.GetInvocationList()
                .Cast<Func<Task>>()
                .Select(async f => await f())
            );
            uiInstance.clickingWatchAd = fireAndBan;
        }
        return fireAndBan;
    }

    /// <summary>
    /// 开始执行按钮回调委托，同时禁用按钮响应，直到委托执行完毕
    /// <br/><br/>
    /// 用于派生自 StaticAdProcessor 的界面类型
    /// </summary>
    /// <typeparam name="T">回调委托的返回值类型</typeparam>
    /// <param name="typeOfUIController">要禁用按钮响应的界面类型</param>
    /// <param name="clickingFunc">要执行的回调委托</param>
    /// <param name="nameOfClickingFunc">要执行的回调委托的字段名称</param>
    /// <returns>须代入按钮回调的新委托</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static Func<T, Task> FireAndBan<T>(
        Type typeOfUIController,
        Func<T, Task> clickingFunc,
        string nameOfClickingFunc = nameof(StaticAdProcessor<MonoBehaviour, object>.clickingWatchAd)
    )
    {
        FieldInfo clickingField = typeOfUIController.GetField(
            nameOfClickingFunc,
            BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public
        );
        if (clickingField?.GetValue(null) is not Func<T, Task> clicking)
            throw new InvalidOperationException(EXCEPITON_STATIC_FIELD_NOT_FOUND
                .Replace("@", typeOfUIController.ToString())
                .Replace("#", nameOfClickingFunc)
                .Replace("$", $"Func<{typeof(T)}, Task>")
            );

        async Task fireAndBan(T arg)
        {
            clickingField.SetValue(null, null);
            await Task.WhenAll(clicking.GetInvocationList()
                .Cast<Func<T, Task>>()
                .Select(async f => await f(arg))
            );
            clickingField.SetValue(null, (Func<T, Task>)fireAndBan);
        }
        return fireAndBan;
    }
#endregion

    void Awake()
    {
        InitUIManager();
        Array.ForEach(Enum.GetValues(typeof(UIType)) as UIType[], t => InitUICallbacks(t));
    }

    void Start()
    {
        //启动时直接显示大厅
        uiInstances[(uint)UIType.Home] = CreateUI(null, homeUIPrefab, InitHomeUI);
        InitInLevelUIs();

//#if UNITY_EDITOR
        // UI加载示例：
        //_= SystemUIManager.LoadUI(UIType.Defeat, 0.6f);           // 失败界面（游戏进度60%）
        //_= SystemUIManager.LoadUI(UIType.Energy);                 // 体力补充界面
        //_= SystemUIManager.LoadUI(UIType.Home);                   // 大厅界面
        //_= SystemUIManager.LoadUI(UIType.Item, ItemType.Shuffle); // 道具界面（洗牌）
        //_= SystemUIManager.LoadUI(UIType.Settings, false);        // 设置界面（关卡外）
        //_= SystemUIManager.LoadUI(UIType.Shop);                   // 商店界面
        //_= SystemUIManager.LoadUI(UIType.UnlockSlot, row);        // 开启槽位界面
        //_= SystemUIManager.LoadUI(UIType.Victory, 10);            // 胜利界面（可领取金币数：10）
//#endif
    }

#region 各界面初始化方法
    private void InitDefeatUI(float progress) => DefeatUIController.progress = progress;
    private void InitEnergyUI()
    {
        EnergyUIController.NumEnergy = PlayerEnergy.GetEnergy();
        EnergyUIController.maxEnergy = PlayerEnergy.MaxEnergy;
    }
    private void InitHomeUI()
    {
        initingHome?.Invoke();
        HomeUIController.NumCoins = PlayerCoin.GetCoin();
        HomeUIController.NumEnergy = PlayerEnergy.GetEnergy();
        HomeUIController.MaxEnergy = PlayerEnergy.MaxEnergy;
        HomeUIController.LevelName = "关卡" + (PlayerProgress.GetCurrentLevel() - 100); //TODO: 修正关卡编号（101 → 1, 102 → 2, ...）
        //HomeUIController.NumEnergyToPlay = PlayerProgress.GetNumEnergyToPlay(); //TODO: 从 PlayerProgress 类获取进关卡扣除的体力值数据
    }
    private void InitItemUI(ItemUIController itemUI, ItemType itemType)
    {
        ValueTuple<string, Sprite> itemInfoIcon = dictItemInfoIcons[itemType];
        string[] infos = itemInfoIcon.Item1.Split('`');

        itemUI.itemType = itemType;
        itemUI.nameText.text = infos[0];
        itemUI.price = int.Parse(infos[1]);
        itemUI.descriptionText.text = infos[2];
        itemUI.iconImage.sprite = itemInfoIcon.Item2;

        void destroyItemUI()
        {
            if (itemUI == null)
                return;
            (uiInstances[(uint)UIType.Item] as LinkedList<ItemUIController>).Remove(itemUI);
            Destroy(itemUI.gameObject);
        }
        itemUI.clickingClose = destroyItemUI;
        if (!ItemUIController.clickingsCache.TryGetValue(itemType, out var cachedClickings))
        {
            cachedClickings = (
                async price =>
                {
                    if (PlayerCoin.TrySpendCoin(price))
                    {
                        PlayerItem.AddItem(itemType, 1);
                        await PopUpTips(TIPS_SUCCESSFUL_REDEEM, Instance.transform);
                        return;
                    }
                    await PopUpTips(TIPS_COIN_LACK, Instance.transform);
                },
                async () =>
                {
                    await Task.Delay(1000); //假装播放1秒广告
                    PlayerItem.AddItem(itemType, 1);
                    await PopUpTips(TIPS_SUCCESSFUL_RECEIVING, Instance.transform);
                }
            );
            ItemUIController.clickingsCache.Add(itemType, cachedClickings);
        }
        itemUI.clickingBuy = async price => { await cachedClickings.Item1(price); destroyItemUI(); };
        itemUI.clickingWatchAd = async () => { await cachedClickings.Item2(); destroyItemUI(); };
        itemUI.clickingWatchAd = FireAndBan(itemUI);
    }
    private void InitSettingsUI(bool isInLevel)
    {
        SettingsUIController.isInLevel = isInLevel;
        //SettingsUIController.numEnergyToPlay = PlayerProgress.GetNumEnergyToPlay(); //TODO: 从 PlayerProgress 类获取进关卡扣除的体力值数据
    }
    private void InitShopUI()
    {
        ShopUIController.NumCoins = PlayerCoin.GetCoin();
        ShopUIController.NumEnergy = PlayerEnergy.GetEnergy();
        ShopUIController.maxEnergy = PlayerEnergy.MaxEnergy;
        ShopUIController.NumWatchedAd = PlayerAd.GetWatchedAd();
    }
    private void InitUnlckSlotUI(Row row) => UnlockSlotUIController.currentRow = row;
    private void InitVictoryUI(int? numRewardCoins = null)
    {
        if (numRewardCoins is int numCoins)
            VictoryUIController.numCoinsToReceive = numCoins;
    }
#endregion

#region 玩家资源更新响应
    private void OnUpdateCoin(int count, bool isInLevel = false)
    {
        if (count > 0)
            AudioManager.Instance.PlayUISFX(UISFXtype.GetCoins);

        int numCoins = PlayerCoin.GetCoin();
        Array.ForEach(inLevelCoinsTexts, t => t.text = numCoins.ToString());
        if (isInLevel)
            return;
        HomeUIController.NumCoins = numCoins;
        ShopUIController.NumCoins = numCoins;
    }
    private void OnUpdateEnergy()
    {
        int numEnergy = PlayerEnergy.GetEnergy();
        EnergyUIController.NumEnergy = numEnergy;
        HomeUIController.NumEnergy = numEnergy;
        ShopUIController.NumEnergy = numEnergy;
    }
    private void OnWatchAd() => ShopUIController.NumWatchedAd = PlayerAd.GetWatchedAd();
#endregion

    private void InitUIManager()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;

        Instance = this;
        uiInstances = new object[Enum.GetValues(typeof(UIType)).Length];
        uiInstances[(uint)UIType.Item] = new LinkedList<ItemUIController>();
        dictItemInfoIcons = itemInfos.ToDictionary(
            s => (ItemType)uint.Parse(s.Split('`', 2)[0]),
            s => (
                s.Split('`', 2)[1].Replace('，', ','),
                itemIcons.First(i => i.name.Split('_', 2)[0] == s.Split('`', 2)[0])
            )
        );
        dictPrefabInitings = new()
        {
            { UIType.Energy, (energyUIPrefab, InitEnergyUI) },
            { UIType.Home, (homeUIPrefab, InitHomeUI) },
            { UIType.Shop, (shopUIPrefab, InitShopUI) }
        };

    #region 初始化各静态委托
        BGMType[] homeBGMTypes = new BGMType[] { BGMType.MainPage1, BGMType.MainPage2 };
        initingHome = () => AudioManager.Instance.PlayBGM(
            homeBGMTypes[DateTimeOffset.UtcNow.ToUnixTimeSeconds() % homeBGMTypes.Length]
        );
        loadingLevel = async (level, _) => LevelManager.Instance.InitCurrentLevel(level);
        addingSteps = async () => StepManager.Instance.AddExtraSteps();
        unlockingSlot = async row => Row.OnChangeMainRowType(row);
    #endregion
    }

    private void InitInLevelUIs()
    {
        GameObject[] inLevelUIs = GameObject.FindGameObjectsWithTag(IN_LEVEL_UI_TAG);

        inLevelCoinsTexts = inLevelUIs
            .Where(o => o.name == inLevelCoinsName)
            .Select(o => o.GetComponentInChildren<TextMeshProUGUI>())
            .ToArray();
        Array.ForEach(inLevelCoinsTexts, t => t.text = PlayerCoin.GetCoin().ToString());

        Button[] inLevelButtons = inLevelUIs
            .Select(o => { o.TryGetComponent(out Button b); return b; })
            .Where(b => b != null)
            .ToArray();
        Array.ForEach(inLevelButtons, b => b.onClick.AddListener(() => AudioManager.Instance.PlayUISFX(UISFXtype.ClickButton)));

        inLevelItemButtons = inLevelButtons
            .Where(b => Enum.GetNames(typeof(ItemType))
                .Any(n => n.Equals(b.name, StringComparison.OrdinalIgnoreCase))
            ).ToArray();
        InitItemButtons();
    }

    private void InitItemButtons()
    {
        foreach (var i in Enumerable.Range(0, inLevelItemButtons.Length))
        {
            Action[] usingItems = new Action[]
            {
                async () => await RowManager.Instance.HintTry(),
                DeckManager.Instance.ShuffleDeck
            };
            inLevelItemButtons[i].onClick.AddListener(async () =>
            {
                ItemType type = (ItemType)i + 1;
                if (PlayerItem.TrySpendItem(type, 1))
                {
                    usingItems[i]();
                    return;
                }
                await LoadUI(UIType.Item, type);
                await PopUpTips(TIPS_ASKING_ITEM);
            });
        }
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

    /// <summary>
    /// SystemUIManager.LoadUI 的非静态版本。无法通过 await 等待
    /// <br/><br/>
    /// 是为了直接从 Unity Editor 的 Inspector 面板将界面加载方法绑定到按钮而准备的，
    /// <br/><br/>
    /// 返回类型不得不定义为 void。代码中应避免调用该方法
    /// </summary>
    /// <param name="args">将本应传入 SystemUIManager.LoadUI 的参数连成字符串，两两之间用逗号隔开（例："Defeat, 0.5f"）</param>
    /// <exception cref="ArgumentException"></exception>
    public async void FireLoadUI(string args) => await (TryParseLoadUIArgs(args, out UIType type, out object[] loadUIArgs)
        ? LoadUI(type, loadUIArgs)
        : Task.FromException(new ArgumentException(EXCEPITON_ILLEGAL_ENUM_ARG
            .Replace("@", nameof(type))
            .Replace("#", $"{type}")
            .Replace("$", nameof(UIType))
        ))
    );

    private bool TryParseLoadUIArgs(string str, out UIType type, out object[] args)
    {
        type = (UIType)(-1);
        args = null;
        if (str is null or { Length: 0 })
            return false;
        IEnumerable<string> splitedArgs = str.Split(',').Select(s => s.Trim());
        if (!Enum.TryParse(splitedArgs.ElementAt(0), true, out type))
            return false;
        args = splitedArgs.Skip(1).Cast<object>().Select(s =>
            Enum.TryParse(s as string, true, out ItemType itemType)
                ? itemType
                : float.TryParse(s as string, out float floatNumber)
                    ? floatNumber
                    : bool.TryParse(s as string, out bool boolean)
                        ? boolean
                        : s
        ).ToArray();
        return true;
    }
}
