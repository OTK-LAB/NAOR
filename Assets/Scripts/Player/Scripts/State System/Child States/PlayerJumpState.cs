using UnityEngine;

namespace UltimateCC
{
    public class PlayerJumpState : MainState, IMove2D
    {
        enum Phase { SpeedUp, SlowDown, TurnBack, Null }
        Phase phase;
        float cutJumpTime;
        float xCurveTime;
        private float localXVelovity;
        private int turnBackStartDirection;
        PlayerData.JumpVariables.JumpInfo jumpInfo;
        public PlayerJumpState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            if (playerData.Jump.Jumps.Count <= 1) playerData.Jump.NextJumpInt = 1;
            switch (playerData.Jump.NextJumpInt)
            {
                case 1:
                    jumpInfo = playerData.Jump.Jumps[0];
                    break;
                case 2:
                    jumpInfo = playerData.Jump.Jumps[1];
                    break;
                case 3:
                    jumpInfo = playerData.Jump.Jumps[2];
                    break;
                default:
                    Debug.LogError("jump array error");
                    break;
            }
            playerData.Jump.NextJumpInt++;
            player.Animator.SetBool(_animEnum.ToString(), false);
            player.Animator.SetBool(jumpInfo.Animation.ToString(), true);
            player.Rigidbody2D.velocity = new Vector2(player.Rigidbody2D.velocity.x, jumpInfo.MaxHeight);
            playerData.Jump.JumpBufferTimer = 0;
            cutJumpTime = 0f;
            playerData.Physics.CutJump = false;
            playerData.Jump.NewJump = false;
            rigidbody2D.gravityScale = playerData.Jump.Physics2DGravityScale;
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
            if (cutJumpTime == 0 && playerData.Physics.CutJump)
            {
                cutJumpTime = localTime;
                localTime = cutJumpTime * (cutJumpTime + (jumpInfo.JumpTime - cutJumpTime) * jumpInfo.TimeScaleOnCut) / jumpInfo.JumpTime;
            }
            Move2D();
            rigidbody2D.velocity += playerData.Physics.Platform.DampedVelocity;
            xCurveTime += Time.fixedDeltaTime;
        }

        public override void Exit()
        {
            base.Exit();
            player.Animator.SetBool(jumpInfo.Animation.ToString(), false);
            playerData.Physics.CutJump = false;
        }

        public override void PhysicsCheck()
        {
            base.PhysicsCheck();
            if (localTime > 0.2f)
            {
                EssentialPhysics.CheckHingeJoint(playerData, player);
                EssentialPhysics.LedgeCheck(playerData, player);
            }
        }

