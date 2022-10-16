using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField]
    private int currentHealth;
    [SerializeField]
    private int maxHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void Damage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if(currentHealth < 0)
        {
            currentHealth = 0;
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
