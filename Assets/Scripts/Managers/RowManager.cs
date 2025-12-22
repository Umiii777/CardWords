using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
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

    //判定失败相关
    public List<CardData> AllCurentCardsData = new List<CardData>();
    public List<CardData> AllCurentCardsMangerData = new List<CardData>();

    private CardData firsitDefeatData;
    private CardData secondDefeatData;

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


    //

    [ContextMenu("提示测试")]
    public async Task HintTry()
    {
        Debug.Log("开始测试");
        Debug.Log(mainRows.Count);
        //await SystemUIManager.PopUpTips("开始测试");
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i].cardsOnRow.Count >= 1)
            {
                firstHintCard = rows[i].cardsOnRow[rows[i].cardsOnRow.Count - 1];
                for (int j = 0; j < rows.Count; j++)
                {
                    if (j != i)
                    {
                        if (rows[j].cardsOnRow.Count >= 1)
                        {
                            secondHintCard = rows[j].cardsOnRow[rows[j].cardsOnRow.Count - 1];
                            if (firstHintCard.cardData.mainId == secondHintCard.cardData.mainId && !secondHintCard.cardData.isMainCard)
                            {
                                //await SystemUIManager.PopUpTips("找到了相同的卡牌");
                                Debug.Log("找到了相同的卡牌");
                                // firstHintCard.CardHint();
                                // secondHintCard.CardHint();
                                firstHintCard.HintAnimPlay();
                                secondHintCard.HintAnimPlay();
                                return;
                            }
                            else
                            {

                            }
                        }
                    }
                    else
                    {

                    }
                }
                List<Card> currentMaincards = CardManager.Instance.GetCurrentLevelCardOnFront();

                foreach (var maincard in currentMaincards)
                {
                    Debug.Log("执行了找主卡的方法");
                    if (firstHintCard.cardData.mainId == maincard.cardData.mainId)
                    {
                        //await SystemUIManager.PopUpTips("找到了相同的卡牌");
                        // firstHintCard.CardHint();
                        // maincard.CardHint();
                        firstHintCard.HintAnimPlay();
                        maincard.HintAnimPlay();
                        Debug.Log("找到了相同的卡牌");
                        return;
                    }
                }
            }

        }
        await SystemUIManager.PopUpTips("请点击牌库获得新卡牌");
        Debug.Log("没找到可合成相同的卡牌");

    }
    public void CrashDefeatCheck()
    {
        // AllCurentCardsMangerData = CardManager.Instance.allCards.ConvertAll(card => card.cardData);
        // AllCurentCardsData.AddRange(AllCurentCardsData);
        // allcurent + DeckManager.Instance.currentDeckCardDatas;

        //开始遍历所有的row
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i].cardsOnRow.Count >= 1)
            {
                firsitDefeatData = rows[i].cardsOnRow[rows[i].cardsOnRow.Count - 1].cardData;
                for (int j = 1; j < rows.Count; j++)
                {
                    if (rows[j].cardsOnRow.Count >= 1)
                    {
                        secondDefeatData = rows[j].cardsOnRow[rows[j].cardsOnRow.Count - 1].cardData;
                        if (firsitDefeatData.mainId == secondDefeatData.mainId && !secondDefeatData.isMainCard && !firsitDefeatData.isMainCard)
                        {
                            return;
                        }
                    }
                }
                foreach (var cardData in DeckManager.Instance.currentDeckCardDatas)
                {
                    secondDefeatData = cardData;
                    if (firsitDefeatData.mainId == secondDefeatData.mainId && !secondDefeatData.isMainCard && !firsitDefeatData.isMainCard)
                    {
                        return;
                    }
                }
            }
            else
            {
                return;
            }
        }
        List<Card> currentMaincards = CardManager.Instance.GetCurrentLevelCardOnFront();
        foreach (var maincard in currentMaincards)
        {
            if (firsitDefeatData.mainId == maincard.cardData.mainId && !maincard.cardData.isMainCard && !firsitDefeatData.isMainCard)
            {
                return;
            }
        }
        _=SystemUIManager.PopUpTips("卡死了");
        Debug.LogError("卡死了");
    }
}
