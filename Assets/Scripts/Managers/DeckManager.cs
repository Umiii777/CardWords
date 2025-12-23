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
    public DeckHandler deckHandler;
    public ObjectEventSO DrawACardFromDeck;


    public int currentLevelDeckTotalNum;  //当前关卡的牌库卡牌持有数量

    private CardData currentCardData;
    public List<CardData> currentDeckCardDatas;

    private List<Card> currentDeckCardsHasSet = new List<Card>();

    private int currentDeckCount = 0;//牌库中的第x张牌
    private int hasRemovedNum = 0;

    //参数设置
    private Vector2 firstPresetPos = new Vector2(0, -780);

    private int presetNumMax = 3;//预备区域的最大张数量
    private int presetOffset = 50;//预备区域的偏移量
    private int currentPresetNum = 0;//当前预备区域的卡牌数

    //新堆叠样式相关
    List<Card> deckStacking = new List<Card>();


    private void Awake()
    {
        Instance = this;
    }


    //每关卡开始时，初始化牌库
    public void InitCardDeck(List<CardData> currentFullDeckDatas)
    {
        //当前deck中的卡牌数据list
        currentDeckCardDatas = currentFullDeckDatas;
        //当钱deck中的卡牌总数
        currentLevelDeckTotalNum = currentDeckCardDatas.Count;
        deckStacking.Clear();

        Debug.Log("当前的deck中有" + currentLevelDeckTotalNum + "张卡牌");
    }

    public void DrawCard()
    {
        if (currentDeckCardDatas.Count > 0)
        {
            if (currentDeckCount == currentDeckCardDatas.Count - 1)
            {
                deckHandler.DeckStyleReadyShuffle();
                DrawACardFromDeck.RaiseEvent(this, this);
            }
            if (currentDeckCount == currentDeckCardDatas.Count)
            {
                Debug.Log("执行了ResetDeck");
                ResetDeck();
                AudioManager.Instance.PlayGameSFX(AudioManager.GameSFXtype.Shuffle);
                DrawACardFromDeck.RaiseEvent(this, this);
                RowManager.Instance.CrashDefeatCheck();
            }
            else
            {
                GameObject cardObj = cardPool.Get();
                SetACardFromDeck(cardObj);
                AudioManager.Instance.PlayGameSFX(AudioManager.GameSFXtype.DrawCard);
                //currentCardRect.SetParent(dragla)
                DrawACardFromDeck.RaiseEvent(this, this);
            }
        }
        else
        {
            _ = SystemUIManager.PopUpTips("牌库已经没有更多牌了");
            RowManager.Instance.CrashDefeatCheck();
        }
        //当牌库已经被取出数据数量


    }
    private void SetACardFromDeck(GameObject cardObj)
    {
        Card currentCard = cardObj.GetComponent<Card>();
        currentCard.isFront = true;
        currentCard.isFromDeck = true;
        currentCard.cardData = currentDeckCardDatas[currentDeckCount];
        //currentDeckCardDatas.Remove(currentCard.cardData);      //从当前的数据库中移出这个数据
        currentCard.SetCardVisual();
        RectTransform currentCardRect = cardObj.GetComponent<RectTransform>();
        currentCardRect.SetParent(UIManager.Instance.dragLayer, false);
        currentCardRect.anchoredPosition = firstPresetPos;
        currentDeckCount++;

        currentDeckCardsHasSet.Add(currentCard);
        DeckStackedAdd(currentCard);

    }
    public void DeckStackedAdd(Card card)
    {
        deckStacking.Add(card);
        DeckStackUpdate();
    }
    public void DeckStackUpdateRemove(Card card)
    {
        deckStacking.Remove(card);
        DeckStackUpdate();
    }
    public void DeckStackUpdate()
    {
        if (deckStacking.Count == 1)
        {
            deckStacking[deckStacking.Count - 1].rectTransform.anchoredPosition = firstPresetPos;
            deckStacking[deckStacking.Count - 1].ResetStyle();
            deckStacking[deckStacking.Count - 1].cg.blocksRaycasts = true;
            return;
        }
        if (deckStacking.Count == 2)
        {
            deckStacking[deckStacking.Count - 1].rectTransform.anchoredPosition = firstPresetPos;
            deckStacking[deckStacking.Count - 1].ResetStyle();
            deckStacking[deckStacking.Count - 1].cg.blocksRaycasts = true;

            deckStacking[deckStacking.Count - 2].rectTransform.anchoredPosition = firstPresetPos + new Vector2(presetOffset, 0);
            deckStacking[deckStacking.Count - 2].InDeckStyle();
            deckStacking[deckStacking.Count - 2].cg.blocksRaycasts = false;
        }
        if (deckStacking.Count >= 3)
        {
            deckStacking[deckStacking.Count - 1].rectTransform.anchoredPosition = firstPresetPos;
            deckStacking[deckStacking.Count - 1].ResetStyle();
            deckStacking[deckStacking.Count - 1].cg.blocksRaycasts = true;


            deckStacking[deckStacking.Count - 3].rectTransform.anchoredPosition = firstPresetPos + new Vector2(presetOffset * 2, 0);
            deckStacking[deckStacking.Count - 3].InDeckStyle();
            deckStacking[deckStacking.Count - 3].cg.blocksRaycasts = false;

            deckStacking[deckStacking.Count - 2].rectTransform.anchoredPosition = firstPresetPos + new Vector2(presetOffset, 0);
            deckStacking[deckStacking.Count - 2].InDeckStyle();
            deckStacking[deckStacking.Count - 2].cg.blocksRaycasts = false;
        }
    }



    public void ResetDeck()
    {
        deckHandler.DeckStyleNormal();
        foreach (var card in currentDeckCardsHasSet)
        {
            CardManager.Instance.UnregisterCard(card);
            CardManager.Instance.UnregisterPoolCards(card);
        }
        currentDeckCardsHasSet.Clear();
        deckStacking.Clear();
        currentDeckCount = 0;
        hasRemovedNum = 0;
    }

    public void DeckMinusEvent(Card cardFromDeck)
    {
        Debug.Log("减少了一张卡牌" + cardFromDeck.cardData.cardContent);
        currentDeckCardDatas.Remove(cardFromDeck.cardData);
        currentDeckCardsHasSet.Remove(cardFromDeck);
        hasRemovedNum++;
        currentDeckCount--;

        DeckStackUpdateRemove(cardFromDeck);

        //堆叠样式变化
    }
    public void ShuffleDeck()
    {
        Debug.Log("开始打乱牌库");
        ResetDeck();
        deckHandler.ShuffleAnim();
        AudioManager.Instance.PlayGameSFX(AudioManager.GameSFXtype.Shuffle);
        currentDeckCardDatas.Shuffle();
        
    }

    public void LevelStartResetDeck()
    {
        deckHandler.NewLevelResetDeckStyle();
        currentDeckCardsHasSet.Clear();
        currentDeckCount = 0;
        hasRemovedNum = 0;
    }


}
