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
    private Vector2 currentPlayerPos;
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
    bool slow = false;

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
                coolDown(2);
                break;
            case State.STATE_HIT:
                hitState();
                break;
            case State.STATE_SHIELD:
                ChangeAnimationState(shield);
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
            moveSpeed = firstmoveSpeed; // baþlangýç hareket hýzý
        moveDirectionX = moveDirection;
        step = moveSpeed * moveDirectionX;
        rb.velocity = new Vector3(step, rb.velocity.y);
    }

    void checkPlayer()
    {
        enemyPosition = new Vector2(rb.position.x, rb.position.y); // Düþmanýn konumu
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
        // Baþlangýç konumuna ulaþtýðýnda, Walking state'ine geç
        if (Vector2.Distance(transform.position, startPoint) < 0.1f)
        {
            hasTurned = false;
            state = State.STATE_STARTINGMOVE;
        }


    }
    public void speedReduction(int i)
    {
        slow = true;
        tempMoveSpeed = moveSpeed - i;
        moveSpeed = tempMoveSpeed;
    }
    public void speedFix()
    {
        slow = false;
        moveSpeed = firstmoveSpeed;
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
            knockbackDistance = -2f;
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

    void OnHit(object sender, EventArgs e)
    {
        if (!IsDead)
        {
            state = State.STATE_HIT;
            isHit = true;
        }
    }
    void OnShield(object sender, EventArgs e )
    {
        if (!IsDead)
        {
            checkBehind();
            if (isBehind)
            {
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
