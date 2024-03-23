using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class DamageReciever : MonoBehaviour
{
    protected static readonly string DEFAULT_TYPE = "Default";

    public List<string> damageStrengths = new();
    [Range(0, 1)] public float damageReduction = 0.5f;

    public List<string> damageWeaknesses = new();
    [Min(0)] public float weaknessMultiplier;

    public void AddStrength(string strength) => damageStrengths.Add(strength);
    public void RemoveStrength(string strength) => damageStrengths.Remove(strength);

    public void AddWeakness(string weakness) => damageWeaknesses.Add(weakness);
    public void RemoveWeakness(string weakness) => damageWeaknesses.Remove(weakness);

    protected HealthComponent health;

    private void Start()
    {
        health = GetComponent<HealthComponent>();
    }

    public float Receive(int damage, string type)
    {
        float finalDamage = damage;
        if (damageStrengths.Contains(type))
        {
            finalDamage *= (1 - damageReduction);
        }
        else if (damageWeaknesses.Contains(type))
        {
            finalDamage *= weaknessMultiplier;
        }
        
        if (health)
            health.Damage(finalDamage);

        return finalDamage;
    }

    public void Receive(int damage)
    {
        Receive(damage, DEFAULT_TYPE);
    }
}
