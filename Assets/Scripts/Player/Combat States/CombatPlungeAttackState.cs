using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPlungeAttackState : CombatBaseState
{
    SpringController spring = GameObject.Find("Spring")?.GetComponent<SpringController>();
    float endtime=5;
    bool exit=false;
    bool isPlungeAttack = false;
    public CombatPlungeAttackState(PlayerController currentContext, CombatStateFactory combatStateFactory, PlayerStateFactory movementStateFactory, float damage) : 
    base(currentContext, combatStateFactory, movementStateFactory, damage)
    {
    }

    public override void EnterState()
    {
        Ctx.IsAttackPressed = false;
        Ctx.CanHeavyAttack = false;
        Ctx.Rigidbod.velocity= Vector3.zero;
        Ctx.PlayerAnimator.Play("PlayerPlungeAttack");
        isPlungeAttack = true;
    }
    public override void UpdateState()
    {
        //first and second animation must have same animation time to run smooth
        if (Ctx.IsOnGround && !exit)
        {
            Ctx.PlayerAnimator.Play("PlayerPlungeAttackExit");
            endtime= Time.time+Ctx.PlayerAnimator.GetCurrentAnimatorStateInfo(0).length;
            exit= true; // exit bool uses for run code only once
            isPlungeAttack= false;
        }
        CheckSwitchStates();
        

    }
    public override void ExitState()
    {
        
    }
    public override void CheckSwitchStates()
    {
        if (Time.time >= endtime)
        {
            float bounce = 16f;
            if (isPlungeAttack)
                bounce *= 2f;
            
            if (spring != null)
            {
                spring.bounce = bounce;
            }
            SwitchState(CombatFactory.Peaceful());
        }
    }



}
