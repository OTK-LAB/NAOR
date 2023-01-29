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
    GameObject parent;
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

        if (!invincible && gameObject.tag== "shield")
        {
            damageAmount = 100;
            parent = this.transform.parent.gameObject;
            newDamageAmount= damageAmount- 35;
            Debug.Log(gameObject.tag+ " ac�mad� ki hehehe " + newDamageAmount);
            if (newDamageAmount >= 0)
            {
                parent.GetComponent<EnemyHealthSystem>().onShield = true;
                parent.GetComponent<EnemyHealthSystem>().Damage(newDamageAmount);     
            }           
        }
        else if (!invincible)
        {
            Debug.Log(gameObject.tag+" girdim, ald���m hasar: " + damageAmount);
            currentHealth -= damageAmount;
            if (!onShield)
                OnHit?.Invoke(this, EventArgs.Empty); //BATU & ZEYNEP bunu unutma ! hasar animasyonunu oynat�p hasar almamas�n� istiyorsak bunu if d���na ��kartal�m ama d��manlar� da ona g�re d�zenleyelim
            else
            {
                OnShield?.Invoke(this, EventArgs.Empty);
                onShield = false;
            }
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnDead?.Invoke(this, EventArgs.Empty);
            }
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