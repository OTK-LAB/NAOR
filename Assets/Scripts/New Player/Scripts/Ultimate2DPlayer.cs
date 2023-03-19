using System;
using UnityEngine;

public class Ultimate2DPlayer : MonoBehaviour
{
    private PlayerStateMachine _stateMachine;
    [NonEditable, Space(5)] public StateName CurrentState;
    public State IdleState, WalkState, JumpState, LandState, DashState, CrouchState;
    public AttackState BasicAttack1State, BasicAttack2State, BasicAttack3State;
    public enum StateName { Idle, Walk, Jump, Land, Dash, Crouch, BasicAttack1, BasicAttack2, BasicAttack3 }

    [NonSerialized] public Animator Animator;
    [NonSerialized] public Rigidbody2D Rigidbody2D;
    [NonSerialized] public PlayerInputManager InputManager;
    [NonSerialized] public CapsuleCollider2D CapsuleCollider2D;
    [NonSerialized] public HealthSystem HealthSystem;
    [NonSerialized] public ManaSoulSystem ManaSoulSystem;

    public PlayerData PlayerData;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        Rigidbody2D = GetComponent<Rigidbody2D>();
        InputManager = GetComponent<PlayerInputManager>();
        CapsuleCollider2D = GetComponent<CapsuleCollider2D>();
        HealthSystem = GetComponent<HealthSystem>();
        ManaSoulSystem = GetComponent<ManaSoulSystem>();

        _stateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, _stateMachine, StateName.Idle, PlayerData);
        WalkState = new PlayerWalkState(this, _stateMachine, StateName.Walk, PlayerData);
        JumpState = new PlayerJumpState(this, _stateMachine, StateName.Jump, PlayerData);
        LandState = new PlayerLandState(this, _stateMachine, StateName.Land, PlayerData);
        DashState = new PlayerDashState(this, _stateMachine, StateName.Dash, PlayerData);
        CrouchState = new PlayerCrouchState(this, _stateMachine, StateName.Crouch, PlayerData);

        BasicAttack1State = new PlayerBasicAttack1State(this, _stateMachine, StateName.BasicAttack1, PlayerData);
        BasicAttack2State = new PlayerBasicAttack2State(this, _stateMachine, StateName.BasicAttack2, PlayerData);
        BasicAttack3State = new PlayerBasicAttack3State(this, _stateMachine, StateName.BasicAttack3, PlayerData);
    }

    private void Start()
    {
        _stateMachine.Initialize(IdleState);

        //derivatives for jump and land curves
        PlayerData.Jump.JumpVelocityCurve = PlayerData.Jump.JumpHeightCurve.Derivative();
        PlayerData.Land.LandVelocityCurve = PlayerData.Land.LandHeightCurve.Derivative();
        PlayerData.Dash.DashYVelocityCurve = PlayerData.Dash.DashHeightCurve.Derivative();
    }
    private void Update()
    {
        _stateMachine.CurrentState.Update();
    }

    private void FixedUpdate()
    {
        _stateMachine.CurrentState.FixedUpdate();
    }
}
