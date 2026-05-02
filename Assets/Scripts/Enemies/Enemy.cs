using System;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected float MaxHealth = 0f;
    [SerializeField] protected float Speed = 0f;
    [SerializeField] protected float DamageMultiplier = 1f;
    [SerializeField] protected int bleedMoney = 5;
    [SerializeField] protected int deathMoney = 100;
    private float CurrentHealth = 0f;

    public event Action<Enemy> OnDied;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    // Gets the enemy's current health
    public float GetHealth()
    {
        return CurrentHealth;
    }

    // Deals damage to the enemy, if the enemies health goes to zero or lower, it will call the Die function.
    public virtual void TakeDamage(float damage)
    {
        CurrentHealth -= (damage * DamageMultiplier);
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    // What the enemy will do when it dies.
    protected virtual void Die()
    {
        if (this == null) return;
        if (OnDied == null) return;
        OnDied.Invoke(this);
        GameObject.Destroy(gameObject);
    }


}
