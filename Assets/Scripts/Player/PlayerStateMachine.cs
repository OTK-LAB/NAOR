using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//TODO:
//  Add ledge detection and create PlayerVaultState
//  Implement sliding physics
//  Move the toggle logic into grounded state
public class PlayerStateMachine : MonoBehaviour
{
    // state variables
    private PlayerBaseState _currentState;
    private PlayerStateFactory _states;
    private PlayerInputActions _playerInputActions;
    private Animator _animator;
    
    //Movement
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    Vector2 _currentMovementInput;
    float _appliedMovementX;
    bool _isMovementPressed; 
    bool _facingRight = true;
    bool _canFlip = true;
    bool _isOnGround;
    bool _isCrouching = false;
    //Dragging
    bool _toggleDrag = false;
    public Transform frontCheck;
    bool _thereIsSomething;
    bool _canDrag;
    RaycastHit2D hit2D;


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
    public Animator PlayerAnimator { get { return _animator;}}
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; } set { _currentMovementInput = value; }}
    public float AppliedMovement { get { return _appliedMovementX; } set { _appliedMovementX = value; }}
    
    public bool IsMovementPressed { get {return _isMovementPressed; } set { _isMovementPressed = value; }}
    public bool IsJumpPressed { get { return _isJumpPressed; } set { _isJumpPressed = value; }}
    public bool IsOnGround { get { return _isOnGround; }}
    public bool IsCrouching { get { return _isCrouching; }}
    public bool DragToggle { get { return _toggleDrag; }}
    public bool CanFlip { get {return _canFlip; } set { _canFlip = value; }}

    public float JumpForce { get { return jumpForce; } set { jumpForce = value; }}
    public float MovementSpeed { get { return movementSpeed; }}
    public Rigidbody2D Rigidbod { get { return _rb; }}
    public RaycastHit2D Ray { get {return hit2D; }}
    //For workaround in Drag. Will be changed
    public Transform GroundCheck { get {return groundCheck; }}
    public Transform FrontCheck { get {return frontCheck; }}
    void Awake()
    {
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
        _animator = GetComponent<Animator>();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Move.started += OnMovementInput;
        _playerInputActions.Player.Move.canceled += OnMovementInput;
        _playerInputActions.Player.Move.performed += OnMovementInput;

        _playerInputActions.Player.Jump.started += OnJump;
        _playerInputActions.Player.Jump.canceled += OnJump;

        _playerInputActions.Player.Crouch.started += OnCrouch;

        _playerInputActions.Player.Drag.started += OnDrag;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckForGround();
        CheckFront();
        FlipPlayer();
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
    void OnCrouch(InputAction.CallbackContext context)
    {
        _isCrouching = !_isCrouching;
    }
    void OnDrag(InputAction.CallbackContext context)
    {
        if(_toggleDrag)
        {
            _toggleDrag = false;
        }
        else if(_canDrag)
        {
            _toggleDrag = true;
        }

    }
    void CheckForGround()
    {
        _isOnGround = Physics2D.OverlapCircle(groundCheck.position, 0.25f, groundLayer);
    }
    void CheckFront(){
        _thereIsSomething = Physics2D.Raycast(frontCheck.position, transform.right, 50f, groundLayer);
        hit2D = Physics2D.Raycast(frontCheck.position, transform.right, 50f, groundLayer);
        if(_thereIsSomething && hit2D.collider.CompareTag("Movable")){
            _canDrag = true;
        }
        else
        {
            _canDrag = false;
        }
    }
    void Move(float movementInput)
    {
        _rb.velocity = new Vector2(movementInput, _rb.velocity.y);
    }
    void FlipPlayer()
    {
        if(_canFlip)
        {
            if((_currentMovementInput.x < 0 && _facingRight) || (_currentMovementInput.x > 0 && !_facingRight))
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                _facingRight = !_facingRight;
            }
        }
    }
}
