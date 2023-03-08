using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using TMPro;

// Dear programmer:
// When I wrote this code, only god and
// I knew how it worked.
// Now, only god knows it!
//
// Therefore if you are trying to optimize
// this routine and it fails (most surely),
// please increase this counter as a
// warning for the next person:
//
// total_hours_wasted_here = 22

//TODO:
//  Add coyote time
//  Implement jump cooldown
//  Refine FrontCheck()
//  Upgrade grounded detection with collider raycast
//  Add hit logic with events and a new state
public class PlayerController : MonoBehaviour
{
    // state variables
    private PlayerBaseState _currentState;
    private CombatBaseState _combatState;
    private PlayerStateFactory _movementStates;
    private CombatStateFactory _combatStates;
    private PlayerInputActions _playerInputActions;
    private Animator _animator;
    private CapsuleCollider2D _playerCollider;
    [SerializeField] private int _lastAttack;

    //Movement
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float groundDetectionDistance;
    Vector2 _currentMovementInput;
    float _appliedMovementX;
    bool _isMovementPressed; 
    bool _facingRight = true;
    bool _canFlip = true;
    bool _canMove = true;
    bool _isOnGround;
    bool _isCrouching = false;
    float _defaultGravity;

    RaycastHit2D _plungeRayCast;
    //Sliding
    Collider2D _groundCollider;
    bool _isOnSlope;

    [SerializeField] private Rigidbody2D _rb;

    //Jumping
    [Header("Jumping")]
    public LayerMask groundLayer;
    private bool _isJumpPressed;
    [SerializeField] private float jumpForce;
    public float jumpTimer;
    private float jumpTimeCounter;
    public Transform groundCheck;

    //Dash
    [Header("Dash")]
    [SerializeField]private LayerMask _dashLayers;
    [SerializeField]private Transform frontCheck;
    private RaycastHit2D _dashRay;
    bool _isDashPressed;
    bool _dashPassCheck;
    [SerializeField] private float _dashingVelocity;
    [SerializeField] private float _dashingTime;
    private Vector2 _dashingDir;
    private bool _isDashing=false;
    private bool _canDash=true;
    [SerializeField] private float _detectionDistance;
    [SerializeField] public float _dashDetectionDistance;


    //Combat
    //[Header("Combat")]
    public Vector3 lastCheckpointPosition = Vector3.zero;
    private bool _isAttackPressed;
    private bool _isHeavyAttackPressed;
    private bool _isDownPressed;
    private bool _canNotPlunge;
    private bool _canHeavyAttack;
    private bool _chargeCanceled;
    private bool _isHeavyAttackPerformed = false;
    private bool _canCombo;
    private bool _comboTriggered;
    private bool _isHit;
    private bool _isDead;
    HealthSystem _healthSystem;
    ManaSoulSystem _manaSoulSystem;
    
    //Debugging
    public TextMeshProUGUI _movementHierarchyText;
    public TextMeshProUGUI _combatStateText;

