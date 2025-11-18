using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseEventSO<T> : ScriptableObject
{
    public string description;

    public UnityAction<T> onEventRaised;    //查看unity开发文档：unityevent

    public string lastSender;

    public void RaiseEvent(T value, object sender)
    {
        onEventRaised?.Invoke(value); //避免空引用导致异常

        lastSender = sender.ToString();
    }
    
 
}
