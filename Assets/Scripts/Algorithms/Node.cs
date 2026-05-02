using System.Collections.Generic;
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

        // Sets the heuristic of this specific node, which is a weight based of distance from the end node.
        // This only applies to AStar
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

        // Creates a visual path to neighbor nodes. Is completely visual and has no real effect.
        public void DrawPathsToNeighbors()
        {
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

        // When unity revalidates this node, checks if the neighbor nodes contain this node, and if it doesn't it adds them to their neighbor nodes.
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

        // Similar to the validation function, when the node is destroyed it makes sure to delete itself from other nodes neighbor list.
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
