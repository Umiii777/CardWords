using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Row : MonoBehaviour, IDropHandler
{
    public RowType rowType;
    public bool isEmpty;

    public void OnDrop(PointerEventData eventData)
    {
        CardData currentCardData = eventData.pointerDrag.GetComponent<Card>().cardData;
        Debug.Log(currentCardData.cardContent);
        // if (isEmpty)
        // {
        //     if (currentCardData.isMainCard && rowType == RowType.main)
        //     {
                
        //     }
        // }

        //if(currentCard.cardData.)
    }
}
