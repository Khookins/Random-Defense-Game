using UnityEngine;
using Pathfinding;

// Generates all path visuals of all nodes.
[ExecuteInEditMode]
public class GeneratePathVisual : MonoBehaviour
{
    private Node[] nodes;

    private void Update()
    {
        nodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
    }

    [ContextMenu("Generate Paths")]
    private void RegenerateAllPaths()
    {
        foreach (Node node in nodes)
        {
            node.DrawPathsToNeighbors();
        }
    }
}
