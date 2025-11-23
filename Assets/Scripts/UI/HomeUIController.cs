using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// 大厅界面控制器
/// <br/><br/>
/// 为该类的静态属性或字段赋值以更新各个组件的显示内容及行为
/// </summary>
public class HomeUIController : MonoBehaviour
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
            }
            numEnergy = value;
        }
    }
    private static int numEnergy = 0;
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
                sharedMaxEnergyTips.SetActive(value <= numEnergy);
            maxEnergy = value;
        }
    }
    private static int maxEnergy = 999;
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
    /// 进入关卡时要扣除的体力值（请代入负数）
    /// </summary>
    public static int NumEnergyForPlay
    {
        get => numEnergyForPlay;
        set
        {
            if (sharedEnergyForPlayText != null)
                sharedEnergyForPlayText.text = value.ToString();
            numEnergyForPlay = value;
        }
    }
    private static int numEnergyForPlay = -1;
    private static TextMeshProUGUI sharedEnergyForPlayText;
    [SerializeField]
    private TextMeshProUGUI energyForPlayText;
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

#region 静态委托
    /// <summary>
    /// 设置按钮回调
    /// </summary>
    public static UnityAction clickingSettings;
    /// <summary>
    /// 补充体力按钮回调
    /// </summary>
    public static UnityAction clickingAddEnergy;
    /// <summary>
    /// 商店按钮回调
    /// </summary>
    public static UnityAction clickingShop;
    /// <summary>
    /// 进入关卡按钮回调
    /// </summary>
    public static UnityAction clickingStart;
    /// <summary>
    /// 该委托在 HomeUIController 的协程中每帧执行一次
    /// </summary>
    public static UnityAction runningCoroutine;
#endregion

#region 按钮回调方法
    public void OnClickSettings() => clickingSettings?.Invoke();
    public void OnClickAddEnergy() => clickingAddEnergy?.Invoke();
    public void OnClickStart() => clickingStart?.Invoke();
    public void OnClickShop()
    {
        clickingSettings?.Invoke();
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
        sharedEnergyForPlayText = energyForPlayText;
        sharedShopHint = shopHint;

        sharedCoinCountText.text = numCoins.ToString();
        sharedEnergyCountText.text = numEnergy.ToString();
        sharedMaxEnergyTips.SetActive(numEnergy >= maxEnergy);
        sharedLevelNameText.text = levelName;
        sharedEnergyForPlayText.text = numEnergyForPlay.ToString();
        sharedShopHint.SetActive(isShopHintShown);
    }
}
