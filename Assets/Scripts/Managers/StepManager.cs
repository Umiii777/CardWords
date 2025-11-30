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

    public void StepsChange(Object obj)
    {
        currentSteps--;
        tmPro.text = currentSteps.ToString();

        //当变化为0的时候，步数耗尽方法
        if (currentSteps <= 0)
        {
            StepsRunningOut();
        }
    }


    public void TestSetSteps()
    {
        maxSteps = 999;
        currentSteps = maxSteps;
        tmPro.text = currentSteps.ToString();
    }
    //步数耗尽的方法
    public void StepsRunningOut()
    {

    }
}
