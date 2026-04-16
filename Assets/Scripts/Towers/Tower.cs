using UnityEngine;
using Pathfinding;
using System;

public class Tower : Defense
{
    [SerializeField] protected float AttackDamage = 0f;
    [SerializeField] protected float AttackInterval = 0f;
    [SerializeField] protected float AttackRange = 0f;

    public override bool AffectsWeight(Node a, Node b)
    {
        Vector2 center = new Vector2(transform.position.x, transform.position.z);
        Vector2 pointA = new Vector2(a.transform.position.x, a.transform.position.z);
        Vector2 pointB = new Vector2(b.transform.position.x, b.transform.position.z);

        Vector2 ab = pointB - pointA;
        Vector2 ac = center - pointA;

        float time = Mathf.Clamp01(Vector2.Dot(ac, ab) / ab.sqrMagnitude);
        Vector2 closest = pointA + time * ab;

        return (closest - center).sqrMagnitude <= AttackRange * AttackRange;
    }

    public override float GetWeightPenalty(Node a, Node b = null)
    {
        return baseWeightPenalty;
    }
}
