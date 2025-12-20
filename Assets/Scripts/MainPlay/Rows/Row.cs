     using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Row : MonoBehaviour
{
    public RowType rowType;
    public bool isEmpty = false;
    public int cardCount = 0;
    public int rowNum = -1;

    public TextMeshProUGUI unlockedText;
    public Image unlockedIcon;




    // 当前行上的卡（按从 top 到子牌顺序或你需要的顺序）
    public List<Card> cardsOnRow = new List<Card>();
    
    public RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

    }

    public void OnClickUnlockedRow()
    {
        _= SystemUIManager.LoadUI(UIType.UnlockSlot, this);
    }


    // public void OnChangeUnlockRowType()
    // {
    //     if (rowType == RowType.unlocked)
    //     {
    //         rowType = RowType.main;
    //     }
    // }
    //每次在下方成功拖动的时候应该调用
    public void CheckIfRowEmpty()
    {
        SetCardCount();
        if (cardCount == 0)
        {
            isEmpty = true;
        }
        else if (cardCount > 0)
        {
            isEmpty = false;
        }
        else if (cardCount < 0)
        {
            Debug.LogError("他妈的row当前卡数量成负数了");
        }
        SetCardFront();
        DebugRowOrder();
    }
    public void CheckIfMainRowEmpty()
    {

    }



    //获得当前row的card总数
    public void SetCardCount()
    {
        cardCount = cardsOnRow.Count;
    }

    //将当前cardsOnRow的最上层变成isFront
    public void SetCardFront()
    {
        if (cardsOnRow == null || cardsOnRow.Count == 0)
        {
            Debug.Log("列表为空，没有卡可以翻面");
            return;
        }
        Card lastCard = cardsOnRow[cardsOnRow.Count - 1];
        if (lastCard != null && !lastCard.isFront)
        {
            // Debug.Log("执行了设置卡翻面");
            // Debug.Log(cardsOnRow[cardsOnRow.Count - 1].cardData.cardContent);
            cardsOnRow[cardsOnRow.Count - 1].SetFront();

        }
        else
        {
            Debug.Log("当前这张卡已经是正面了");
            return;
        }
    }

    //CardManger加载保存
    // private void OnEnable()
    // {
    //     CardManager.Instance.allRows.Add(this);
    // }

    private void OnDisable()
    {
        CardManager.Instance.allRows.Remove(this);
    }


    //TODO:传入多个Card
    public void DebugRowOrder()
    {
        Debug.Log("----- Row Order -----");
        for (int i = 0; i < cardsOnRow.Count; i++)
        {
            Debug.Log(i + ": " + cardsOnRow[i].cardData.cardContent);
        }
    }

//执行改变Rowtype——unlocked——main
    public static void OnChangeMainRowType(Row row)
    {
        if (row.rowType == RowType.unlocked)
        {
            row.rowType = RowType.main;
            row.OnUpdateMainRowType();
        }
    }
    public void OnUpdateMainRowType()
    {
        unlockedIcon = transform.Find("Image_Unlock").GetComponent<Image>();
        unlockedText = transform.Find("text_unlock").GetComponent<TextMeshProUGUI>();
        if (rowType == RowType.unlocked)
        {
            unlockedIcon.enabled = true;
            unlockedText.enabled = true;
        }
        else if (rowType == RowType.main)
        {
            unlockedIcon.enabled = false;
            unlockedText.enabled = false;
        }
    }
}