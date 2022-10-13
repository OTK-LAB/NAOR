using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{

    enum State
    {
        STATE_STARTINGMOVE,
        STATE_FOLLOWING,
        STATE_ATTACK,
        STATE_COOLDOWN,
        STATE_NOTDAMAGE
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

    //Following
    public Transform playerPos;
    private Vector2 currentPlayerPos;
    public float distance;
    public float speedEnemy=5f;
    public GameObject wall;
    public GameObject wall2;


    void Start()
    {
        animator = GetComponent<Animator>();
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;


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
                ChangeAnimationState(attack);
                attacktoPlayer();
                state = State.STATE_COOLDOWN; //ataktan hemen sonra cool down
                break;
            case State.STATE_COOLDOWN:
                ChangeAnimationState(idle);
                coolDown();
                //3? saniye sonra state de�i�imi
                //oyuncu radardaysa ama yak�n�nda de�ilse
                state = State.STATE_FOLLOWING;
                //oyuncu radarda ve yak�ndaysa
                state = State.STATE_ATTACK;
                //oyuncu radarda de�ilse
                state = State.STATE_FOLLOWING;
                break;
            case State.STATE_NOTDAMAGE:
                ChangeAnimationState(notdamage);
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
        Debug.Log(Vector2.Distance(transform.position, playerPos.position));
        if (Vector2.Distance(transform.position, playerPos.position) < distance)
            state = State.STATE_FOLLOWING;
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

    }
    void coolDown()
    {

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
            if (Moveright)  Moveright = false; 
            else  Moveright = true; 
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
}

