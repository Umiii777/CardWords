using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Row : MonoBehaviour
{
    public RowType rowType;
    public bool isEmpty = false;
    public int cardCount = 0;

    // 当前行上的卡（按从 top 到子牌顺序或你需要的顺序）
    public List<Card> cardsOnRow = new List<Card>();

    public RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }



    public void OnChangeUnlockRowType()
    {
        if (rowType == RowType.unlocked)
        {
            rowType = RowType.main;
        }
    }
    //每次成功拖动的时候应该调用
    public void CheckIfRowEmpty()
    {
        SetCardCount();
        if (cardCount == 0)
        {
            isEmpty = true;
        }
        else if (cardCount > 0)
        {
            isEmpty = false;
        }
        else
        {
            Debug.LogError("他妈的row当前卡数量成负数了");
        }
    }
    //获得当前row的card总数
    public void SetCardCount()
    {
        cardCount = cardsOnRow.Count;
    }

    //将当前cardsOnRow的最上层变成isFront
    public void SetCardFront()
    {
        if (!cardsOnRow[cardsOnRow.Count - 1].isFront)
        {
            cardsOnRow[cardsOnRow.Count - 1].isFront = true;
        }
        else
        {
            Debug.Log("当前这张卡已经是正面了");
        }

    }
}