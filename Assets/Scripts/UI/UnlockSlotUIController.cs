using System;
using System.Threading.Tasks;

public class UnlockSlotUIController : StaticAdProcessor<UnlockSlotUIController, Row>, IAudioTrigger
{
    public static Row currentRow;

#region 按钮回调委托
    public static Action clickingClose;
#endregion

#region 按钮回调方法
    public void OnClickClose() => ProcessClicking(clickingClose);
    public async void OnClickUnlock()
    {
        isWatchingAd = true;
        await (clickingWatchAd is null ? Task.CompletedTask : clickingWatchAd(currentRow));
        isWatchingAd = false;
    }
#endregion

    public void PlayClickingAudio() => (this as IAudioTrigger).PlayClickingAudio(default);
}
