using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// 失败界面控制器
/// <br/><br/>
/// 为该类的静态字段赋值以更新各个按钮和进度条的显示内容和及行为
/// </summary>
public class DefeatUIController : MonoBehaviour
{
#region 静态公开字段
    /// <summary>
    /// 要显示的游戏完成进度（0到1之间）
    /// </summary>
    public static float progress = 0.01f;
    /// <summary>
    /// 要显示在重玩按钮上的体力值（请代入负数）
    /// </summary>
    public static int numEnergy = 0;
    /// <summary>
    /// 返回主界面按钮回调
    /// </summary>
    public static UnityAction clickingHome;
    /// <summary>
    /// 继续游戏按钮回调
    /// </summary>
    public static UnityAction clickingContinue;
    /// <summary>
    /// 重玩按钮回调
    /// </summary>
    public static UnityAction clickingReplay;
    /// <summary>
    /// 该委托在 DefeatUIController 的协程中每帧执行一次
    /// <br/><br/>
    /// 在这里使用 DefeatUIController.progress += ... 可以实现进度条逐渐填充
    /// </summary>
    public static UnityAction runningCoroutine;
#endregion

#region 私有字段
    /// <summary>
    /// 完成度进度条的最小宽度
    /// </summary>
    private const int LEAST_PROGRESS_BAR_WIDTH = 50;

    /// <summary>
    /// 重玩按钮上的体力值文本组件
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI energyCountText;
    /// <summary>
    /// 显示游戏完成进度的文本组件
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI progressText;
    /// <summary>
    /// 用于控制游戏完成度进度条
    /// </summary>
    [SerializeField]
    private RectTransform progressBarTransform;
    private RectTransform progressBarParentTransform;
#endregion

#region 按钮回调方法
    public void OnClickHome()
    {
        clickingHome?.Invoke();
    }
    public void OnClickContinue()
    {
        clickingContinue?.Invoke();
    }
    public void OnClickReplay()
    {
        clickingReplay?.Invoke();
    }
#endregion

    void Awake()
    {
        energyCountText.text = numEnergy.ToString();
        progressBarParentTransform = progressBarTransform.parent.GetComponent<RectTransform>();
        StartCoroutine(nameof(CoroutineUpdate));
    }

    private IEnumerator CoroutineUpdate()
    {
        runningCoroutine?.Invoke();
        progressText.text = progress.ToString("P0");
        progressBarTransform.sizeDelta = new Vector2(
            Math.Max(progressBarParentTransform.sizeDelta.x * progress, LEAST_PROGRESS_BAR_WIDTH),
            progressBarTransform.sizeDelta.y
        );
        yield return null;
    }
}
