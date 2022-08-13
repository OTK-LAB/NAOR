using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    // state variables
    private PlayerBaseState _currentState;
    PlayerStateFactory _states;

    PlayerInputActions _playerInputActions;
    
    //Movement
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    Vector2 _currentMovementInput;
    float _appliedMovementX;
    bool _isMovementPressed; 
    bool _isOnGround;

    [SerializeField] private Rigidbody2D _rb;

    //Jumping
    [Header("Jumping")]
    bool _isJumpPressed;
    [SerializeField] private float jumpForce;
    public float jumpTimer;
    private float jumpTimeCounter;
    public LayerMask groundLayer;
    public Transform groundCheck;


    // getters and setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; }}
    public PlayerInputActions PlayerInputActions { get { return _playerInputActions; } set { _playerInputActions = value; }}
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; } set { _currentMovementInput = value; }}
    public float AppliedMovement { get { return _appliedMovementX; } set { _appliedMovementX = value; }}
    
    public bool IsMovementPressed { get {return _isMovementPressed; } set { _isMovementPressed = value; }}
    public bool IsJumpPressed { get { return _isJumpPressed; } set { _isJumpPressed = value; }}
    public bool IsOnGround { get { return _isOnGround; }}

    public float JumpForce { get { return jumpForce; } set { jumpForce = value; }}
    public float MovementSpeed { get { return movementSpeed; } }
    public Rigidbody2D Rigidbod { get { return _rb; }}
    void Awake()
    {
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Move.started += OnMovementInput;
        _playerInputActions.Player.Move.canceled += OnMovementInput;
        _playerInputActions.Player.Move.performed += OnMovementInput;

        _playerInputActions.Player.Jump.started += OnJump;
        _playerInputActions.Player.Jump.canceled += OnJump;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckForGround();
        CurrentState.UpdateStates();
    }
    private void FixedUpdate() {
        Move(_appliedMovementX);
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }
    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
    }

    void CheckForGround()
    {
        _isOnGround = Physics2D.OverlapCircle(groundCheck.position, 0.25f, groundLayer);
    }

    void Move(float movementInput)
    {
        _rb.velocity = new Vector2(movementInput, _rb.velocity.y);
    }
}
