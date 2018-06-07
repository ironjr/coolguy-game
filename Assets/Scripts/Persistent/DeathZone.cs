using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : Singleton<DeathZone>
{
    public float DeathZoneBoundaryY = -6.4f;
    public float BombingRate = 0.7f;
    public float MaxDelayedKill = 1.6f;
    public PooledObject[] Explosions;

    public float XMax = 4.8f;
    public float XMin = -4.8f;
    public float YMax = -6.0f;
    public float YMin = -8.0f;

    private float _timer;

	void Start()
    {
        _timer = 0.0f;
	}
	
    // Schedule bombing randomly with the bombing rate.
	void Update()
    {
        _timer += Time.deltaTime;
        if (Random.Range(0, 1.0f / BombingRate) < _timer)
        {
            GenerateExplosion(new Vector3(1.0f, 1.0f, 1.0f));
            _timer = 0.0f;
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;
        if (collidedObject.CompareTag("Enemy"))
        {
            StartCoroutine(WaitedKill(collidedObject));
        }
    }
    
    public IEnumerator WaitedKill(GameObject toKill)
    {
        yield return new WaitForSeconds(Random.Range(0, MaxDelayedKill));
        if (toKill != null)
        {
            GenerateExplosion(toKill.transform.position, new Vector3(1, 1));
            BasicEnemyBehaviour behaviour = toKill.GetComponent<BasicEnemyBehaviour>();
            behaviour.ReceiveDamage((int)behaviour.MaxHealth, gameObject);
        }
    }

    public void GenerateExplosion(Vector3 scale)
    {
        PooledObject explosion = Explosions[Random.Range(0, Explosions.Length)].GetObject();
        Transform explosionTransform = explosion.transform;
        explosionTransform.SetPositionAndRotation(GetRandomCoord(), GetRandomQuaternion());
        explosionTransform.localScale = scale;
    }

    public void GenerateExplosion(Vector3 coord, Vector3 scale)
    {
        PooledObject explosion = Explosions[Random.Range(0, Explosions.Length)].GetObject();
        Transform explosionTransform = explosion.transform;
        explosionTransform.SetPositionAndRotation(coord, GetRandomQuaternion());
        explosionTransform.localScale = scale;
    }

    private Vector3 GetRandomCoord()
    {
        return new Vector3(Random.Range(XMin, XMax), Random.Range(YMin, YMax));
    }

    private Quaternion GetRandomQuaternion()
    {
        int random = Random.Range(0, 16);
        int flipX = random & 0x1;
        int flipY = random & 0x2;
        int rotate = random >> 2;
        return Quaternion.Euler(flipX * 180.0f, flipY * 180.0f, rotate * 90.0f);
    }
}
