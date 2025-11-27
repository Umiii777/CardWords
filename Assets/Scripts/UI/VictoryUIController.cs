using System;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public abstract class StaticAdProcessor<UIControllerType, U> : MonoBehaviour where UIControllerType : MonoBehaviour
{
    /// <summary>
    /// 看广告领东西按钮回调
    /// </summary>
    public static Func<U, Task> clickingWatchAd;
}

/// <summary>
/// 胜利界面控制器
/// <br/><br/>
/// 为该类的静态字段赋值以更新各个按钮的显示内容及行为
/// </summary>
public class VictoryUIController : StaticAdProcessor<VictoryUIController, object>
{
#region 静态公开字段
    /// <summary>
    /// 要显示在领取金币按钮上的金币数量
    /// </summary>
    public static int numCoinsToReceive = 10;
    /// <summary>
    /// 返回主界面按钮回调
    /// <br/><br/>
    /// 应将 Destroy(victoryUI.gameObject) 加到最后
    /// </summary>
    public static Action clickingHome;
    /// <summary>
    /// 领取奖励按钮回调
    /// <br/><br/>
    /// 应将 Destroy(victoryUI.gameObject) 加到最后
    /// </summary>
    public static Action clickingReceive;
#endregion

    /// <summary>
    /// 领取金币按钮上的金币数量文本组件
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI coinCountText;

#region 按钮回调方法
    public void OnClickHome() => clickingHome?.Invoke();
    public void OnClickReceive() => clickingReceive?.Invoke();
    public void OnClickReceiveMore() => SystemUIManager.ProcessAd(clickingWatchAd, null, 0);
#endregion

    void Start()
    {
        coinCountText.text = numCoinsToReceive.ToString();
    }
}
