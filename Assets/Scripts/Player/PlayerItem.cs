using System;
using UnityEngine;

public enum ItemType
{
    /// <summary>
    /// 提示
    /// </summary>
    Hint = 1,
    /// <summary>
    /// 洗牌
    /// </summary>
    Shuffle
}
public struct Prop
{
    public Func<int> Get;
    public Action<int> Set;
}

public class PlayerItem
{
    private const string Key_Hint = "ItemHint";
    private const string Key_Shuffle = "ItemShuffle";

    private static int Hint
    {
        get => PlayerPrefs.GetInt(Key_Hint, 0);
        set
        {
            PlayerPrefs.SetInt(Key_Hint, value);
            PlayerPrefs.Save();
        }
    }
    private static int Shuffle
    {
        get => PlayerPrefs.GetInt(Key_Shuffle, 0);
        set
        {
            PlayerPrefs.SetInt(Key_Shuffle, value);
            PlayerPrefs.Save();
        }
    }

    private static readonly Prop[] props =
    {
        new() { Get = () => Hint, Set = v => Hint = v },
        new() { Get = () => Shuffle, Set = v => Shuffle = v }
    };

    public static int GetItem(ItemType type) => props[(uint)type - 1].Get();
    public static void SetItem(ItemType type, int count) => props[(uint)type - 1].Set(count);
    public static void AddItem(ItemType type, int count) => props[(uint)type - 1].Set(GetItem(type) + count);
    public static bool TrySpendItem(ItemType type, int count = 1)
    {
        int item = GetItem(type);
        if (item < count)
            return false;
        SetItem(type, item - count);
        return true;
    }
    public static void ResetItem()
    {
        PlayerPrefs.DeleteKey(Key_Hint);
        PlayerPrefs.DeleteKey(Key_Shuffle);
        PlayerPrefs.Save();
    }
}
