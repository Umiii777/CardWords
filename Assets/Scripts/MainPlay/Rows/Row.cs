using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Row : MonoBehaviour, IDropHandler
{
    public RowType rowType;
    public bool isEmpty = true;
    public int cardCount = 0;


    public RectTransform rectTransform;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        CardData currentCardData = eventData.pointerDrag.GetComponent<Card>().cardData;
        Debug.Log("row上面有" + currentCardData.cardContent);
        // if (isEmpty)
        // {
        //     if (currentCardData.isMainCard && rowType == RowType.main)
        //     {

        //     }
        // }

        //if(currentCard.cardData.)
    }
    public void UpdateRowState()
    {
        SetRowEmpty();
    }
    public void SetRowEmpty()
    {
        if (rowType == RowType.normal)
        {
            if (cardCount <= 0)
                isEmpty = true;
            else
                isEmpty = false;
        }
        else if (rowType == RowType.main)
        {
            isEmpty =  true;
            return;
        }
        else
        {
            return;
        }
    }
    public void DragOncard(Card card)
    {

    }
    private void OnEnable()
    {
        CardManager.Instance.allRows.Add(this);
    }
    private void OnDisable()
    {
        CardManager.Instance.allRows.Remove(this);
    }

    public void OnChangeUnlockRowType()
    {
        if (rowType == RowType.unlocked)
        {
            rowType = RowType.main;
        }
    }
}
