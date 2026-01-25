using System;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class EnergyUIController : StaticAdProcessor<EnergyUIController, object>, IAudioTrigger
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

    /// <summary>
    /// 购买体力按钮回调
    /// </summary>
    public static Func<int, Task> clickingBuy;

    public async void OcClickBuy(string price) => await ProcessClicking(clickingBuy, int.Parse(price));

    void Start() => InitSharedFields();

    private void InitSharedFields()
    {
        sharedEnergyCountText = energyCountText;
        sharedClockText = clockText;

        NumEnergy = numEnergy;
        SecondsToRecover = secondsToRecover;
    }

    public void PlayClickingAudio() => (this as IAudioTrigger).PlayClickingAudio(default);
}
