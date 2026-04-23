using UnityEngine;
using Pathfinding;
using System;

public class Tower : Defence
{
    [SerializeField] protected float AttackDamage = 0f;
    [SerializeField] protected float AttackInterval = 0f;
    [SerializeField] protected float AttackRange = 0f;
    [SerializeField] protected TargetingMode targetingMode = TargetingMode.First;
    private float cooldown = 0f;
    private Vector3 lastAttackedPoint = Vector3.zero;

    private void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0 && Attack())
        {
            cooldown = AttackInterval;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        if (lastAttackedPoint != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position,lastAttackedPoint);
            lastAttackedPoint = Vector3.zero;
        }
    }

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

    public float GetRange()
    {
        return AttackRange;
    }

    protected virtual bool EnemyInRange(Enemy enemy)
    {
        Vector2 center = new Vector2(transform.position.x, transform.position.z);
        Vector2 pointA = new Vector2(enemy.transform.position.x, enemy.transform.position.z);

        float distance = (center - pointA).magnitude;
        return distance <= AttackRange;
    }

    protected virtual bool Attack()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            if (!EnemyInRange(enemy)) continue;
            enemy.TakeDamage(AttackDamage);
            lastAttackedPoint = enemy.transform.position;
            return true;
        }
        return false;
    }
}

public enum TargetingMode
{
    First,
    Last,
    Closest,
    Farthest,
    Random
}
