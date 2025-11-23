using System;
using UnityEngine;

public class PlayerEnergy
{
    private const string Key_Energy = "Energy";
    private const string Key_MaxEnergy = "MaxEnergy";

    public static int MaxEnergy
    {
        get => PlayerPrefs.GetInt(Key_MaxEnergy, 999);
        set
        {
            PlayerPrefs.SetInt(Key_MaxEnergy, value);
            PlayerPrefs.Save();
        }
    }
    private static int Energy
    {
        get => PlayerPrefs.GetInt(Key_Energy, 0);
        set
        {
            PlayerPrefs.SetInt(Key_Energy, Math.Min(value, MaxEnergy));
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
        int e = Energy + count;
        Energy += count;
        return Energy < e;
    }
    public static bool TrySpendEnergy(int count)
    {
        if (Energy < count)
            return false;
        Energy -= count;
        return true;
    }
}
