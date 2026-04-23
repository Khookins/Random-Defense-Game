using Pathfinding;
using UnityEngine;

public abstract class Defence : MonoBehaviour
{
    [SerializeField] protected float baseWeightPenalty = 1f;
    public abstract bool AffectsWeight(Node a, Node b);
    public abstract float GetWeightPenalty(Node a, Node b = null);
}
