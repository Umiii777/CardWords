using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public abstract class StaticAdProcessor<Tag, U> : MonoBehaviour where Tag : MonoBehaviour
{
    /// <summary>
    /// 看广告领东西按钮回调
    /// </summary>
    public static Func<U, Task> clickingWatchAd;

    public static async void ProcessAd<T>(T toWait, Action afterWait = null, params object[] toWaitArgs) where T : Delegate
    {
        if (toWait is not null)
            await Task.WhenAll(toWait.GetInvocationList()
                .Select(d =>
                    d.Method.ReturnType == typeof(Task)
                    ? d.Method.Invoke(d.Target, toWaitArgs) as Task
                    : Task.FromException(new ArgumentException($"nameof(StaticAdProcessor<Tag, U>).nameof(ProcessAd) 的第一个参数只能是 Func<..., Task> 类型"))
                )
            );
        afterWait?.Invoke();
    }
}

public abstract class AdProcessor : MonoBehaviour
{
    /// <summary>
    /// 看广告领东西按钮回调
    /// </summary>
    public Func<Task> clickingWatchAd;

    public static async void ProcessAd<T>(T toWait, Action afterWait = null, params object[] toWaitArgs) where T : Delegate =>
        StaticAdProcessor<MonoBehaviour, object>.ProcessAd(toWait, afterWait, toWaitArgs);
}