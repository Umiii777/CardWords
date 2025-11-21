using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// 胜利界面控制器：
/// 为该类的静态字段赋值以更新各个按钮的显示内容和行为
/// </summary>
public class VictoryUIController : MonoBehaviour
{
#region Static Fields
    /// <summary>
    /// 要显示在领取金币按钮上的金币数量
    /// </summary>
    public static int numCoinsToRecieve = 0;
    /// <summary>
    /// 返回主界面按钮回调
    /// </summary>
    public static UnityAction clickingHome;
    /// <summary>
    /// 十倍领取按钮回调
    /// </summary>
    public static UnityAction clickingRecieveMore;
    /// <summary>
    /// 领取金币按钮回调
    /// </summary>
    public static UnityAction clickingRecieve;
#endregion

    /// <summary>
    /// 领取金币按钮上的金币数量文本组件
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI coinsCountText;

    void Awake()
    {
        coinsCountText.text = numCoinsToRecieve.ToString();
    }

    public void OnClickHome()
    {
        clickingHome?.Invoke();
    }

    public void OnClickRecieveMore()
    {
        clickingRecieveMore?.Invoke();
    }

    public void OnClickRecieve()
    {
        clickingRecieve?.Invoke();
    }
}
