using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    public List<Slot> slots;

    public Card onDragCard;
    public Card endDragCard;



    public void InitSlots(int[] rows, Card[] onSlotCards)
    {
        slots = new List<Slot>();

        int cardIndex = 0;
        Debug.Log(onSlotCards.Length);

        for (int i = 0; i < rows.Count(); i++)
        {
            slots.Add(new Slot());

            Debug.Log($"第{i + 1}的slots有：");

            for (int j = 0; j < rows[i]; j++)
            {

                slots[i].slotDatas.Push(onSlotCards[cardIndex]);
                Debug.Log(slots[i].slotDatas.Peek().cardData.cardContent);
                cardIndex++;
            }
        }
    }


    public void OnSuccessDragCard()
    {
        Debug.Log("正在执行OnSuccessDragCard");
        if (onDragCard.isFromDeck)
        {
            return;
        }
        else
        {
            int startSlot = onDragCard.slotCount;
            if (slots[startSlot].slotDatas.Count <= 0)
            {
                return;
            }
            else
            {
                slots[startSlot].slotDatas.Pop();
            }

            if (slots[startSlot].slotDatas != null && slots[startSlot].slotDatas.Count > 0)
            {
                SetCardsFront(slots[startSlot].slotDatas.Peek());
            }
            else
            {
                return;
            }
        }


    }

    public void SetDragCard(Card card)
    {
        Debug.Log("正在执行SetDragCard");
        onDragCard = card;
    }
    public void setEndDragCard(Card card)
    {
        Debug.Log("正在执行setEndDragCard");
        endDragCard = card;
    }
    public void SetCardsFront(Card card)
    {
        card.SetFront();
    }
}