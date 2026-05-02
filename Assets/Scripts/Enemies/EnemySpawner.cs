using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private RoundData roundData;
    [SerializeField] private Node spawnPoint;
    [SerializeField] private Node goalNode;
    [SerializeField] private Transform enemyParent;
    [SerializeField] public PathfindingAlgorithm currentAlgorithm = PathfindingAlgorithm.A_Star;
    private readonly HashSet<GameObject> activeEnemies = new HashSet<GameObject>();
    private bool finishedSpawning = false;
    private int waveCounter = 0;

    public event Action<PathfindingAlgorithm> OnAlgorithmChanged;

    private void OnEnable()
    {
        Game.OnRoundStarted += SpawnAllWaves;
    }

    private void OnDisable()
    {
        Game.OnRoundStarted -= SpawnAllWaves;
    }

    // Updates the algorithm to use when spawning enemies.
    public void UpdateAlgorithm(PathfindingAlgorithm algorithm)
    {
        currentAlgorithm = algorithm;
        OnAlgorithmChanged.Invoke(algorithm);
    }

    // Spawns all the waves for this current round.
    private void SpawnAllWaves()
    {
        finishedSpawning = false;
        activeEnemies.Clear();
        if (roundData.waves.Count > waveCounter)
        {
            StartCoroutine(SpawnWave(roundData.waves[waveCounter]));
        }
    }

    // Check if the round has been finished.
    private void CheckRoundComplete()
    {
        print(finishedSpawning);
        print(activeEnemies.Count);
        if (finishedSpawning && activeEnemies.Count == 0)
        {
            waveCounter++;
            Game.Instance.EndRound();
        }
    }

    // Spawns a singular wave of enemies.
    private IEnumerator SpawnWave(WaveData wave)
    {
        foreach (EnemySpawnEntry enemyEntry in wave.enemySpawns)
        {
            for (int i = 0; i < enemyEntry.spawnCount; i++)
            {
                Enemy enemy = GameObject.Instantiate(enemyEntry.enemyPrefab, spawnPoint.transform.position, Quaternion.identity, enemyParent);
                FollowPath pathfinder = enemy.GetComponent<FollowPath>();
                pathfinder.SetAlgorithm(currentAlgorithm);
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

    // Code that runs when an enemy in the current wave dies.
    private void HandleEnemyDeath(Enemy enemy)
    {
        FollowPath pathfinder = enemy.GetComponent<FollowPath>();
        activeEnemies.Remove(enemy.gameObject);
        CheckRoundComplete();
        enemy.OnDied -= HandleEnemyDeath;
        pathfinder.OnGoalReached -= HandleEnemyGoalReached;
    }

    // Code than runs when an enemy in the current wave reaches the player base.
    private void HandleEnemyGoalReached(GameObject enemyObject)
    {
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        FollowPath pathfinder = enemy.GetComponent<FollowPath>();
        activeEnemies.Remove(enemyObject);
        CheckRoundComplete();
        enemy.OnDied -= HandleEnemyDeath;
        pathfinder.OnGoalReached -= HandleEnemyGoalReached;
        Game.Instance.TakePlayerDamage(enemy.GetHealth());
    }
}
