using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    /*虽然是CardManger，但是也有row的事情*/

    public static CardManager Instance;

    public List<Card> allCards = new List<Card>();
    public List<Row> allRows = new List<Row>();

    public ObjectEventSO CompleteLevel;

    private void Awake()
    {
        Instance = this;
    }

    //Cards
    public void RegisterCard(Card card)
    {
        if (!allCards.Contains(card))
            allCards.Add(card);
    }

    public void UnregisterCard(Card card)
    {
        Debug.Log("移除了一张卡牌,还剩" + allCards.Count + "张卡牌");
        if (allCards.Contains(card))
        {
            allCards.Remove(card);
            Debug.Log("移除了一张卡牌,还剩" + allCards.Count + "张卡牌");
            CardPool.Instance.Release(card.gameObject);
            if (allCards.Count == 0)
            {
                Debug.Log("移除了一张卡牌,还剩" + allCards.Count + "张卡牌");
                OnCompleteLevel();
            }
        }
        else if (allCards.Count == 0)
        {
            Debug.Log("移除了一张卡牌,还剩" + allCards.Count + "张卡牌");
            OnCompleteLevel();
        }
    }

    //Rows
    public void RegisterRow(Row row)
    {
        if (!allRows.Contains(row))
        {
            allRows.Add(row);
        }
    }
    public void UnregisterRow(Row row)
    {
        if (allRows.Contains(row))
        {
            allRows.Remove(row);
        }
    }

    public void OnCompleteLevel()
    {
        //让levelManager监听胜利
        Debug.Log("关卡胜利了");
        CompleteLevel.RaiseEvent(this, this);
        
    }
    public List<Card> GetCurrentLevelCardOnFront()
    {
        List<Card> currentLevelCardsOnFront = new List<Card>();
        foreach (var card in allCards)
        {
            if (card.isFront && card.isOnMainRow && card.cardData.isMainCard)
            {
                currentLevelCardsOnFront.Add(card);
            }
        }
        return currentLevelCardsOnFront;
    }
}
