using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowManager : MonoBehaviour
{
    public List<Row> rows = new List<Row>();
    public List<Row> mainRows = new List<Row>();

    public int beginNum = -1;
    public int endNum = -1;


    // public void InitMainRows(int mainRowCount)
    // {
    //     mainRows = new List<Row>();

    //     for (int i = 0; i < mainRowCount; i++)
    //     {
    //         Row newMainRow = new Row();
    //         newMainRow.cardCount = 0;
    //         newMainRow.isEmpty = true;
    //         mainRows.Add(newMainRow);
    //     }
    // }
    public void SetDragCard(Card card)
    {
        Debug.Log("正在执行RowSetDragCard");
        beginNum = card.slotCount;
    }
    public void setEndDragCard(Card card)
    {
        Debug.Log("正在执行RowsetEndDragCard");
        endNum = card.slotCount;
    }
    public void OnSuccessDragCard(Object obj)
    {
        Debug.Log("row变更数据");
        Debug.Log(beginNum);
        Debug.Log(endNum);
        rows[beginNum].cardCount--;
        if (endNum < 0)
        {
            return;
        }
        else
        {
            rows[endNum].cardCount++;
            rows[endNum].UpdateRowState();
            rows[beginNum].UpdateRowState();
            Debug.Log("更新完成");

        }

        ResetNums();

    }
    public void ResetNums()
    {
        beginNum = -1;
        endNum = -1;
    }


}
