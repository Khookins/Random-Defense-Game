using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using System;

public class FollowPath : MonoBehaviour
{
    public event Action<GameObject> OnGoalReached;

    protected Dijkstra pathFinder;
    [SerializeField] protected PathfindingAlgorithm algorithm = PathfindingAlgorithm.A_Star;
    [SerializeField] protected Node startNode;
    [SerializeField] protected Node goalNode;
    protected int currentNodeIndex = 0;
    protected List<Node> path;
    private bool destroying = false;

    public virtual void SetPath(Node start, Node goal)
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

    protected virtual void Update()
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
