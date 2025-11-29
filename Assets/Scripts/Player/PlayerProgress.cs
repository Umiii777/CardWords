using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class PlayerProgress : MonoBehaviour
{
    private const string Key_CurrentLevel = "CurrentLevel";

    // 获取当前已解锁的最大关卡ID（默认是1）
    public static int GetCurrentLevel()
    {
        return PlayerPrefs.GetInt(Key_CurrentLevel, 1);
    }

    // 更新玩家的最高关卡进度
    public static void SetCurrentLevel(int levelID)
    {
        int current = GetCurrentLevel();
        if (levelID > current)
        {
            PlayerPrefs.SetInt(Key_CurrentLevel, levelID);
            PlayerPrefs.Save();
            Debug.Log($"✅ 存档更新：玩家已解锁到第 {levelID} 关");
        }
    }

    // 清除进度（调试或重新开始用）
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(Key_CurrentLevel);
        PlayerPrefs.Save();
    }
}
