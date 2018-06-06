using UnityEngine;

public class SwingSkill : MonoBehaviour
{
    public float Elasticity = 2.0f;
    public GameObject FX;

    void OnEnable()
    {
        Instantiate(FX, transform.parent.transform);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Projectile")) return;
        BasicBulletBehaviour behaviour = other.gameObject.GetComponent<BasicBulletBehaviour>();
        if (behaviour.FlagReflected) return;
        behaviour.Reflect(Elasticity);
    }
}
