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
    public event EventHandler OnFreeze;


    float newDamageAmount;
    [HideInInspector] public bool onShield = false;
    public bool onFreeze = false;
    public float shieldProtection;

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
                   // damageAmount = 100; // silinebilir
                    damageAmount = damageAmount - shieldProtection;
                }
            }
           
            OnFreeze?.Invoke(this, EventArgs.Empty);
            if(onFreeze) {
                damageAmount += 15;
            }
           
            currentHealth -= damageAmount;
            
            if (!onShield)
                OnHit?.Invoke(this, EventArgs.Empty);
  
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnDead?.Invoke(this, EventArgs.Empty);
            }
            onShield = false;
            onFreeze = false;
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