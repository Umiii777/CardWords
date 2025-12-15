using System;
using System.Threading.Tasks;
using UnityEngine;

public abstract class StaticAdProcessor<This, T> : MonoBehaviour where This : StaticAdProcessor<This, T>
{
    /// <summary>
    /// 看广告领东西按钮回调
    /// </summary>
    public static Func<T, Task> clickingWatchAd;

    protected static bool isWatchingAd;

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
    protected static async Task ProcessClicking<U>(Func<U, Task> clickingFunc, U clickingArg)
    {
        if (!isWatchingAd)
            await (clickingFunc is null ? Task.CompletedTask : clickingFunc(clickingArg));
    }

    /*TODO: 移除所有 FireAndBan 方法，重新启用 ProcessAd
    public static async Task ProcessAd<T>(T toWait, Action afterWait = null, params object[] toWaitArgs) where T : Delegate
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

public abstract class AdProcessor<This> : StaticAdProcessor<This, object> where This : AdProcessor<This>
{
    /// <summary>
    /// 看广告领东西按钮回调
    /// </summary>
    public new Func<Task> clickingWatchAd;

    /*TODO: 移除所有 FireAndBan 方法，重新启用 ProcessAd
    public static async Task ProcessAd<T>(T toWait, Action afterWait = null, params object[] toWaitArgs) where T : Delegate =>
        await StaticAdProcessor<MonoBehaviour, object>.ProcessAd(toWait, afterWait, toWaitArgs);
    */
}
