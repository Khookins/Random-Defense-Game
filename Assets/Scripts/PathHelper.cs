using Unity.Android.Gradle.Manifest;
using UnityEngine;

namespace Pathfinding
{
    public class PathHelper : FollowPath
    {
        private ParticleSystem particle;

        private void OnEnable()
        {
            Game.OnGameStateChanged += ToggleVisibility;
        }

        private void OnDisable()
        {
            Game.OnGameStateChanged -= ToggleVisibility;
        }
        private void Awake()
        {
            particle = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            SetPath(startNode, goalNode);
        }

        public override void SetPath(Node start, Node goal)
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

        private void ToggleVisibility(Game.GameState state)
        {
            ParticleSystem.EmissionModule emitter = particle.emission;
            emitter.enabled = state == Game.GameState.Preparation ? true : false;
        }

        protected override void Update()
        {
            if (path == null || path.Count == 0) return;
            if (currentNodeIndex < path.Count)
            {
                transform.position = Vector3.MoveTowards(transform.position, path[currentNodeIndex].transform.position, 1f);
                if (Vector3.Distance(transform.position, path[currentNodeIndex].transform.position) < 0.05f)
                {
                    currentNodeIndex++;
                }
            }
            else
            {
                particle.Clear();
                currentNodeIndex = 0;
                transform.position = startNode.transform.position;
                if (path == null || path.Count == 0) return;
                pathFinder.GetAllDefenses();
                path = pathFinder.FindShortestPath
                    (startNode,
                    goalNode);
            }
        }
    }
}
