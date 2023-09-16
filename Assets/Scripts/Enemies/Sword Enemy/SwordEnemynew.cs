using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SwordEnemynew : MonoBehaviour
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

    EnemyHealthSystem _healthSystem;

    public Transform[] movePoints; // Ýki nokta arasý hareket için noktalar
    public Transform player; // Oyuncu referansý
    public float moveSpeed = 3f; // Hareket hýzý
    //Attack
    public float attackRange = 2f; // Saldýrý menzili
    [SerializeField] public GameObject attackPoint;

    private int currentPointIndex = 0; // Þu anki hedef nokta
    private bool isAttacking = false; // Saldýrý durumu
    bool IsDead = false;
    bool isHit = false;



    void Awake()
    {
        _healthSystem = GetComponent<EnemyHealthSystem>();
        animator = GetComponent<Animator>();
        _healthSystem.OnHit += OnHit;
        _healthSystem.OnDead += OnDead;

    }

    void Start()
    {
        
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
               // checkPlayer();
                ChangeAnimationState(startingmove);
                MoveBetweenPoints();
                break;
                /*
            case State.STATE_FOLLOWING:
                checkPlayer();
                ChangeAnimationState(follow);
                following();
                break;
            case State.STATE_ATTACK:
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
                nDamage();
                break;
                */
        }
    }
    private void MoveBetweenPoints()
    {
        // Hedef noktaya doðru hareket et
        Transform targetPoint = movePoints[currentPointIndex];
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, step);

        // Hedef noktaya ulaþýldýðýnda, diðer hedefe geç
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % movePoints.Length;
        }
    }
    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
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
         //   StartCoroutine(SpawnSoul(0.8f));
            IsDead = true;
            ChangeAnimationState(death);
            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;
            GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
    }
}
