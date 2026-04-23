using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using System;

public class FollowPath : MonoBehaviour
{
    public event Action<GameObject> OnGoalReached;

    private Dijkstra pathFinder;
    [SerializeField] private PathfindingAlgorithm algorithm = PathfindingAlgorithm.A_Star;
    [SerializeField] private Node startNode;
    [SerializeField] private Node goalNode;
    private int currentNodeIndex = 0;
    private List<Node> path;
    bool destroying = false;

    public void SetPath(Node start, Node goal)
    {
        startNode = start;
        goalNode = goal;

        if (algorithm == PathfindingAlgorithm.A_Star)
        {
            pathFinder = gameObject.AddComponent<AStar>();
        }
        else
        {
            pathFinder = gameObject.AddComponent<Dijkstra>();
        }

        pathFinder.GetAllNodes();
        pathFinder.GetAllDefenses();

        path = pathFinder.FindShortestPath
            (startNode,
            goalNode);
        pathFinder.DebugPath(path);
    }

    private void Update()
    {
        if (path == null || path.Count == 0 || destroying) return;
        if (currentNodeIndex < path.Count)
        {
            transform.position = Vector3.MoveTowards(transform.position, path[currentNodeIndex].transform.position, 0.05f);
            if (Vector3.Distance(transform.position, path[currentNodeIndex].transform.position) < 0.05f)
            {
                currentNodeIndex++;
            }
        }
        else
        {
            destroying = true;
            OnGoalReached.Invoke(gameObject);
            GameObject.Destroy(gameObject);
        }
    }
}

public enum PathfindingAlgorithm
{
    A_Star,
    Dijkstra
}
