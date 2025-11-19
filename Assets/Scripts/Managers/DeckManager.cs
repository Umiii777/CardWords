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
    private List<CardData> currentDeckCardDatas;

    private List<Card> currentSetCards;

    private int currentDeckCount;//牌库中的第x张牌
    private int DeckSetNum;

    //参数设置
    private Vector2 firstPresetPos = new Vector2(0, -780);

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
        currentDeckCardDatas = currentFullDeckDatas;
        currentLevelDeckTotalNum = currentFullDeckDatas.Count;
        Debug.Log("当前的deck中有" + currentLevelDeckTotalNum + "张卡牌");
    }

    public void DrawCard()
    {
        //当牌库已经被取出数据数量
        if (DeckSetNum > currentLevelDeckTotalNum)
        {
            ResetDeck();
        }
        else
        {
            GameObject cardObj = cardPool.Get();
            SetACardFromDeck(cardObj);
            //currentCardRect.SetParent(dragla)
        }
    }
    private void SetACardFromDeck(GameObject cardObj)
    {
        Card currentCard = cardObj.GetComponent<Card>();
        currentCard.isFront = true;
        currentCard.cardData = currentDeckCardDatas[currentDeckCount];
        //currentDeckCardDatas.Remove(currentCard.cardData);      //从当前的数据库中移出这个数据
        currentCard.SetCardVisual();
        RectTransform currentCardRect = cardObj.GetComponent<RectTransform>();
        currentCardRect.SetParent(canvas, false);
        currentCardRect.anchoredPosition = firstPresetPos;
        currentDeckCount++;
    }
    public void ResetDeck()
    {

    }


}
