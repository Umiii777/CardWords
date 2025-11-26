using System;
using UnityEngine;
using TMPro;

/// <summary>
/// 退出游戏确认界面控制器
/// </summary>
public class QuitUIController : MonoBehaviour
{
#region 静态公开字段
    /// <summary>
    /// 退出游戏要扣除的体力值
    /// </summary>
    public static int numEnergy = 1;
    /// <summary>
    /// 关闭界面按钮回调
    /// <br/><br/>
    /// 应将 Destroy(quitUI.gameObject) 加到最后
    /// </summary>
    public static Action clickingClose;
    /// <summary>
    /// 确认退出退出按钮回调
    /// <br/><br/>
    /// 应将 Application.Quit() 加到最后
    /// </summary>
    public static Action clickingQuit;
    /// <summary>
    /// 继续游戏按钮回调
    /// </summary>
    public static Action clickingContinue;
#endregion

    [SerializeField]
    private TextMeshProUGUI numEnergyText;

#region 按钮回调方法
    public void OnClickClose() => clickingClose?.Invoke();
    public void OnClickQuit() => clickingQuit?.Invoke();
    public void OnClickContinue() => clickingContinue?.Invoke();
#endregion

    void Start()
    {
        numEnergyText.text = numEnergy.ToString();
    }
}
