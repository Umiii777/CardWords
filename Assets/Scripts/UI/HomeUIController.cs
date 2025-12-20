using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

/// <summary>
/// 大厅界面控制器
/// <br/><br/>
/// 为该类的静态属性或字段赋值以更新各个组件的显示内容及行为
/// </summary>
public class HomeUIController : MonoBehaviour, IAudioTrigger
{
#region 玩家金币数量
    /// <summary>
    /// 玩家拥有的金币数量
    /// </summary>
    public static int NumCoins
    {
        get => numCoins;
        set
        {
            if (sharedCoinCountText != null)
                sharedCoinCountText.text = value.ToString();
            numCoins = value;
        }
    }
    private static int numCoins = 0;
    private static TextMeshProUGUI sharedCoinCountText;
    [SerializeField]
    private TextMeshProUGUI coinCountText;
#endregion

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
            {
                sharedEnergyCountText.text = value.ToString();
                sharedMaxEnergyTips.SetActive(value >= maxEnergy);
                sharedAddEnergyButton.SetActive(value < maxEnergy);
            }
            numEnergy = value;
        }
    }
    private static int numEnergy;
    private static TextMeshProUGUI sharedEnergyCountText;
    [SerializeField]
    private TextMeshProUGUI energyCountText;
#endregion

#region 体力值上限
    /// <summary>
    /// 玩家体力值上限
    /// </summary>
    public static int MaxEnergy
    {
        get => maxEnergy;
        set
        {
            if (sharedMaxEnergyTips != null)
            {
                sharedMaxEnergyTips.SetActive(value <= numEnergy);
                sharedAddEnergyButton.SetActive(value > numEnergy);
            }
            maxEnergy = value;
        }
    }
    private static int maxEnergy;
    private static GameObject sharedMaxEnergyTips;
    [SerializeField]
    private GameObject maxEnergyTips;
#endregion

#region 关卡名称
    /// <summary>
    /// 要显示在进入关卡按钮上的关卡名称
    /// </summary>
    public static string LevelName
    {
        get => levelName;
        set
        {
            if (sharedLevelNameText != null)
                sharedLevelNameText.text = value.ToString();
            levelName = value;
        }
    }
    private static string levelName = "关卡1";
    private static TextMeshProUGUI sharedLevelNameText;
    [SerializeField]
    private TextMeshProUGUI levelNameText;
#endregion

#region 进关卡扣除的体力值
    /// <summary>
    /// 进入关卡时要扣除的体力值
    /// </summary>
    public static int NumEnergyToPlay
    {
        get => numEnergyToPlay;
        set
        {
            if (sharedEnergyToPlayText != null)
                sharedEnergyToPlayText.text = (-value).ToString();
            numEnergyToPlay = value;
        }
    }
    private static int numEnergyToPlay = 1;
    private static TextMeshProUGUI sharedEnergyToPlayText;
    [SerializeField]
    private TextMeshProUGUI energyToPlayText;
#endregion

#region 补充体力按钮
    private static GameObject sharedAddEnergyButton;
    [SerializeField]
    private GameObject addEnergyButton;
#endregion

#region 商店按钮右上角红点
    public static bool IsShopHintShown
    {
        get => isShopHintShown;
        set
        {
            if (sharedShopHint != null)
                sharedShopHint.SetActive(value);
            isShopHintShown = value;
        }
    }
    private static bool isShopHintShown = true;
    private static GameObject sharedShopHint;
    [SerializeField]
    private GameObject shopHint;
#endregion

#region 按钮回调及协程内委托
    /// <summary>
    /// 设置按钮回调
    /// </summary>
    public static Func<Task> clickingSettings;
    /// <summary>
    /// 补充体力按钮回调
    /// </summary>
    public static Func<Task> clickingAddEnergy;
    /// <summary>
    /// 商店按钮回调
    /// </summary>
    public static Func<Task> clickingShop;
    /// <summary>
    /// 进入关卡按钮回调
    /// </summary>
    public static Func<Task> clickingStart;
    /// <summary>
    /// 该委托在 HomeUIController 的协程中每帧执行一次
    /// </summary>
    public static Action runningCoroutine;
#endregion

#region 按钮回调方法
    public async void OnClickSettings() => await (clickingSettings is null ? Task.CompletedTask : clickingSettings());
    public async void OnClickAddEnergy() => await (clickingAddEnergy is null ? Task.CompletedTask : clickingAddEnergy());
    public async void OnClickStart() => await(clickingStart is null ? Task.CompletedTask : clickingStart());
    public async void OnClickShop()
    {
        await (clickingShop is null ? Task.CompletedTask : clickingShop());
        IsShopHintShown = false;
    }
#endregion

    void Start()
    {
        InitSharedFields();
        StartCoroutine(nameof(CoroutineUpdate));
    }

    private IEnumerator CoroutineUpdate()
    {
        while (true)
        {
            runningCoroutine?.Invoke();
            yield return null;
        }
    }

    private void InitSharedFields()
    {
        sharedCoinCountText = coinCountText;
        sharedEnergyCountText = energyCountText;
        sharedMaxEnergyTips = maxEnergyTips;
        sharedLevelNameText = levelNameText;
        sharedEnergyToPlayText = energyToPlayText;
        sharedAddEnergyButton = addEnergyButton;
        sharedShopHint = shopHint;

        NumCoins = numCoins;
        NumEnergy = numEnergy;
        LevelName = levelName;
        NumEnergyToPlay = numEnergyToPlay;
        IsShopHintShown = isShopHintShown;
    }

    public void PlayClickingAudio() => (this as IAudioTrigger).PlayClickingAudio(default);
}
