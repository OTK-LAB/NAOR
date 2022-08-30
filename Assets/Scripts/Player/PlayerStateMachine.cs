using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//TODO:
//  Add coyote time
//  Implement jump cooldown
//  Refine FrontCheck()
//  Upgrade grounded detection with collider raycast
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
    float _defaultGravity;

    //Dragging and Ledge Detection
    [SerializeField] private float _detectionDistance;
    bool _toggleDrag = false;
    public Transform frontCheck;
    public Transform _ledgeCheckTop;
    public Transform _ledgeCheckBot;
    bool _thereIsGroundFront;
    bool _thereIsGroundTop;
    bool _thereIsGroundBot;
    bool _canDrag;
    bool _canClimbLedge;
    RaycastHit2D frontRay;

    //Sliding
    Collider2D _groundCollider;
    bool _isOnSlope;

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
    
    public bool IsMovementPressed { get { return _isMovementPressed; } set { _isMovementPressed = value; }}
    public bool IsJumpPressed { get { return _isJumpPressed; } set { _isJumpPressed = value; }}
    public bool IsOnGround { get { return _isOnGround; }}
    public bool IsCrouching { get { return _isCrouching; }}
    public bool DragToggle { get { return _toggleDrag; }}
    public bool CanFlip { get {return _canFlip; } set { _canFlip = value; }}
    public bool IsOnSlope { get { return _isOnSlope; }}
    public bool CanClimbLedge { get { return _canClimbLedge; }}
    public bool FacingRight { get { return _facingRight;}}

    public Collider2D GroundCollider { get { return _groundCollider;}}
    public float JumpForce { get { return jumpForce; } set { jumpForce = value; }}
    public float MovementSpeed { get { return movementSpeed; }}
    public Rigidbody2D Rigidbod { get { return _rb; }}
    public RaycastHit2D Ray { get {return frontRay; }}
    //For workaround in Drag. Will be changed
    public Transform GroundCheck { get {return groundCheck; }}
    public Transform FrontCheck { get {return frontCheck; }}
    public float DefaultGravity { get {return _defaultGravity;}}
    void Awake()
    {
        _defaultGravity = _rb.gravityScale;
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
        CurrentState.UpdateStates();
        CheckForLedges();
        CheckForGround();
        FlipPlayer();
        CheckFront();
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
        if(_currentState == _states.Grounded() && !_isCrouching)
        {
            _isCrouching = true;
        }
        else
        {
            _isCrouching = false;
        }
    }
    void OnDrag(InputAction.CallbackContext context)
    {   
        if(_currentState == _states.Grounded() && !_toggleDrag && _canDrag)
        {
            _toggleDrag = true;
        }
        else
        {
            _toggleDrag = false;
        }
    }
    void CheckForGround()
    {
        _groundCollider = Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);
        _isOnGround = _groundCollider;
        if(_isOnGround && _groundCollider.CompareTag("Slope"))
        {
            _isOnSlope = true;
        }
        else{
            _isOnSlope = false;
        }
        //Debug.Log("IS ON SLOPE: " + _isOnSlope);
    }
    public void CheckFront(){
        frontRay = Physics2D.Raycast(frontCheck.position, transform.right, _detectionDistance, groundLayer);
        _thereIsGroundFront = frontRay;

        if(_thereIsGroundFront && frontRay.collider.CompareTag("Movable")){
            _canDrag = true;
            //Debug.Log("Candrag");
        }
        else
        {
            _canDrag = false;
        }
    }

    public void CheckForLedges()
    {
        _thereIsGroundTop = Physics2D.Raycast(_ledgeCheckTop.position, transform.right, _detectionDistance, groundLayer);
        _thereIsGroundBot = Physics2D.Raycast(_ledgeCheckBot.position, transform.right, _detectionDistance, groundLayer);

        if(_thereIsGroundBot && !_thereIsGroundTop)
        {
            _canClimbLedge = true;
        }
        else
        {
            _canClimbLedge = false;
        }
        //Debug.Log("Can Climb Ledge: " + _canClimbLedge);
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
