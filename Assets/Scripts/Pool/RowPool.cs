using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class RowPool : MonoBehaviour
{
    [Header("列预制体")]
    [SerializeField] private GameObject rowPrefab;

    [Header("预生成数量（根据关卡规模调整）")]
    [SerializeField] private int preloadCount = 3;

    private IObjectPool<GameObject> pool;

    private void Start()
    {
        pool = new ObjectPool<GameObject>(
            createFunc: CreateRow,  
            actionOnGet: OnGetRow,
            actionOnRelease: OnReleaseRow,
            actionOnDestroy: OnDestroyRow,
            collectionCheck: false,
            defaultCapacity: preloadCount,
            maxSize: preloadCount * 2
        );
        Prewarm();
    }

    private GameObject CreateRow()
    {
        var obj = Instantiate(rowPrefab);
        obj.name = rowPrefab.name;
        return obj;
    }
    private void OnGetRow(GameObject row)
    {
        row.SetActive(true);
    }
    private void OnReleaseRow(GameObject row)
    {
        row.SetActive(false);
    }
    private void OnDestroyRow(GameObject row)
    {
        Destroy(row);
    }
    public GameObject Get() => pool.Get();
    public void Release(GameObject row) => pool.Release(row);

    private void Prewarm()
    {
        //预生成对象池
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
