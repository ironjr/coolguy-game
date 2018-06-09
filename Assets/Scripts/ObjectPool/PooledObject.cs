using UnityEngine;

/// <summary>
/// Defines basic characteristics of GameObject pooled in object pool.
/// For those objects implementing object pool design concept, make them
/// inherit PooledObject instead of MonoBehaviour.
/// </summary>
/// <example>
/// public class SampleObject : PooledObject
/// {
///     // Optional
///     public override void OnInstantiate() { }
///     public override void OnFetchFromPool() { }
///     public override void OnReturnToPool() { }
/// }
/// </example>
public abstract class PooledObject : MonoBehaviour
{
    /// <summary>
    /// Pool ID for searching the pool.
    /// </summary>
    [SerializeField]
    public string ID;

    /// <summary>
    /// Default pool size when generated.
    /// Default value is 3.
    /// </summary>
    [SerializeField]
    public int PooledAmount = 3;

    /// <summary>
    /// Determines whether the pool can be increased in size if needed.
    /// Default value is true.
    /// </summary>
    [SerializeField]
    public bool WillGrow = true;

    /// <summary>
    /// Get corresponding object pool from PoolManager.
    /// If the pool doesn't exist, calling Pool generates one.
    /// </summary>
    public ObjectPool Pool
    {
        get
        {
            ObjectPool pool = PoolManager.Instance.GetPool(ID);
            if (pool == null)
            {
                pool = PoolManager.Instance.NewPool(ID, gameObject, PooledAmount, WillGrow);
            }
            return pool;
        }
    }

    /// <summary>
    /// Call this method instead of disabling directly.
    /// </summary>
    public void ReturnToPool()
    {
        OnReturnToPool();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Get object from the pool. Short alternatives to
    /// pooledObject.Pool.GetObject().
    /// </summary>
    /// <returns>An instance of an activated object from the pool.</returns>
    public PooledObject GetObject()
    {
        return Pool.GetObject();
    }

    /// <summary>
    /// Get object from the pool. Short alternatives to
    /// pooledObject.Pool.GetObject(parentTransform).
    /// </summary>
    /// <param name="parentTransform">Parent transform of the object instance
    /// will spawn.</param>
    /// <returns>An instance of an activated object from the pool.</returns>
    public PooledObject GetObject(Transform parentTransform)
    {
        return Pool.GetObject(parentTransform);
    }

    /// <summary>
    /// Called when this object is instantiated by the pool.
    /// </summary>
    public virtual void OnInstantiate() { }

    /// <summary>
    /// Called when this object is checked out from the pool.
    /// </summary>
    public virtual void OnFetchFromPool() { }

    /// <summary>
    /// Called when this object is disabled and returning to the pool.
    /// </summary>
    public virtual void OnReturnToPool() { }
}
