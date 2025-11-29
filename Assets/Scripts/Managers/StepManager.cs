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

    public void StepMinusOne()
    {
        if (currentSteps <= 0)
        {
            Debug.Log("步骤用完，关卡失败");
            //调用关卡失败的事件
            StepsCheck();
        }
        else
        {
            currentSteps -= 1;
            tmPro.text = currentSteps.ToString();
        }

    }
    public void StepsCheck()
    {
        //调用关卡失败的事件
        return;

    }

    public void TestSetSteps()
    {
        maxSteps = 999;
        currentSteps = maxSteps;
        tmPro.text = currentSteps.ToString();
    }

}
