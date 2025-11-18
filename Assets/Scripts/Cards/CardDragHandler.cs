using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDragHandler : MonoBehaviour,
    IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [Header("数值部分")]
    public bool isDragging;
    private Vector2 offset;
    private Vector2 originalPos;

    private float stackOffset = 50;

    private CanvasGroup cg;
    private Card currentCard;
    private Card otherCard;
    private CardData currentCardData;

    private RectTransform rectTransform; // 当前实际操作的 RectTransform（可能是 top 的）
    private Canvas canvas;
    private bool canDrag = true;

    private Transform dragLayer;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 获取被点击的 card（trigger 所在的卡）
        Card clicked = GetComponent<Card>();
        if (clicked == null) return;

        // 如果在 stack 中，拖动应该转发到 top（堆顶）
        Card top = clicked.topParent ?? clicked;

        // 设置当前操作卡为 top（如果 top 不是被点击的 card，我们要把 top 作为 drag target）
        currentCard = top;
        rectTransform = currentCard.GetComponent<RectTransform>();
        cg = currentCard.GetComponent<CanvasGroup>();

        // dragLayer 来自 UIManager
        dragLayer = UIManager.Instance.dragLayer;
        canvas = currentCard.GetComponentInParent<Canvas>();

        // 现在允许拖动（只要 top 存在并且卡是可拖的）
        // 如果点击的不是 top（clicked != top），仍然允许 - 因为我们是把拖拽转发给 top
        if (currentCard == null) return;

        // 记录原位置（基于 top）
        originalPos = rectTransform.anchoredPosition;

        // 将 top 放到 dragLayer（全体堆放在同个父节点下）
        if (!currentCard.isInStack)
        {
            currentCard.transform.SetParent(dragLayer, true);
            currentCard.transform.SetAsLastSibling();
        }
        // 屏幕坐标 -> local 坐标（基于当前 canvas）
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out var localMousePos))
        {
            offset = rectTransform.anchoredPosition - localMousePos;
        }
        else
        {
            offset = Vector2.zero;
        }

        isDragging = true;
        cg.blocksRaycasts = false;

        // 如果这个 top 有 childCards，确保子卡的 raycast 被关闭（避免子卡拦截事件）
        foreach (var child in currentCard.childCards)
        {
            var childCg = child.GetComponent<CanvasGroup>();
            if (childCg) childCg.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || currentCard == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out var localMousePos))
        {
            rectTransform.anchoredPosition = localMousePos + offset;

            // 移动整个堆叠（基于 top）
            MoveChildStack(currentCard);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging || currentCard == null)
        {
            // 无效拖拽直接返回
            return;
        }

        isDragging = false;
        if (cg) cg.blocksRaycasts = true;

        // 恢复子卡 raycast（如果需要的话）
        foreach (var child in currentCard.childCards)
        {
            var childCg = child.GetComponent<CanvasGroup>();
            if (childCg) childCg.blocksRaycasts = true;
        }

        // 注意：eventData.pointerEnter 表示松手时鼠标下的 UI 元素
        otherCard = eventData.pointerEnter?.GetComponent<Card>();
        Row row = eventData.pointerEnter?.GetComponent<Row>();

        // 下面使用 currentCard（top）来判断落点
        currentCardData = currentCard.cardData;

        // 如果落到主row
        if (row && row.rowType == RowType.main)
        {
            if (currentCardData.isMainCard)
            {
                rectTransform.anchoredPosition = row.GetComponent<RectTransform>().anchoredPosition;
                currentCard.isOnMainRow = true;
                MoveChildStack(currentCard);
                return;
            }
            else
            {
                rectTransform.anchoredPosition = originalPos;
                MoveChildStack(currentCard);
                return;
            }
        }

        // 落到普通row
        if (row && row.rowType == RowType.normal)
        {
            if (currentCardData.isMainCard)
            {
                rectTransform.anchoredPosition = originalPos;
                MoveChildStack(currentCard);
                return;
            }
            else
            {
                rectTransform.anchoredPosition = row.GetComponent<RectTransform>().anchoredPosition;
                MoveChildStack(currentCard);
                return;
            }
        }

        // 落在主卡上
        if (otherCard && otherCard.cardData.isMainCard)
        {
            if (otherCard.isOnMainRow)
            {
                rectTransform.anchoredPosition = otherCard.GetComponent<RectTransform>().anchoredPosition;
                MoveChildStack(currentCard);
                return;
            }
            else
            {
                rectTransform.anchoredPosition = originalPos;
                MoveChildStack(currentCard);
                return;
            }
        }

        // 落在普通卡上（合并）
        if (otherCard && !otherCard.cardData.isMainCard)
        {
            // 如果是同一个主题词，合并到 otherCard 的 top 中
            if (currentCardData.mainId == otherCard.cardData.mainId && otherCard.isOnRow)
            {
                AddToStack(parentCard: otherCard, childCard: currentCard);
                MoveChildStack(currentCard);
                return;
            }
        }

        // 默认：回到原点
        rectTransform.anchoredPosition = originalPos;
        MoveChildStack(currentCard);
    }

    public void OnDrop(PointerEventData eventData)
    {
        // 这个方法可以留空或用于其他逻辑
        Card targetCard = GetComponent<Card>();
        Card dragCard = eventData.pointerDrag?.GetComponent<Card>();
        if (targetCard == null || dragCard == null)
        {
            return;
        }
        if (targetCard.cardData.mainId == dragCard.cardData.mainId)
        {
            targetCard.isInStack = true;
            targetCard.SetCardVisual();
            Debug.Log(targetCard.cardData.cardContent.ToString() + "上面落了一张牌" + targetCard.isInStack);
        }
    }

    private void AddToStack(Card parentCard, Card childCard)
    {
        if (parentCard == null || childCard == null) return;

        // 找到 parent 的真正 top
        var top = parentCard.topParent ?? parentCard;

        // 如果 child 自己也是 top（带有 child），需要把 child 整棵树并入 top（把 child 和 child.childCards 一起并入）
        // 为简单起见，这里假设 childCard 本身不是带下级的 top（若是，需要做整体合并）
        childCard.topParent = top;
        childCard.isInStack = true;

        // 将 child 加入 top 的 child 列表
        top.childCards.Add(childCard);
        childCard.stackIndex = top.childCards.Count; // 1-based index

        // 把 child 的 transform 父对象设置为 top 的父对象（和 top 相同层级）
        childCard.transform.SetParent(top.transform.parent, true);

        // 更新 child 的 anchoredPosition（按 top 的位置 + offset）
        RectTransform topRT = top.GetComponent<RectTransform>();
        RectTransform childRT = childCard.GetComponent<RectTransform>();

        childRT.anchoredPosition = topRT.anchoredPosition + new Vector2(0, -stackOffset * childCard.stackIndex);

        // // 禁用 child 自身的拖拽拦截（使其点击时由 top 处理）
        // var childDragHandler = childCard.GetComponent<CardDragHandler>();
        // if (childDragHandler) childDragHandler.canDrag = false;
    }

    private void MoveChildStack(Card parent)
    {
        if (parent == null) return;

        // 以 top 为基准移动整组
        var top = parent.topParent ?? parent;
        RectTransform topRT = top.GetComponent<RectTransform>();
        if (topRT == null) return;

        for (int i = 0; i < top.childCards.Count; i++)
        {
            var child = top.childCards[i];
            if (child == null) continue;
            RectTransform cRT = child.GetComponent<RectTransform>();
            if (cRT == null) continue;

            cRT.anchoredPosition = topRT.anchoredPosition + new Vector2(0, -stackOffset * (i + 1));
        }
    }
}