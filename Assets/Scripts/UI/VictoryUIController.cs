using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// 胜利界面控制器
/// <br/><br/>
/// 为该类的静态字段赋值以更新各个按钮的显示内容及行为
/// </summary>
public class VictoryUIController : MonoBehaviour
{
#region 静态公开字段
    /// <summary>
    /// 要显示在领取金币按钮上的金币数量
    /// </summary>
    public static int numCoinsToRecieve = 10;
    /// <summary>
    /// 返回主界面按钮回调
    /// <br/><br/>
    /// 应将 Destroy(victoryUI) 加到最后
    /// </summary>
    public static UnityAction clickingHome;
    /// <summary>
    /// 十倍领取按钮回调
    /// <br/><br/>
    /// 应将 Destroy(victoryUI) 加到最后
    /// </summary>
    public static Func<Task> clickingRecieveMore;
    /// <summary>
    /// 领取金币按钮回调
    /// <br/><br/>
    /// 应将 Destroy(victoryUI) 加到最后
    /// </summary>
    public static UnityAction clickingRecieve;
#endregion

    /// <summary>
    /// 领取金币按钮上的金币数量文本组件
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI coinCountText;

#region 按钮回调方法
    public void OnClickHome() => clickingHome?.Invoke();
    public void OnClickRecieve() => clickingRecieve?.Invoke();
    public void OnClickRecieveMore()
    {
        static async Task Process()
        {
            if (clickingRecieveMore is not null)
                foreach (var f in clickingRecieveMore.GetInvocationList().Cast<Func<Task>>())
                    await f();
        }
        _ = Process();
    }
#endregion

    void Start()
    {
        coinCountText.text = numCoinsToRecieve.ToString();
    }
}
