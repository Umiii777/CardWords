using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
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
    #region Drag接口实现
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!card.isFront)
        {
            // 翻面的卡不可拖拽
            top = null;
            return;
        }
        if (card.isOnMainRow)
        {
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

        }
        // 2. 落到卡上（合并逻辑）
        if (!best.cardData.isMainCard)
        {
            TryMerge(best);
            return;
        }
        else if (best.cardData.isMainCard)
        {
            Debug.Log("落在了主卡上");
            PlaceOnMainCard(best);
        }

        // 默认：回原位
        top.rectTransform.anchoredPosition = originalPos;
        CardStack.UpdateStackPositions(top);
    }

    // 修复：正确从 eventData 指向的被拖拽物体获取拖拽的 top（不要依赖本实例的 private top）

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
        Debug.Log("当前卡片的堆叠区域是" + maxArea);
        return maxArea > 0.5 ? best : null; // 用 maxArea，而不是 bestArea
    }
    private Row FindBestOverlapRow()
    {
        Debug.Log("执行了findBestRow");
        float maxArea = 0;
        Row best = null;
        Debug.Log("当前CardmanagerInstance的row总量为" + CardManager.Instance.allRows.Count);
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
        Debug.Log("当前卡片的堆叠区域是" + maxArea);
        return maxArea > 0.5 ? best : null; //和maxArea比较的值就是吸附的参数
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
            Debug.Log("主卡拖到了mainRow上面");
            top.rectTransform.anchoredPosition = row.rectTransform.anchoredPosition;

            //变更拖拽上去的卡状态
            top.isOnMainRow = true;
            top.isOnRow = false;
            //变更row状态
            row.isEmpty = false;    //主卡槽直接置空，TODO清空主卡槽的逻辑
            //变更所有卡状态
            CardStack.UpdateStackPositions(top);
            OnSuccessDrag.RaiseEvent(this, this);
            //row.DragOncard(top);

            return;
        }

        top.rectTransform.anchoredPosition = row.rectTransform.anchoredPosition;

        CardStack.UpdateStackPositions(top);
    }

    private void PlaceOnMainCard(Card target)
    {
        if (!target.isOnMainRow)
        {
            top.rectTransform.anchoredPosition = originalPos;
            return;
        }
        else
        {
            if (top.cardData.mainId == target.cardData.mainId)
            {
                //触发card自身的被合成东西+表现
                top.gameObject.SetActive(false);
                target.SetMainCardVisualOnCombine();
            }
            else
            {
                top.rectTransform.anchoredPosition = originalPos;
                return;
            }
        }
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
