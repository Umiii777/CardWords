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
    public static void SetWatchedAd(int count) => WatchedAd = count;
    public static void AddWatchedAdd(int count = 1) => WatchedAd += count;
    public static void ResetAd()
    {
        PlayerPrefs.DeleteKey(Key_WatchedAd);
        PlayerPrefs.Save();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetAdWhenQuitGame() => Application.quitting += ResetAd;
}
