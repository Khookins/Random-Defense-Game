using System;
using TMPro;
using UnityEngine;

public class PathfindingDropdown : MonoBehaviour
{
    private EnemySpawner spawner;
    private TMP_Dropdown dropdown;

    private void OnEnable()
    {
        Game.OnGameStateChanged += SetEnabled;
    }

    private void OnDisable()
    {
        Game.OnGameStateChanged -= SetEnabled;
    }

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    private void Start()
    {
        spawner = Game.Instance.GetComponent<EnemySpawner>();
    }

    private void SetEnabled(Game.GameState state)
    {
        dropdown.interactable = state == Game.GameState.Preparation;
    }

    public void UpdateAlgorithm(Int32 value)
    {
        if (value == 0)
        {
            spawner.UpdateAlgorithm(PathfindingAlgorithm.A_Star);
        }
        else
        {
            spawner.UpdateAlgorithm(PathfindingAlgorithm.Dijkstra);
        }
    }
}
