using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeleeEnemy : MonoBehaviour
{

    enum State
    {
        STATE_STARTINGMOVE,
        STATE_FOLLOWING,
        STATE_ATTACK,
        STATE_COOLDOWN,
        STATE_NOTDAMAGE,
        STATE_HIT
    };

    State state = State.STATE_STARTINGMOVE;

    //Animations
    private Animator animator;
    private string currentState;
    const string idle = "cooldown";
    const string hit = "hit";
    const string attack = "attack";
    const string death = "dead";
    const string follow = "following";
    const string notdamage = "notdamage";
    const string startingmove = "startingmove";

    //Move
    Vector3 movement;
    bool Moveright = true;

    //Following & CoolDown
    private GameObject player;
    private Transform playerPos;
    private Vector2 currentPlayerPos;
    public float distance;
    public float speedEnemy = 5f;
    public GameObject wall;
    public GameObject wall2;
    float timer;

    //Attack
    public GameObject attackPoint;
    public float attackRange;
    public float damageamount;
    bool attackable = true;
    int random_nd; //random_notdamage
    bool IsDead = false;
    public bool isHit = false;

    LayerMask enemyLayers;
    HealthSystem _healthSystem; 
    
    void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();

        _healthSystem.OnHit += OnHit;
        _healthSystem.OnDead += OnDead; 

    }
    void Start()
    {
        animator = GetComponent<Animator>();
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindGameObjectWithTag("Player");
        attackPoint = GameObject.FindGameObjectWithTag("sword") ;

    }

    void Update()
    {
        checkState();
        if (isHit)
            state = State.STATE_HIT;
    }
    void checkState()
    {
        switch (state)
        {
            case State.STATE_STARTINGMOVE:
                checkPlayer();
                ChangeAnimationState(startingmove);
                startingMove();
                break;

            case State.STATE_FOLLOWING:
                checkPlayer();
                ChangeAnimationState(follow);
                following();
                break;
            case State.STATE_ATTACK:
                ChangeAnimationState(attack);
                attacktoPlayer();
                break;
            case State.STATE_COOLDOWN:
                ChangeAnimationState(idle);
                coolDown(2);
                break;
            case State.STATE_HIT:
                hitState();
                break;
            case State.STATE_NOTDAMAGE:
                isHit = false;
                ChangeAnimationState(notdamage);
                coolDown(2);
                break;
        }
    }
    void startingMove()
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
    void hitState()
    {
        if (isHit)
        {
            if (Moveright)
                transform.position = new Vector2((float)(transform.position.x - 0.2), transform.position.y);
            else
                transform.position = new Vector2((float)(transform.position.x + 0.2), transform.position.y);
            ChangeAnimationState(hit);
            Debug.Log("Vurdu");
            isHit = false;
            attackable = true;
            StartCoroutine(backtoPlayerCheck());
        }
    }
    IEnumerator backtoPlayerCheck()
    {
        yield return new WaitForSeconds(1f);
        checkPlayer();
    }

    void checkPlayer()
    {
        if (Vector2.Distance(transform.position, playerPos.position) < distance)
        {
            if (Vector2.Distance(transform.position, playerPos.position) <= 1.5f)
                state = State.STATE_ATTACK;
            else
                state = State.STATE_FOLLOWING;
        }
        else
        {
            state = State.STATE_STARTINGMOVE;
            wall.transform.parent = GameObject.FindGameObjectWithTag("parent").transform;
            wall2.transform.parent = GameObject.FindGameObjectWithTag("parent").transform;
        }

    }
    void following()
    {
        flip();
        currentPlayerPos = new Vector2(playerPos.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, currentPlayerPos, speedEnemy * Time.deltaTime);
        wall.transform.parent = this.transform;
        wall2.transform.parent = this.transform;
    }
    void attacktoPlayer()
    {
        random_nd = UnityEngine.Random.Range(0, 100);
        if (attackable && !isHit)
        {
           attackable = false;
           Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange);
            foreach(Collider2D enemy in hitPlayer)
            {
                if (enemy.tag == "Player")
                {
                   // player.GetComponent<HealthSystem>().Damage(damageamount); PLAYER'A HEALTH SYSTEM EKLENİNCE AKTİF EDİLMELİ
                    Debug.Log("We hit" + enemy.name);
                }
            }
            StartCoroutine(backtoCoolDown());
        }       

    }
    IEnumerator backtoCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        if (!isHit)
        {
            if (random_nd <= 15)
            {
                _healthSystem.Invincible = true;
                state = State.STATE_NOTDAMAGE;
            }
            else
                state = State.STATE_COOLDOWN;
        }
        else
            state = State.STATE_HIT;
    }
    void coolDown(int i)
    {
        timer += Time.deltaTime;
        if (timer >= i)
        {
            checkPlayer();
            attackable = true;
            timer = 0;
            _healthSystem.Invincible = false;
        }
    }

    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
    }
    private void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.CompareTag("wall"))
        {
            if (Moveright) Moveright = false;
            else Moveright = true;
            transform.Rotate(0f, 180f, 0f);
            // transform.position = transform.position + movement * Time.deltaTime;
        }
    }

    void OnHit(object sender, EventArgs e)
    {
        if (!IsDead)
        { 
            state = State.STATE_HIT; 
            isHit = true;
        }
    }
    void OnDead(object sender, EventArgs e)
    {
        if(!IsDead)
        {
            IsDead = true;
            ChangeAnimationState(death);
            Debug.Log("öl");
            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;
            GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        }
    }
    void flip()
    {
        if (playerPos.position.x > (transform.position.x + 0.5f))
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
    }
}
