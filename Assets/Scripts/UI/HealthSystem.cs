using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class HealthSystem
{
    private bool invincible = false;
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private float damageMultiplier;
    //float smoothing = 5;

    public delegate void HealthHandler(float amount);
    public delegate void DeathHandler();
    public event HealthHandler OnHealthChanged;
    public event HealthHandler OnMaxHealthChanged;
    public event DeathHandler OnDied;
    public bool Invincible { set { invincible = value; } }
    public float CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = value;
            OnHealthChanged(value);
        }
    }
    public float MaxHealth
    {
        get
        {
            return maxHealth;
        }
        set
        {
            maxHealth = value;
            OnMaxHealthChanged(value);
        }
    }

    public float DamageMultiplier { get { return damageMultiplier; } set { damageMultiplier = value; } }

    public void Damage(float damageAmount)
    {
        if (!invincible)
        {
            currentHealth -= damageAmount * damageMultiplier;
            if (currentHealth <= 0 )
            {
                currentHealth = 0;
                OnDied();
            }
            OnHealthChanged(currentHealth);
        }
        //healthBar.SetValue(currentHealth);
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnHealthChanged(currentHealth);
        //healthBar.SetValue(currentHealth);
    }
}