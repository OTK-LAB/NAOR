using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class PlayerController : MonoBehaviour
{
    //Movement
    [Header("Movement")]
    public float runSpeed;
    public float walkSpeed;
    private float xAxis;
    private bool walkToggle;
    [HideInInspector] public bool isStunned = false;
    [HideInInspector] public bool facingRight = true;
    private Rigidbody2D rb;
    private bool isPraying;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool canFlip = true;

    [Header("Roll")]
    public float rollSpeed;
    private bool isRolling = false;
    public GameObject rollColl;
    public float iFrame = 0.3f;

    //Jumping
    [Header("Jumping")]
    public float jumpForce;
    private bool isJumping = false;
    public float jumpTimer;
    private float jumpTimeCounter;
    [HideInInspector] public bool isGrounded;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;

    [Header("Wall Slide")]
    public Transform wallGrabPointFront;
    public Transform wallGrabPointBack;
    public float wallSlideSpeed = 0.2f;
    private bool isWallSliding = false;
    private bool grabFront, grabBack, canGrab;
    public float wallDistance = 0.05f;

    [Header("Wall Jump")]
    public float wallJumpTime = 0.1f;
    public float xWallForce = 5f;
    public float wallJumpLerp = 1f;
    private float jumpTime;
    private int wallDirection;
    private bool wallJumpPressed = false;
    private bool isWallJumping = false;

    [Header("Ledge Climb")]
    public Transform ledgeCheckUp;
    public Transform ledgeCheckDown;
    private bool isTouchingLedgeUp;
    private bool isTouchingLedgeDown;
    private bool canClimbLedge = false;
    private bool ledgeDetected;
    private Vector2 ledgePosBot;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;
    RaycastHit2D hit;

    [SerializeField]
    private float ledgeDistance;
    public float ledgeXOffset1 = 0.0f;
    public float ledgeYOffset1 = 0.0f;
    public float ledgeXOffset2 = 0.0f;
    public float ledgeYOffset2 = 0.0f;
    private float gravity;

    //Animations
    private Animator animator;
    private string currentState;
    const string idle = "PlayerIdle";
    const string run = "PlayerRun";
    const string jump = "PlayerJump";
    const string fall = "PlayerFall";
    const string roll = "PlayerRoll";
    const string pray = "PlayerPray";
    const string parry = "PlayerParry";
    const string fallattack = "PlayerFallAttack";
    const string climb = "PlayerClimb";
    const string stun = "PlayerStun";
    const string wallSlide = "PlayerWallSlide";
    const string ledgeClimb = "PlayerLedgeClimb";
    const string ledgeHang = "PlayerLedgeHang";


    //Combat
    [Header("Combat")]
    public Transform attackPoint;
    public Transform fallAttackBox;
    public Vector3 fallAttackSize;
    private float attackTime = 0.0f;
    private int attackCount = 0;
    public float attackRange = 0.5f;
    public float attackDamage = 10;
    public LayerMask enemyLayers;
    public bool isAttacking;
    private bool isFallAttacking;
    private PlayerManager playerManager;
    private float stamina;
    [HideInInspector] public bool inCheckpointRange;
    [HideInInspector] public bool dead = false;
    [HideInInspector] public bool isCombo = false;
    [HideInInspector] public bool isGuarding = false;
    private float guardTimer;
    private bool parryStamina;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject daggerobj;
    [SerializeField] private int daggerAmount;
    [SerializeField] public float cooldownTime;
    private CooldownController daggerCooldownController;
    private ItemStack daggerStack;

    //Gems
    public float rollStaminaRate=0;
    public float lifeStealRate = 0;
    public float rollStaminaCost =30f;
    public float shieldStamina = 10f;
    public float rollSeconds = 0.5f;
    public bool isRegen = false;

    [Header("Miscellaneous")]
    //Move list
    public GameObject moveList;

    private void Awake()
    {
        daggerStack = GetComponent<ItemStack>();
        daggerStack.SetItem(daggerobj, daggerAmount);
        daggerCooldownController = GetComponent<CooldownController>();
        daggerCooldownController.SetCooldown(cooldownTime);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravity = rb.gravityScale;
        animator = GetComponent<Animator>();
        playerManager = GetComponent<PlayerManager>();
    }
    void Update()
    {
        CheckState();
        CheckInputs();
        CheckAttack();
        ChangeAnimations();
        FlipPlayer();
        CheckLedgeClimb();
        stamina = StaminaBar.instance.currentStamina;
        if (daggerCooldownController.GetQueue().Count > 0)
        {
            if (Time.time >= daggerCooldownController.GetDequeueTime())
            {
                //DequeueLastItem() fonksiyonuyla Queue'dan cikardigin dagger'i Stack'e koy
                daggerStack.PushToStack(daggerCooldownController.DequeueLastItem());
            }
        }
    }
    void FixedUpdate()
    {
        Move();
        //Jump();
        WallJump();
        WallSlide();
    }

    
    void CheckState()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        grabFront = Physics2D.OverlapCircle(wallGrabPointFront.position, wallDistance, groundLayer);
        grabBack = Physics2D.OverlapCircle(wallGrabPointBack.position, wallDistance, groundLayer);
        if(facingRight)
        {
            isTouchingLedgeUp = Physics2D.Raycast(ledgeCheckUp.position, transform.right, ledgeDistance, groundLayer);
            hit = Physics2D.Raycast(ledgeCheckDown.position, transform.right, ledgeDistance, groundLayer);
            isTouchingLedgeDown = hit;
            if(isTouchingLedgeDown)
            {
                ledgePosBot = ledgeCheckDown.position;
             }
        }
        else
        {
            isTouchingLedgeUp = Physics2D.Raycast(ledgeCheckUp.position, -transform.right, ledgeDistance, groundLayer);
            hit = Physics2D.Raycast(ledgeCheckDown.position, -transform.right, ledgeDistance, groundLayer);
            isTouchingLedgeDown = hit;
            if(isTouchingLedgeDown)
            { 
                ledgePosBot = ledgeCheckDown.position;
            }        
        }

        if(grabFront || grabBack)
        {
            canGrab = true;
        }
        else
        {
            canGrab = false;
        }

        if(grabFront && !grabBack)
        {
            if(wallGrabPointFront.transform.position.x > wallGrabPointBack.transform.position.x)
            {
                wallDirection = -1;
            }
            else
            {
                wallDirection = 1;
            }
        }
        else if(!grabFront && grabBack)
        {
            if(wallGrabPointBack.transform.position.x > wallGrabPointFront.transform.position.x)
            {
                wallDirection = -1;
            }
            else
            {
                wallDirection = 1;
            }
        }

        if(isTouchingLedgeDown && !isTouchingLedgeUp && hit.collider.CompareTag("Climbable") && !ledgeDetected ){
            ledgeDetected = true;
        }

        //stop rolling if not grounded
        if(isRolling && !isGrounded)
        {
            StopCoroutine(Roll());
            rb.linearVelocity = new Vector2(xAxis * runSpeed, rb.linearVelocity.y);
        }

    }
    void CheckInputs()
    {
        //Get Horizontal Input
        if(!isWallJumping && !canClimbLedge)
        {
            xAxis = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            xAxis = 0;
        }
        //Walk Toggle
        if (Input.GetKeyDown(KeyCode.LeftAlt))
            walkToggle = !walkToggle;
        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded && !isPraying && !isAttacking && !isFallAttacking && !isWallSliding && !playerManager.isHealing && !isStunned)
            {
                if (isRolling)
                {
                    StopCoroutine(Roll());
                    rb.linearVelocity = new Vector2(xAxis * runSpeed, rb.linearVelocity.y);
                }
                isJumping = true;
                jumpTimeCounter = jumpTimer;
                Jump();
            }
            if (isWallSliding)
                wallJumpPressed = true;
        }
        if (Input.GetButton("Jump") && isJumping)
            if (jumpTimeCounter > 0)
            {
                jumpTimeCounter -= Time.deltaTime;
                Jump();
            }
            else 
                isJumping = false;
                
        if(Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }
        //Pray
        if (Input.GetKeyDown(KeyCode.C))
            if (isGrounded && inCheckpointRange && !isPraying && !isAttacking && !isFallAttacking && !isGuarding && !isRolling && !playerManager.hitAnimRunning && !playerManager.isHealing && !isStunned)
            {
                isPraying = true;
                rb.linearVelocity = new Vector2(0, 0);
            }
        //Roll
        if (Input.GetKeyDown(KeyCode.LeftShift))
            if (isGrounded && !isRolling && !isPraying && !isAttacking && !isFallAttacking && !playerManager.hitAnimRunning && !playerManager.isHealing && !isStunned && stamina >= 30 && !isGuarding && !isJumping)
                StartCoroutine(Roll()); 
        //Guard
        if (Input.GetMouseButton(1))
            if (isGrounded && !playerManager.hitAnimRunning && stamina >= 10)
                if(!isFallAttacking){
                    performGuard();
                }
        if (Input.GetMouseButtonUp(1))
            if (isGuarding)
            {
                isGuarding = false;
                parryStamina = false;
                guardTimer = 0;
            }
        //Dagger
        if (Input.GetMouseButtonDown(2))
            if (!isBusy())
                ThrowDagger();
        //Potion
        if (Input.GetKeyDown(KeyCode.R))
        {
            if(isGrounded && !isRolling && !isAttacking && !isFallAttacking && !isPraying && !playerManager.hitAnimRunning && !playerManager.isReviving && !playerManager.isHealing  && !isStunned)
            {
                if (Potion.instance.potionCount > 0 && playerManager.CurrentHealth < 100 && playerManager.lives == 4)
                {
                    playerManager.HealthPotion(33);
                    Potion.instance.UsePotions(1);
                }
            }
        }
        //Toggle Move List
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            moveList.SetActive(!moveList.activeInHierarchy);
        }
    }
    
    
    void ChangeAnimations()
    {
        //Ground Animations --> Idle, Run, Attack and Roll
        if(isGrounded && !isFallAttacking && !playerManager.hitAnimRunning && !playerManager.isReviving && !playerManager.isHealing && !canClimbLedge)
        {
            if(!isRolling && !isGuarding)
            {
                    if(!isAttacking && !isPraying && !isStunned)
                    { 
                        if(xAxis == 0)
                            ChangeAnimationState(idle);
                        else
                            ChangeAnimationState(run); 
                    }
                    if(isAttacking)
                    {                 
                        ChangeAnimationState("PlayerAttack" + attackCount);
                        if(attackTime > 0.6f)    
                            isAttacking = false;
                    }
                    if(isPraying)
                    {
                        ChangeAnimationState(pray);
                        StartCoroutine(StopPraying());
                    }
                    if(isStunned)
                        ChangeAnimationState(stun);
            }
            else if(isRolling && !isGuarding)
                ChangeAnimationState(roll);
        }

        //Air Animations --> Jump and Fall
        if(!isGrounded && !canClimbLedge)
        {
            if(!isAttacking && !isFallAttacking && !isWallSliding)
            {
                if(rb.linearVelocity.y > 0)
                    ChangeAnimationState(jump);
                if(rb.linearVelocity.y < 0)
                    ChangeAnimationState(fall);    
            }
            else
            {       
                if(isAttacking) 
                {
                    ChangeAnimationState("PlayerAttack" + attackCount);
                    if(attackTime > 0.6f)    
                        isAttacking = false;
                }       
                if(isFallAttacking)
                {
                    ChangeAnimationState(fallattack);
                }
                if (isWallSliding)
                    ChangeAnimationState(wallSlide);
            }
        }
   
     }
    public void ChangeAnimationState(string newState)
    {
        if(currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
    }
    IEnumerator StopPraying()
    {
        yield return new WaitForSeconds(1.7f);
        isPraying = false;
    }
    private void OnDrawGizmosSelected() 
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireCube(fallAttackBox.position, fallAttackSize);
    }

    public bool isBusy()
    {
        if (isAttacking || isFallAttacking || isGuarding || isPraying || isRolling || playerManager.isReviving || playerManager.hitAnimRunning || playerManager.isHealing || canClimbLedge)
            return true;
        else
            return false;
    } 

    public ItemStack GetDaggerStack(){
        return daggerStack;
    }
}
