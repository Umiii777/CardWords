using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;
    //组件引用
    public CardPool cardPool;
    public RectTransform canvas;

    public int currentLevelDeckTotalNum;  //当前关卡的牌库卡牌持有数量

    private CardData currentCardData;
    private List<CardData> deckCardDatas;

    private List<Card> currentSetCards;

    private int currentDeckCount;//牌库中的第x张牌
    private int DeckSetNum;

    //参数设置
    public Vector2 firstPresetPos = new Vector2(350,-780);

    private int presetNumMax = 3;//预备区域的最大张数量
    private int presetOffset = 50;//预备区域的偏移量
    private int currentPresetNum = 0;//当前预备区域的卡牌数

    
    private void Awake()
    {
        Instance = this;
    }


    //每关卡开始时，初始化牌库
    public void InitCardDeck(List<CardData> currentFullDeckDatas)
    {

        deckCardDatas = currentFullDeckDatas;
        currentLevelDeckTotalNum = deckCardDatas.Count;
        Debug.Log("当前的deck中有" + currentLevelDeckTotalNum + "张卡牌");
    }

    public void DrawCard()
    {
        if (DeckSetNum > currentLevelDeckTotalNum)
        {
            ResetDeck();
        }
        else
        {
            GameObject CardObj = cardPool.Get();
            Card currentCard = CardObj.GetComponent<Card>();
            currentCard.cardData = deckCardDatas[currentDeckCount];
            currentCard.SetCardVisual();

            RectTransform currentCardRect = CardObj.GetComponent<RectTransform>();
            currentCardRect.SetParent(canvas, false);
            currentCardRect.anchoredPosition = firstPresetPos;
            //currentCardRect.SetParent(dragla)


            
        }
    }
    public void ResetDeck()
    {
        
    }
    

}
