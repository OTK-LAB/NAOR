using UnityEngine;
namespace UltimateCC
{
    public class PlayerLandState : MainState, IMove2D
    {
        enum Phase { SpeedUp, SlowDown, TurnBack, Null }
        Phase phase;
        float xCurveTime;
        private float localXVelovity;
        private int turnBackStartDirection;
        public PlayerLandState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            rigidbody2D.gravityScale = playerData.Land.Physics2DGravityScale;
            if (rigidbody2D.velocity.x != 0)
            {
                phase = inputManager.Input_Walk != 0 ? Phase.SpeedUp : Phase.SlowDown;
            }
            else
            {
                phase = Phase.Null;
            }
            localXVelovity = 0;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            Move2D();
            rigidbody2D.velocity += playerData.Physics.Platform.DampedVelocity;
            if (playerData.Physics.IsMultipleContactWithNonWalkableSlope)
            {
                rigidbody2D.velocity = new(0f, -1f);
            }
            xCurveTime += Time.fixedDeltaTime;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void PhysicsCheck()
        {
            base.PhysicsCheck();
            EssentialPhysics.CheckHingeJoint(playerData, player);
            EssentialPhysics.LedgeCheck(playerData, player);
        }

        public override void SwitchStateLogic()
        {
            base.SwitchStateLogic();
           if(playerData.Physics.LedgeHangPosition != Vector2.zero && localTime > 0.3f)
            {
                stateMachine.ChangeState(player.HangState);
            }
            else if ((((playerData.Physics.IsGrounded && !playerData.Physics.IsOnNotWalkableSlope)) && inputManager.Input_Walk == 0) || playerData.Physics.IsMultipleContactWithNonWalkableSlope)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else if ((playerData.Physics.IsGrounded && !playerData.Physics.IsOnNotWalkableSlope))
            {
                stateMachine.ChangeState(player.WalkState);
            }
            else if (inputManager.Input_Jump && playerData.Physics.CanJump)
            {
                stateMachine.ChangeState(player.JumpState);
            }
            else if (inputManager.Input_Jump && playerData.Physics.CanWallJump)
            {
                stateMachine.ChangeState(player.WallJumpState);
            }
            else if (inputManager.Input_Dash && playerData.Dash.DashCooldownTimer <= 0f)
            {
                stateMachine.ChangeState(player.DashState);
            }
            else if (playerData.Physics.IsNextToWall)
            {
                if (inputManager.Input_WallGrab && playerData.Walls.CurrentStamina > 0)
                {
                    stateMachine.ChangeState(player.WallGrabState);
                }
                else if (inputManager.Input_Walk != 0 && Mathf.Sign(inputManager.Input_Walk) == Mathf.Sign(playerData.Physics.WallDirection))
                {
                    stateMachine.ChangeState(player.WallSlideState);
                }
            }
            else if (playerData.Physics.CanPlungeAttack && inputManager.Input_PlungeAttack)
            {
                stateMachine.ChangeState(player.PlungeAttackDiveState);
            }
            else if (playerData.Physics.ConnectedHingeJoint)
            {
                stateMachine.ChangeState(player.SwingState);
            }
            else if (playerData.Physics.CanGlideByHeight && inputManager.Input_Jump && playerData.Glide.GlideBufferTimer > 0f)
            {
                stateMachine.ChangeState(player.GlideState);
            }
        }

        public void Move2D()
        {
            Vector2 _newVelocity = Vector2.zero;
            _newVelocity.y = playerData.Land.LandVelocityCurve.Evaluate(localTime / playerData.Land.LandTime);
            _newVelocity.y *= playerData.Jump.Jumps[0].MaxHeight * 1 / playerData.Land.LandTime;
            _newVelocity.y = Mathf.Clamp(_newVelocity.y, playerData.Land.MinLandSpeed, float.MaxValue);

            if (!playerData.Physics.IsOnNotWalkableSlope || playerData.Physics.FacingDirection != Mathf.Sign(playerData.Physics.ContactPosition.x - rigidbody2D.position.x))
            {
                _newVelocity.x = VelocityOnx();
            }
            rigidbody2D.velocity = _newVelocity;
            localXVelovity = rigidbody2D.velocity.x;
        }

        private float VelocityOnx()
        {
            float XVelocity;
            if (inputManager.Input_Walk != 0 && (localXVelovity == 0 || Mathf.Sign(inputManager.Input_Walk) == Mathf.Sign(localXVelovity))
                && (phase != Phase.TurnBack || xCurveTime > playerData.Walk.TurnBackTime))
            {
                if (phase != Phase.SpeedUp && (phase != Phase.TurnBack || xCurveTime > playerData.Walk.TurnBackTime))
                {
                    xCurveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Land.XSpeedUpCurve, Mathf.Abs(rigidbody2D.velocity.x) / playerData.Land.MaxXSpeed, 1, true);
                    xCurveTime *= playerData.Land.XSpeedUpTime;
                    phase = Phase.SpeedUp;
                }
                if (xCurveTime < playerData.Land.XSpeedUpTime)
                {
                    XVelocity = playerData.Land.XSpeedUpCurve.Evaluate(xCurveTime / playerData.Land.XSpeedUpTime);
                }
                else
                {
                    XVelocity = playerData.Land.XSpeedUpCurve.Evaluate(1f);
                }
                XVelocity *= inputManager.Input_Walk * playerData.Land.MaxXSpeed;
            }
            else if (inputManager.Input_Walk != 0 && ((localXVelovity != 0 && Mathf.Sign(inputManager.Input_Walk) != Mathf.Sign(localXVelovity)) || phase == Phase.TurnBack))
            {
                if (phase != Phase.TurnBack)
                {
                    xCurveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Land.XTurnBackCurve, Mathf.Abs(localXVelovity) / playerData.Land.MaxXSpeed, 2f, false);
                    xCurveTime *= playerData.Land.XTurnBackTime;
                    phase = Phase.TurnBack;
                    turnBackStartDirection = (int)Mathf.Sign(localXVelovity);
                }
                if (xCurveTime < playerData.Land.XTurnBackTime)
                {
                    XVelocity = playerData.Land.XTurnBackCurve.Evaluate(xCurveTime / playerData.Land.XTurnBackTime);
                }
                else
                {
                    XVelocity = playerData.Land.XTurnBackCurve.Evaluate(2f);
                }
                XVelocity *= playerData.Land.MaxXSpeed * turnBackStartDirection;
            }
            else if (localXVelovity != 0)
            {
                if (phase != Phase.SlowDown)
                {
                    xCurveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Land.XSlowDownCurve, Mathf.Abs(rigidbody2D.velocity.x) / playerData.Land.MaxXSpeed, 1, false);
                    xCurveTime *= playerData.Land.XSlowDownTime;
                    phase = Phase.SlowDown;
                }
                if (xCurveTime < playerData.Land.XSlowDownTime)
                {
                    XVelocity = playerData.Land.XSlowDownCurve.Evaluate(xCurveTime / playerData.Land.XSlowDownTime);
                }
                else
                {
                    XVelocity = playerData.Land.XSlowDownCurve.Evaluate(1f);
                }
                XVelocity *= playerData.Land.MaxXSpeed * playerData.Physics.FacingDirection;
            }
            else
            {
                XVelocity = 0f;
                phase = Phase.Null;
            }
            return XVelocity;
        }
    }
}
