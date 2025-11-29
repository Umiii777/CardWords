using System;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

/// <summary>
/// 设置界面控制器
/// </summary>
class  SettingsUIController : MonoBehaviour
{
#region 进关卡时扣除的体力值
    public static int numEnergyToPlay = 1;
    [SerializeField]
    private TextMeshProUGUI energyCountText;
#endregion

    /// <summary>
    /// 界面是否在关卡内打开
    /// </summary>
    public static bool isInLevel = false;
    [SerializeField]
    private GameObject inLevelUI;
    [SerializeField]
    private GameObject outLevelUI;
    [SerializeField]
    private GameObject confirmUI;


#region 按钮回调委托
    public static Action clickingClose;
    public static Func<Task> clickingReplay;
    public static Func<Task> clickingConfirm;
#endregion

#region 按钮回调方法
    public void OnClickClose() => clickingClose?.Invoke();
    public void OnClickHome()
    {
        energyCountText.text = numEnergyToPlay.ToString();
        confirmUI.SetActive(true);
    }
    public async void OnClickReplay() => await (clickingReplay is null ? Task.CompletedTask : clickingReplay());
    public async void OnClickConfirm() => await (clickingConfirm is null ? Task.CompletedTask : clickingConfirm());
    public void OnClickCancel() => confirmUI.SetActive(false);
#endregion

    void Start()
    {
        inLevelUI.SetActive(isInLevel);
        outLevelUI.SetActive(!isInLevel);
    }
}