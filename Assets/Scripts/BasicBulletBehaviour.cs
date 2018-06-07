using System;
using UnityEngine;

public class BasicBulletBehaviour : PooledObject
{
    public int Damage = 1;
    public float Speed = 5.0f;
    public Sprite[] BulletSprites;
    public PooledObject[] HitEffects;
    public PooledObject EquivalentTargetingBullet;

    protected int _numSprites;
    protected float _speed;
    protected GameObject _origin;
    protected GameObject _target;
    protected bool _flagReflected = false;

    public bool FlagReflected
    {
        get
        {
            return _flagReflected;
        }
    }

    void Awake()
    {
        _speed = Speed;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _target)
        {
            if (_target.CompareTag("Player"))
            {
                _target.GetComponent<PlayerHealth>().ReceiveDamage(Damage, _origin);
            }
            else if (_target.CompareTag("Enemy"))
            {
                _target.GetComponent<BasicEnemyBehaviour>().ReceiveDamage(Damage, _origin);
            }
            PooledObject hitEffect = HitEffects[UnityEngine.Random.Range(0, HitEffects.Length)].GetObject();
            hitEffect.transform.position = transform.position;
            ReturnToPool();
        }
    }

    public override void OnFetchFromPool()
    {
        _speed = Speed;
        base.OnFetchFromPool();
    }

    public void Set_speed(float speed)
    {
        _speed = speed;
    }

    public void SetOrigin(GameObject origin)
    {
        _origin = origin;
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
    }

    public virtual void Reflect(float elasticity)
    {
        if (_origin != null)
        {
            PooledObject bulletPO = EquivalentTargetingBullet.GetObject();
            Transform bulletTransform = bulletPO.transform;
            bulletTransform.SetParent(_origin.transform);
            bulletTransform.SetPositionAndRotation(transform.position, new Quaternion());
            try
            {
                TargetingBulletBehaviour behaviour = (TargetingBulletBehaviour)bulletPO;
                behaviour.SetTarget(_origin);
                behaviour.SetOrigin(_target);
                behaviour._flagReflected = true;
                behaviour.Set_speed(behaviour._speed * elasticity);
            }
            catch (InvalidCastException) { }
            ReturnToPool();
        }
        else
        {
            ReturnToPool();
        }
    }

    protected int SetSpriteByDirection(Vector3 normalizedDirection)
    {
        float temp = Mathf.Acos(normalizedDirection.x) * 180 / Mathf.PI;
        float angle = normalizedDirection.y >= 0 ? temp : -temp;
        int spriteIndex = ((int)Mathf.Floor(angle / 360.0f * _numSprites + .5f) + _numSprites) % _numSprites;
        gameObject.GetComponent<SpriteRenderer>().sprite = BulletSprites[spriteIndex];
        return spriteIndex;
    }
}