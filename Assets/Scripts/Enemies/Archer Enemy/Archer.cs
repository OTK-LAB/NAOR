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
    const string death = "death_";
    const string startingmove = "run";

    public Material material;

    //Following & CoolDown
    public GameObject wall;
    public GameObject wall2;
    float timer;
    private GameObject player;
    private Transform playerPos;
    private Vector2 currentPlayerPos;
    public float distance;
    public float moveSpeed;

    //Attack
    bool attackable = true;
    bool IsDead = false;
    bool isHit = false;
    public GameObject Arrow;
    public float LaunchForce;
    public GameObject attackPoint;
    float verticalTolerance = 1.5f; //enemy alttayken player üstteyse onu algýlamasýn diye eklendi

    //Move
    Vector3 movement;
    bool Moveright = true;
    float moveDirectionX;
    public int moveDirection = 1;


    //Hit
    Vector2 temp;
    Rigidbody2D rb;
    LayerMask enemyLayers;
    EnemyHealthSystem _healthSystem;

    public GameObject soul;

    State state = State.STATE_STARTINGMOVE;
    void Awake()
    {
        _healthSystem = GetComponent<EnemyHealthSystem>();
        animator = GetComponent<Animator>();
        _healthSystem.OnHit += OnHit;
        _healthSystem.OnDead += OnDead;

    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
                if (attackable)
                {
                    rb.velocity = Vector2.zero;
                    ChangeAnimationState(attack);
                }
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
        // moveSpeed = 3f; // baþlangýç hareket hýzý
        float moveDirectionX = moveDirection;
        float step = moveSpeed * moveDirectionX;
        rb.velocity = new Vector3(step, rb.velocity.y);
    }

    void checkPlayer()
    {
        //float distanceToPlayer = Vector2.Distance(rb.position, playerPos.position);
        Vector2 enemyPosition = new Vector2(rb.position.x, rb.position.y); // Düþmanýn konumu
        Vector2 playerPosition = new Vector2(playerPos.position.x, playerPos.position.y); // Oyuncunun konumu

        float distanceToPlayer = Vector2.Distance(enemyPosition, playerPosition);
        if (distanceToPlayer < distance && Mathf.Abs(enemyPosition.y - playerPosition.y) < verticalTolerance)
        {
            state = State.STATE_ATTACK;
            flip();
        }
        else
        {
            state = State.STATE_STARTINGMOVE;
        }

    }
    public void ArrowMechanism()
    {
        GameObject ArrowIns = Instantiate(Arrow, attackPoint.transform.position, attackPoint.transform.rotation);
        attackable = false;
    }
    void coolDown(float i)
    {
        state = State.STATE_COOLDOWN;
        timer += Time.deltaTime;
        if (timer >= i)
        {
            attackable = true;
            timer = 0;
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
        if (trig.CompareTag("wall") && state == State.STATE_STARTINGMOVE)
        {
            if (Moveright) Moveright = false;
            else Moveright = true;
            moveDirection *= -1;
            transform.Rotate(0f, 180f, 0f);
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
                moveDirection *= -1;
            }
        }
        else
        {
            if (Moveright)
            {
                transform.Rotate(0f, 180f, 0f);
                Moveright = false;
                moveDirection *= -1;
            }
        }
    }
    void hitState()
    {
        if (isHit)
        {
            temp = new Vector2((rb.position.x + 2), rb.position.y);
            if (Moveright)
                rb.MovePosition((Vector2)rb.position + (temp * moveSpeed * Time.deltaTime));
            else
                rb.MovePosition((Vector2)rb.position - (temp * moveSpeed * Time.deltaTime));

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
            StartCoroutine(SpawnSoul(0.8f));
            ChangeAnimationState(death);
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;
            GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        }
    }
    IEnumerator SpawnSoul(float wait)
    {
        yield return new WaitForSeconds(wait);
        Instantiate(soul, transform.position, Quaternion.identity).GetComponent<SoulMovement>().player = player.transform;
    }
}

