using System;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class EnergyUIController : StaticAdProcessor<EnergyUIController, object>
{
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
                sharedEnergyCountText.text = value.ToString();
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
    public static int maxEnergy = 0;
#endregion

#region 距下一次回复体力的时间
    /// <summary>
    /// 距下一次回复体力还有多少秒
    /// </summary>
    public static int SecondsToRecover
    {
        get => secondsToRecover;
        set
        {
            if (sharedClockText != null)
                sharedClockText.text = numEnergy < maxEnergy
                    ? $"{value / 60:00}:{value % 60:00}"
                    : "满";
            secondsToRecover = value;
        }
    }
    private static int secondsToRecover = 300;
    private static TextMeshProUGUI sharedClockText;
    [SerializeField]
    private TextMeshProUGUI clockText;
#endregion

#region 按钮回调委托
    /// <summary>
    /// 关闭界面按钮回调
    /// <br/><br/>
    /// 应将 Destroy(shopUI.gameObject) 加到最后
    /// </summary>
    static public Action clickingClose;
    /// <summary>
    /// 购买体力按钮回调
    /// </summary>
    static public Func<int, Task> clickingBuy;
#endregion

#region 按钮回调方法
    public void OnClickClose() => clickingClose?.Invoke();
    public async void OnClickReceive() => await AdProcessor.ProcessAd(clickingWatchAd, null, 0);
    public async void OcClickBuy(string price) => await (clickingBuy is null ? Task.CompletedTask : clickingBuy(int.Parse(price)));
#endregion

    void Start()
    {
        InitSharedFields();
    }

    private void InitSharedFields()
    {
        sharedEnergyCountText = energyCountText;
        sharedClockText = clockText;

        NumEnergy = numEnergy;
        SecondsToRecover = secondsToRecover;
    }
}
