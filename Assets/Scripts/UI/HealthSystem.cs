using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class HealthSystem
{
    private bool invincible = false;
    [SerializeField] private float currentHealth = 1000;
    [SerializeField] private float maxHealth = 1000;
    [SerializeField] private float damageMultiplier = 1;
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
            OnHealthChanged?.Invoke(value);
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
            OnMaxHealthChanged?.Invoke(value);
        }
    }

    public float DamageMultiplier { get { return damageMultiplier; } set { damageMultiplier = value; } }

    public void Damage(float damageAmount)
    {
        if (!invincible)
        {
            CurrentHealth -= damageAmount * damageMultiplier;
            if (currentHealth <= 0 )
            {
                currentHealth = 0;
                OnDied?.Invoke();
            }
            OnHealthChanged?.Invoke(currentHealth);
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
        OnHealthChanged?.Invoke(currentHealth);
        //healthBar.SetValue(currentHealth);
    }
}