        public override void SwitchStateLogic()
        {
            base.SwitchStateLogic();
            if(playerData.Physics.LedgeHangPosition != Vector2.zero && localTime > 0.3f)
            {
                stateMachine.ChangeState(player.HangState);
            }
            else if (localTime >= jumpInfo.JumpTime || (playerData.Physics.IsOnHeadBump && playerData.Physics.CanBumpHead))
            {
                stateMachine.ChangeState(player.LandState);
            }
            else if (inputManager.Input_Dash && playerData.Dash.DashCooldownTimer <= 0f)
            {
                stateMachine.ChangeState(player.DashState);
            }
            else if (playerData.Physics.IsNextToWall)
            {
                if (inputManager.Input_WallGrab)
                {
                    stateMachine.ChangeState(player.WallGrabState);
                }
                else if (localTime > playerData.Jump.IgnoreWallSlideTime && inputManager.Input_Walk != 0 && Mathf.Sign(inputManager.Input_Walk) == Mathf.Sign(playerData.Physics.WallDirection))
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
            else if(playerData.Physics.CanGlideByHeight && inputManager.Input_Jump && playerData.Glide.GlideBufferTimer > 0f)
            {
                stateMachine.ChangeState(player.GlideState);
            }
        }

        public void Move2D()
        {
            Vector2 _newVelocity = Vector2.zero;
            if (localTime <= jumpInfo.JumpTime && !playerData.Physics.CutJump)
            {
                _newVelocity.y = jumpInfo.JumpVelocityCurve.Evaluate(localTime / jumpInfo.JumpTime);
                _newVelocity.y *= jumpInfo.MaxHeight * 1 / jumpInfo.JumpTime;
            }
            else if (localTime <= cutJumpTime + ((jumpInfo.JumpTime - cutJumpTime) * jumpInfo.TimeScaleOnCut) && playerData.Physics.CutJump)
            {
                _newVelocity.y = jumpInfo.JumpVelocityCurve.Evaluate(localTime / (cutJumpTime + ((jumpInfo.JumpTime - cutJumpTime) * jumpInfo.TimeScaleOnCut)));
                _newVelocity.y *= jumpInfo.MaxHeight * 1 / jumpInfo.JumpTime;
                _newVelocity.y *= 1 - jumpInfo.JumpCutPower;
            }
            else
            {
                stateMachine.ChangeState(player.LandState);
            }
            if (!playerData.Physics.IsOnNotWalkableSlope)
            {
                _newVelocity.x = VelocityOnx();
            }
            else
            {
                _newVelocity.x = VelocityOnx() * playerData.Physics.Slope.CurrentSlopeAngle / 100;
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
                    xCurveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Jump.XSpeedUpCurve, Mathf.Abs(rigidbody2D.velocity.x) / playerData.Jump.MaxXSpeed, 1, true);
                    xCurveTime *= playerData.Jump.XSpeedUpTime;
                    phase = Phase.SpeedUp;
                }
                if (xCurveTime < playerData.Jump.XSpeedUpTime)
                {
                    XVelocity = playerData.Jump.XSpeedUpCurve.Evaluate(xCurveTime / playerData.Jump.XSpeedUpTime);
                }
                else
                {
                    XVelocity = playerData.Jump.XSpeedUpCurve.Evaluate(1f);
                }
                XVelocity *= inputManager.Input_Walk * playerData.Jump.MaxXSpeed;
            }
            else if (inputManager.Input_Walk != 0 && ((localXVelovity != 0 && Mathf.Sign(inputManager.Input_Walk) != Mathf.Sign(localXVelovity)) || phase == Phase.TurnBack))
            {
                if (phase != Phase.TurnBack)
                {
                    xCurveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Jump.XTurnBackCurve, Mathf.Abs(localXVelovity) / playerData.Jump.MaxXSpeed, 2f, false);
                    xCurveTime *= playerData.Jump.XTurnBackTime;
                    phase = Phase.TurnBack;
                    turnBackStartDirection = (int)Mathf.Sign(localXVelovity);
                }
                if (xCurveTime < playerData.Jump.XTurnBackTime)
                {
                    XVelocity = playerData.Jump.XTurnBackCurve.Evaluate(xCurveTime / playerData.Jump.XTurnBackTime);
                }
                else
                {
                    XVelocity = playerData.Jump.XTurnBackCurve.Evaluate(2f);
                }
                XVelocity *= playerData.Jump.MaxXSpeed * turnBackStartDirection;
            }
            else if (localXVelovity != 0)
            {
                if (phase != Phase.SlowDown)
                {
                    xCurveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Jump.XSlowDownCurve, Mathf.Abs(rigidbody2D.velocity.x) / playerData.Jump.MaxXSpeed, 1, false);
                    xCurveTime *= playerData.Jump.XSlowDownTime;
                    phase = Phase.SlowDown;
                }
                if (xCurveTime < playerData.Jump.XSlowDownTime)
                {
                    XVelocity = playerData.Jump.XSlowDownCurve.Evaluate(xCurveTime / playerData.Jump.XSlowDownTime);
                }
                else
                {
                    XVelocity = playerData.Jump.XSlowDownCurve.Evaluate(1f);
                }
                XVelocity *= playerData.Jump.MaxXSpeed * playerData.Physics.FacingDirection;
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
