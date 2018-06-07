using UnityEngine;

public class FixedLifeSpanPooled : PooledObject
{
    public float Lifespan = 1.0f;

    protected PlayRandomAudio _playRandomAudio = null;
    protected Animator _animator = null;

    void Awake()
    {
        _playRandomAudio = gameObject.GetComponent<PlayRandomAudio>();
        _animator = gameObject.GetComponent<Animator>();
    }

    public override void OnFetchFromPool()
    {
        if (_playRandomAudio != null)
        {
            _playRandomAudio.Play();
        }
        if (_animator != null)
        {
            _animator.Rebind();
        }
        Invoke("ReturnToPool", Lifespan);
    }

    public override void OnReturnToPool()
    {
        CancelInvoke();
    }
}
