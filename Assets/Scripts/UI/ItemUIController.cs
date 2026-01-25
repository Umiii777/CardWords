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
        (Func<int, Task<bool>> ClickingBuy, Func<Task> ClickingWatchAd)
    > clickingsCache = new();

    /// <summary>
    /// 购买道具按钮回调
    /// </summary>
    public Func<int, Task> clickingBuy;
    public ItemType itemType;
    public int price = 150;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;

    [SerializeField]
    private TextMeshProUGUI priceText;

    public async void OcClickBuy() => await ProcessClicking(clickingBuy, price);

    void Start() => priceText.text = price.ToString();

    public void PlayClickingAudio() => (this as IAudioTrigger).PlayClickingAudio(default);
}
