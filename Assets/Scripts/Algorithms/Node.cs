using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Pathfinding
{
    [ExecuteInEditMode]
    public class Node : MonoBehaviour
    {
        private Transform PathParent;
        public List<Node> Neighbours;
        private GameObject pathPrefab;
        private List<GameObject> generatedPaths;
        private float pathWeight;

        public float PathWeight
        {
            get { return pathWeight; }

            set { pathWeight = value; }
        }

        public float Heuristic { get; set; }

        public float heuristicPathWeight
        {
            get => Heuristic + pathWeight;
        }

        public float SetHeuristic(Vector3 goal)
        {
            Heuristic = Vector3.Distance(transform.position, goal);
            return Heuristic;
        }

        private Node previousNode;

        public Node PreviousNode
        {
            get => previousNode;
            set => previousNode = value;
        }

        // public int X { get; private set; }

        public void Reset()
        {
            pathWeight = float.PositiveInfinity;
            previousNode = null;
        }

        private void Awake()
        {
            pathPrefab = Resources.Load<GameObject>("Path");
            PathParent = GameObject.Find("Paths").transform;
        }

        private void LateUpdate()
        {
            DrawPathsToNeighbors();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, radius: 0.2f);

            Gizmos.color = Color.grey;
            foreach (var node in Neighbours)
            {
                if (node == null) continue;
                Vector3 direction = node.transform.position - transform.position;
                Vector3 right = Vector3.Cross(direction, Vector3.up).normalized * 0.03f;

                Gizmos.DrawRay(transform.position + right, direction);
            }
        }

        public void DrawPathsToNeighbors()
        {
            if (generatedPaths == null) return;
            foreach (GameObject path in generatedPaths)
            {
                GameObject.DestroyImmediate(path);
            }
            generatedPaths.Clear();
            foreach (var node in Neighbours)
            {
                if (node == null) continue;

                Vector3 direction = node.transform.position - transform.position;
                float length = direction.magnitude;
                Vector3 midpoint = transform.position + direction * 0.5f;
                Quaternion rotation = Quaternion.LookRotation(direction.normalized);

                GameObject path = Instantiate(pathPrefab, midpoint + Vector3.down, rotation, PathParent);
                generatedPaths.Add(path);
                path.transform.localScale = new Vector3(path.transform.localScale.x, path.transform.localScale.y, length += 0.5f);
            }
        }

        private void OnValidate() => ValidateNeighbours();

        private void ValidateNeighbours()
        {
            foreach (var node in Neighbours)
            {
                if (node == null) continue;

                if (!node.Neighbours.Contains(this))
                {
                    node.Neighbours.Add(this);
                }
            }
        }

        private void OnDestroy() => RemoveFromNeighbours();
        private void RemoveFromNeighbours()
        {
            foreach (var node in Neighbours)
            {
                if (node == null) continue;
                node.Neighbours.Remove(this);
                node.Neighbours.Remove(null);
            }
        }
    }
}
