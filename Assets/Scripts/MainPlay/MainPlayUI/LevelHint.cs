using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelHint : MonoBehaviour
{
    public static LevelHint Instance;
    public TextMeshProUGUI levelHint;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        levelHint = GetComponent<TextMeshProUGUI>();
    }

    public void SetLevelHint(int levelNum)
    {
        levelHint.text = "关卡" + " " + (levelNum - 100);
    }
}
