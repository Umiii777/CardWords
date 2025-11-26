using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUIController : MonoBehaviour
{
#region 静态委托
    /// <summary>
    /// 关闭界面按钮回调
    /// <br/><br/>
    /// 应将 Destroy(shopUI.gameObject) 加到最后
    /// </summary>
    static public Action clickingClose;
    /// <summary>
    /// 购买道具按钮回调
    /// </summary>
    static public Action<int> clickingBuy;
    /// <summary>
    /// 领取道具按钮回调
    /// </summary>
    static public Func<Task> clickingReceive;
#endregion

    public int itemId;
    public int price = 150;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;

#region 按钮回调方法
    public void OnClickClose() => clickingClose?.Invoke();
    public void OnClickReceive() => SystemUIManager.ProcessAd(clickingReceive);
    public void OcClickBuy() => clickingBuy?.Invoke(price);
#endregion

    void Start()
    {
        priceText.text = price.ToString();
    }
}
