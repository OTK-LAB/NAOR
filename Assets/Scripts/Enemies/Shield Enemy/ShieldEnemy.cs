using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShieldEnemy : MonoBehaviour
{


    enum State
    {
        STATE_STARTINGMOVE,
        STATE_FOLLOWING,
        STATE_SHIELD,
        STATE_ATTACK,
        STATE_COOLDOWN,
        STATE_HIT
    };



    //Animations
    private Animator animator;
    private string currentState;
    const string cooldown = "idle";
    const string hit = "hit";
    const string attack = "attack";
    const string death = "death";
    const string shield = "shield";
    const string startingmove = "startingmove";

    //Following & CoolDown
    private GameObject player;
    private Transform playerPos;
    private Vector2 currentPlayerPos;
    [SerializeField] public float distance;
    [SerializeField] public float speedEnemy = 5f;
    [SerializeField] public GameObject wall;
    [SerializeField] public GameObject wall2;

    //Move
    Vector3 movement;
    bool Moveright = true;

    //Attack
    public GameObject attackPoint;
    [SerializeField] public float attackRange;
    [SerializeField] public float damageamount;
    float timer;
    bool IsDead = false;
   // bool isHit = false;
   // bool isShield = false;

    //Hit
    Vector2 temp;
    Rigidbody2D rb;
    LayerMask enemyLayers;
    EnemyHealthSystem _healthSystem;
    public bool isBehind = false;

    State state = State.STATE_STARTINGMOVE;
   
    void Awake()
    {
        _healthSystem = GetComponent<EnemyHealthSystem>();

       _healthSystem.OnHit += OnHit;
       _healthSystem.OnDead += OnDead;
       _healthSystem.OnShield += OnShield;

    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        checkState();
    }
    void checkState()
    {
        switch (state)
        {
            case State.STATE_STARTINGMOVE:
                coolDown(3);
                ChangeAnimationState(startingmove);
                startingMove();
                break;
            case State.STATE_FOLLOWING:
                ChangeAnimationState(startingmove);
                checkPlayer();
                following();
                break;
            case State.STATE_ATTACK:
                flip();
                ChangeAnimationState(attack);
                attacktoPlayer();
                break;
            case State.STATE_COOLDOWN:
                ChangeAnimationState(cooldown);
                coolDown(2);
                break;
            case State.STATE_HIT:
                ChangeAnimationState(hit);
                //   hitState();
                break;
            case State.STATE_SHIELD:
                ChangeAnimationState(shield);
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


    void checkPlayer()
    {
        if (Vector2.Distance(transform.position, playerPos.position) < distance)
        {
            if (Vector2.Distance(transform.position, playerPos.position) <= 3)
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

            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange);
            foreach (Collider2D enemy in hitPlayer)
            {
                if (enemy.tag == "Player")
                {
                    player.GetComponent<HealthSystem>().Damage(damageamount);
                }
            }

    }
    public void setCoolDown()
    {
        state = State.STATE_COOLDOWN;
    }
    public void coolDown(float i)
    {
        timer += Time.deltaTime;
        if (timer >= i)
        {
            timer = 0;
            checkPlayer();

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
    public void checkBehind()
    {

        if (playerPos.position.x > (transform.position.x + 0.5f) && Moveright)
            isBehind = true;
        else if (playerPos.position.x <= (transform.position.x + 0.5f) && !Moveright)
            isBehind = true;
        else
            isBehind = false;
    }
    private void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.CompareTag("wall") && state == State.STATE_STARTINGMOVE)
        {
            turn();
        }
    }
    public void turn()
    {
        if (state == State.STATE_STARTINGMOVE)
        {
            if (Moveright) Moveright = false;
            else Moveright = true;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    void OnHit(object sender, EventArgs e)
    {
        if (!IsDead)
        {
            Debug.Log("hit");
            state = State.STATE_HIT;
           // isHit = true;
        }
    }
    void OnShield(object sender, EventArgs e )
    {
        if (!IsDead)
        {
            checkBehind();
            if (isBehind)
            {
                Debug.Log("shield");
                state = State.STATE_SHIELD;
              //  isShield = true;
                gameObject.GetComponent<EnemyHealthSystem>().onShield = true;
            }

        }

    }
    void OnDead(object sender, EventArgs e)
    {
        if (!IsDead)
        {
            IsDead = true;
            ChangeAnimationState(death);
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;
            GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
    }
    
    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
    }
}
