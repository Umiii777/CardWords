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
    public RectTransform localTransform;
    public SpriteRenderer spr;
    public TextMeshProUGUI tmContent;
    public TextMeshProUGUI tmLength;
    public Image cardBaseStyle;

    //展示相关的变量
    public bool isFront;
    public Sprite[] sprites; //0是卡背面，1是显示字的内容面


    //跟拖拽相关的变量
    public bool isOnRow;
    public bool isOnMainRow = false;

    //合成一堆相关的变量
    public Card topParent = null;
    public int stackIndex = 0;
    public bool isInStack = false;
    public List<Card> childCards = new List<Card>();

    private int textStackOffsetPos = 100;
    private int textStackOffsetSize = 30;


    //private BoxCollider2D collider;
    private void Awake()
    {
        localTransform = gameObject.GetComponent<RectTransform>();
        cardBaseStyle = gameObject.GetComponent<Image>();

    }
    //每次拖拽完成后都应该调用这个SetCardVisual
    public void SetCardVisual()
    {

        if (isFront)
        {
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
                if (isInStack)
                {
                    Debug.Log("执行了更新卡牌样式的方法");
                    //设置卡牌的别样样式
                    InStackStyle();
                }
            }
        }
        else
        {
            //TODO:切换到卡背面
            tmContent.alpha = 0f;
            cardBaseStyle.sprite = CardVisualManager.Instance.spriteBack;
        }
    }

    public void InStackStyle()
    {
        tmContent.rectTransform.anchoredPosition = tmContent.rectTransform.anchoredPosition + new Vector2(0, textStackOffsetPos);
        tmContent.fontSize = textStackOffsetSize;
    }
    public void InPresetStyle()
    {
        tmContent.rectTransform.anchoredPosition = tmContent.rectTransform.anchoredPosition + new Vector2(0, textStackOffsetPos);
        tmContent.fontSize = textStackOffsetSize;
    }
}
