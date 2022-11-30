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
    public float damageamount;
    bool attackable = true;
    int random_nd; //random_notdamage
    bool IsDead = false;
    public bool isHit = false;
    [HideInInspector] public float CalculatedTime;
    public float LaunchForce;
    public float TimeBtwEachShot;
    [HideInInspector] public Vector2 target;
    public GameObject Arrow;

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
        CalculatedTime = TimeBtwEachShot;
    }


    void Update()
    {
        checkState();
    }

    void checkState()
    {
        Debug.Log(state);
        switch (state)
        {
            case State.STATE_STARTINGMOVE:
                checkPlayer();
                ChangeAnimationState(startingmove);
                startingMove();
                break;
            case State.STATE_ATTACK:
                Debug.Log("attack");
                ArrowMechanism();
                break;
            case State.STATE_COOLDOWN:
                Debug.Log("cooldown");
                ChangeAnimationState(cooldown);
                coolDown(2);
                break;
            case State.STATE_HIT:
             //   hitState();
                break;
        }
    }
    void startingMove()
    {
        Debug.Log("move");
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

            //target = new Vector2(PlayerPosition.position.x - transform.position.x, PlayerPosition.position.y - transform.position.y);
            ChangeAnimationState(attack);
            GameObject ArrowIns = Instantiate(Arrow, transform.position, transform.rotation);
        //  ArrowIns.GetComponent<Rigidbody2D>().AddForce(target* LaunchForce);
        //Instantiate(Arrow, transform.position, Quaternion.LookRotation(Vector3.forward, transform.position - PlayerPosition.position));
        attackable = false;
        state = State.STATE_COOLDOWN;

    }
    void coolDown(int i)
    {
        timer += Time.deltaTime;
        if (timer >= i)
        {
            attackable = true;
            timer = 0;
            _healthSystem.Invincible = false;
            Debug.Log("false");
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
        if (trig.CompareTag("wall"))
        {
            if (Moveright) Moveright = false;
            else Moveright = true;
            transform.Rotate(0f, 180f, 0f);
            // transform.position = transform.position + movement * Time.deltaTime;
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
            Debug.Log("öl");
            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;
            GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        }
    }
}
