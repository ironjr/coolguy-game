using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class ObjectPool
{
    public GameObject PoolableObject;
    public Transform ParentTransform = null;
    public int PooledAmount = 3;
    public bool WillGrow = true;

    [System.NonSerialized]
    protected List<GameObject> _pool;
    [System.NonSerialized]
    protected IPoolable _poolable;

    public int Count
    {
        get
        {
            return _pool.Count;
        }
    }

    #region Interface
    public ObjectPool(GameObject go, Transform parent, int pooledAmount, bool willGrow)
    {
        PoolableObject = go;
        ParentTransform = parent;
        PooledAmount = pooledAmount;
        WillGrow = willGrow;

        _poolable = PoolableObject.GetComponent<IPoolable>();
        Assert.IsNotNull(_poolable);
        _pool = new List<GameObject>();
        InitializePool();
	}

    public GameObject GetObject()
    {
        for (int i = 0; i < _pool.Count; ++i)
        {
            if (!_pool[i].activeInHierarchy)
            {
                _poolable.OnCheckout();
                return _pool[i];
            }
        }

        if (WillGrow)
        {
            if (ParentTransform == null)
            {
                GameObject go = GameObject.Instantiate(PoolableObject);
                _pool.Add(go);
            }
            else
            {
                GameObject go = GameObject.Instantiate(PoolableObject, ParentTransform);
                _pool.Add(go);
            }
            _poolable.OnInstantiate();
            _poolable.OnCheckout();
        }

        return null;
    }

    public void InitializePool()
    {
        if (ParentTransform == null)
        {
            for (int i = 0; i < PooledAmount; ++i)
            {
                GameObject go = GameObject.Instantiate(PoolableObject);
                go.SetActive(false);
                _pool.Add(go);
                _poolable.OnInstantiate();
            }
        }
        else
        {
            for (int i = 0; i < PooledAmount; ++i)
            {
                GameObject go = GameObject.Instantiate(PoolableObject, ParentTransform);
                go.SetActive(false);
                _pool.Add(go);
                _poolable.OnInstantiate();
            }
        }
    }

    public void DestroyPool()
    {
        int len = _pool.Count;
        for (int i = 0; i < len; ++i)
        {
            GameObject.Destroy(_pool[0]);
            _pool.RemoveAt(0);
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
