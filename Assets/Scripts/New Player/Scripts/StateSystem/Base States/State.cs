using Unity.VisualScripting;
using UnityEngine;

public class State
{
    protected Ultimate2DPlayer player;
    protected PlayerStateMachine stateMachine;
    protected Rigidbody2D rigidbody2D;
    private readonly Ultimate2DPlayer.StateName _animEnum;
    protected PlayerData playerData;
    protected PlayerInputManager inputManager;

    protected float localTime;

    public State(Ultimate2DPlayer player, PlayerStateMachine stateMachine, Ultimate2DPlayer.StateName animEnum, PlayerData playerData)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        _animEnum = animEnum;
        this.playerData = playerData;
        this.inputManager = player.InputManager;
    }

    public virtual void Enter()
    {
        rigidbody2D = player.Rigidbody2D;
        PhysicsCheck();
        player.Animator.SetBool(_animEnum.ToString(), true);
        player.CurrentState = _animEnum;
        localTime = 0f;
    }

    public virtual void Update()
    {
        localTime += Time.deltaTime;
        if (inputManager.Input_Walk != 0 && Mathf.Sign(inputManager.Input_Walk) != Mathf.Sign(player.transform.localScale.x)) RotatePlayer();

        SwitchStateLogic();
    }

    public virtual void FixedUpdate()
    {
        PhysicsCheck();
    }

    public virtual void Exit()
    {
        player.Animator.SetBool(_animEnum.ToString(), false);
        localTime = 0f;
    }

    public virtual void PhysicsCheck()
    {
        playerData.Check.GroundCheckPosition = new (0, -Mathf.Abs(player.CapsuleCollider2D.bounds.extents.x - player.CapsuleCollider2D.bounds.extents.y));
        playerData.Check.GroundCheckPosition += player.transform.position;
        float _radius = player.CapsuleCollider2D.bounds.extents.x * 1.1f;
        playerData.Check.IsGrounded = Physics2D.OverlapCircle(playerData.Check.GroundCheckPosition, _radius, playerData.Check.GroundLayer);

        playerData.Check.CanJump = (playerData.Jump.JumpBufferTimer > 0 && playerData.Jump.CoyoteTimeTimer > 0)
                            || (playerData.Check.IsGrounded && (playerData.Jump.CoyoteTimeMaxTime == 0 || playerData.Jump.JumpBufferMaxTime == 0));

        SlopeCheck();
        playerData.Check.IsOnMovableSlope = playerData.Check.IsOnSlope && playerData.Check.CurrentSlopeAngle <= playerData.Check.MaxSlopeAngle;
        SetPhysicsMaterial();

    }
    public virtual void SwitchStateLogic() { }

    #region Protected Functions
    protected void RotatePlayer()
    {
        player.transform.localScale = Vector3.Scale(player.transform.localScale, new Vector3(-1, 1, 1));
    }

    protected void SetPhysicsMaterial()
    {
        if (playerData.Check.IsOnSlope)
        {
            if (playerData.Check.CurrentSlopeAngle < playerData.Check.MaxSlopeAngle && player.InputManager.Input_Walk == 0)
            {
                player.Rigidbody2D.sharedMaterial = playerData.Material.InfFriction;
            }
            else if (playerData.Check.CurrentSlopeAngle < playerData.Check.MaxSlopeAngle && player.InputManager.Input_Walk != 0)
            {
                player.Rigidbody2D.sharedMaterial = playerData.Material.ZeroFriction;
            }
        }
        else
        {
            player.Rigidbody2D.sharedMaterial = playerData.Material.ZeroFriction;
        }
    }

    protected float SetCurveTimeByValue(AnimationCurve curve, float value, float maxTime, bool allowGreater)
    {
        float curveTime = 0f;
        while ((allowGreater && curve.Evaluate(curveTime) < value) || (!allowGreater && curve.Evaluate(curveTime) >= value))
        {
            curveTime += Time.fixedDeltaTime;
            if (curveTime > maxTime) break;
        }
        return curveTime;
    }
    #endregion
    #region Private Functions
    private void SlopeCheck()
    {
        if (player.CapsuleCollider2D.GetContacts(playerData.Check.ColliderContacs) > 0 && playerData.Check.IsGrounded)
        {
            Vector2 min = playerData.Check.ColliderContacs[0].point, max = min;
            for (int i = 0; i < playerData.Check.ColliderContacs.Count && !(playerData.Check.ColliderContacs[i].point == Vector2.zero && i != 0); i++)
            {
                min = playerData.Check.ColliderContacs[i].point.x < min.x ? playerData.Check.ColliderContacs[i].point : min;
                max = playerData.Check.ColliderContacs[i].point.x > max.x ? playerData.Check.ColliderContacs[i].point : max;
            }
            RaycastHit2D newHit = Physics2D.Raycast(player.transform.localScale.x > 0 ? max : min, Vector2.down, 0.01f, playerData.Check.GroundLayer);
            if (newHit)
            {
                playerData.Check.SlopeContactPosition = newHit.point;
                playerData.Check.OnSlopeSpeedDirection = Vector2.Perpendicular(newHit.normal).normalized;
                playerData.Check.CurrentSlopeAngle = Vector2.Angle(newHit.normal, Vector2.up);
                playerData.Check.IsOnSlope = (playerData.Check.CurrentSlopeAngle != 0 && playerData.Check.IsGrounded && newHit.point.y < playerData.Check.GroundCheckPosition.y + 0.25f);
                Debug.DrawRay(newHit.point, playerData.Check.OnSlopeSpeedDirection, Color.green);
                Debug.DrawRay(newHit.point, newHit.normal, Color.red);
            }
        }
        else
        {
            playerData.Check.SlopeContactPosition = Vector2.zero;
            if (playerData.Check.IsOnSlope) playerData.Check.IsOnSlope = false;
            if (playerData.Check.CurrentSlopeAngle != 0f) playerData.Check.CurrentSlopeAngle = 0f;
        }
    }
    #endregion
}
