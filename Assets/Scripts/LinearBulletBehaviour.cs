using UnityEngine;

public class LinearBulletBehaviour : BasicBulletBehaviour
{
    private Vector3 _direction;

    void Start()
    {
        _numSprites = BulletSprites.Length;
    }

    void Update()
    {
        Vector3 bulletPos = transform.position;

        // Setup anguler rotation.
        SetSpriteByDirection(_direction);

        // Setup translational movement.
        float step = _speed * Time.deltaTime;
        transform.position = bulletPos + (_direction * step);
	}

    public void SetDirection(Vector3 direction)
    {
        _direction = direction.normalized;
    }
}
