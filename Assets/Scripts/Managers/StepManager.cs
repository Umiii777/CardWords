using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    public int maxSteps;
    public int currentSteps;
    private TextMeshProUGUI tmPro;
    private void Awake()
    {
        tmPro = GetComponent<TextMeshProUGUI>();
    }

    public void InitSetSteps(int currentLevelStep)
    {
        maxSteps = currentLevelStep;
        currentSteps = maxSteps;
        tmPro.text = currentSteps.ToString();
    }

    public void StepsChange(int offsetStep)
    {
        currentSteps = offsetStep + currentSteps;
        tmPro.text = currentSteps.ToString();

        //当变化为0的时候，call关卡失败（其他弹窗）

    }

    
    public void TestSetSteps()
    {
        maxSteps = 999;
        currentSteps = maxSteps;
        tmPro.text = currentSteps.ToString();
    }

}
