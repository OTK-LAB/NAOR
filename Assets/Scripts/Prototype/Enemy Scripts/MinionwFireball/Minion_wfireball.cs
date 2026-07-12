using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion_wfireball : MonoBehaviour
{
    //Animations
    private Animator animator;
    private string currentState;
    const string idle = "EnemyIdle";
    const string hit = "EnemyHit";
    const string attack = "EnemyAttack";
    const string death = "EnemyDeath";
    
    
    //Move
    Vector3 movement;
    bool Moveright = true;

    //Hit
    public float maxHealth = 20;
    public float currentHealth;
    bool hurt = false;
    bool alive = true;


    //Attack
    public float CalculatedTime;
    public float TimeBtwEachShot;
    public GameObject Fireball;
    bool playerOnline = false;
    Transform PlayerPosition;
    public float minimumFiringDistance;
    public float damage = 15;
    private GameObject player;
    bool playerAlive = true;

    void Start()
    {
        animator = GetComponent<Animator>();   
        currentHealth = maxHealth * Random.Range(1, 1.2001f);
        CalculatedTime = TimeBtwEachShot; 
        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (playerAlive)
            CheckAttack();
        AutoMove();
        CheckPlayerDead();
    }
    void flip()
    {
        if (PlayerPosition.position.x > (transform.position.x + 0.5f))
        {
            if (!Moveright)
            {
                transform.Rotate(0f, 180f, 0f);
                Moveright = true;
            }
        }
        else
        {
            if (Moveright)
            {
                transform.Rotate(0f, 180f, 0f);
                Moveright = false;
            }
        }
    }
    void CheckPlayerDead()
    {
        if (player.GetComponent<PlayerManager>().dead == true)
        {
            playerOnline = false;
            playerAlive = false;
        }
        else
            playerAlive = true;
    }
    void CheckAttack()
    {
        if (Vector2.Distance(transform.position, PlayerPosition.position) <= minimumFiringDistance)
        {
            flip();
            playerOnline = true;          
            FireballMechanism();
        }
        else
            playerOnline = false;
    }
    void AutoMove()
    {
        if (!playerOnline)
        {
            if (Moveright)
            {
                movement = new Vector3(2, 0f, 0f);
                transform.position = transform.position + movement * Time.deltaTime;
            }
            else
            {
                movement = new Vector3(-2, 0f, 0f);
                transform.position = transform.position + movement * Time.deltaTime;
            }
        }
    }
   
    private void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.CompareTag("turn")&& !playerOnline)
        {
            if (Moveright) Moveright = false;
            else Moveright = true;
            transform.Rotate(0f, 180f, 0f);
        }
        /*if (trig.CompareTag("Player"))
        {       
            trig.transform.SendMessage("DamagePlayer", damage);
        }*/

    }
    void FireballMechanism()
    {
        if (CalculatedTime <= 0)
        {
            ChangeAnimations();
            Instantiate(Fireball, transform.position, Quaternion.LookRotation(Vector3.forward, transform.position - PlayerPosition.position));
            CalculatedTime = TimeBtwEachShot;
        }
        else
        {
            CalculatedTime -= Time.deltaTime;
            ChangeAnimations();
        }
    }

    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
    }
    void ChangeAnimations()
    {
        //attack
        if (playerOnline && CalculatedTime <= 0)
        { 
            ChangeAnimationState(attack);
            StartCoroutine(backtoIdle());
        }
      
        //hit
        if (hurt && alive)
        {
            ChangeAnimationState(hit);
            StartCoroutine(backtoIdle());
        }

    }
    IEnumerator backtoIdle()
    {
       
        yield return new WaitForSeconds(0.5f);
        if(alive)
            ChangeAnimationState(idle);
        hurt = false;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            alive = false;
            hurt = false;
            Die();           
        }
        else {
            hurt = true;
            ChangeAnimations();
        }
    }
    void Die()
    {
        ChangeAnimationState(death);
        movement = new Vector3(transform.position.x, -3.17f, transform.position.z);
        transform.position = movement;
        GetComponent<Collider2D>().enabled = false;
        ColiseumKey.instance.deadEnemyCount++;
        this.enabled = false;

    }
}
