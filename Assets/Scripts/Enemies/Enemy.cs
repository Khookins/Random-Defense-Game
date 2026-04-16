using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected float MaxHealth = 0f;
    [SerializeField] protected float Speed = 0f;
    [SerializeField] protected float DamageMultiplier = 1f;
    private float CurrentHealth = 0f;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
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
        Object.Destroy(gameObject);
    }


}
