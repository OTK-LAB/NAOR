using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Behaviour : MonoBehaviour
{
    #region public variables
    public float attackDistance; //minimum distance for attack
    public float moveSpeed;
    public float timer; //cooldown between attacks
    [HideInInspector] public Transform target;
    [HideInInspector] public bool inRange;
    public GameObject hotZone;
    public GameObject triggerArea;

    public GameObject deathColl;
    public Animator anim;
    public Transform leftLimit;
    public Transform rightLimit;
    public Collider2D[] colliders;
    //Hit
    public float maxHealth = 110;
    public float currentHealth;
    private bool alive = true;
    Vector3 movement;
    #endregion

    #region private variables
    private GameObject player;
    private float distance; //distance btween enemy and player

    private bool isAvailable;
    private float intTimer;
    private bool isHurt;

    private bool attackMode;
    
    

    Transform PlayerPosition;
    #endregion

    private void Awake()
    {
        isAvailable = true;
        SelectTarget();
        intTimer = timer;
        anim = GetComponent<Animator>();

        
    }
    void Start()
    {
        currentHealth = maxHealth;
        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindGameObjectWithTag("Player");
        colliders = gameObject.GetComponentsInChildren<Collider2D>();
    }

    void Update()
    {
            if (!attackMode)
            {
                CloseGap();
            }

            if (!InsideOfLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
            {
                SelectTarget();
            }

            if (inRange == true)
            {
                EnemyLogic();
            }


        if (!isAvailable)
        {
            Cooldown();
        }

        
    }

    void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.position);

        if(distance > attackDistance)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
            {
                attackMode = false;
            }
        }
        
        if (distance <= attackDistance)  //sald�r� menzili i�inde
        {
            attackMode = true;
            Attack();
        }

    }

    void CloseGap()
    {
            anim.Play("Enemy_walk"); //animasyon

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack")) //animation name here
        {
            Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);     
        }
    }

    void Attack()
    {
        if (isHurt && !isAvailable && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            return;
        }
        if (!isAvailable)
        {
            anim.Play("Enemy_idle");
            return;
        }
        else if (isAvailable)
        {
            anim.Play("Enemy_attack");
        }
        
        timer = intTimer; //reset timer when player attacks

    }

    void Cooldown()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            isAvailable = true;
            timer = intTimer;
        }
    }

    public void TriggerCooldown()     //could be improvised / is used in animation events
    {
        isAvailable = false;
        attackMode = false;
    }

    private bool InsideOfLimits() //limit thingy
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;

    }
    public void SelectTarget()
    {
        float distanceToLeft = Vector2.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector2.Distance(transform.position, rightLimit.position);

        if (distanceToLeft > distanceToRight)
        {
            target = leftLimit;
        }
        else if (distanceToLeft < distanceToRight)
        {
            target = rightLimit;
        }
        Flip();
    }

    public void Flip()
    {
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            Vector3 rotation = transform.eulerAngles;
            if(transform.position.x > target.position.x)
            {
              rotation.y = 180f;
            }
            else if (transform.position.x < target.position.x)
            {
              rotation.y = 0f;
            }
            transform.eulerAngles = rotation;
        }
        

    }

    private void CheckforPlayer()               //animation event     //could be improvised
    {
        if(inRange == false)
        {
            attackMode = false;
        }
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            isHurt = true;
            anim.Play("Enemy_hit");    
        }
    }
    public void isHurta()
    {
        isHurt = false;
    }

    void Die()
    {
        GetComponent<Collider2D>().enabled = false;
        deathColl.SetActive(true);
        ColiseumKey.instance.deadEnemyCount++;
        this.enabled = false;
        alive = false;
        anim.Play("Enemy_dead");
        Invoke("Eliminate", 1f);     
    }
    private void Eliminate()
    {
         Destroy(gameObject);
    }

    public bool isAlive()
    {
        return alive;
    }
}
