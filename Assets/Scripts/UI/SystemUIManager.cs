using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    Defeat,
    Energy,
    Home,
    Item,
    Quit,
    Shop,
    Victory
}

public class SystemUIManager : MonoBehaviour
{
    public static SystemUIManager Instance;
    /// <summary>
    /// 加载关卡
    /// </summary>
    public static Func<Task> loadingLevel;

#region public 游戏内所有道具的信息和图标
    public string[] itemInfos;
    public Sprite[] itemIcons;
    private Dictionary<uint, string> itemIDInfos;
#endregion

    private object[] uiInstances;

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

    void Awake()
    {
        Instance = this;
        uiInstances = new object[Enum.GetNames(typeof(UIType)).Length];
        uiInstances[(uint)UIType.Item] = new List<ItemUIController>();
        //homeUI = CreateUI(null, homeUIPrefab, InitHomeUI);
        itemIDInfos = itemInfos.ToDictionary(
            s => uint.Parse(s.Split('`', 2)[0]),
            s => s.Split('`', 2)[1].Replace('，', ',')
        );
    }

    public static void LoadUI(UIType type, params object[] args)
    {
        switch (type)
        {
            case UIType.Defeat:
                float progress;
                if (args.Length > 0 && float.TryParse(args[0].ToString(), out progress))
                    uiInstances[(uint)UIType.Defeat] = CreateUI(
                        uiInstances[(uint)UIType.Defeat],
                        defeatUIPrefab,
                        () => InitDefeatUI(progress)
                    );
                break;
            case UIType.Item:
                uint itemId;
                if (args.Length > 0 && uint.TryParse(args[0].ToString(), out itemId))
                {
                    ItemUIController itemUI = CreateUI(uiInstances[(uint)UIType.Item], itemUIPrefab);
                    InitItemUI(itemUI, itemId);
                    uiInstances[(uint)UIType.Item] = itemUI;
                    //TODO: 把所有Prefab放进一个数组，依照UIType中的顺序进行排序
                }
                break;
            default:
                ;
        }
    }

    public static async void ProcessAd<T>(T toWait, Action afterWait = null, params object[] toWaitArgs) where T : Delegate
    {
        if (toWait is not null)
            await Task.WhenAll(toWait.GetInvocationList()
                .Select(d =>
                    d.Method.ReturnType == typeof(Task)
                    ? d.Method.Invoke(d.Target, toWaitArgs) as Task
                    : Task.FromException(new Exception("SystemUIManager.ProcessAd的第一个参数只能是 Func<..., Task> 类型！"))
                )
            );
        afterWait?.Invoke();
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

    private void OnSpendEnergy()
    {
        // TODO: 更新各界面体力值显示
    }

    /// <summary>
    /// 失败界面初始化
    /// </summary>
    /// <param name="progress">玩家达成的游戏进度（0到1之间）</param>
    private void InitDefeatUI(float progress)
    {
        Action destroy = () => Destroy(defeatUI.gameObject);

        DefeatUIController.progress = progress;

        DefeatUIController.clickingHome += () =>
        {
            homeUI = CreateUI(homeUI, homeUIPrefab, InitHomeUI);
            destroy();
        };

        DefeatUIController.clickingContinue += async () =>
        {
            await Task.Delay(3000); // 假装播放3秒广告
            _ = loadingLevel?.Invoke(); // TODO: 这一行加载关卡
            destroy();
        };

        DefeatUIController.clickingReplay += () =>
        {
            if (PlayerEnergy.TrySpendEnergy(1))
            {
                OnSpendEnergy();
                _ = loadingLevel?.Invoke(); // TODO: 这一行加载关卡
                destroy();
                return;
            }
            // TODO: 显示体力不足警告及体力补充界面
        };
    }

    /// <summary>
    /// 大厅界面初始化
    /// </summary>
    private void InitHomeUI()
    {
        
    }

    private void InitItemUI(ItemUIController itemUI, uint itemId)
    {
        Task.Run(() =>
        {
            string[] infos = itemIDInfos[itemId].Split('`');
            itemUI.itemId = itemId;
            itemUI.nameText.text = infos[0];
        });
    }
}
