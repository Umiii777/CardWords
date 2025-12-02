using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItemManager : MonoBehaviour
{
    //道具Manager
    /* 道具种类：
            1.提示
            2.洗牌
        每种道具都应该有对应的当前数量管理
     */


    public int currentHintNum = 0;
    public int currentShuffleNum = 0;

    public void UseHint()
    {
        currentHintNum--;

    }





    #region 增加道具数量
    public void AddShuffle(int num)
    {
        currentShuffleNum += num;
    }
    public void AddHint(int num)
    {
        currentHintNum += num;
    }
    #endregion



}
