using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    //组件
    public CardData cardData;
    public RectTransform rectTransform;
    public SpriteRenderer spr;
    public TextMeshProUGUI tmContent;
    public TextMeshProUGUI tmLength;
    public Image cardBaseStyle;

    //展示相关的变量
    public bool isFront;
    public Sprite[] sprites; //0是卡背面，1是显示字的内容面
    public bool isFromDeck = false;
    private bool isStackStyleApplied = false;

    //跟拖拽相关的变量
    public bool isOnRow;
    public bool isOnMainRow = false;

    public CanvasGroup cg;
    private Transform dragLayer;


    //合成一堆相关的变量
    public Card topParent = null;
    public int stackIndex = 0;
    public bool isInStack = false;
    public List<Card> childCards = new List<Card>();

    public int cardNums = 1;//代表这个牌未堆叠的时候，只有一张，用于传递给Row

    public int slotCount = -1;  //用于给deck判断


    private int textStackOffsetPos = 100;
    private int textStackOffsetSize = 30;

    //触发的事件
    public CardDataEventSO deckSuccessDrag;




    //private BoxCollider2D collider;
    private void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        cardBaseStyle = gameObject.GetComponent<Image>();
        cg = GetComponent<CanvasGroup>();
    }
    //每次拖拽完成后都应该调用这个SetCardVisual
    public void SetCardVisual()
    {
        if (isFront)
        {
            tmContent.alpha = 1f;

            if (cardData.isMainCard)
            {
                //TODO;再增加一个Sprite数组内容，用于放置MainCard专属的东西
                tmContent.text = cardData.cardContent;
                cardBaseStyle.sprite = CardVisualManager.Instance.spriteFrontMain;
                tmLength.text = cardData.currentLength.ToString() + "/" + cardData.mainTotalLength.ToString();
            }
            else
            {
                tmContent.text = cardData.cardContent;
                cardBaseStyle.sprite = CardVisualManager.Instance.spriteFrontNormal;
                tmLength.text = "";

            }
        }
        else
        {
            //TODO:切换到卡背面
            tmContent.alpha = 0f;
            cardBaseStyle.sprite = CardVisualManager.Instance.spriteBack;
        }
    }
    public void SetFront()
    {
        if (!isFront)
        {
            isFront = true;
            SetCardVisual();
        }
    }



    public void InStackStyle()
    {
        if (!isStackStyleApplied)
        {
            tmContent.rectTransform.anchoredPosition = tmContent.rectTransform.anchoredPosition + new Vector2(0, textStackOffsetPos);
            tmContent.fontSize = textStackOffsetSize;
            isStackStyleApplied = true;
        }

    }
    public void InPresetStyle()
    {
        tmContent.rectTransform.anchoredPosition = tmContent.rectTransform.anchoredPosition + new Vector2(0, textStackOffsetPos);
        tmContent.fontSize = textStackOffsetSize;
    }


    #region Stack相关

    public Card GetTop()
    {
        return topParent ? topParent : this;
    }

    public bool IsTop()
    {
        return GetTop() == this;
    }

    #endregion

    #region CardManger相关
    private void OnEnable()
    {
        CardManager.Instance.allCards.Add(this);
    }

    private void OnDisable()
    {
        CardManager.Instance.allCards.Remove(this);
    }
    #endregion
    #region MainCard方法
    public void SetMainCardVisualOnCombine()
    {
        cardData.currentLength++;
        tmLength.text = cardData.currentLength.ToString() + "/" + cardData.mainTotalLength.ToString();
        MainCardCheckLength();
    }
    public void MainCardCheckLength()
    {
        if (cardData.currentLength == cardData.mainTotalLength)
        {
            //执行maincard的消除逻辑和表现
            CompleteMainCard();
        }
    }

    //消除表现
    public void CompleteMainCard()
    {

    }
    #endregion

    public void CompleteNormalCard()
    {
        gameObject.SetActive(false);
    }

}
