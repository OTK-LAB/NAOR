using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    private bool invincible = false;
    [SerializeField]
    public float currentHealth;
    [SerializeField]
    public float maxHealth;
    float smoothing = 5;

    public delegate void DamageHandler(float amount);
    public event DamageHandler OnHit;
    public event EventHandler OnDead;

    public bool Invincible { set { invincible = value; } }

    public ProgressBar healthBar;
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxValue(maxHealth);
    }

    private void Update()
    {
        /*if (currentHealth != healthBar.slider.value)
            healthBar.SetValue(Mathf.Lerp(healthBar.slider.value, currentHealth, smoothing * Time.deltaTime));
        */
    }
    public float GetHealth()
    {
        return currentHealth;
    }

    public void Damage(float damageAmount)
    {
        smoothing = 10;
        if (!invincible)
        {
            currentHealth -= damageAmount;
            if (currentHealth <= 0 )
            {
                currentHealth = 0;
            }
            OnHit(currentHealth);
        }
        //healthBar.SetValue(currentHealth);
    }

    public void Heal(float healAmount)
    {
        smoothing = 5;
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnHit(currentHealth);
        //healthBar.SetValue(currentHealth);
    }
}