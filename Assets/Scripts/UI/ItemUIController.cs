using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class NonSingletonAdProcessor : MonoBehaviour
{
    /// <summary>
    /// 看广告领东西按钮回调
    /// </summary>
    public Func<Task> clickingWatchAd;
}

/// <summary>
/// 道具获取界面控制器
/// </summary>
public class ItemUIController : NonSingletonAdProcessor
{
#region 按钮回调委托
    /// <summary>
    /// 关闭界面按钮回调
    /// <br/><br/>
    /// 应将 Destroy(itemUI.gameObject) 加到最后
    /// </summary>
    public Action clickingClose;
    /// <summary>
    /// 购买道具按钮回调
    /// </summary>
    public Action<int> clickingBuy;
#endregion

    public ItemType itemType;
    public int price = 150;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;

    [SerializeField]
    private TextMeshProUGUI priceText;

#region 按钮回调方法
    public void OnClickClose() => clickingClose?.Invoke();
    public void OnClickReceive() => SystemUIManager.ProcessAd(clickingWatchAd);
    public void OcClickBuy() => clickingBuy?.Invoke(price);
#endregion

    void Start()
    {
        priceText.text = price.ToString();
    }
}
