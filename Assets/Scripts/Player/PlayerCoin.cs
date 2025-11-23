using UnityEngine;

public class PlayerCoin
{
    private const string Key_Coin = "Coin";

    private static int Coin
    {
        get => PlayerPrefs.GetInt(Key_Coin, 0);
        set
        {
            PlayerPrefs.SetInt(Key_Coin, value);
            PlayerPrefs.Save();
        }
    }

    public static int GetCoin() => Coin;
    public static void SetCoin(int count) => Coin = count;
    public static void AddCoin(int count) => Coin += count;
    public static bool TrySpendCoin(int count)
    {
        if (Coin < count)
            return false;
        Coin -= count;
        return true;
    }
}
