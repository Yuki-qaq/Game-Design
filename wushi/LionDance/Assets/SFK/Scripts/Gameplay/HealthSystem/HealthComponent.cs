
using System;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] protected float health = 100;
    [SerializeField] protected float maxHealth = 100;

    public bool destroyOnDeath = false;
    [Min(0)] public float deathDestroyDelay = 1;

    [Space]
    public UnityEvent DeathEvent;
    public UnityEvent<float> HealthChangeEvent;

    public float Health => health;
    public float MaxHealth => maxHealth;

    public void Heal(float amount)
    {
        health = Mathf.Min(maxHealth, health + amount);
        HealthChangeEvent?.Invoke(health);
    }

    public void Damage(float damage)
    {
        health = Mathf.Max(0, health - damage);
        HealthChangeEvent?.Invoke(health);

        if (health == 0)
            Death();
    }

    private void Death()
    {
        DeathEvent?.Invoke();
        if (destroyOnDeath)
        {
            Destroy(gameObject, deathDestroyDelay);
        }
    }
}
