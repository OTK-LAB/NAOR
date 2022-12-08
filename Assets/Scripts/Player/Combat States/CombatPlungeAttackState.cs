using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPlungeAttackState : CombatBaseState
{
    float endtime;

    public CombatPlungeAttackState(PlayerController currentContext, CombatStateFactory combatStateFactory, PlayerStateFactory movementStateFactory, float damage) : 
    base(currentContext, combatStateFactory, movementStateFactory, damage)
    {
    }

    public override void EnterState()
    {
        Ctx.CanHeavyAttack = false;
        Debug.Log("Enter Plunge");
        Ctx.PlayerAnimator.Play("PlayerPlungeAttack");
        endtime = Time.time + Ctx.PlayerAnimator.GetCurrentAnimatorStateInfo(0).length;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
    }
    public override void CheckSwitchStates()
    {
        if (Time.time >= endtime)
        {
            SwitchState(CombatFactory.Peaceful());
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
