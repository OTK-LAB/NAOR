using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    private bool invincible = false;
    [SerializeField]
    public float currentHealth;
    [SerializeField]
    public float maxHealth;

    public event EventHandler OnHit;
    public event EventHandler OnShield;
    public event EventHandler OnDead;
    float newDamageAmount;
    public bool onShield = false;

    public bool Invincible { set { invincible = value; } }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public void Damage(float damageAmount)
    {

        if (!invincible)
        {
            if(gameObject.tag == "shield" )
            {
                OnShield?.Invoke(this, EventArgs.Empty);
                if(onShield)
                {
                    damageAmount = 100;
                    damageAmount = damageAmount - 35;
                    Debug.Log(" acýmadý ki hehehe ");
                }
            }

            currentHealth -= damageAmount;
            if (!onShield)
            {
                OnHit?.Invoke(this, EventArgs.Empty);
                Debug.Log(" vurdun beni god damn it " + damageAmount);
            }
  
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnDead?.Invoke(this, EventArgs.Empty);
            }
            onShield = false;
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}