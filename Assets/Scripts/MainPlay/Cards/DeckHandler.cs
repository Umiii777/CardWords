using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class DeckHandler : MonoBehaviour, IPointerClickHandler
{
    //防止连续点击
    public float clickInterval = 0.2f;
    private float lastClickTime = 0f;

    private Vector3 shakeEffectScale = new Vector3(0.8f, 0.8f, 1);
    private Vector3 orginalScale = new Vector3(1, 1, 1);

    public ObjectEventSO DrawACardFromDeck;
    private RectTransform rectTransform;
    public Image deckStyle;
    public TextMeshProUGUI tmPro;

    public void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        deckStyle = GetComponent<Image>();
        tmPro = GetComponentInChildren<TextMeshProUGUI>();

    }

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
        rectTransform.DOScale(shakeEffectScale, 0.1f).onComplete = () =>
        {
            rectTransform.DOScale(orginalScale, 0.1f);
        };


        DrawACardFromDeck.RaiseEvent(this, this);
    }
    public void DeckStyleReadyShuffle()
    {
        deckStyle.sprite = CardVisualManager.Instance.refreshDeck;
        tmPro.text = "刷新牌库";
    }
    public void DeckStyleNormal()
    {
        deckStyle.sprite = CardVisualManager.Instance.normalDeck;
        tmPro.text = "";

    }
    public void ShuffleAnim()
    {
        rectTransform.DOShakeAnchorPos(1f, 5f, 10, 90f, false, true, ShakeRandomnessMode.Harmonic);
    }
}
