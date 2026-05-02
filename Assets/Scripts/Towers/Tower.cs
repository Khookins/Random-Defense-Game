using System.Collections;
using Pathfinding;
using UnityEngine;

public class Tower : Defence
{
    [SerializeField] protected float AttackDamage = 0f;
    [SerializeField] protected float AttackInterval = 0f;
    [SerializeField] protected float AttackLength = 0f;
    [SerializeField] protected float AttackRange = 0f;
    [SerializeField] protected GameObject AttackVisual;
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

    // Used by nodes to check if a tower is in range of their path.
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

    // Gets the penalty to apply on nodes for being in range.
    public override float GetWeightPenalty(Node a, Node b = null)
    {
        return baseWeightPenalty;
    }

    // Gets the towers range.
    public float GetRange()
    {
        return AttackRange;
    }

    // Checks if an enemy is in range of the tower to attack.
    protected virtual bool EnemyInRange(Enemy enemy)
    {
        Vector2 center = new Vector2(transform.position.x, transform.position.z);
        Vector2 pointA = new Vector2(enemy.transform.position.x, enemy.transform.position.z);

        float distance = (center - pointA).magnitude;
        return distance <= AttackRange;
    }

    // Attacks an enemy when within range and attack isn't on cooldown.
    protected virtual bool Attack()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            if (!EnemyInRange(enemy)) continue;
            enemy.TakeDamage(AttackDamage);
            StartCoroutine(VisualizeAttack(enemy.transform.position));
            lastAttackedPoint = enemy.transform.position;
            return true;
        }
        return false;
    }

    // Creates a bullet visual to show when a tower attacks an enemy.
    protected virtual IEnumerator VisualizeAttack(Vector3 enemyPosition)
    {
        if (AttackLength <= 0f || AttackVisual == null) yield return null;
        Vector3 midVector = (transform.position + enemyPosition) * 0.5f;
        Vector3 direction = (transform.position - enemyPosition);
        float distance = direction.magnitude;
        Quaternion rotation = Quaternion.LookRotation(direction, transform.up);

        GameObject bullet = GameObject.Instantiate(AttackVisual, midVector, rotation);
        bullet.transform.localScale = new Vector3(1f, 1f, distance);

        float elapsed = 0f;
        while (elapsed < AttackLength)
        {
            elapsed += Time.deltaTime;
            float time = elapsed / AttackLength;

            bullet.transform.localScale = Vector3.Lerp(bullet.transform.localScale, new Vector3(0,0, distance), time);

            yield return null;
        }

        GameObject.Destroy(bullet);
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
