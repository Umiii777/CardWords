using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    /*虽然是CardManger，但是也有row的事情*/

    public static CardManager Instance;

    public List<Card> allCards = new List<Card>();
    public List<Row> allRows = new List<Row>();

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
        if (allCards.Contains(card))
            allCards.Remove(card);
    }

    //Rows
    public void RegisterRow(Row row)
    {
        if (!allRows.Contains(row))
        {
            allRows.Add(row);
        }
    }
    public void UnregisterCard(Row row)
    {
        if(allRows.Contains(row))
        {
            allRows.Remove(row);
        }
    }
}
