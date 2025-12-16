using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using TreeEditor;

public class StepManager : MonoBehaviour
{
    public int maxSteps;
    public int currentSteps;
    private int dangerSteps = 5;   //危险步数变色
    private int extraSteps = 10;    //复活ad增加的步数
    private Vector3 shakeEffectScale = new Vector3(0.6f, 0.6f, 1);
    private Vector3 orginalScale = new Vector3(1, 1, 1);

    private TextMeshProUGUI tmPro;
    private RectTransform tmTransform;

    public ObjectEventSO onLevelDefeat;

    public static StepManager Instance;

    private void Awake()
    {
        tmPro = GetComponent<TextMeshProUGUI>();
        tmTransform = GetComponent<RectTransform>();
        Instance = this;
    }

    public void InitSetSteps(int currentLevelStep)
    {
        maxSteps = currentLevelStep;
        currentSteps = maxSteps;
        tmPro.text = currentSteps.ToString();
        tmPro.color = Color.white;
    }

    public void StepsChange(Object obj)
    {
        OnStepChangeVisualEffect();
        currentSteps--;
        tmPro.text = currentSteps.ToString();

        //当变化为0的时候，步数耗尽方法
        if (currentSteps == dangerSteps)
        {
            tmPro.color = Color.red;
        }

        if (currentSteps <= 0)
        {
            currentSteps = 0;
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

        onLevelDefeat.RaiseEvent(this, this);
    }
    //步数增加的方法
    public void AddExtraSteps()
    {
        currentSteps += extraSteps;
        tmPro.text = currentSteps.ToString();
        tmPro.color = Color.white;
    }

    public void OnStepChangeVisualEffect()
    {
        tmTransform.DOScale(shakeEffectScale, 0.1f).onComplete = () =>
        {
            tmTransform.DOScale(orginalScale, 0.1f);
        };
    }
}
