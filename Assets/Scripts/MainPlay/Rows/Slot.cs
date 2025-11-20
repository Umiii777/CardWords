using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot 
{
    public Stack<CardData> slotDatas;
    public Slot()
    {
        slotDatas = new Stack<CardData>();
    }
}
