using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowManager : MonoBehaviour
{
    public List<Row> rows = new List<Row>();
    public List<Row> mainRows = new List<Row>();

    public void AddRow(Row rowFromLevelManager)
    {
        rows.Add(rowFromLevelManager);
    }

    public void AddMainRow(Row mainRowFromLevelManger)
    {
        mainRows.Add(mainRowFromLevelManger);
    }

    /* 应该有如下监听的事件，卡牌成功拖拽的时候
    1.更新row的数量，通过
    */
}
