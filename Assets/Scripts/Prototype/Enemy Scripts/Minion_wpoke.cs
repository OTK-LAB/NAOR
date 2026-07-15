using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion_wpoke : MonoBehaviour
{
    //Animations
    private Animator animator;
    private string currentState;
    const string idle = "enemy_idle";
    const string hit = "enemy_hit";
    const string attack = "enemy_attack";
    const string death = "enemy_death";

    //Move
    [Header("Movement")]
    public float speed = 2;
    private Rigidbody2D rb;
    Vector3 movement;
    bool Moveright = true;
    float step = 2;
    bool change = false;


    //Hit
    [Header("Health")]
    public float maxHealth = 40;
    public float currentHealth;
    bool hurt = false, alive = true;

    //Attack
    [Header("Attack")]
    public float TimeBtwEachShot=3;
    public float minimumFiringDistance, damage = 15;
    float targetx,targety, crx,cry;
    bool collision = false, playerOnline = false, playerAlive = true, ifAttack = false;
    Transform PlayerPosition;
    private GameObject player;


    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindGameObjectWithTag("Player");
        rb = this.GetComponent<Rigidbody2D>();
        cry = transform.position.y;
        playerAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        targetx = PlayerPosition.position.x - transform.position.x;
        targety = PlayerPosition.position.y - transform.position.y;
        moveDecision();
        CheckPlayerDead();
    }
    void moveDecision()
    {
        if (!playerOnline && !collision)
        {
            if (!ifAttack)
            {
                speed=2.5f;
                step = 2f;
                AutoMove();
                CheckAttack();
            }
        }
        if (!collision && playerOnline)
        {
            flip();
            Enemy_AttackMove(targetx, targety);
        }
        else if (collision && playerOnline)
            IIflip();
        if (collision && !playerOnline) //player hortlam��
            attackDirection();
    }
    void attackDirection()
    {
        if (Moveright)
        {
            if(transform.position.y >= cry)
            {
                if (transform.position.x >= crx)
                {
                    playerOnline = false;
                    collision = false;
                    ifAttack = true;
                    flip();
                    waitForAttack();
                }
                else
                    Enemy_Move(crx);
            }
            else if (transform.position.x >= crx)
                Enemy_Movey(-cry);
            else
                Enemy_AttackMove(crx, -cry);
        }
        else
        {
            if (transform.position.y >= cry)
            {
                if (transform.position.x < crx)
                {
                    playerOnline = false;
                    collision = false;
                    ifAttack = true;
                    flip();
                    waitForAttack();
                }
                else
                    Enemy_Move(-crx);
            }
            else if (transform.position.x < crx)
                Enemy_Movey(-cry);
            else
                Enemy_AttackMove(-crx, -cry);
          
        }
    }
    void waitForAttack()
    {
        flip();
        Enemy_Move(0f);
        StartCoroutine(waitdarling());
    }
    IEnumerator waitdarling()
    {
        yield return new WaitForSeconds(TimeBtwEachShot);
        step = 2f;
        ifAttack = false;
        CheckAttack();
    }
    void Enemy_Move(float count)
    {
        Vector3 direction;
        direction = new Vector3(count, 0f, 0f);
        direction.Normalize();
        movement = direction;
        moveCharacter(movement);
    }
    void Enemy_Movey(float count)
    {
        Vector3 direction;
        direction = new Vector3(0f, count, 0f);
        direction.Normalize();
        movement = direction;
        moveCharacter(movement);
    }
    void Enemy_AttackMove (float x, float y)
    {

        Vector3 direction;
        direction = new Vector3(x, y, 0f);
        direction.Normalize();
        movement = direction;
        moveCharacter(movement);
    }
    void moveCharacter(Vector3 direction)
    {
        transform.position=transform.position + direction* speed * Time.deltaTime;
      //  rb.MovePosition((Vector2)transform.position + (direction * speed * Time.deltaTime));
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
    void IIflip()
    {
        if(!change)
        { 
            if (PlayerPosition.position.x > (transform.position.x + 0.5f))
            {
                transform.Rotate(0f, 180f, 0f);
                Moveright = false;
            }
            else
            {
                transform.Rotate(0f, 180f, 0f);
                Moveright = true;
            }
            change = true;
        }
        attackDirection();
    }
    void AutoMove()
    {
        step = 6f;
        if (Moveright)
            Enemy_Move(step);
        else
            Enemy_Move(-step);
    }

    private void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.CompareTag("turn") && !playerOnline)
        {
            if (Moveright) Moveright = false;
            else Moveright = true;
            transform.Rotate(0f, 180f, 0f);
        }
        if (trig.CompareTag("Player"))
        {
            ChangeAnimations();
            trig.transform.SendMessage("DamagePlayer", damage);
            collision = true;
            change = false;
        }

    }
    void CheckAttack()
    {
        if (!playerOnline)
        {
            if (Vector2.Distance(transform.position, PlayerPosition.position) <= minimumFiringDistance)
            {
                speed = 5f;
                crx = transform.position.x;

                playerOnline = true;
                flip();             
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
        {
            playerAlive = true;
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
        if (playerOnline /*&& CalculatedTime <= 0*/)
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
        if (alive)
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
        else
        {
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
        this.enabled = false;

    }

}
