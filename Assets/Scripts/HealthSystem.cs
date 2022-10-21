using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public bool denemexd = false;
    [SerializeField]
    private float currentHealth;
    [SerializeField]
    private float maxHealth;

    public event EventHandler OnHit;
    public event EventHandler OnDead;

    void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if(denemexd)
        {
            Damage(10.0f);
        }
    }
    public float GetHealth()
    {
        return currentHealth;
    }

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if(currentHealth < 0)
        {
            currentHealth = 0;
            OnDead?.Invoke(this, EventArgs.Empty);
        }

        OnHit?.Invoke(this, EventArgs.Empty);
        denemexd = false;
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}