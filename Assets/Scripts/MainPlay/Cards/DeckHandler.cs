using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckHandler : MonoBehaviour, IPointerClickHandler
{
    //防止连续点击
    public float clickInterval = 0.2f;
    private float lastClickTime = 0f;

    public ObjectEventSO DrawACardFromDeck;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.unscaledTime - lastClickTime < clickInterval)
        {
            // 在限制时间内，忽略点击
            return;
        }

        //重置点击时间
        lastClickTime = Time.unscaledTime;


        Debug.Log("IsClickingDeck");
        DeckManager.Instance.DrawCard();

        DrawACardFromDeck.RaiseEvent(this, this);
    }
}
