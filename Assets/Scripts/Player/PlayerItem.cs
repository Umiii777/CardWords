using System;
using UnityEngine;

public enum ItemType { Hint, Refresh }
struct Prop
{
    public Func<int> get;
    public Action<int> set;
}

public class PlayerItem
{
    private const string Key_Hint = "ItemHint";
    private const string Key_Refresh = "ItemRefresh";

    private static int Hint
    {
        get => PlayerPrefs.GetInt(Key_Hint, 0);
        set
        {
            PlayerPrefs.SetInt(Key_Hint, value);
            PlayerPrefs.Save();
        }
    }
    private static int Refresh
    {
        get => PlayerPrefs.GetInt(Key_Refresh, 0);
        set
        {
            PlayerPrefs.SetInt(Key_Refresh, value);
            PlayerPrefs.Save();
        }
    }

    private static Prop[] props =
    {
        new Prop { get = () => Hint, set = v => Hint = v },
        new Prop { get = () => Refresh, set = v => Refresh = v }
    };

    public static int GetItem(ItemType type) => props[(int)type].get();
    public static void SetItem(ItemType type, int count) => props[(int)type].set(count);
    public static void AddHint(ItemType type, int count) => props[(int)type].set(GetItem(type) + count);
    public static bool TrySpendItem(ItemType type, int count = 1)
    {
        int item = GetItem(type);
        if (item < count)
            return false;
        SetItem(type, item - count);
        return true;
    }
}
