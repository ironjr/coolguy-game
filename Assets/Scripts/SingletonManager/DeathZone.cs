using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : Singleton<DeathZone>
{
    public float DeathZoneBoundaryY = -6.4f;
    public float BombingRate = 1.0f;
    public int CameraShakeRate = 10;
    public float MaxDelayedKill = 1.6f;
    public float XMax = 4.8f;
    public float XMin = -4.8f;
    public float YMax = -6.0f;
    public float YMin = -8.0f;
    public PooledObject[] Explosions;
    public PooledObject[] PitHoles;

    private float _bombTimer = 0.0f;
    private int _bombCounter;
    private CameraShake _cameraShake;

    protected override void OnAwake()
    {
        _bombCounter = Random.Range(0, 2 * CameraShakeRate);
        _cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
    }
	
    // Schedule bombing randomly with the bombing rate.
	void Update()
    {
        _bombTimer += Time.deltaTime;
        if (Random.Range(0, 1.0f / BombingRate) < _bombTimer)
        {
            GenerateExplosion(new Vector3(1.0f, 1.0f, 1.0f));
            _bombTimer = 0.0f;
            if (--_bombCounter <= 0)
            {
                _cameraShake.ShakeCamera();
                _bombCounter = Random.Range(0, 2 * CameraShakeRate);
            }
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
        explosionTransform.SetPositionAndRotation(GetRandomCoord(), RandomFlip2D.NextFlipXYRotsqZ());
        explosionTransform.localScale = scale;
        PooledObject pitHole = PitHoles[Random.Range(0, PitHoles.Length)].GetObject();
        pitHole.transform.SetPositionAndRotation(explosionTransform.position, RandomFlip2D.NextFlipXY());
    }

    public void GenerateExplosion(Vector3 coord, Vector3 scale)
    {
        PooledObject explosion = Explosions[Random.Range(0, Explosions.Length)].GetObject();
        Transform explosionTransform = explosion.transform;
        explosionTransform.SetPositionAndRotation(coord, RandomFlip2D.NextFlipXYRotsqZ());
        explosionTransform.localScale = scale;
        PooledObject pitHole = PitHoles[Random.Range(0, PitHoles.Length)].GetObject();
        pitHole.transform.SetPositionAndRotation(explosionTransform.position, RandomFlip2D.NextFlipXY());
    }

    private Vector3 GetRandomCoord()
    {
        return new Vector3(Random.Range(XMin, XMax), Random.Range(YMin, YMax));
    }
}
