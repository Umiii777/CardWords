using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStack
{
    private const float offsetY = 50f;

    // 添加 child 到 parent 堆顶
    public static void AddToStack(Card parent, Card child)
    {
        Card top = parent.GetTop();

        child.topParent = top;
        child.isInStack = true;
        top.childCards.Add(child);

        child.transform.SetParent(top.transform.parent, true);
        UpdateStackPositions(top);
    }

    // 移动整个 stack
    public static void UpdateStackPositions(Card parent)
    {
        Card top = parent.GetTop();
        RectTransform topRT = top.rectTransform;

        for (int i = 0; i < top.childCards.Count; i++)
        {
            RectTransform cRT = top.childCards[i].rectTransform;
            cRT.anchoredPosition = topRT.anchoredPosition + new Vector2(0, -offsetY * (i + 1));
        }
    }
}
