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

    //同时存在的时候先减再增加，传递给row的事件，让row帮助翻牌库的牌，同时检测是否为空
    //对应事件由rowManager来订阅
    public CardEventSO EndDragAdd;      //从deck来的牌只增加不减少
    public CardEventSO EndDragMinus;    //向上面走的时候只减少不增加，



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
        top = card.GetTop();    //给topParent赋值

        cg = top.GetComponent<CanvasGroup>();
        canvas = top.GetComponentInParent<Canvas>();

        originalPos = top.rectTransform.anchoredPosition;

        if (!top.isInStack)
        {
            top.transform.SetParent(UIManager.Instance.dragLayer, true);
            top.transform.SetAsLastSibling();


        }
        if (top.isInStack)
        {
            //如果拖拽的卡是堆，那么直接用stack的方法遍历整个设置lastsibiling
            CardStack.ChangeAllStackSibings(top);
        }


        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out var localMouse);

        offset = top.rectTransform.anchoredPosition - localMouse;

        cg.blocksRaycasts = false;

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
        if (best && !best.cardData.isMainCard && !best.isFromDeck && best.isFront)
        {
            TryMerge(best);
            return;
        }
        else if (best && best.cardData.isMainCard)
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

    private void PlaceOnRow(Row row)    //放到普通row上，有下面到下面，也有下面到上面main
    {
        //普通卡放不到mainRow
        if (row.rowType == RowType.main && !top.cardData.isMainCard)
        {
            top.rectTransform.anchoredPosition = originalPos;
            CardStack.UpdateStackPositions(top);
            return;
        }
        //普通卡可以放到空row
        else if (row.rowType == RowType.normal && row.isEmpty)
        {
            top.rectTransform.anchoredPosition = row.rectTransform.anchoredPosition;
            CardStack.UpdateStackPositions(top);

            //注意：从空row上转移的时候，应该只有一个top操作，已经获得到了row，直接用row赋值
            EndDragMinus.RaiseEvent(top, this);
            top.slotCount = row.rowNum;
            EndDragAdd.RaiseEvent(top, this);

            return;
        }
        // 主卡可以放到mainRow
        else if (row.rowType == RowType.main && top.cardData.isMainCard)
        {
            Debug.Log("主卡拖到了mainRow上面");
            top.rectTransform.anchoredPosition = row.rectTransform.anchoredPosition;

            //变更拖拽上去的卡状态
            top.isOnMainRow = true;
            top.isOnRow = false;

            //变更row状态
            row.isEmpty = false;    //主卡槽直接置空，TODO清空主卡槽的逻辑传递给消除完的mainRow事件
            //变更所有卡状态
            CardStack.UpdateStackPositions(top);

            EndDragMinus.RaiseEvent(top, this); //成功拖拽，拖上去，只增不减

            return;
        }

        top.rectTransform.anchoredPosition = row.rectTransform.anchoredPosition;

        CardStack.UpdateStackPositions(top);
    }

    private void PlaceOnMainCard(Card target)   //合成到主卡上，从下面到上面
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
                EndDragMinus.RaiseEvent(top, this);
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
            CardStack.OneAddToOne(target, top);

            //设置首次堆叠的样式，但是应该每次在ondrag或者onBeginDrag的时候就设置
            target.transform.SetAsLastSibling();
            top.transform.SetAsLastSibling();

            EndDragMinus.RaiseEvent(top, this);
            top.slotCount = target.slotCount;
            EndDragAdd.RaiseEvent(top, this);
            return;
        }

        // 第二种情况: 单到多（把单张加入到已有的 stack 顶部）
        if (target.cardData.mainId == top.cardData.mainId &&
            target.isFront && target.isInStack && !top.childCards.Contains(target))
        {
            Debug.Log("准备单到多");
            // 允许把单张放到已有堆栈
            CardStack.OneToStack(target, top);

            EndDragMinus.RaiseEvent(top, this);
            top.slotCount = target.slotCount;
            EndDragAdd.RaiseEvent(top, this);
            return;
        }
        //第三种情况，多到单（把已有的stack加入到单张上面

        //第四种情况，多到多（把已有的


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
