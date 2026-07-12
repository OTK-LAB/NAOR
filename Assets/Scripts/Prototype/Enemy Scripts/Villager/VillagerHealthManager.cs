using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerHealthManager : MonoBehaviour
{
    Animator animator;
    string currentState;
    const string hurt = "VillagerHurt";
    const string death = "VillagerDeath";
    public float maxHealth = 100;
    [SerializeField]
    float currentHealth;
    [HideInInspector] public bool isHurting;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void TakeDamage(float damage)
    {

        currentHealth -= damage;
        ChangeAnimationState(hurt);
        isHurting = true;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Debug.Log("Enemy died!");
        GetComponent<Collider2D>().enabled = false;
        GetComponent<VillagerRunning>().enabled = false;
        GetComponent<VillagerHealthManager>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        
    }
    void ChangeAnimationState(string newState)
    {
        if(currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
    }


    void OnTriggerEnter2D(Collider2D other)
        
    {

       if( other.CompareTag("fire"))
        {
            TakeDamage(100);
        }





        
    }
}