    // getters and setters
    public int LastAttack { get { return _lastAttack;} set { _lastAttack = value;}}
    public RaycastHit2D DashRay { get { return _dashRay;} }
    public bool DashPassCheck { get {return _dashPassCheck;}}
    public bool IsDead { get { return _isDead;} set { _isDead = value;}}
    public bool IsHit { get { return _isHit;} set { _isHit = value;}}
    public bool CanMove{ get { return _canMove;} set { _canMove = value;}}
    public HealthSystem HealthSystem { get { return _healthSystem;} set { _healthSystem = value;}}
    public PlayerBaseState CurrentMovementState { get { return _currentState; } set { _currentState = value; }}
    public CombatBaseState CurrentCombatState { get { return _combatState; } set { _combatState = value; }}
    public CombatStateFactory CombatFactory { get { return _combatStates;}}
    public PlayerInputActions PlayerInputActions { get { return _playerInputActions; } set { _playerInputActions = value; }}
    public Animator PlayerAnimator { get { return _animator;}}
    public CapsuleCollider2D PlayerCollider { get { return _playerCollider; } }
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; } set { _currentMovementInput = value; }}
    public float AppliedMovement { get { return _appliedMovementX; } set { _appliedMovementX = value; }}
    
    public bool IsMovementPressed { get { return _isMovementPressed; } set { _isMovementPressed = value; }}
    public bool IsJumpPressed { get { return _isJumpPressed; } set { _isJumpPressed = value; }}
    public bool IsDashPressed { get { return _isDashPressed; } set { _isDashPressed = value; } }
    public bool IsDashing { get { return _isDashing; } set { _isDashing = value; } }

    public bool IsAttackPressed { get { return _isAttackPressed;} set { _isAttackPressed = value;}}
    public bool IsHeavyAttackPressed { get { return _isHeavyAttackPressed; } set { _isHeavyAttackPressed = value;} }
    public bool IsDownPressed { get { return _isDownPressed; } set { _isDownPressed = value;} }
    public bool CanNotPlunge { get { return _canNotPlunge; } set { _canNotPlunge = value;} }

    public bool CanHeavyAttack { get { return _canHeavyAttack; } set { _canHeavyAttack = value;} }
    public bool ChargeCanceled { get { return _chargeCanceled; } set { _chargeCanceled = value;} }
    public bool CanCombo { get { return _canCombo;}}
    public bool ComboTriggered { get { return _comboTriggered;} set { _comboTriggered = value;}}

    public bool IsOnGround { get { return _isOnGround; }}
    public bool IsCrouching { get { return _isCrouching; }}
    public bool CanFlip { get {return _canFlip; } set { _canFlip = value; }}
    public bool CanDash { get { return _canDash; } set { _canDash = value; } }
    public bool IsOnSlope { get { return _isOnSlope; }}
    public bool FacingRight { get { return _facingRight;}}

    public Collider2D GroundCollider { get { return _groundCollider;}}
    public float JumpForce { get { return jumpForce; } set { jumpForce = value; }}
    public float DashingVelcoity { get { return _dashingVelocity; } set {_dashingVelocity = value; }}
    public float DashingTime { get { return _dashingTime; } set { _dashingTime = value; }}

    public Vector2 DashingDirection { get { return _dashingDir; } set { _dashingDir = value; }}
    public float MovementSpeed { get { return movementSpeed; }}
    public Rigidbody2D Rigidbod { get { return _rb; }}
    public Transform GroundCheck { get {return groundCheck; }}
    public float DefaultGravity { get {return _defaultGravity;}}
    void Awake()
    {

        //FIXME:

        _playerCollider = GetComponent<CapsuleCollider2D>();
        _defaultGravity = _rb.gravityScale;
        _movementStates = new PlayerStateFactory(this);
        _combatStates = new CombatStateFactory(this, _movementStates);
        _combatState = _combatStates.Peaceful();
        _currentState = _movementStates.InAir();
        _combatState.EnterState();
        _currentState.EnterState();
        _animator = GetComponent<Animator>();
        _healthSystem = GetComponent<HealthSystem>();
        _manaSoulSystem = GetComponent<ManaSoulSystem>();

        HealthSystem.OnHit += OnHit;
        HealthSystem.OnDead += OnDead;

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Move.started += OnMovementInput;
        _playerInputActions.Player.Move.canceled += OnMovementInput;
        _playerInputActions.Player.Move.performed += OnMovementInput;

        //_playerInputActions.Player.Attack.started += OnAttackPressed;
        _playerInputActions.Player.Attack.performed += OnAttackPressed;
        //_playerInputActions.Player.Attack.canceled += OnAttackPressed;

        _playerInputActions.Player.Attack.started += OnHeavyAttackPressed;
        _playerInputActions.Player.Attack.performed += OnHeavyAttackPressed;
        _playerInputActions.Player.Attack.canceled += OnHeavyAttackPressed;

        _playerInputActions.Player.Down.started += OnDown;
        _playerInputActions.Player.Down.performed += OnDown;
        _playerInputActions.Player.Down.canceled += OnDown;

        _playerInputActions.Player.Jump.performed += OnJump;
        //_playerInputActions.Player.Jump.started += OnJump;
        //_playerInputActions.Player.Jump.canceled += OnJump;

        //_playerInputActions.Player.Dash.started += OnDash;
        //_playerInputActions.Player.Dash.canceled += OnDash;
        _playerInputActions.Player.Dash.performed += OnDash;

        _playerInputActions.Player.Crouch.started += OnCrouch;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CurrentCombatState.UpdateState();
        CurrentMovementState.UpdateStates();
        CheckForGround();
        FlipPlayer();
        CheckPassable();
    }
    private void FixedUpdate()
    {
            Move(_appliedMovementX);
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }
    void OnDown(InputAction.CallbackContext context)
    {
        _isDownPressed = context.ReadValueAsButton();
    }
    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = true;
    } 
    public void OnDash(InputAction.CallbackContext context)
    {
        if (_manaSoulSystem.currentMana >= 10)
        {
            _isDashPressed = true;
            _manaSoulSystem.UseMana(10);
        }
        else
            _isDashPressed = false;
    }
    void OnCrouch(InputAction.CallbackContext context)
    {
        if(_currentState == _movementStates.Grounded() && !_isCrouching)
        {
            _isCrouching = true;
        }
        else
        {
            _isCrouching = false;
        }
    }
    void OnAttackPressed(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction) {
            if(_canCombo)
            {
                _comboTriggered = true;
            }
            else
            {
                _isAttackPressed = true;
            }
        }


    }
    void OnHeavyAttackPressed(InputAction.CallbackContext context)
    {
       if (context.interaction is HoldInteraction) {
            //TODO:
            //  Combo yaparken charge'a girebiliyor, onu düzelt
            if (context.phase is InputActionPhase.Started)
            {
                _isHeavyAttackPressed = true;
                _isHeavyAttackPerformed = false;
            }
            else if (context.phase is InputActionPhase.Performed)
            {
                _canHeavyAttack = true;
                _isHeavyAttackPerformed = true;
            }
            else if (context.phase is InputActionPhase.Canceled)
            {
                if (!_isHeavyAttackPerformed)
                {
                    // heavy attacktan peacefula geçtikten sonra cancaled gerçekleşiyor charge cancaled true kalıyor.
                    _chargeCanceled = true;
                }
            }

        }
    }

    void OnHit(object sender, EventArgs e)
    {
        if(!IsDead)
        {
            IsHit = true;
        }
    }

    void OnDead(object sender, EventArgs e)
    {
        if(!IsDead)
        {
            IsDead = true;
        }
    }
    void CheckForGround()
    {
        _groundCollider = Physics2D.OverlapCircle(groundCheck.position, groundDetectionDistance, groundLayer);
        _isOnGround = _groundCollider;

        _plungeRayCast = Physics2D.Raycast(groundCheck.position, -transform.up, 7f, groundLayer);

        _canNotPlunge = _plungeRayCast;
    }
    public void CheckPassable(){ 

        // Create dash check 
        if(_facingRight)
        {
            _dashRay = Physics2D.Raycast(frontCheck.position, transform.right, _dashDetectionDistance, _dashLayers);
        }
        else
        {
            _dashRay = Physics2D.Raycast(frontCheck.position, -transform.right, _dashDetectionDistance, _dashLayers);
        }

        _dashPassCheck = _dashRay;
    }
    void Move(float movementInput)
    {
        if(_canMove)
            _rb.velocity = new Vector2(movementInput, _rb.velocity.y);
        /*else
            _rb.velocity = new Vector2(0, _rb.velocity.y);*/
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

    void SetCanComboTrue()
    {
        _canCombo = true;
    }

    void SetCanComboFalse()
    {
        _canCombo = false;
    }
}
