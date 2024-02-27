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
        STATE_HIT,
        STATE_FROZEN,
        STATE_BACKTOWALL
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
    float distanceToPlayer;
    [SerializeField] public float distance;
    [SerializeField] public float moveSpeed;


    //Move
    [Header("Left Wall")]
    public GameObject wall;
    [Header("Right Wall")]
    public GameObject wall2;
    Vector3 startPoint;
    bool isBetweenWalls;
    float moveDirectionX;
    float step;
    float firstmoveSpeed;
    bool Moveright = true;
    public int moveDirection = 1;

    //Slow
    public float slowRate;
    float slowSpeed;
    bool slow = false;
    float slowTime;

    //Attack
    Vector2 enemyPosition;
    public GameObject attackPoint;
    [SerializeField] public float attackRange;
    [SerializeField] public float damageamount;
    float timer;
    bool IsDead = false;
    bool isHit = false;
    bool attackable = true;
    // bool isShield = false;
    float verticalTolerance = 2f;
    float tempMoveSpeed;
    //Hit
    public float knockbackDistance; //geri sekmesi
    Rigidbody2D rb;
    LayerMask enemyLayers;
    EnemyHealthSystem _healthSystem;
    bool isBehind = false;
    private bool hasTurned = false;


    State state = State.STATE_STARTINGMOVE;
   
    void Awake()
    {
        _healthSystem = GetComponent<EnemyHealthSystem>();
        animator = GetComponent<Animator>();
        _healthSystem.OnHit += OnHit;
       _healthSystem.OnDead += OnDead;
       _healthSystem.OnShield += OnShield;

    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindGameObjectWithTag("Player");
        startPoint = transform.position;
        firstmoveSpeed = moveSpeed;
        slowSpeed = (firstmoveSpeed / 100) * (100 - slowRate);
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
            case State.STATE_FOLLOWING:
                checkPlayer();
                ChangeAnimationState(startingmove);
                following();
                break;
            case State.STATE_ATTACK:
                rb.velocity = Vector2.zero;
                attacktoPlayer();
                break;
            case State.STATE_COOLDOWN:
                ChangeAnimationState(cooldown);
                coolDown(0.5f);
                break;
            case State.STATE_HIT:
                hitState();
                break;
            case State.STATE_SHIELD:
                ChangeAnimationState(shield);
                StartCoroutine(ChangeToNewState(2.0f, cooldown, State.STATE_COOLDOWN));
                break;
            case State.STATE_FROZEN:
                ChangeAnimationState(cooldown);
                rb.velocity = Vector2.zero;
                coolDown(5);
                break;
            case State.STATE_BACKTOWALL:
                ChangeAnimationState(startingmove);
                backtoWall();
                break;

        }
    }
    void startingMove()
    {
        if (!slow)
            moveSpeed = firstmoveSpeed; // ba�lang�� hareket h�z�
        moveDirectionX = moveDirection;
        step = moveSpeed * moveDirectionX;
        rb.velocity = new Vector3(step, rb.velocity.y);
    }

    void checkPlayer()
    {
        enemyPosition = new Vector2(rb.position.x, rb.position.y); // D��man�n konumu
     //   Vector2 playerPosition = new Vector2(playerPos.position.x, playerPos.position.y); // Oyuncunun konumu
        distanceToPlayer = Vector2.Distance(enemyPosition, playerPos.position);
        isBetweenWalls = transform.position.x >= wall.transform.position.x && transform.position.x <= wall2.transform.position.x;
        if (distanceToPlayer < distance && Mathf.Abs(enemyPosition.y - playerPos.position.y) < verticalTolerance)
        {
            hasTurned = false;
            if (distanceToPlayer <= 2)
                state = State.STATE_ATTACK;
            else
                state = State.STATE_FOLLOWING;
        }
        else if (isBetweenWalls)
            state = State.STATE_STARTINGMOVE;
        else
            state = State.STATE_BACKTOWALL;
    }

    void following()
    {
        flip();
        if (!slow)
            moveSpeed = firstmoveSpeed + 2;
        Vector2 currentPlayerPos = new Vector2(playerPos.position.x, rb.position.y);
        rb.velocity = (currentPlayerPos - rb.position).normalized * moveSpeed;
    }
    void backtoWall()
    {
        moveSpeed = firstmoveSpeed;
        Vector2 startDirection = startPoint - transform.position;
        if (!hasTurned && Vector3.Dot(startDirection, transform.right) < 0f)
        {
            hasTurned = true;
            if (Moveright) Moveright = false;
            else Moveright = true;
            moveDirection *= -1;
            transform.Rotate(0f, 180f, 0f);
        }
        rb.velocity = startDirection.normalized * moveSpeed;
        checkPlayer();
        // Ba�lang�� konumuna ula�t���nda, Walking state'ine ge�
        if (Vector2.Distance(transform.position, startPoint) < 0.1f)
        {
            hasTurned = false;
            state = State.STATE_STARTINGMOVE;
        }


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
    void hitState()
    {
        if (isHit)
        {
            /*
            temp = new Vector2((rb.position.x + 2), rb.position.y);
            if (Moveright)
                rb.MovePosition((Vector2)rb.position + (temp * moveSpeed * Time.deltaTime));
            else
                rb.MovePosition((Vector2)rb.position - (temp * moveSpeed * Time.deltaTime));
            */
            knockbackDistance = -0.5f;
            Vector2 knockbackVector = Moveright ? Vector2.right : Vector2.left;
            rb.MovePosition(rb.position + knockbackVector * knockbackDistance);

            ChangeAnimationState(hit);
            isHit = false;
            attackable = true;
        }
    }

    void attacktoPlayer()
    {
        if (attackable && !isHit)
        {
            ChangeAnimationState(attack);
            attackable = false;
            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange);
            foreach (Collider2D enemy in hitPlayer)
            {
              /*  if (enemy.tag == "Player")
                {
                    player.GetComponent<HealthSystem>().Damage(damageamount);
                }*/
            }
        }
    }
    IEnumerator backtoCoolDown()
    {
        if (!isHit)
        {
            yield return new WaitForSeconds(0.01f);
            state = State.STATE_COOLDOWN;
        }
        else
            state = State.STATE_HIT;
    }
    public void coolDown(float i)
    {
        timer += Time.deltaTime;
        if (timer >= i)
        {
            attackable = true;
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
            turn();

    }
    public void turn()
    {
        if (state == State.STATE_STARTINGMOVE)
        {
            if (Moveright) Moveright = false;
            else Moveright = true;
            moveDirection *= -1;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    void OnHit(object sender, float knockdistance)
    {
        if (IsDead) return;
        
        state = State.STATE_HIT;
        isHit = true;
    }
    void OnShield(object sender, EventArgs e )
    {
        if (IsDead) return;
        
        checkBehind();
        if (isBehind)
        {
            state = State.STATE_SHIELD;
          //  isShield = true;
            gameObject.GetComponent<EnemyHealthSystem>().onShield = true;
        }
    }
    void OnDead(object sender, EventArgs e)
    {
        if (IsDead) return;
        
        IsDead = true;
        ChangeAnimationState(death);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero;
        this.enabled = false;
        GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
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

    IEnumerator ChangeToNewState(float waitTime, string animationStateName, State state_)
    {
        yield return new WaitForSeconds(waitTime);
        
        ChangeAnimationState(animationStateName);
        this.state = state_;
    }
}
