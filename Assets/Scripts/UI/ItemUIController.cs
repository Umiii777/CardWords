using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 道具获取界面控制器
/// </summary>
public class ItemUIController : AdProcessor<ItemUIController>, IAudioTrigger
{
    public static Dictionary<
        ItemType,
        ValueTuple<Func<int, Task>, Func<Task>>
    > clickingsCache = new();

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
    public Func<int, Task> clickingBuy;
#endregion

    public ItemType itemType;
    public int price = 150;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;

    [SerializeField]
    private TextMeshProUGUI priceText;

#region 按钮回调方法
    public void OnClickClose() => ProcessClicking(clickingClose);
    public async void OnClickReceive() => await ProcessClicking(clickingWatchAd);
    public async void OcClickBuy() => await ProcessClicking(clickingBuy, price);
#endregion

    void Start()
    {
        priceText.text = price.ToString();
    }

    public void PlayClickingAudio() => (this as IAudioTrigger).PlayClickingAudio(default);
}
