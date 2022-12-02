using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Archer : MonoBehaviour
{
    enum State
    {
        STATE_STARTINGMOVE,
        STATE_ATTACK,
        STATE_COOLDOWN,
        STATE_HIT
    };


    
    //Animations
    private Animator animator;
    private string currentState;
    const string cooldown = "cooldown";
    const string hit = "hit";
    const string attack = "attack";
    const string death = "death";
    const string startingmove = "run";

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
    bool attackable = true;
    bool IsDead = false;
    public bool isHit = false;
    public GameObject Arrow;
    public float LaunchForce;
    public GameObject attackPoint;

    //Move
    Vector3 movement;
    bool Moveright = true;

    //Hit
    Vector2 temp;
    Rigidbody2D rb;
    LayerMask enemyLayers;
    HealthSystem _healthSystem;

    State state = State.STATE_STARTINGMOVE;
    void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();

        _healthSystem.OnHit += OnHit;
        _healthSystem.OnDead += OnDead;

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
                checkPlayer();
                ChangeAnimationState(startingmove);
                startingMove();
                break;
            case State.STATE_ATTACK:
                ArrowMechanism();
                break;
            case State.STATE_COOLDOWN:
                ChangeAnimationState(cooldown);
                coolDown(2);
                break;
            case State.STATE_HIT:
                hitState();
                break;
        }
    }
    void startingMove()
    {
        if (Moveright)
        {
            movement = new Vector3(3, 0f, 0f);
            transform.position = transform.position + movement * Time.deltaTime;
        }
        else
        {
            movement = new Vector3(-3, 0f, 0f);
            transform.position = transform.position + movement * Time.deltaTime;
        }
    }

    void checkPlayer()
    {
        if (Vector2.Distance(transform.position, playerPos.position) < distance)
        {
            state = State.STATE_ATTACK;
            flip();
        }

        else
             state = State.STATE_STARTINGMOVE;
    }
    void ArrowMechanism()
    {
        if(attackable)
        {
            ChangeAnimationState(attack);

            GameObject ArrowIns = Instantiate(Arrow, attackPoint.transform.position, attackPoint.transform.rotation);
            attackable = false;

        }
    }
    void coolDown(float i)
    {
        state = State.STATE_COOLDOWN;
        timer += Time.deltaTime;
        if (timer >= i)
        {
            attackable = true;
            timer = 0;
            _healthSystem.Invincible = false;
            checkPlayer();
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
        if (trig.CompareTag("wall") && state== State.STATE_STARTINGMOVE)
        {
            if (Moveright) Moveright = false;
            else Moveright = true;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    void flip()
    {
            Debug.Log(state);
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
    void hitState()
    {
        if (isHit)
        {
            temp = new Vector2((transform.position.x + 2), transform.position.y);
            Debug.Log(temp);
            if (Moveright)
                rb.MovePosition((Vector2)transform.position + (temp * speedEnemy * Time.deltaTime));
            else
                rb.MovePosition((Vector2)transform.position - (temp * speedEnemy * Time.deltaTime));

            ChangeAnimationState(hit);
            isHit = false;
            attackable = true;
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
}
