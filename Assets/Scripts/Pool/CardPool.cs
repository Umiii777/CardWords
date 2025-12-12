using UnityEngine;
using UnityEngine.Pool;

public class CardPool : MonoBehaviour
{
    [Header("卡牌预制体")]
    [SerializeField] private GameObject cardPrefab;

    [Header("预生成数量（根据关卡规模调整）")]
    [SerializeField] private int preloadCount = 100;

    private IObjectPool<GameObject> pool;

    private Transform DragLayer;

    private void Start()
    {
        pool = new ObjectPool<GameObject>(
            createFunc: CreateCard,
            actionOnGet: OnGetCard,
            actionOnRelease: OnReleaseCard,
            actionOnDestroy: OnDestroyCard,
            collectionCheck: false,
            defaultCapacity: preloadCount,
            maxSize: preloadCount * 2
        );
        DragLayer = UIManager.Instance.dragLayer;
        // 提前预热，避免首帧卡顿
        Prewarm();
    }

    private GameObject CreateCard()
    {
        var obj = Instantiate(cardPrefab);
        obj.name = cardPrefab.name; // 保持一致命名方便调试
        return obj;
    }

    private void OnGetCard(GameObject card)
    {
        var r = card.GetComponent<Card>();
        if (r != null)
        {
            r.isOnMainRow = false;
            r.isFromDeck = false;
            r.childCards.Clear(); // default
            r.isFront =false;
        }
        card.SetActive(true);
    }

    private void OnReleaseCard(GameObject card)
    {
        card.SetActive(false);
        card.transform.SetParent(transform); // 回收到pool节点下，保持Hierarchy整洁
    }

    private void OnDestroyCard(GameObject card)
    {
        Destroy(card);
    }

    public GameObject Get() => pool.Get();

    public void Release(GameObject card) => pool.Release(card);

    private void Prewarm()
    {
        var tempList = new System.Collections.Generic.List<GameObject>();
        for (int i = 0; i < preloadCount; i++)
        {
            tempList.Add(pool.Get());
        }
        foreach (var obj in tempList)
        {
            pool.Release(obj);
        }
    }
}