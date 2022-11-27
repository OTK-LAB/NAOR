using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    private bool invincible = false;
    [SerializeField]
    private float currentHealth;
    [SerializeField]
    private float maxHealth;

    public event EventHandler OnHit;
    public event EventHandler OnDead;

    public bool Invincible { set { invincible = value; } }

    public ProgressBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxValue(maxHealth);
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public void Damage(float damageAmount)
    {

        if (!invincible)
        {
            currentHealth -= damageAmount;
            OnHit?.Invoke(this, EventArgs.Empty); //BATU & ZEYNEP bunu unutma ! hasar animasyonunu oynatýp hasar almamasýný istiyorsak bunu if dýþýna çýkartalým ama düþmanlarý da ona göre düzenleyelim
            if (currentHealth < 0 )
            {
                currentHealth = 0;
                OnDead?.Invoke(this, EventArgs.Empty);             
            }

        }
        healthBar.SetValue(currentHealth);
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar.SetValue(currentHealth);
    }
}