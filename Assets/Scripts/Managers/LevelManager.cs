using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("测试相关")]
    public int testNum = 115;

    [Header("生成的列位置相关")]
    public float rowY = 240;
    public float mainRowY = 500;
    public float startX = 0f;
    private int maxRowNum = 5;
    public RectTransform canvas;
    [Header("生成的卡牌位置相关")]
    private float cardColumnOffset = 50;

    [Header("生成的卡牌动画———关卡进入发牌")]
    public Vector2 anim_DealPosStart;
    private Vector2 anim_DealPosEnd;
    public float flyTime = 0.1f;
    float dealInterval = 0.08f; // 发牌节奏




    [Header("对象池")]
    public CardPool cardPool;
    public RowPool rowPool;
    public MainRowPool mainRowpool;

    [Header("引用")]
    public StepManager stepManager;
    private Transform dragLayer;
    private Transform rowLayer;
    public DeckManager cardDeck;
    public RowManager rowManager;

    public CardLayoutManager cardLayoutManager;

    [Header("临时关卡数据")]
    private LevelData currentLevelData;
    private List<WordsData> currentWordsData = new List<WordsData>();
    private int currentCardInitNum;

    private int currentlevelcardsNum;
    private int curretNormalCardsnum;
    private int currentMainCardsNum;

    private List<CardData> currentNormalCardDatas = new List<CardData>();
    private List<CardData> currentMainCardDatas = new List<CardData>();
    private List<CardData> currentTotalCardDatas = new List<CardData>();

    private List<CardData> currentCardDeckDatas = new List<CardData>();

    private List<Card> currentCardsOnSlot = new List<Card>();

    private float[] currentRowX;
    private float[] currentMainRowX;

    public int currentLevelNum;

    public static LevelManager Instance;

    private List<Row> mornalRowPoolList = new List<Row>();
    private List<Row> mainRowPoolList = new List<Row>();

    // [Header("事件")]
    // public ObjectEventSO completeLevel;


    private void Awake()
    {
        Instance = this;

        cardLayoutManager = new CardLayoutManager();
        PlayerPrefs.DeleteAll();
        Debug.Log(PlayerProgress.GetCurrentLevel());
        PlayerProgress.SetCurrentLevel(101);
    }
    private void Start()
    {
        dragLayer = UIManager.Instance.dragLayer;
        rowLayer = UIManager.Instance.rowLayer;
    }
    #region  关卡初始化
    public void InitCurrentLevel(int currentLevelNUm)
    {
        ClearLevel();


        //清空此前数据
        currentNormalCardDatas.Clear();
        currentMainCardDatas.Clear();
        currentTotalCardDatas.Clear();
        currentWordsData.Clear();//用于从words表中获取内容的操作列表
        currentCardDeckDatas.Clear();
        currentCardsOnSlot.Clear();
        rowManager.rows.Clear();
        rowManager.mainRows.Clear();

        currentCardInitNum = 0;// 根据这个顺序先给下方的卡牌赋值

        //数据层
        currentLevelData = LevelConfigLoader.Instance.GetLevelData(currentLevelNUm);

        stepManager.InitSetSteps(currentLevelData.steps);
        LevelHint.Instance.SetLevelHint(currentLevelNUm);
        //表现层,可用列数，可用主槽位数，初始的卡牌排布
        int[] normalRows = currentLevelData.rows;
        int[] mainGroup = currentLevelData.mainGroup;
        int mainRows = currentLevelData.column;  //是否应该从这里改变整体的unlockRowNum

        Debug.Log(normalRows.Count());
        Debug.Log(mainRows);

        //设置关卡的列排布
        SetLevelLayout(normalRows, mainRows);
        //初始化关卡数据,得到的总卡结果是：currentTotalCardDatas
        SetLevelCardsData(currentLevelData);
        //初始化牌组内容


        //表现层，根据排列数组生成卡牌,同时也把剩下的数据给了CardDeck
        SetLevelCardLayout(normalRows);
        AudioManager.Instance.PlayBGM(AudioManager.BGMType.PlayTheme);

    }
    #endregion
    #region 处理关卡整体排布
    public void SetLevelLayout(int[] rows, int mainRow)
    {
        //规定行列的高
        //数据层，获得rows的所有排列方式
        currentRowX = cardLayoutManager.SetRowLayoutX(rows.Count());
        currentMainRowX = cardLayoutManager.SetMainRowLayoutX(5);
        //处理row
        SetLevelRow(currentRowX, rows);
        //处理mainrow
        SetLevelMainRow(currentMainRowX, mainRow);
    }
    #endregion
    //生成关卡的普通列分布
    #region 处理关卡普通列

    public void SetLevelRow(float[] rowx, int[] rows)
    {
        for (int i = 0; i < rowx.Count(); i++)
        {
            //初始化内容
            GameObject normalRow = rowPool.Get();
            RectTransform currentTransform = normalRow.GetComponent<RectTransform>();
            Row currentRow = normalRow.GetComponent<Row>();
            currentRow.cardCount = rows[i];
            currentRow.rowNum = i;
            currentRow.rowType = RowType.normal;
            rowManager.AddRow(currentRow);
            CardManager.Instance.allRows.Add(currentRow);
            mornalRowPoolList.Add(currentRow);



            //层级位置设置
            currentTransform.SetParent(canvas, false);
            currentTransform.anchoredPosition = new Vector2(rowx[i] + startX, rowY);
            currentTransform.SetParent(rowLayer, false);
            //normalRow.transform.position = new Vector2(rowx[i] + (float)startX, rowY);


        }
        Debug.Log(rowManager.rows);
    }
    #endregion
    #region 处理关卡主列
    //生成关卡的主列分布
    public void SetLevelMainRow(float[] mainRowx, int mainrow)
    {
        for (int i = 0; i < mainRowx.Count(); i++)
        {
            if (i < mainrow)
            {
                GameObject mainRow = mainRowpool.Get();
                Row currentMainRow = mainRow.GetComponent<Row>();
                RectTransform currentTransform = currentMainRow.GetComponent<RectTransform>();
                currentMainRow.isEmpty = true;
                currentMainRow.rowType = RowType.main;
                currentMainRow.OnUpdateMainRowType();
                CardManager.Instance.allRows.Add(currentMainRow);
                mainRowPoolList.Add(currentMainRow);

                rowManager.mainRows.Add(currentMainRow);
                currentTransform.SetParent(canvas, false);
                currentTransform.anchoredPosition = new Vector2(mainRowx[i] + startX, mainRowY);
                currentTransform.SetParent(rowLayer, false);
            }
            else
            {
                GameObject mainRow = mainRowpool.Get();
                Row currentMainRow = mainRow.GetComponent<Row>();
                RectTransform currentTransform = currentMainRow.GetComponent<RectTransform>();
                currentMainRow.isEmpty = true;
                currentMainRow.rowType = RowType.unlocked;
                currentMainRow.OnUpdateMainRowType();
                CardManager.Instance.allRows.Add(currentMainRow);
                mainRowPoolList.Add(currentMainRow);

                rowManager.mainRows.Add(currentMainRow);
                currentTransform.SetParent(canvas, false);
                currentTransform.anchoredPosition = new Vector2(mainRowx[i] + startX, mainRowY);
                currentTransform.SetParent(rowLayer, false);
            }

        }
    }
    #endregion
    #region 读取关卡排列,生成卡牌
    //读取关卡的卡牌排列
    public void SetLevelCardLayout(int[] rows)
    {
        //确定共有多少个普通列卡牌
        for (int i = 0; i < rows.Count(); i++)
        {
            //按照列配置的值去生成这个值对应的列卡牌数量
            for (int j = 0; j < rows[i]; j++, currentCardInitNum++)
            {
                //使用对象池生成卡牌GameObject，初始化carddata
                GameObject cardObj = cardPool.Get();
                Card card = cardObj.GetComponent<Card>();

                card.cardData = currentTotalCardDatas[currentCardInitNum];

                card.slotCount = i;
                //确认生成的牌，如果在最底部，贼直接揭示，如果不是，则先背面朝上，TODO，待优化为一个处理的事件Check
                if (j + 1 == rows[i])
                {
                    card.isFront = true;
                    card.SetCardVisual();
                }
                else
                {
                    card.isFront = false;
                    card.SetCardVisual();
                }
                //场上的卡设置为在场上
                card.isOnRow = true;
                //初始化结束后，加入slot
                currentCardsOnSlot.Add(card);
                rowManager.rows[i].cardsOnRow.Add(card);

                //改变生成的卡牌的布局
                RectTransform currentTransform = card.GetComponent<RectTransform>();
                currentTransform.SetParent(canvas, false);
                currentTransform.localScale = Vector3.one;
                //currentTransform.anchoredPosition = new Vector2(currentRowX[i] + startX, rowY - j * cardColumnOffset);
                currentTransform.SetParent(dragLayer, false);
                currentTransform.localScale = Vector3.one;


                //发牌动画
                currentTransform.anchoredPosition = anim_DealPosStart;
                anim_DealPosEnd = new Vector2(currentRowX[i] + startX, rowY - j * cardColumnOffset);
                float delay = currentCardInitNum * dealInterval;
                currentTransform.DOAnchorPos(anim_DealPosEnd, flyTime).SetDelay(delay).SetEase(Ease.OutCubic);



            }
        }
        //TODO：将剩下的卡牌分给CardDeck,数据传递
        currentCardDeckDatas = currentTotalCardDatas.Skip(currentCardInitNum).ToList();
        cardDeck.InitCardDeck(currentCardDeckDatas);
    }
    #endregion
    #region 处理关卡卡牌数据
    //获得关卡卡牌的词汇数据
    public void SetLevelCardsData(LevelData currentLevelData)
    {
        int[] currentWordsID = currentLevelData.mainGroup;
        for (int i = 0; i < currentWordsID.Count(); i++)
        {
            //当前关卡的所有主主词汇数量
            currentWordsData.Add(LevelConfigLoader.Instance.GetWordsData(currentWordsID[i]));
        }

        //初始化卡牌数据
        for (int i = 0; i < currentWordsData.Count(); i++)
        {
            //初始化普通卡牌数据
            for (int j = 0; j < currentWordsData[i].words.Count(); j++)
            {
                currentNormalCardDatas.Add(new CardData
                {
                    languageId = currentWordsData[i].words[j],
                    isMainCard = false,
                    cardContent = LevelConfigLoader.Instance.GetTextById(currentWordsData[i].words[j]),
                    mainId = currentWordsData[i].id
                });
            }
            //初始化主题卡牌数据
            currentMainCardDatas.Add(new CardData
            {
                languageId = currentWordsData[i].theme,
                isMainCard = true,
                cardContent = LevelConfigLoader.Instance.GetTextById(currentWordsData[i].theme),
                mainId = currentWordsData[i].id,
                mainTotalLength = currentWordsData[i].words.Count()
            });

        }
        //得到总表
        currentTotalCardDatas = currentNormalCardDatas.Concat(currentMainCardDatas).ToList();
        //洗掉总表
        currentTotalCardDatas.Shuffle();
        Debug.Log("当前关卡牌总数量" + currentTotalCardDatas.Count);
        Debug.Log("当前关卡的第一个词组是" + currentTotalCardDatas[0].cardContent + currentTotalCardDatas[0].mainTotalLength);
    }

    #endregion
    #region 完成关卡
    //完成关卡，玩家数据加1
    public async void OnLevelComplete()
    {
        currentLevelNum++;
        Debug.Log("关卡胜利");
        PlayerProgress.SetCurrentLevel(currentLevelNum);

        await SystemUIManager.LoadUI(UIType.Victory);
    }
    #endregion

    #region 关卡失败
    public void OnLevelDefeat()
    {

        Debug.Log("关卡失败");
        if (CardManager.Instance.allCards.Count != 0)
        {
            _ = SystemUIManager.LoadUI(UIType.Defeat, 0);
        }

    }
    #endregion

    #region 清除关卡
    public void ClearLevel()
    {
        //Debug.LogError("HasInClearLevel");
        rowManager.rows.Clear();
        rowManager.mainRows.Clear();
        DeckManager.Instance.LevelStartResetDeck();

        List<Card> cards = CardManager.Instance.allCards;
        for (int i = 0; i < cards.Count; i++)
        {
            cardPool.Release(cards[i].gameObject);
        }
        cards.Clear();
        CardManager.Instance.allCards.Clear();


        List<Row> rows = CardManager.Instance.allRows;

        foreach (var normalRow in mornalRowPoolList)
        {
            rowPool.Release(normalRow.gameObject);
        }
        foreach (var mainRow in mainRowPoolList)
        {
            mainRowpool.Release(mainRow.gameObject);
        }
        mornalRowPoolList.Clear();
        mainRowPoolList.Clear();
        rows.Clear();


        CardManager.Instance.allRows.Clear();


    }
    #endregion
    #region 测试功能方法组
    [ContextMenu("直接通关,到下一关")]
    public void TestLevelComplete()
    {
        currentLevelNum++;
        PlayerProgress.SetCurrentLevel(currentLevelNum);
        _ = SystemUIManager.LoadUI(UIType.Victory, 0);
    }
    [ContextMenu("直接从11关开始")]
    public void TestLevel30()
    {
        currentLevelNum = testNum;
        PlayerProgress.SetCurrentLevel(currentLevelNum);
        _ = SystemUIManager.LoadUI(UIType.Victory, 0);
    }
    #endregion

}
