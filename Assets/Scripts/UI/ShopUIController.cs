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
public class ShopUIController : StaticAdProcessor<ShopUIController, int[]>
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
    public static int maxEnergy = 5;
    private static GameObject sharedMaxEnergyTips;
    [SerializeField]
    private GameObject maxEnergyTips;
#endregion

#region 已观看广告数
    /// <summary>
    /// 玩家本次进入商店后已观看的广告数
    /// </summary>
    public static int NumWatchedAd
    {
        get => numWatchedAd;
        set
        {
            if (sharedWatchedAdTexts != null)
                foreach (var t in sharedWatchedAdTexts)
                    t.text = value + "/" + t.text.Split('/')[1];
            numWatchedAd = value;
        }
    }
    private static int numWatchedAd = 0;
    private static TextMeshProUGUI[] sharedWatchedAdTexts;
    [SerializeField]
    private string watchedAdTextName;
    [SerializeField]
    private Transform coinPacksParent;
#endregion

#region 按钮回调委托
    /// <summary>
    /// 设置按钮回调
    /// </summary>
    public static Func<Task> clickingSettings;
    /// <summary>
    /// 关闭界面按钮回调
    /// <br/><br/>
    /// 应将 Destroy(shopUI.gameObject) 加到最后
    /// </summary>
    public static Action clickingClose;
    /// <summary>
    /// 购买礼包按钮回调
    /// </summary>
    public static Func<ItemType, int, int, Task> clickingBuy;
#endregion

#region 按钮回调方法
    public async void OnClickSettings() => await (clickingSettings is null ? Task.CompletedTask : clickingSettings());
    public void OnClickClose() => clickingClose?.Invoke();
    public async void OcClickBuy(string config)
    {
        int[] c = config.Split(',').Select(s => int.Parse(s.Trim())).ToArray();
        await (clickingBuy is null ? Task.CompletedTask : clickingBuy((ItemType)c[0], c[1], c[2]));
    }
    public async void OnClickReceive(string config) => await AdProcessor.ProcessAd(
        clickingWatchAd,
        null,
        config.Split(',').Select(s => int.Parse(s.Trim())).ToArray()
    );
#endregion

    void Start()
    {
        InitSharedFields();
    }

    private void InitSharedFields()
    {
        sharedCoinCountText = coinCountText;
        sharedEnergyCountText = energyCountText;
        sharedMaxEnergyTips = maxEnergyTips;
        sharedWatchedAdTexts = coinPacksParent.GetComponentsInChildren<TextMeshProUGUI>(true)
            .Where(t => t.name == watchedAdTextName)
            .ToArray();

        NumCoins = numCoins;
        NumEnergy = numEnergy;
        NumWatchedAd = numWatchedAd;
    }
}
