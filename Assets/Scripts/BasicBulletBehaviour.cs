using UnityEngine;

public class BasicBulletBehaviour : MonoBehaviour, IPoolable
{
    public int Damage = 1;
    public float Speed = 5.0f;
    public Sprite[] BulletSprites;
    public GameObject[] HitEffects;
    public GameObject EquivalentTargetingBullet;

    protected int _numSprites;
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

    #region IPoolable
    public void OnInstantiate() { }
    
    public void OnCheckout() { }

    public void OnReturn() { }
    #endregion

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
            GameObject hitEffect = Instantiate(HitEffects[Random.Range(0, HitEffects.Length)]);
            hitEffect.transform.position = transform.position;
            Destroy(gameObject);
        }
    }

    public void SetSpeed(float speed)
    {
        Speed = speed;
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
            GameObject bullet = Instantiate(EquivalentTargetingBullet, _origin.transform);
            bullet.transform.SetPositionAndRotation(transform.position, new Quaternion());
            TargetingBulletBehaviour behaviour = bullet.GetComponent<TargetingBulletBehaviour>();
            behaviour.SetTarget(_origin);
            behaviour.SetOrigin(_target);
            behaviour._flagReflected = true;
            behaviour.SetSpeed(behaviour.Speed * elasticity);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
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