using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseEventListener<T> : MonoBehaviour
{
    public BaseEventSO<T> eventSO;

    public UnityEvent<T> response;
    

    private void OnEnable()//添加事件
    {
        if (eventSO != null)
        {
            eventSO.onEventRaised += OnEventRaised;
        }

    }
    private void OnDisable()//删除事件
    {
        if (eventSO != null)
        {
            eventSO.onEventRaised -= OnEventRaised;
        }
    }

    private void OnEventRaised(T value)
    {
        response.Invoke(value);
    }
}
