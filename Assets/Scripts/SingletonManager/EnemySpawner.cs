using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
    public EnemyDictionary Enemies;
    public EnemySpawnPatternData[] Patterns;

    public float MinX = -4.0f;
    public float MaxX = 4.0f;
    public float MinY = 9.0f;
    public float MaxY = 16.0f;
    public float SpawnRate = 0.3f;

    private EnemySpawnPatternData _pattern;
    private float _updateDelay;
    private float _deltaTime = .0f;
    private float _meanSpawnAmount;
    private BoardManager _boardManager;

    void Awake()
    {
        _boardManager = BoardManager.Instance;
        _updateDelay = BoardManager.TILE_SIZE / BoardManager.TILE_SIZE_PIXEL *
            _boardManager.WalkSpeed;
        _meanSpawnAmount = _updateDelay * SpawnRate;
    }

    void Update()
    {
        _deltaTime += Time.deltaTime;

        if (_deltaTime >= _updateDelay)
        {
            // Update deltaTime.
            _deltaTime -= _updateDelay;

            // Get a pattern.
            int numObjects = (int)Mathf.Round(_meanSpawnAmount +
                (float)RandomDistributionGenerator.NextGaussianDouble(new System.Random()));

            // Instantiate a pattern.
            PooledObject newEnemy;
            for (int i = 0; i < numObjects; ++i)
            {
                newEnemy = Enemies.Values[Random.Range(0, Enemies.Values.Count)].GetObject();
                float x = Random.Range(MinX, MaxX);
                float y = Random.Range(MinY, MaxY);
                float z = (y - MinY) / 10.0f;
                newEnemy.transform.SetPositionAndRotation(
                    new Vector3(x, y, z),
                    new Quaternion());
            }
        }
	}

    private void LoadTheme()
    {
        int themeID = _boardManager.ThemeID;
        _pattern = Patterns[themeID];
    }
}
