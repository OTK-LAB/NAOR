using UnityEngine;
using UnityEngine.InputSystem;
using static Ultimate2DPlayer;


public class PlayerInputManager : MonoBehaviour
{
    private Ultimate2DPlayer player;
    private PlayerData playerData;
    public InputActions playerControls;

    [SerializeField, NonEditable] private float input_Walk;
    [SerializeField, NonEditable] private bool input_Jump;
    [SerializeField, NonEditable] private bool input_Dash;
    [SerializeField, NonEditable] private bool input_Crouch;
    [SerializeField, NonEditable] private bool input_BasicAttack;


    public float Input_Walk => input_Walk;
    public bool Input_Jump => input_Jump;
    public bool Input_Dash => input_Dash;
    public bool Input_Crouch => input_Crouch;
    public bool Input_BasicAttack { get { return input_BasicAttack; } set { input_BasicAttack = value; } }

    public PlayerInputManager(Ultimate2DPlayer player, PlayerData playerData)
    {
        this.player = player;
        this.playerData = playerData;
    }


    private void Awake()
    {
        playerControls = new InputActions();
        player = GetComponent<Ultimate2DPlayer>();
        playerData = player.PlayerData;
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
        playerControls.Player.Jump.started += OnJumpStarted;
        playerControls.Player.Jump.canceled += OnJumpCanceled;
        playerControls.Player.Walk.performed += OnWalk;
        playerControls.Player.Walk.canceled += OnWalk;
        playerControls.Player.Dash.performed += OnDash;
        playerControls.Player.Dash.canceled += OnDash;
        playerControls.Player.Crouch.performed += OnCrouch;
        playerControls.Player.Crouch.canceled += OnCrouch;
        playerControls.Player.BasicAttack.started += OnBasicAttack;
    }

    private void FixedUpdate()
    {
        UpdateTimers();
    }

    private void UpdateTimers()
    {
        if (playerData.Check.IsGrounded && player.CurrentState != StateName.Jump)
        {
            playerData.Jump.CoyoteTimeTimer = playerData.Jump.CoyoteTimeMaxTime;
        }

        playerData.Jump.JumpBufferTimer -= playerData.Jump.JumpBufferTimer > 0 ? Time.deltaTime : 0f;
        playerData.Jump.CoyoteTimeTimer -= playerData.Jump.CoyoteTimeTimer > 0 ? Time.deltaTime : 0f;
    }

    #region Input Event Functions
    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        input_Jump = context.ReadValueAsButton();
        playerData.Jump.JumpBufferTimer = playerData.Jump.JumpBufferMaxTime;
        playerData.Check.CutJump = false;
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        input_Jump = context.ReadValueAsButton();
        playerData.Check.CutJump = true;
    }

    private void OnWalk(InputAction.CallbackContext context)
    {
        input_Walk = context.ReadValue<float>();
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        input_Dash = context.ReadValueAsButton();
    }

    private void OnCrouch(InputAction.CallbackContext context)
    {
        input_Crouch = context.ReadValueAsButton();
    }

    private void OnBasicAttack(InputAction.CallbackContext context)
    {
        input_BasicAttack = context.ReadValueAsButton();
    }
    #endregion
}
