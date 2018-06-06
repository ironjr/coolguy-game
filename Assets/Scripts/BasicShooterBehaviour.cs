using UnityEngine;

public class BasicShooterBehaviour : MonoBehaviour
{
    public float FireRate = 1.0f;
    public float MaximumDistanceToShoot = 5.0f;
    public GameObject Bullet;
    public Transform[] BulletSpawns;

    private float _deltaTime = .0f;
    private float _fireDelay;

    private enum State
    {
        EnableShoot,
        DisableShoot
    }

    private State _state = State.EnableShoot;
    private GameObject _target;

	void Start()
    {
        _fireDelay = 1.0f / FireRate;
        _target = GameObject.FindGameObjectWithTag("Player");
	}
	
	void Update()
    {
        switch (_state)
        {
            case State.EnableShoot:
                _deltaTime += Time.deltaTime;

                if (_target != null && _deltaTime > _fireDelay)
                {
                    _deltaTime -= _fireDelay;

                    Transform targetTransform = _target.transform;
                    float dist = Vector3.Distance(transform.position, targetTransform.position);
                    if (dist < MaximumDistanceToShoot)
                    {
                        int len = BulletSpawns.Length;
                        for (int i = 0; i < len; ++i)
                        {
                            Shoot(ref BulletSpawns[i]);
                        }
                    }
                }
                break;
            case State.DisableShoot:
                _deltaTime = 0.0f;
                break;
        }
	}

    protected void Shoot(ref Transform transform_)
    {
        if (transform_ == null) return;

        // Instantiate the bullet at the mentioned transform.
        GameObject bullet = Instantiate(Bullet, _target.transform);
        bullet.transform.SetPositionAndRotation(transform_.position, new Quaternion());

        // Setup the default behaviour of the bullet.
        BasicBulletBehaviour behaviour = bullet.GetComponent<BasicBulletBehaviour>();
        behaviour.SetTarget(_target);
        behaviour.SetOrigin(gameObject);

        // Setup the behaviour of the linear bullet if it has one.
        LinearBulletBehaviour linear = bullet.GetComponent<LinearBulletBehaviour>();
        if (linear != null)
        {
            linear.SetDirection(transform_.localRotation * Vector3.right);
        }

        // Setup the behaviour of the targeting bullet if it has one.
        //TargetingBulletBehaviour targeting = bullet.GetComponent<TargetingBulletBehaviour>();
    }
}
