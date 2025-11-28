using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
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
    public static float progress = 1f / 100;
    /// <summary>
    /// 要显示在重玩按钮上的体力值（请代入负数）
    /// </summary>
    public static int numEnergy = -1;
    /// <summary>
    /// 返回主界面按钮回调
    /// <br/><br/>
    /// 应将 Destroy(defeatUI.gameObject) 加到最后
    /// </summary>
    public static Action clickingHome;
    /// <summary>
    /// 继续游戏按钮回调
    /// <br/><br/>
    /// 应将 Destroy(defeatUI.gameObject) 加到最后
    /// </summary>
    public static Func<Task> clickingContinue;
    /// <summary>
    /// 重玩按钮回调
    /// <br/><br/>
    /// 应将 Destroy(defeatUI.gameObject) 加到最后
    /// </summary>
    public static Action clickingReplay;
    /// <summary>
    /// 该委托在 DefeatUIController 的协程中每帧执行一次
    /// <br/><br/>
    /// 在这里使用 DefeatUIController.progress += ... 可以控制进度条填充速度
    /// </summary>
    public static Action runningCoroutine;
#endregion

#region 私有字段
    /// <summary>
    /// 游戏完成进度条每帧默认填充量
    /// </summary>
    private const float PROGRESS_FILLING_DURATION = 5f;
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
    public void OnClickHome() => clickingHome?.Invoke();
    public void OnClickReplay() => clickingReplay?.Invoke();
    public void OnClickContinue() => AdProcesser.ProcessAd(clickingContinue);
#endregion

    void Start()
    {
        energyCountText.text = numEnergy.ToString();
        progressBarParentTransform = progressBarTransform.parent.GetComponent<RectTransform>();
        StartCoroutine(nameof(CoroutineUpdate));
    }

    private IEnumerator CoroutineUpdate()
    {
        float startProgress = progress, filledProgress = 0f;

        runningCoroutine?.Invoke();
        Action updateProgress = progress.Equals(startProgress)
            ? () =>
            {
                DisplayProgress(filledProgress);
                filledProgress = Math.Min(
                    progress,
                    filledProgress + Time.deltaTime / PROGRESS_FILLING_DURATION / progress
                );
            }
            : () => DisplayProgress(progress);

        while (true)
        {
            updateProgress();
            yield return null;

            runningCoroutine?.Invoke();
        }
    }

    private void DisplayProgress(float p)
    {
        progressText.text = p.ToString("P0");
        progressBarTransform.sizeDelta = new(
            Math.Max(progressBarParentTransform.sizeDelta.x * p, LEAST_PROGRESS_BAR_WIDTH),
            progressBarTransform.sizeDelta.y
        );
    }
}
