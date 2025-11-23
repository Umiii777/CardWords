using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Card card;
    private Card top;
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

        top.transform.SetParent(UIManager.Instance.dragLayer, true);
        top.transform.SetAsLastSibling();

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
        // if (target.cardData.mainId == top.cardData.mainId &&
        //     target.isOnRow && target.isFront)
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

        // 不满足合并，回原位
        top.rectTransform.anchoredPosition = originalPos;
        CardStack.UpdateStackPositions(top);
    }

    //成功拖动时，改变card的slotCount



}




