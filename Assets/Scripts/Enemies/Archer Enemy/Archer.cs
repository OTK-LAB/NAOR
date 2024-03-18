using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Archer : StateManager<Archer.State>
{
    public enum State
    {
        STATE_STARTINGMOVE,
        STATE_ATTACK,
        STATE_COOLDOWN,
        STATE_HIT,
        STATE_DEAD
    };
    
    private Animator animator;
    private GameObject player;
    public GameObject Arrow;
    public float LaunchForce;
    public GameObject attackPoint;
    EnemyHealthSystem _healthSystem;
    public GameObject soul;
    public float moveSpeed;

    void Awake()
    {
        _healthSystem = GetComponent<EnemyHealthSystem>();
        animator = GetComponent<Animator>();
        _healthSystem.OnHit += OnHit;
        _healthSystem.OnDead += OnDead;
    }

    new void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        GameObject archerGameObject = gameObject;
        
        _states = new Dictionary<State, BaseState<State>>
        {
            { State.STATE_HIT, new Archer_Hit(archerGameObject, player) },
            { State.STATE_DEAD, new Archer_Dead(archerGameObject, player) },
            { State.STATE_ATTACK, new Archer_Attack(archerGameObject, player) },
            { State.STATE_COOLDOWN, new Archer_Cooldown(archerGameObject, player, 1) },
            { State.STATE_STARTINGMOVE, new Archer_StartingMove(archerGameObject, player) }
        };

        _currentState = _states[State.STATE_STARTINGMOVE];
        base.Start();
    }

    new void Update()
    {
        base.Update();
    }
    
    public void ArrowMechanism()
    {
        if (_currentState.stateKey != State.STATE_ATTACK) return;
        
        Instantiate(Arrow, attackPoint.transform.position, attackPoint.transform.rotation);
    }
    void coolDown(float i)
    {
        TransitionToState(State.STATE_COOLDOWN);
    }
    
    public void ChangeAnimationState(string newState)
    {
        animator.Play(newState);
    }
    
    void OnHit(object sender, float knockdistance)
    {
        if (_currentState.stateKey == State.STATE_DEAD) return;
     
        TransitionToState(State.STATE_HIT);
    }
    
    void OnDead(object sender, EventArgs e)
    {
        if (_currentState.stateKey == State.STATE_DEAD) return;
        TransitionToState(State.STATE_DEAD);
    }
    
    IEnumerator SpawnSoul(float wait)
    {
        yield return new WaitForSeconds(wait);
        Instantiate(soul, transform.position, Quaternion.identity).GetComponent<SoulMovement>().player = player.transform;
    }
}

