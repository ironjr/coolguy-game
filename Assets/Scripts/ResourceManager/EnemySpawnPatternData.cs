using System.Collections.Generic;

[System.Serializable]
public class EnemySpawnPatternEntry
{
    public int EnemyID;
    public float SpawnX;
    public float SpawnY;
    public bool WaitForNextEntry;
    public float WaitAmount;
}

[System.Serializable]
public class EnemySpawnPattern
{
    public List<EnemySpawnPatternEntry> Pattern;
}

[System.Serializable]
public class EnemySpawnPatternData
{
    public Dictionary<int, string> EnemyDictionary;
    public EnemySpawnPattern[] Patterns;
}
