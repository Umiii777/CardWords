using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Card card;
    private Card top;
    private Card onDropCard;

    private Canvas canvas;
    private CanvasGroup cg;

    private Vector2 offset;
    private Vector2 originalPos;
    private Transform bestSlot = null;
    private float bestArea = 0f;

    public CardEventSO OnBeginCardDrag;
    public CardEventSO OnEndDragCard;
    public ObjectEventSO OnSuccessDrag;


    private void Awake()
    {
        card = GetComponent<Card>();
        cg = GetComponent<CanvasGroup>();
    }
    #region Drag接口实现
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!card.isFront)
        {
            // 前置卡不可拖拽
            top = null;
            return;
        }

        // 把拖拽转发到 top
        top = card.GetTop();

        cg = top.GetComponent<CanvasGroup>();
        canvas = top.GetComponentInParent<Canvas>();

        originalPos = top.rectTransform.anchoredPosition;

        if (!top.isInStack)
        {
            top.transform.SetParent(UIManager.Instance.dragLayer, true);
            top.transform.SetAsLastSibling();
        }


        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out var localMouse);

        offset = top.rectTransform.anchoredPosition - localMouse;

        cg.blocksRaycasts = false;
        OnBeginCardDrag.RaiseEvent(top, this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (top == null)
        {
            return;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out var localMouse))
        {
            //移动单张
            top.rectTransform.anchoredPosition = localMouse + offset;
            //整体移动
            CardStack.UpdateStackPositions(top);
        }
        else
        {
            return;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        offset = Vector2.zero;

        Debug.Log("执行了onEndDrag");
        if (top == null)
        {
            Debug.Log("top为空");
            return;
        }
        cg.blocksRaycasts = true;

        Card best = FindBestOverlapCard();
        Row bestRow = FindBestOverlapRow();
        // 1. 落到 Row 上
        if (bestRow)
        {
            if (bestRow.isEmpty)
            {
                PlaceOnRow(bestRow);
                return;
            }
            else
            {
                // 如果不是空行，按你的规则决定是否允许放置；这里先回退
                top.rectTransform.anchoredPosition = originalPos;
                CardStack.UpdateStackPositions(top);
                return;
            }
        }
        // 2. 落到卡上（合并逻辑）
        if (best)
        {
            TryMerge(best);
            return;
        }


        // 默认：回原位
        top.rectTransform.anchoredPosition = originalPos;
        CardStack.UpdateStackPositions(top);
    }

    // 修复：正确从 eventData 指向的被拖拽物体获取拖拽的 top（不要依赖本实例的 private top）
    public void OnDrop(PointerEventData eventData)
    {
        onDropCard = card; // 当前接收 Drop 的卡

        // 从 pointerDrag 找到真正的被拖拽物（可能是原始卡，或其所在对象）
        if (eventData == null || eventData.pointerDrag == null)
        {
            return;
        }

        var draggedCardComp = eventData.pointerDrag.GetComponent<Card>();
        if (draggedCardComp == null)
        {
            // pointerDrag 可能是包含其他组件的对象，尝试用 GetComponentInParent
            draggedCardComp = eventData.pointerDrag.GetComponentInParent<Card>();
            if (draggedCardComp == null) return;
        }

        var draggedTop = draggedCardComp.GetTop(); // 安全地取得真正的 top（如果实现了 GetTop）
        Debug.Log("Ondrag" + draggedTop);
        if (draggedTop == null) return;

        // 只有当目标和拖拽物都是在主行，且 mainId 相同，才进行消除逻辑
        if (onDropCard.isOnMainRow && draggedTop.cardData.mainId == onDropCard.cardData.mainId)
        {
            Debug.Log("执行了OnDrop - 匹配到相同 mainId，开始消除");
            if (draggedTop.childCards != null)
            {
                List<Card> eliminateCards = draggedTop.childCards;
                OnCardEliminate(eliminateCards);
            }
            else
            {
                OnCardEliminate(draggedTop);
            }

        }
        else
        {
            return;
        }
    }
    #endregion
    #region 私有工具类

    private Card FindBestOverlapCard()
    {
        float maxArea = 0;
        Card best = null;


        foreach (var c in CardManager.Instance.allCards)
        {
            if (c == top) continue;

            float area = CardOverlap.GetOverlapArea(
                top.rectTransform, c.rectTransform);

            if (area > maxArea)
            {
                maxArea = area;
                best = c;
            }
        }
        Debug.Log("当前Allcard有多少牌" + CardManager.Instance.allCards.Count);

        return maxArea > 1 ? best : null; // 用 maxArea，而不是 bestArea
    }
    private Row FindBestOverlapRow()
    {
        float maxArea = 0;
        Row best = null;

        foreach (var r in CardManager.Instance.allRows)
        {
            float area = CardOverlap.GetOverlapArea(
                top.rectTransform, r.rectTransform);

            if (area > maxArea)
            {
                maxArea = area;
                best = r;
            }
        }

        return maxArea > 1 ? best : null;
    }
    private void PlaceOnRow(Row row)
    {
        if (row.rowType == RowType.main && !top.cardData.isMainCard)
        {
            //不是主卡不能放到主卡行
            top.rectTransform.anchoredPosition = originalPos;
            CardStack.UpdateStackPositions(top);
            return;
        }
        else if (row.rowType == RowType.normal && row.isEmpty)
        {
            top.rectTransform.anchoredPosition = row.rectTransform.anchoredPosition;
            CardStack.UpdateStackPositions(top);
            OnSuccessDrag.RaiseEvent(this, this);
            return;
        }
        else if (row.rowType == RowType.main && top.cardData.isMainCard)
        {
            top.rectTransform.anchoredPosition = row.rectTransform.anchoredPosition;
            top.isOnMainRow = true;
            CardStack.UpdateStackPositions(top);
            OnSuccessDrag.RaiseEvent(this, this);
            //row.DragOncard(top);

            return;
        }

        top.rectTransform.anchoredPosition = row.rectTransform.anchoredPosition;

        CardStack.UpdateStackPositions(top);
    }

    private void TryMerge(Card target)
    {
        Debug.Log($"Trying merge: {top.cardData.cardContent}  vs  {target.cardData.cardContent}");

        // 第一种情况: 单到单（目标是单张且正面）
        if (target.cardData.mainId == top.cardData.mainId &&
            target.isFront && !target.isInStack)
        {
            CardStack.AddToStack(target, top);
            target.InStackStyle();

            OnEndDragCard.RaiseEvent(target, this);
            OnSuccessDrag.RaiseEvent(top, this);
            top.slotCount = target.slotCount;
            return;
        }

        // 第二种情况: 单到多（把单张加入到已有的 stack 顶部）
        if (target.cardData.mainId == top.cardData.mainId &&
            (target.isInStack || target.isFront))
        {
            // 允许把单张放到已有堆栈（需要 CardStack.AddToStack 能处理 target 已经是 stack 的情况）
            CardStack.AddToStack(target, top);
            //target.InStackStyle();

            OnEndDragCard.RaiseEvent(target, this);
            OnSuccessDrag.RaiseEvent(top, this);
            top.slotCount = target.slotCount;
            return;
        }

        // 默认：不匹配，回退
        top.rectTransform.anchoredPosition = originalPos;
        CardStack.UpdateStackPositions(top);
        return;
    }

    //成功拖动时，改变card的slotCount

    //消除卡牌的行为
    public void OnCardEliminate(List<Card> eliminateCards)
    {
        foreach (var item in eliminateCards)
        {
            Destroy(item.gameObject);
        }
    }
    public void OnCardEliminate(Card eliminateCard)
    {

        Destroy(eliminateCard);

    }

    #endregion
}
