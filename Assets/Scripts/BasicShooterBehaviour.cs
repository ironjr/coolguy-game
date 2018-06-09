using System;
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
    private GameManager _gameManager;

	void Start()
    {
        _fireDelay = 1.0f / FireRate;
        _target = GameObject.FindGameObjectWithTag("Player");
        _gameManager = GameManager.Instance;
	}
	
	void Update()
    {
        if (_gameManager.IsOver)
        {
            _state = State.DisableShoot;
        }

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
                            Shoot(BulletSpawns[i]);
                        }
                    }
                }
                break;
            case State.DisableShoot:
                _deltaTime = 0.0f;
                break;
        }
	}

    protected void Shoot(Transform transform_)
    {
        if (transform_ == null) return;

        // Get the bullet from its pool at the mentioned transform.
        PooledObject bulletPO = Bullet.GetComponent<BasicBulletBehaviour>();
        PooledObject bulletInstance = bulletPO.GetObject(_target.transform);
        Vector3 spawnPosition = transform_.position;
        bulletInstance.transform.SetPositionAndRotation(
            new Vector3(spawnPosition.x, spawnPosition.y),
            new Quaternion());

        // Setup the default behaviour of the bullet.
        BasicBulletBehaviour behaviour = (BasicBulletBehaviour)bulletInstance;
        behaviour.SetTarget(_target);
        behaviour.SetOrigin(gameObject);

        // Setup the behaviour of the linear bullet if it has one.
        try
        {
            LinearBulletBehaviour linear = (LinearBulletBehaviour)bulletInstance;
            linear.SetDirection(transform_.localRotation * Vector3.right);
        }
        catch (InvalidCastException) { }

        // Setup the behaviour of the targeting bullet if it has one.
        //TargetingBulletBehaviour targeting = bullet.GetComponent<TargetingBulletBehaviour>();
    }
}
