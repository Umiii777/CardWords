using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot 
{
    public Stack<Card> slotDatas;
    public Slot()
    {
        slotDatas = new Stack<Card>();
    }
}
