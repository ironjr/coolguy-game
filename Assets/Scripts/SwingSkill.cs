using UnityEngine;

public class SwingSkill : BasicSkillBehaviour
{
    public float Elasticity = 2.0f;
    public bool IsLeft = true;
    public PooledObject SwooshFX;
    public PooledObject ClingFX;

    private Transform _transform;

    void Awake()
    {
        _transform = transform;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject go = other.gameObject;
        if (!go.CompareTag("Projectile")) return;

        Transform tf = go.transform;
        if (IsLeft != (tf.position.x < 0.0f)) return;

        BasicBulletBehaviour behaviour = other.gameObject.GetComponent<BasicBulletBehaviour>();
        if (behaviour.FlagReflected) return;
        behaviour.Reflect(Elasticity);

        PooledObject clingFX = ClingFX.GetObject(_transform.parent);
        clingFX.transform.SetPositionAndRotation(
            tf.position,
            Quaternion.Euler(new Vector3(0, 180.0f * (IsLeft ? 0 : 1))));
    }
    
    public override void OnFetchFromPool()
    {
        base.OnFetchFromPool();
        SwooshFX.GetObject(_transform.parent);
    }
}
