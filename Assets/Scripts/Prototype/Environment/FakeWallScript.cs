using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeWallScript : MonoBehaviour
{
    public GameObject deleteThisWhenDestroyed;
    Animator animator;
    public float maxHealth = 1;
    [SerializeField] float currentHealth;
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
        isHurting = true;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        deleteThisWhenDestroyed.SetActive(false);
        Debug.Log("Wall died!");
        GetComponent<Collider2D>().enabled = false;
        GetComponent<FakeWallScript>().enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
        
    {
       if( other.CompareTag("fire"))
        {
            TakeDamage(100);
        }
    }
}
