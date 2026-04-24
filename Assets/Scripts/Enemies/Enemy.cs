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

    public float GetHealth()
    {
        return CurrentHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        CurrentHealth -= (damage * DamageMultiplier);
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (this == null) return;
        if (OnDied == null) return;
        OnDied.Invoke(this);
        GameObject.Destroy(gameObject);
    }


}
