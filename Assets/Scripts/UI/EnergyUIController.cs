using System;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class EnergyUIController : MonoBehaviour
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
    private static int maxEnergy = 0;
#endregion

#region 距下一次恢复体力的时间
    private static TextMeshProUGUI sharedClockText;
    [SerializeField]
    private TextMeshProUGUI clockText;
#endregion

#region 静态委托
    /// <summary>
    /// 关闭界面按钮回调
    /// <br/><br/>
    /// 应将 Destroy(shopUI.gameObject) 加到最后
    /// </summary>
    static public Action clickingClose;
    /// <summary>
    /// 购买体力按钮回调
    /// </summary>
    static public Action<int> clickingBuy;
    /// <summary>
    /// 领取体力按钮回调
    /// </summary>
    static public Func<Task> clickingReceive;
#endregion

#region 按钮回调方法
    public void OnClickClose() => clickingClose?.Invoke();
    public void OnClickReceive() => SystemUIManager.ProcessAd(clickingReceive);
    public void OcClickBuy(string price) => clickingBuy?.Invoke(int.Parse(price));
#endregion

    void Start()
    {
        NumEnergy = PlayerEnergy.GetEnergy();
        maxEnergy = PlayerEnergy.MaxEnergy;
        InitSharedFields();
        UpdateClockText();
        PlayerEnergy.timing += UpdateClockText;
    }

    private void InitSharedFields()
    {
        sharedEnergyCountText = energyCountText;
        sharedClockText = clockText;

        sharedEnergyCountText.text = numEnergy.ToString();
    }

    private static void UpdateClockText() => sharedClockText.text = numEnergy < maxEnergy ? GetTimeToRecover() : "满";
    private static string GetTimeToRecover() => $"{PlayerEnergy.secondsToRecover / 60:00}:{PlayerEnergy.secondsToRecover % 60:00}";
}
