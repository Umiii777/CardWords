using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAd
{
    private const string Key_WatchedAd = "WatchedAd";

    private static int WatchedAd
    {
        get => PlayerPrefs.GetInt(Key_WatchedAd, 0);
        set
        {
            PlayerPrefs.SetInt(Key_WatchedAd, value);
            PlayerPrefs.Save();
        }
    }

    public static int GetWatchedAd() => WatchedAd;
    public static void IncreaseWatchedAd() => WatchedAd++;
    public static void ResetWatchedAd() => WatchedAd = 0;
    public static async Task ProcessAd<T>(T toWait, Action afterWait = null, params object[] toWaitArgs) where T : Delegate
    {
        if (toWait is not null)
            await Task.WhenAll(toWait.GetInvocationList()
                .Select(d =>
                    d.Method.ReturnType == typeof(Task)
                    ? d.Method.Invoke(d.Target, toWaitArgs) as Task
                    : Task.FromException(new Exception("PlayerAd.ProcessAd的第一个参数只能是 Func<..., Task> 类型！"))
                )
            );
        afterWait?.Invoke();
    }
}
