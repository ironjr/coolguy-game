using UnityEngine;

public class SwingSkill : BasicSkillBehaviour
{
    public float Elasticity = 3.0f;
    public bool IsLeft = true;
    public float HitboxDegreeFront = 33.0f;
    public float HitboxDegreeRear = -22.0f;
    public PooledObject SwooshFX;
    public PooledObject ClingFX;

    private Transform _transform;
    private float _hitboxDegreeFront;
    private float _hitboxDegreeRear;

    void Awake()
    {
        _transform = transform;
        if (IsLeft)
        {
            _hitboxDegreeFront = 90.0f - HitboxDegreeFront;
            _hitboxDegreeRear = 270.0f - HitboxDegreeRear;
        }
        else
        {
            _hitboxDegreeFront = 90.0f + HitboxDegreeFront;
            _hitboxDegreeRear = -90.0f + HitboxDegreeRear;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject go = other.gameObject;

        // Reflect
        if (go.CompareTag("Projectile"))
        {
            Transform tf = go.transform;
            Vector3 relativePos = tf.position - _transform.position;
            if (IsLeft)
            {
                // 0 - 360
                float direction = ((Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg) + 360.0f % 360.0f);
                if (direction < _hitboxDegreeFront || direction > _hitboxDegreeRear) return;
            }
            else
            {
                // -180 - 180
                float direction = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
                if (direction > _hitboxDegreeFront || direction < _hitboxDegreeRear) return;
            }

            BasicBulletBehaviour behaviour = go.GetComponent<BasicBulletBehaviour>();
            if (behaviour.FlagReflected) return;
            behaviour.Reflect(Elasticity);

            PooledObject clingFX = ClingFX.GetObject(_transform.parent);
            clingFX.transform.SetPositionAndRotation(
                tf.position,
                Quaternion.Euler(new Vector3(0, 180.0f * (IsLeft ? 0 : 1))));
        }
        else if (go.CompareTag("Enemy"))
        {
            BasicEnemyBehaviour behaviour = go.GetComponent<BasicEnemyBehaviour>();
            behaviour.Kill(_transform.parent.parent.gameObject);
        }
    }
    
    public override void OnFetchFromPool()
    {
        base.OnFetchFromPool();
        SwooshFX.GetObject(_transform.parent);
    }
}
