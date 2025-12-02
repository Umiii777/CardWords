using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RowManager : MonoBehaviour
{
    public List<Row> rows = new List<Row>();
    public List<Row> mainRows = new List<Row>();

    public CardEventSO onDragFromDeck;

    private Card firstHintCard;
    private Card secondHintCard;

    public static RowManager Instance;

    public void Awake()
    {
        Instance = this;
    }
    


    
    public void AddRow(Row rowFromLevelManager)
    {
        rows.Add(rowFromLevelManager);
    }

    public void AddMainRow(Row mainRowFromLevelManger)
    {
        mainRows.Add(mainRowFromLevelManger);
    }

    /* 应该有如下监听的事件，卡牌成功拖拽的时候
    1.更新row的数量，通过
    */
    //跟row添加Card相关的方法
    public void CardsOnRowChangeAdd(Card topCard)
    {
        //传入的card本身只有原来的sloutCount,如果是从deck传上来的，顺便改下deckCard的东西
        if (topCard.isFromDeck)
        {
            topCard.isFromDeck = false;
            onDragFromDeck.RaiseEvent(topCard, this);
        }


        rows[topCard.slotCount].cardsOnRow.Add(topCard);
        rows[topCard.slotCount].CheckIfRowEmpty();
    }

    //和上面的中间必须有一个更改slotCount的间隔,先减再加
    public void CardOnRowChangeMinus(Card topCard)
    {


        rows[topCard.slotCount].cardsOnRow.Remove(topCard);
        rows[topCard.slotCount].CheckIfRowEmpty();
    }

    // public void HintTry()
    // {
    //     for (int i = 0; i < rows.Count; i++)
    //     {
    //         if (rows[i].cardsOnRow.Count > 1)
    //         {
    //             firstHintCard = rows[i].cardsOnRow[rows[i].cardsOnRow.Count - 1];
    //         }
    //         else
    //         {
    //             i++;
    //         }

    //     }
    // }
}
