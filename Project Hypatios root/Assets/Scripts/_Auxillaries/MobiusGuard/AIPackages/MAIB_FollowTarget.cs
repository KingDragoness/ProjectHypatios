using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAIB_FollowTarget : MobiusAIBehaviour
{

    public float move_Speed = 1f;
    public float animFloatParam_Speed = 0.4f;

    public override int CalculatePriority()
    {
        return base.CalculatePriority();
    }

    public override bool IsConditionFulfilled()
    {
        return base.IsConditionFulfilled();
    }

    public override void OnBehaviourActive()
    {
        mobiusGuardScript.Set_EnableAiming();

    }

    public override void OnBehaviourDisable()
    {
        mobiusGuardScript.animator.SetFloat("Speed", 0f);
        base.OnBehaviourDisable();
    }

    public override void Execute()
    {
        if (mobiusGuardScript.target == null) return;

        
        mobiusGuardScript.Set_StartMoving(move_Speed, animFloatParam_Speed);
        mobiusGuardScript.agent.SetDestination(mobiusGuardScript.target.position);
        mobiusGuardScript.OverrideAimingTarget();


    }
}
