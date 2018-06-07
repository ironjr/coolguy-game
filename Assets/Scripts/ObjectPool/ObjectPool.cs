using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// ObjectPool defines the pool of an object (prefab).
/// Each ObjectPool manages one type of GameObject. The ObjectPools
/// are managed by a Singleton PoolManager.
/// </summary>
[System.Serializable]
public class ObjectPool
{
    public GameObject PoolableObject;
    public int PooledAmount = 3;
    public bool WillGrow = true;

    [System.NonSerialized]
    protected List<PooledObject> _pool;

    public int Count
    {
        get
        {
            return _pool.Count;
        }
    }

    #region Interface
    public ObjectPool(GameObject go, int pooledAmount, bool willGrow)
    {
        PoolableObject = go;
        PooledAmount = pooledAmount;
        WillGrow = willGrow;

        PooledObject po = PoolableObject.GetComponent<PooledObject>();
        Assert.IsNotNull(po);
        _pool = new List<PooledObject>();
        InitializePool();
	}

    public PooledObject GetObject()
    {
        for (int i = 0; i < _pool.Count; ++i)
        {
            if (!_pool[i].gameObject.activeInHierarchy)
            {
                _pool[i].gameObject.SetActive(true);
                _pool[i].OnFetchFromPool();
                return _pool[i];
            }
        }

        if (WillGrow)
        {
            GameObject go = GameObject.Instantiate(PoolableObject);
            PooledObject po = go.GetComponent<PooledObject>();
            _pool.Add(po);
            po.OnInstantiate();
            po.OnFetchFromPool();
            return po;
        }

        return null;
    }

    public PooledObject GetObject(Transform parentTransform)
    {
        for (int i = 0; i < _pool.Count; ++i)
        {
            PooledObject po = _pool[i];
            GameObject go = po.gameObject;
            Transform tf = po.transform;
            if (!go.activeInHierarchy)
            {
                tf.SetParent(parentTransform);
                tf.localPosition = new Vector3();
                go.SetActive(true);
                po.OnFetchFromPool();
                return po;
            }
        }

        if (WillGrow)
        {
            GameObject go = GameObject.Instantiate(PoolableObject, parentTransform);
            PooledObject po = go.GetComponent<PooledObject>();
            _pool.Add(po);
            po.OnInstantiate();
            po.OnFetchFromPool();
            return po;
        }

        return null;
    }

    public void InitializePool()
    {
        PoolableObject.SetActive(false);
        for (int i = 0; i < PooledAmount; ++i)
        {
            GameObject go = GameObject.Instantiate(PoolableObject);
            PooledObject po = go.GetComponent<PooledObject>();
            _pool.Add(po);
            po.OnInstantiate();
        }
    }

    public void DestroyPool()
    {
        int len = _pool.Count;
        for (int i = len - 1; i >= 0; --i)
        {
            GameObject.Destroy(_pool[i]);
            _pool.RemoveAt(i);
        }
    }
    #endregion // Interface

    // TODO
    protected void ValidatePool()
    {
        int len = _pool.Count;
        for (int i = 0; i < len; )
        {
            if (_pool[i] == null)
            {
                _pool.RemoveAt(i);
            }
            else
            {
                ++i;
            }
        }
    }
}
