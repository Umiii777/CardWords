using System;
using UnityEngine;
using TMPro;

/// <summary>
/// 设置界面控制器
/// </summary>
class  SettingsUIController : MonoBehaviour
{
#region 关卡内UI
    public static GameObject sharedInLevelUI;
    [SerializeField]
    private GameObject inLevelUI;
#endregion

#region 关卡外UI
    public static GameObject sharedOutLevelUI;
    [SerializeField]
    private GameObject outLevelUI;
#endregion

#region 按钮回调委托
    public static Action clickingContinue;
    public static Action clickingHome;
    public static Action clickingReplay;
#endregion

    [SerializeField]
    private string homeConfirmMessage;
    [SerializeField]
    private string replayConfirmMessage;
    [SerializeField]
    private GameObject confirmUI;
    [SerializeField]
    private string confirmTextName;
    private TextMeshProUGUI confirmText;
    private Action executing;

#region 按钮回调方法
    public void OnClickContinue() => clickingContinue?.Invoke();
    public void OnClickHome()
    {
        confirmText.text = homeConfirmMessage;
        confirmUI.SetActive(true);
        executing = clickingHome;
    }
    public void OnClickReplay()
    {
        confirmText.text = replayConfirmMessage;
        confirmUI.SetActive(true);
        executing = clickingReplay)
    }
    public void OnClickConfirm()
    {
        executing?.Invoke();
    }
    public void OnClickCancel()
    {
        executing = null;
        Destroy(confirmUI);
    }
#endregion

    void Start()
    {
        InitSharedFields();
        confirmText = confirmUI.GetComponentsInChildren<TextMeshProUGUI>(true).First(t => t.name == confirmTextName);
    }

    private void InitSharedFields()
    {
        sharedInLevelUI = inLevelUI;
        sharedOutLevelUI = outLevelUI;

        sharedInLevelUI.SetActive(false);
        sharedOutLevelUI.SetActive(false);
    }
}