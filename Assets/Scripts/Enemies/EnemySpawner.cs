using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private RoundData roundData;
    [SerializeField] private Node spawnPoint;
    [SerializeField] private Node goalNode;
    [SerializeField] private Transform enemyParent;
    private readonly HashSet<GameObject> activeEnemies = new HashSet<GameObject>();
    private bool finishedSpawning = false;
    private int waveCounter = 0;

    private void OnEnable()
    {
        Game.OnRoundStarted += HandleRoundStateChanged;
    }

    private void OnDisable()
    {
        Game.OnRoundStarted -= HandleRoundStateChanged;
    }

    private void HandleRoundStateChanged()
    {
        SpawnAllWaves();
    }

    private void SpawnAllWaves()
    {
        finishedSpawning = false;
        activeEnemies.Clear();
        if (roundData.waves.Count > waveCounter)
        {
            StartCoroutine(SpawnWave(roundData.waves[waveCounter]));
        }
    }

    private void CheckRoundComplete()
    {
        if (finishedSpawning && activeEnemies.Count == 0)
        {
            waveCounter++;
            Game.Instance.EndRound();
        }
    }

    private IEnumerator SpawnWave(WaveData wave)
    {
        foreach (EnemySpawnEntry enemyEntry in wave.enemySpawns)
        {
            for (int i = 0; i < enemyEntry.spawnCount; i++)
            {
                Enemy enemy = GameObject.Instantiate(enemyEntry.enemyPrefab, spawnPoint.transform.position, Quaternion.identity, enemyParent);
                FollowPath pathfinder = enemy.GetComponent<FollowPath>();
                pathfinder.SetPath(spawnPoint, goalNode);
                activeEnemies.Add(enemy.gameObject);
                enemy.OnDied += HandleEnemyDeath;
                pathfinder.OnGoalReached += HandleEnemyGoalReached;
                yield return new WaitForSeconds(enemyEntry.spawnDelay);
            }
            yield return new WaitForSeconds(enemyEntry.groupDelay);
        }
        finishedSpawning = true;
        CheckRoundComplete();
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        activeEnemies.Remove(enemy.gameObject);
        CheckRoundComplete();
    }

    private void HandleEnemyGoalReached(GameObject enemyObject)
    {
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        activeEnemies.Remove(enemyObject);
        Game.Instance.TakePlayerDamage(enemy.GetHealth());
        CheckRoundComplete();
    }
}
