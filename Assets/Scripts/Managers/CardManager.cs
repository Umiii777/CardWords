using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    public List<Card> allCards = new List<Card>();
    public List<Row> allRows = new List<Row>();

    private void Awake()
    {
        Instance = this;
    }

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
}
