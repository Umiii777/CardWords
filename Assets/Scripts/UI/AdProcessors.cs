using System;
using System.Threading.Tasks;
using UnityEngine;

public abstract class AdProcessor<TSelf> : MonoBehaviour where TSelf : AdProcessor<TSelf>
{
#region 按钮回调委托
    /// <summary>
    /// 关闭界面按钮回调
    /// </summary>
    public Action clickingClose;
    /// <summary>
    /// 观看广告按钮回调
    /// </summary>
    public Func<Task> clickingWatchAd;
#endregion

    protected static bool isWatchingAd;

#region 按钮回调方法
    public void OnClickClose() => ProcessClicking(clickingClose);
    public async void OnClickReceive()
    {
        isWatchingAd = true;
        await (clickingWatchAd is null ? Task.CompletedTask : clickingWatchAd());
        isWatchingAd = false;
    }
#endregion

    protected static void ProcessClicking(Action clickingAction)
    {
        if (!isWatchingAd)
            clickingAction?.Invoke();
    }
    protected static async Task ProcessClicking(Func<Task> clickingFunc)
    {
        if (!isWatchingAd)
            await (clickingFunc is null ? Task.CompletedTask : clickingFunc());
    }
    protected static async Task ProcessClicking<T>(Func<T, Task> clickingFunc, T clickingArg)
    {
        if (!isWatchingAd)
            await (clickingFunc is null ? Task.CompletedTask : clickingFunc(clickingArg));
    }

    /*TODO: 002_移除所有 FireAndBan 方法，重新编写并启用 ProcessAd 方法
    protected static async Task ProcessAd<T>(T toWait, Action afterWait = null, params object[] toWaitArgs) where T : Delegate =>
        await StaticAdProcessor<MonoBehaviour, object>.ProcessAd(toWait, afterWait, toWaitArgs);
    */
}

public abstract class StaticAdProcessor<TSelf, T> : AdProcessor<TSelf> where TSelf : StaticAdProcessor<TSelf, T>
{
#region 按钮回调委托
    /// <summary>
    /// 关闭界面按钮回调
    /// </summary>
    public static new Action clickingClose;
    /// <summary>
    /// 观看广告按钮回调
    /// </summary>
    public static new Func<T, Task> clickingWatchAd;
#endregion

#region 按钮回调方法
    public new void OnClickClose() => ProcessClicking(clickingClose);
    public new async void OnClickReceive()
    {
        isWatchingAd = true;
        await (clickingWatchAd is null ? Task.CompletedTask : clickingWatchAd(default));
        isWatchingAd = false;
    }
    public async void OnClickReceive(T arg)
    {
        isWatchingAd = true;
        await (clickingWatchAd is null ? Task.CompletedTask : clickingWatchAd(arg));
        isWatchingAd = false;
    }
#endregion

    /*TODO: 002_移除所有 FireAndBan 方法，重新编写并启用 ProcessAd 方法
    protected static async Task ProcessAd<T>(T toWait, Action afterWait = null, params object[] toWaitArgs) where T : Delegate
    {
        if (toWait is not null)
            await Task.WhenAll(toWait.GetInvocationList()
                .Select(d =>
                    d.Method.ReturnType == typeof(Task)
                    ? d.Method.Invoke(d.Target, toWaitArgs) as Task
                    : Task.FromException(new ArgumentException($"{nameof(StaticAdProcessor<Tag, U>)}.{nameof(ProcessAd)} 的第一个参数只能是 Func<..., Task> 类型"))
                )
            );
        afterWait?.Invoke();
    }
    */
}
