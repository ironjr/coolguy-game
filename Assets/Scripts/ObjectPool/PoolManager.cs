using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;
    public Dictionary<string, ObjectPool> Pools;

    void Awake()
    {
        Instance = this;
        Pools = new Dictionary<string, ObjectPool>();
    }

    public void NewPool(GameObject prefab)
    {
        ObjectPool pool = new ObjectPool(prefab, null, 3, true);
        Pools.Add(prefab.name, pool);
    }

    public void NewPool(GameObject prefab, int pooledAmount)
    {
        ObjectPool pool = new ObjectPool(prefab, null, pooledAmount, true);
        Pools.Add(prefab.name, pool);
    }
    
    public void NewPool(GameObject prefab, int pooledAmount, bool willGrow)
    {
        ObjectPool pool = new ObjectPool(prefab, null, pooledAmount, willGrow);
        Pools.Add(prefab.name, pool);
    }

    public void NewPool(GameObject prefab, Transform parent)
    {
        ObjectPool pool = new ObjectPool(prefab, parent, 3, true);
        Pools.Add(prefab.name, pool);
    }

    public void NewPool(GameObject prefab, Transform parent, int pooledAmount)
    {
        ObjectPool pool = new ObjectPool(prefab, parent, pooledAmount, true);
        Pools.Add(prefab.name, pool);
    }

    public void NewPool(GameObject prefab, Transform parent, int pooledAmount, bool willGrow)
    {
        ObjectPool pool = new ObjectPool(prefab, parent, pooledAmount, willGrow);
        Pools.Add(prefab.name, pool);
    }

    public ObjectPool GetPool(string name)
    {
        return Pools[name];
    }
}
