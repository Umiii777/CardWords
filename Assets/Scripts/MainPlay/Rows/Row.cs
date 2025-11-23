using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Row : MonoBehaviour, IDropHandler
{
    public RowType rowType;
    public bool isEmpty = true;
    public int cardCount = 0;

    // 当前行上的卡（按从 top 到子牌顺序或你需要的顺序）
    public List<Card> cardsOnRow = new List<Card>();

    public RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // 当用户直接拖放到 Row（EventSystem）也会触发，这里我们尽量把逻辑统一到 AddCards 中
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null) return;

        var draggedCardComp = eventData.pointerDrag.GetComponent<Card>();
        if (draggedCardComp == null)
        {
            draggedCardComp = eventData.pointerDrag.GetComponentInParent<Card>();
            if (draggedCardComp == null) return;
        }

        Card draggedTop = draggedCardComp.GetTop();
        if (draggedTop == null) return;

        // 如果你希望 Row 在 EventSystem.Drop 时直接接管放置，可以调用 AddCards / AddCard
        // 这里仅记录日志，并示例性调用 AddCard（更完整的权限检查由 AddCard 负责）
        Debug.Log($"Row.OnDrop 接收到卡片：{draggedTop.cardData.cardContent}");
        AddCardOrStackToRow(draggedTop);
    }

    // 外部调用：把一个 top（及其 childCards，若存在）放到该行上
    public void AddCardOrStackToRow(Card topCard)
    {
        if (topCard == null) return;

        // 如果 topCard 是堆栈的顶部且包含 childCards，我们把整个堆栈作为一组放置
        if (topCard.isInStack && topCard.childCards != null && topCard.childCards.Count > 0)
        {
            // 把 top 和所有 child 都加入 rows 的 cardsOnRow（顺序：top then children）
            List<Card> group = new List<Card> { topCard };
            group.AddRange(topCard.childCards);

            AddCards(group);
        }
        else
        {
            AddCard(topCard);
        }
    }

    // 添加单张卡到行
    public void AddCard(Card card)
    {
        if (card == null) return;

        // 父级设置：把 card reparent 到 row.transform（使用 false 保持 local 坐标语义一致）
        Vector3 worldPos = card.rectTransform.position;
        card.transform.SetParent(this.transform, true);
        card.rectTransform.position = worldPos;
        card.transform.SetAsLastSibling();

        // 标记并加入列表
        cardsOnRow.Add(card);
        card.isOnMainRow = (rowType == RowType.main);

        // 更新 row 状态并排列
        UpdateRowState();

        // 如果卡是堆栈的一部分，让堆栈进行布局
        CardStack.UpdateStackPositions(card);
    }

    // 添加多张卡（堆栈）
    public void AddCards(List<Card> cards)
    {
        if (cards == null || cards.Count == 0) return;

        foreach (var c in cards)
        {
            if (c == null) continue;
            // 把每张卡 reparent 到 row（保持 world pos 再更新 anchored）
            Vector3 worldPos = c.rectTransform.position;
            c.transform.SetParent(this.transform, true);
            c.rectTransform.position = worldPos;
            c.transform.SetAsLastSibling();

            // 标记
            c.isOnMainRow = (rowType == RowType.main);

            // 单独把它加入行列表（若已在别处，先不做额外移除，这里假设调用方已做好清理）
            cardsOnRow.Add(c);
        }

        UpdateRowState();

        // 若第一张是 top，使用 CardStack.UpdateStackPositions 去布局
        Card top = cards[0].GetTop();
        if (top != null) CardStack.UpdateStackPositions(top);
    }

    public void RemoveCard(Card c)
    {
        if (c == null) return;
        if (cardsOnRow.Remove(c))
        {
            c.isOnMainRow = false;
            UpdateRowState();
        }
    }

    public void UpdateRowState()
    {
        cardCount = cardsOnRow.Count;
        isEmpty = cardCount <= 0;
    }

    private void OnEnable()
    {
        if (CardManager.Instance != null)
            CardManager.Instance.allRows.Add(this);
    }
    private void OnDisable()
    {
        if (CardManager.Instance != null)
            CardManager.Instance.allRows.Remove(this);
    }

    public void OnChangeUnlockRowType()
    {
        if (rowType == RowType.unlocked)
        {
            rowType = RowType.main;
        }
    }
}