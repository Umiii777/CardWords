using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowManager : MonoBehaviour
{
    public List<Row> rows;
    public List<Row> mainRows;

    public int beginNum = -1;
    public int endNum = -1;

    // public void InitRows(List<Row> inRows)
    // {
    //     rows = new List<Row>();
    //     rows = inRows;

    // }
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

        rows[beginNum].cardCount--;

        if (endNum >= 0)
        {
            rows[endNum].cardCount++;
            rows[endNum].UpdateRowState();
        }

        rows[beginNum].UpdateRowState();

        ResetNums();

    }
    public void ResetNums()
    {
        beginNum = -1;
        endNum = -1;
    }


}
