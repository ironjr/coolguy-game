using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PoolManager manages object pools exist in the game scene.
/// There can only be one instance of the PoolManager and thus it is
/// defined as Singleton.
/// </summary>
public class PoolManager : Singleton<PoolManager> 
{
    /// <summary>
    /// Pools are defined as a generic Dictionary.
    /// </summary>
    [System.NonSerialized]
    protected Dictionary<string, ObjectPool> Pools;

    protected override void OnAwake()
    {
        Pools = new Dictionary<string, ObjectPool>();
    }

    public ObjectPool NewPool(string ID, GameObject prefab, int pooledAmount, bool willGrow)
    {
        ObjectPool pool = new ObjectPool(prefab, pooledAmount, willGrow);
        Pools.Add(ID, pool);
        return pool;
    }

    public ObjectPool GetPool(string name)
    {
        ObjectPool pool;
        try
        {
            pool = Pools[name];
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
        return pool;
    }
}
