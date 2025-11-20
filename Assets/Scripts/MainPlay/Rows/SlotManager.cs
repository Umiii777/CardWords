using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    public List<Slot> slots;


    public void InitSlots(int[] rows, CardData[] onSlotCards)
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
                Debug.Log(slots[i].slotDatas.Peek().cardContent);
                cardIndex++;
            }
        }
    }
}
