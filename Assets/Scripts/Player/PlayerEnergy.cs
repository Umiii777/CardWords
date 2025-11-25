using System;
using System.Threading;
using UnityEngine;

public class PlayerEnergy
{
#region 常量
    private const int SECONDS_TO_RECOVER = 300;
    private const string Key_Energy = "Energy";
    private const string Key_MaxEnergy = "MaxEnergy";
#endregion

#region 体力自动回复计时
    public static int secondsToRecover = SECONDS_TO_RECOVER - 1;
    public static Thread timer;
    public static Action timing;
    private static SynchronizationContext unityContext;
#endregion

    public static int MaxEnergy
    {
        get => PlayerPrefs.GetInt(Key_MaxEnergy, 5);
        set
        {
            PlayerPrefs.SetInt(Key_MaxEnergy, value);
            PlayerPrefs.Save();
        }
    }
    private static int Energy
    {
        get => PlayerPrefs.GetInt(Key_Energy, 10);
        set
        {
            PlayerPrefs.SetInt(Key_Energy, value);
            PlayerPrefs.Save();
        }
    }

    public static int GetEnergy() => Energy;
    public static void SetEnergy(int count) => Energy = count;
    /// <summary>
    /// 回复体力值
    /// </summary>
    /// <returns>本次回复是否造成了体力值溢出</returns>
    public static bool AddEnergy(int count)
    {
        if (Energy >= MaxEnergy)
            return false;
        int e = Energy + count;
        Energy = Math.Min(e, MaxEnergy);
        return Energy < e;
    }
    public static bool TryAddEnergy(int count)
    {
        if (Energy + count > MaxEnergy)
            return false;
        Energy += count;
        return true;
    }
    public static bool TrySpendEnergy(int count)
    {
        if (Energy < count)
            return false;
        Energy -= count;
        return true;
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void StartTimer()
    {
        unityContext = SynchronizationContext.Current;
        timer = new Thread(() =>
        {
            while (true)
            {
                Thread.Sleep(1000);
                secondsToRecover = (secondsToRecover - 1 + SECONDS_TO_RECOVER) % SECONDS_TO_RECOVER;
                unityContext.Post(_ =>
                {
                    if (secondsToRecover == 0)
                        AddEnergy(1);
                    timing?.Invoke();
                }, null);
            }
        });
        timer.IsBackground = true;
        timer.Start();
    }
}
