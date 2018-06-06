using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float MinX = -4.0f;
    public float MaxX = 4.0f;
    public float MinY = 9.0f;
    public float MaxY = 16.0f;
    public float SpawnRate = 0.3f;
    public WeightedGameObject[] Enemies;

    private float _updateDelay;
    private float _deltaTime = .0f;
    private float _meanSpawnAmount;
    private static BoardManager _boardManager;

    void Awake()
    {
        _boardManager = transform.parent.GetComponentInChildren<BoardManager>();
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

            int numObjects = (int)Mathf.Round(_meanSpawnAmount +
                (float)RandomGenerator.NextGaussianDouble(new System.Random()));

            GameObject newEnemy;
            for (int i = 0; i < numObjects; ++i)
            {
                newEnemy = Instantiate(Enemies[GetIndexFromWeightedRandom(Enemies)].Object);
                newEnemy.transform.SetPositionAndRotation(
                    new Vector3(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY)),
                    new Quaternion());
            }
        }
	}

    // Redundant code.
    private int GetIndexFromWeightedRandom(WeightedGameObject[] list)
    {
        int len = list.Length;
        uint sum = 0;
        for (int i = 0; i < len; ++i)
        {
            sum += list[i].Weight;
        }
        int random = (int)Random.Range(0, sum);

        int idx;
        for (idx = 0; random >= 0 && idx < len; ++idx)
        {
            random -= (int)list[idx].Weight;
        }
        return idx - 1;
    }
}
