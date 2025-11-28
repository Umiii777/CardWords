using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStack
{
    private const float offsetY = 50f;

    // 添加 child 到 parent 堆顶,单对单
    public static void OneAddToOne(Card parent, Card child)
    {
        //parent是被堆叠的对象，child是当前操作的对象
        Card top = parent.GetTop(); //获取被堆叠对象最顶部的对象
        top.isInStack = true;
        child.topParent = top;  //将新进的顶部对象设置为顶部对象
        Debug.Log(top.cardData.cardContent);
        child.isInStack = true; //新近对象为Instack
        Debug.Log(top.childCards.Count);
        if (top.childCards.Count == 0)
        {
            top.childCards.Add(top);  //只有顶部的卡会有一个List，储存所有的子对象
            top.childCards.Add(child);
        }
        else
        {
            top.childCards.Add(child);
        }
        Debug.Log(top.childCards.Count);
        child.transform.SetParent(top.transform.parent, true);
        UpdateStackPositions(top);
    }
    //单张入栈
    public static void OneToStack(Card parent, Card child)
    {
        Card top = parent.GetTop();
        child.topParent = top;
        child.isInStack = true;
        top.childCards.Add(child);
        child.transform.SetParent(top.transform.parent, true);
        UpdateStackPositions(top);
    }

    //添加单张到整个stack，单对多
    //public static void OneAddToStack(Card parent,)

    // 移动整个 stack
    public static void UpdateStackPositions(Card parent)
    {
        Card top = parent.GetTop();
        RectTransform topRT = top.rectTransform;

        for (int i = 1; i < top.childCards.Count; i++)
        {
            RectTransform cRT = top.childCards[i].rectTransform;
            cRT.anchoredPosition = topRT.anchoredPosition + new Vector2(0, -offsetY * i);
        }
        for (int i = 0; i < top.childCards.Count-1; i++)
        {
            top.childCards[i].InStackStyle();
        }

    }

    public static void ChangeAllStackSibings(Card currentCard)
    {
        Card top = currentCard.GetTop();

        for (int i = 0; i < top.childCards.Count; i++)
        {
            top.childCards[i].transform.SetAsLastSibling();
            Debug.Log(top.childCards[0].cardData.cardContent);
        }
    }
}