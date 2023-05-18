using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateCC{
    public class PlayerHangState : MainState
    {
        public PlayerHangState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
        {
        }

    public override void Enter()
        {
            base.Enter();
            rigidbody2D.velocity = Vector2.zero;
            player.transform.position = playerData.Physics.LedgeHangPosition;
            playerData.Physics.LedgeHangPosition = Vector2.zero;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

        }

        public override void Exit()
        {
            base.Exit();
            playerData.Physics.LedgeHangPosition = Vector2.zero;
        }

        public override void PhysicsCheck()
        {
        }

        public override void SwitchStateLogic()
        {
            base.SwitchStateLogic();
            if(inputManager.Input_Jump)
            {
                stateMachine.ChangeState(player.JumpState);
            }
        }

    }
}
