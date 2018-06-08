using UnityEngine;

public class SwingSkill : BasicSkillBehaviour
{
    public float Elasticity = 2.0f;
    public bool IsLeft = true;
    public PooledObject FX;

    private Transform _transform;

    void Awake()
    {
        _transform = transform;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject go = other.gameObject;
        if (!go.CompareTag("Projectile")) return;
        if (IsLeft != (go.transform.position.x < 0.0f)) return;
        BasicBulletBehaviour behaviour = other.gameObject.GetComponent<BasicBulletBehaviour>();
        if (behaviour.FlagReflected) return;
        behaviour.Reflect(Elasticity);
    }
    
    public override void OnFetchFromPool()
    {
        base.OnFetchFromPool();
        FX.GetObject(_transform.parent);
    }
}
