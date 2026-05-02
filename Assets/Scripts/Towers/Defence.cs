using Pathfinding;
using UnityEngine;

// Base class for defences. Originally was made abstract so we could have towers and also traps which are both types of defences. Traps were eventually scrapped due to scope issues.
public abstract class Defence : MonoBehaviour
{
    [SerializeField] protected float baseWeightPenalty = 1f;
    public abstract bool AffectsWeight(Node a, Node b);
    public abstract float GetWeightPenalty(Node a, Node b = null);
}
