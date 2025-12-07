using System;
using System.Threading.Tasks;
using UnityEngine;

public abstract class StaticAdProcessor<Tag, U> : MonoBehaviour where Tag : MonoBehaviour
{
    /// <summary>
    /// 看广告领东西按钮回调
    /// </summary>
    public static Func<U, Task> clickingWatchAd;

    protected static bool isWatchingAd;

    /* TODO: 重新评估 SystemUIManager.FireAndBan 的设计及其与 ProcessAd 的取舍
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

    public static void ProcessClicking(Action clickingAction)
    {
        if (!isWatchingAd)
            clickingAction?.Invoke();
    }
    public static async Task ProcessClicking(Func<Task> clickingFunc)
    {
        if (!isWatchingAd)
            await (clickingFunc is null ? Task.CompletedTask : clickingFunc());
    }
    public static async Task ProcessClicking<T>(Func<T, Task> clickingFunc, T clickingArg)
    {
        if (!isWatchingAd)
            await (clickingFunc is null ? Task.CompletedTask : clickingFunc(clickingArg));
    }
}

public abstract class AdProcessor<Tag> : MonoBehaviour where Tag : MonoBehaviour
{
    /// <summary>
    /// 看广告领东西按钮回调
    /// </summary>
    public Func<Task> clickingWatchAd;

    /*
    public static async Task ProcessAd<T>(T toWait, Action afterWait = null, params object[] toWaitArgs) where T : Delegate =>
        await StaticAdProcessor<MonoBehaviour, object>.ProcessAd(toWait, afterWait, toWaitArgs);
    */

    protected static void ProcessClicking(Action clickingAction) => StaticAdProcessor<Tag, object>.ProcessClicking(clickingAction);
    protected static async Task ProcessClicking(Func<Task> clickingFunc) => await StaticAdProcessor<Tag, object>.ProcessClicking(clickingFunc);
    protected static async Task ProcessClicking<T>(Func<T, Task> clickingFunc, T clickingArg) => await StaticAdProcessor<Tag, object>.ProcessClicking(clickingFunc, clickingArg);
}
