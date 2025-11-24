using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

/// <summary>
/// 商店界面控制器
/// <br/><br/>
/// 为该类的静态属性或字段赋值以更新各个组件的显示内容及行为
/// </summary>
public class ShopUIController : MonoBehaviour
{
#region 玩家金币数量
    /// <summary>
    /// 玩家拥有的金币数量
    /// </summary>
    public static int NumCoins
    {
        get => numCoins;
        set
        {
            if (sharedCoinCountText != null)
                sharedCoinCountText.text = value.ToString();
            numCoins = value;
        }
    }
    private static int numCoins = 0;
    private static TextMeshProUGUI sharedCoinCountText;
    [SerializeField]
    private TextMeshProUGUI coinCountText;
#endregion

#region 当前体力值
    /// <summary>
    /// 玩家当前体力值
    /// </summary>
    public static int NumEnergy
    {
        get => numEnergy;
        set
        {
            if (sharedEnergyCountText != null)
            {
                sharedEnergyCountText.text = value.ToString();
                sharedMaxEnergyTips.SetActive(value >= maxEnergy);
            }
            numEnergy = value;
        }
    }
    private static int numEnergy = 0;
    private static TextMeshProUGUI sharedEnergyCountText;
    [SerializeField]
    private TextMeshProUGUI energyCountText;
#endregion

#region 体力值上限
    /// <summary>
    /// 玩家体力值上限
    /// </summary>
    private static int maxEnergy = 0;
    private static GameObject sharedMaxEnergyTips;
    [SerializeField]
    private GameObject maxEnergyTips;
#endregion

#region 静态委托
    /// <summary>
    /// 设置按钮回调
    /// </summary>
    public static Action clickingSettings;
    /// <summary>
    /// 关闭界面按钮回调
    /// <br/><br/>
    /// 应将 Destroy(shopUI) 加到最后
    /// </summary>
    public static Action clickingClose;
    /// <summary>
    /// 领取金币按钮回调
    /// </summary>
    public static Func<int, int, Task> clickingReceive;
    /// <summary>
    /// 购买礼包按钮回调
    /// </summary>
    public static Action<ItemType, int, int> clickingBuy;
    /// <summary>
    /// 该委托在 ShopUIController 的协程中每帧执行一次
    /// </summary>
    public static Action runningCoroutine;
#endregion

    [SerializeField]
    private string watchedAdTextName;
    [SerializeField]
    private Transform coinPacksParent;
    private TextMeshProUGUI[] watchedAdTexts;

#region 按钮回调方法
    public void OnClickSettings() => clickingSettings?.Invoke();
    public void OnClickClose() => clickingClose?.Invoke();
    public void OcClickBuy(string config)
    {
        int[] c = config.Split(',').Select(s => int.Parse(s.Trim())).ToArray();
        clickingBuy?.Invoke((ItemType)c[0], c[1], c[2]);
    }
    public void OnClickReceive(string config) => _ = PlayerAd.ProcessAd(
        clickingReceive,
        UpdateWatchedAdText,
        config.Split(',')
            .Select(s => int.Parse(s.Trim()))
            .Cast<object>().ToArray()
    );
#endregion

    void Start()
    {
        InitSharedFields();
        InitWatchedAdText();
        UpdateWatchedAdText();
        NumEnergy = PlayerEnergy.GetEnergy();
        maxEnergy = PlayerEnergy.MaxEnergy;
    }

    private void InitSharedFields()
    {
        sharedCoinCountText = coinCountText;
        sharedEnergyCountText = energyCountText;
        sharedMaxEnergyTips = maxEnergyTips;

        sharedCoinCountText.text = numCoins.ToString();
        sharedEnergyCountText.text = numEnergy.ToString();
        sharedMaxEnergyTips.SetActive(numEnergy >= maxEnergy);
    }

    private void InitWatchedAdText()
    {
        watchedAdTexts = coinPacksParent.GetComponentsInChildren<Transform>(true)
            .Where(t => t.name == watchedAdTextName)
            .Select(t => t.GetComponent<TextMeshProUGUI>())
            .ToArray();
    }

    private void UpdateWatchedAdText()
    {
        foreach (var t in watchedAdTexts)
            t.text = PlayerAd.GetWatchedAd() + "/" + t.text.Split('/')[1];
    }
}
