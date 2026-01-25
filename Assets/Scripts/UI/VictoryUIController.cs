using System;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

/// <summary>
/// 胜利界面控制器
/// <br/><br/>
/// 为该类的静态字段赋值以更新各个按钮的显示内容及行为
/// </summary>
public class VictoryUIController : StaticAdProcessor<VictoryUIController, object>, IAudioTrigger
{
#region 静态公开字段
    /// <summary>
    /// 要显示在领取金币按钮上的金币数量
    /// </summary>
    public static int numCoinsToReceive = 10;
    /// <summary>
    /// 单倍领取按钮回调
    /// <br/><br/>
    /// 应将 Destroy(victoryUI.gameObject) 加到最后
    /// </summary>
    public static Func<object, Task> clickingNormal;
#endregion

    /// <summary>
    /// 领取金币按钮上的金币数量文本组件
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI coinCountText;

    public async void OnClickNormal()
    {
        if (!isWatchingAd)
        {
            isWatchingAd = true;
            await (clickingNormal is null ? Task.CompletedTask : clickingNormal(default));
            isWatchingAd = false;
        }
    }

    void Start() => coinCountText.text = numCoinsToReceive.ToString();

    public void PlayClickingAudio() => (this as IAudioTrigger).PlayClickingAudio(default);
}
