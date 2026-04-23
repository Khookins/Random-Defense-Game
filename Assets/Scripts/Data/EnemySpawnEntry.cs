using System;
using UnityEngine;

[Serializable]
public class EnemySpawnEntry
{
    public Enemy enemyPrefab;
    public int spawnCount;
    public float spawnDelay;
    public float groupDelay;
}
