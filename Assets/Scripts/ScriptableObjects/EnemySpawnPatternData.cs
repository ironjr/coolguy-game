using System.Collections.Generic;
using UnityEngine;

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
    public uint Level;
    public List<EnemySpawnPatternEntry> Instances;
}

[CreateAssetMenu]
public class EnemySpawnPatternData : ScriptableObject
{
    public List<EnemySpawnPattern> Patterns;
}
