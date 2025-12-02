using System;
using System.Threading.Tasks;

public class UnlockSlotUIController : StaticAdProcessor<UnlockSlotUIController, Row>
{
    public static Row currentRow;

#region 按钮回调委托
    public static Action clickingClose;
#endregion

#region 按钮回调方法
    public void OnClickClose() => clickingClose?.Invoke();
    public async void OnClickUnlock() => await (clickingWatchAd is null ? Task.CompletedTask : clickingWatchAd(currentRow));
#endregion
}
