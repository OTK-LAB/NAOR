using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SwordEnemy : MonoBehaviour
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
    const string idle = "Cooldown1";
    const string hit = "Hit1";
    const string attack = "Attack1";
    const string death = "Dead1";
    const string follow = "Run";
    const string startingmove = "StartingMove1";

    public Material material;

    //Movement
    bool Moveright = true;
    public int moveDirection = 1;
    float moveDirectionX;
    public GameObject wall;
    public GameObject wall2;

    //Following & CoolDown
    private GameObject player;
    private Transform playerPos;
    private Vector2 currentPlayerPos;
    public float distance;
    public float moveSpeed;
    float timer;

    //Attack
    [SerializeField] public GameObject attackPoint;
    [SerializeField] public float attackRange;
    [SerializeField] public float damageamount;
    bool attackable = true;

    bool IsDead = false;
    bool isHit = false;
    float verticalTolerance = 0.5f; //enemy alttayken player üstteyse onu algýlamasýn diye eklendi

    //Hit
    Vector2 temp;
    Rigidbody2D rb;
    LayerMask enemyLayers;
    EnemyHealthSystem _healthSystem;

    public GameObject soul;


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
        attackPoint = GameObject.FindGameObjectWithTag("sword");

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
                ChangeAnimationState(follow);
                following();
                break;
            case State.STATE_ATTACK:
                rb.velocity = Vector2.zero;
                attacktoPlayer();
                break;
            case State.STATE_COOLDOWN:
                ChangeAnimationState(idle);
                coolDown(2);
                break;
            case State.STATE_HIT:
                hitState();
                break;
        }
    }
    void startingMove()
    {
        moveSpeed = 3f; // baþlangýç hareket hýzý
        float moveDirectionX = moveDirection;
        float step = moveSpeed * moveDirectionX;
        rb.velocity = new Vector3(step, rb.velocity.y);
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


    void checkPlayer()
    {
        //float distanceToPlayer = Vector2.Distance(rb.position, playerPos.position);
        Vector2 enemyPosition = new Vector2(rb.position.x, rb.position.y); // Düþmanýn konumu
        Vector2 playerPosition = new Vector2(playerPos.position.x, playerPos.position.y); // Oyuncunun konumu

        float distanceToPlayer = Vector2.Distance(enemyPosition, playerPosition);
        if (distanceToPlayer < distance && Mathf.Abs(enemyPosition.y - playerPosition.y) < verticalTolerance)
        {
            if (distanceToPlayer <= 1)
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
        moveSpeed = 5f;
        Vector2 currentPlayerPos = new Vector2(playerPos.position.x, rb.position.y);
        rb.velocity = (currentPlayerPos - rb.position).normalized * moveSpeed;
        wall.transform.parent = transform;
        wall2.transform.parent = transform;

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
                if (enemy.tag == "Player")
                    player.GetComponent<HealthSystem>().Damage(damageamount);
            }
            // StartCoroutine(backtoCoolDown());
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
    void coolDown(int i)
    {
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
        if (trig.CompareTag("wall") && state == State.STATE_STARTINGMOVE)
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
    void OnDead(object sender, EventArgs e)
    {
        if (!IsDead)
        {
            StartCoroutine(SpawnSoul(0.8f));
            IsDead = true;
            ChangeAnimationState(death);
            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;
            GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        }
    }

    IEnumerator SpawnSoul(float wait)
    {
        yield return new WaitForSeconds(wait);
        Instantiate(soul, transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity).GetComponent<SoulMovement>().player = player.transform;
        Instantiate(soul, transform.position + new Vector3(0, -0.3f, 0), Quaternion.identity).GetComponent<SoulMovement>().player = player.transform;
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
    }
}
