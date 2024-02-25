using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class Archer : MonoBehaviour
{
    enum State
    {
        STATE_STARTINGMOVE,
        STATE_ATTACK,
        STATE_COOLDOWN,
        STATE_FROZEN,
        STATE_HIT,
        STATE_DEAD
    };



    //Animations
    private Animator animator;
    private string currentState;
    const string cooldown = "cooldown";
    const string hit = "hit";
    const string attack = "attack";
    const string death = "death";
    const string startingmove = "run";

    public Material material;

    //Following & CoolDown
    public GameObject wall;
    public GameObject wall2;
    float timer;
    private GameObject player;
    private Transform playerPos;
    public float distance;


    //Attack
    bool attackable = true;
    bool IsDead = false;
    bool isHit = false;
    public GameObject Arrow;
    public float LaunchForce;
    public GameObject attackPoint;
    float verticalTolerance = 1.5f; //enemy alttayken player �stteyse onu alg�lamas�n diye eklendi

    //Move
    bool Moveright = true;
    public int moveDirection = 1;
    public float moveSpeed;
    float tempMoveSpeed;
    float firstmoveSpeed;

    //Slow
    public float slowRate;
    float slowSpeed;
    bool slow = false;
    float slowTime;

    //Hit
    Vector2 temp;
    Rigidbody2D rb;
    LayerMask enemyLayers;
    EnemyHealthSystem _healthSystem;
    public float knockbackDistance; //geri sekmesi
    private float hitTimer = 0.0f;

    public GameObject soul;

    State state = State.STATE_STARTINGMOVE;
    void Awake()
    {
        slowSpeed = firstmoveSpeed % (100 - slowRate);
        _healthSystem = GetComponent<EnemyHealthSystem>();
        animator = GetComponent<Animator>();
        _healthSystem.OnHit += OnHit;
        _healthSystem.OnDead += OnDead;
        firstmoveSpeed = moveSpeed;
        slowSpeed = (firstmoveSpeed / 100) * (100 - slowRate);

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
        slowTimer();
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
                coolDown(1);
                break;
            case State.STATE_FROZEN:
                ChangeAnimationState(cooldown);
                rb.velocity = Vector2.zero;
                coolDown(1);
                break;
            case State.STATE_HIT:
                hitState();
                break;
            case State.STATE_DEAD:
                break;
        }
    }
    void startingMove()
    {
        float moveDirectionX = moveDirection;
        float step = moveSpeed * moveDirectionX;
        rb.velocity = new Vector3(step, rb.velocity.y);
    }

    public void slowTimer()
    {
        slowTime -= Time.deltaTime;
        if (slowTime <= 0f)
        {
            slowTime = 0f;
            speedFix();
        }
    }
    public void speedReduction(float time)
    {
        slowTime = time;
        moveSpeed = slowSpeed;
        slow = true;
    }
    public void speedFix()
    {
        moveSpeed = firstmoveSpeed;
        slow = false;
    }
    public void setFrozenState()
    {
        state = State.STATE_FROZEN;
    }
    void checkPlayer()
    {
        Vector2 enemyPosition = new Vector2(rb.position.x, rb.position.y); // D��man�n konumu
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
            knockbackDistance = -0.5f;
            Vector2 knockbackVector = Moveright ? Vector2.right : Vector2.left;
            rb.MovePosition(rb.position + knockbackVector * knockbackDistance);

            ChangeAnimationState(hit);
            isHit = false;
            attackable = true;
            hitTimer = 0.0f;
        }

        hitTimer += Time.deltaTime;
        if (hitTimer >= animator.GetCurrentAnimatorStateInfo(0).length)
        {
            coolDown(1);
        }
    }

    void OnHit(object sender, float knockdistance)
    {
        if (IsDead) return;
        
        state = State.STATE_HIT;
        isHit = true;
    }
    void OnDead(object sender, EventArgs e)
    {
        if (IsDead) return;
    
        IsDead = true;
        // StartCoroutine(SpawnSoul(0.8f));
        ChangeAnimationState(death);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;
        // this.enabled = false;
        state = State.STATE_DEAD;
        GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
    }
    IEnumerator SpawnSoul(float wait)
    {
        yield return new WaitForSeconds(wait);
        Instantiate(soul, transform.position, Quaternion.identity).GetComponent<SoulMovement>().player = player.transform;
    }
}

