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

    public event EventHandler OnHit;
    public event EventHandler OnDead;

    public bool Invincible { set { invincible = value; } }

    public ProgressBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetMaxValue(maxHealth);
    }

    private void Update()
    {
        if (healthBar != null)
        {
            if (currentHealth != healthBar.slider.value)
                healthBar.SetValue(Mathf.Lerp(healthBar.slider.value, currentHealth, smoothing * Time.deltaTime));
        }
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
            Debug.Log("Vurdu");
            currentHealth -= damageAmount;
            OnHit?.Invoke(this, EventArgs.Empty); //BATU & ZEYNEP bunu unutma ! hasar animasyonunu oynatýp hasar almamasýný istiyorsak bunu if dýþýna çýkartalým ama düþmanlarý da ona göre düzenleyelim
            if (currentHealth <= 0 )
            {
                currentHealth = 0;
                OnDead?.Invoke(this, EventArgs.Empty);             
            }

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
        //healthBar.SetValue(currentHealth);
    }
}