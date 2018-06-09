using System.Collections;
using UnityEngine;

public class FixedLifespanPO : PooledObject
{
    public float Lifespan = 1.0f;

    public override void OnFetchFromPool()
    {
        StartCoroutine(WaitedReturnToPool());
    }

    public override void OnReturnToPool()
    {
        CancelInvoke();
    }

    public IEnumerator WaitedReturnToPool()
    {
        yield return new WaitForSeconds(Lifespan);
        ReturnToPool();
    }
}
