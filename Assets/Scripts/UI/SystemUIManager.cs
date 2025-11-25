using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

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

#region 各界面预制体和引用
    [SerializeField]
    private DefeatUIController defeatUIPrefab;
    private DefeatUIController defeatUI;
    [SerializeField]
    private EnergyUIController energyUIPrefab;
    private EnergyUIController energyUI;
    [SerializeField]
    private HomeUIController homeUIPrefab;
    private HomeUIController homeUI;
    [SerializeField]
    private ItemUIController itemUIPrefab;
    private List<ItemUIController> itemUIs = new();
    [SerializeField]
    private QuitUIController quitUIPrefab;
    private QuitUIController quitUI;
    [SerializeField]
    private ShopUIController shopUIPrefab;
    private ShopUIController shopUI;
    [SerializeField]
    private VictoryUIController victoryUIPrefab;
    private VictoryUIController victoryUI;
#endregion

    void Awake()
    {
        Instance = this;
        homeUI = CreateUI(null, homeUIPrefab, InitHomeUI);
        itemIDInfos = itemInfos.ToDictionary(
            s => uint.Parse(s.Split('`', 2)[0]),
            s => s.Split('`', 2)[1].Replace('，', ',')
        );
        defeatUI = CreateUI(null, defeatUIPrefab, () => InitDefeatUI(0.6f));
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

    private void InitItemUI(uint id, ItemUIController ui)
    {
        Task.Run(() =>
        {
            string[] infos = itemIDInfos[id].Split('`');
            ui.nameText.text = infos[0];
        });
    }
}
