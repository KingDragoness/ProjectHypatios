using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAIB_FollowTarget : MobiusAIBehaviour
{

    public float move_Speed = 1f;
    public float animFloatParam_Speed = 0.4f;

    public override int CalculatePriority()
    {
        if (mobiusGuardScript.hasSeenPlayer == false)
        {
            return -100;
        }

        if (!mobiusGuardScript.canLookAtTarget)
        {
            return (int)mobiusGuardScript.survivalEngageLevel + basePriority;
        }


        return base.CalculatePriority();
    }

    public override bool IsConditionFulfilled()
    {
        return base.IsConditionFulfilled();
    }

    public override void OnBehaviourActive()
    {
        mobiusGuardScript.Set_EnableAiming();
        mobiusGuardScript.ShowRifleModel();
        OnEnableBehaviour?.Invoke();
    }

    public override void OnBehaviourDisable()
    {
        mobiusGuardScript.animator.SetFloat("Speed", 0f);
        OnDisableBehaviour?.Invoke();
        base.OnBehaviourDisable();
    }

    public override void Execute()
    {
        if (mobiusGuardScript.currentTarget == null) return;

        
        mobiusGuardScript.Set_StartMoving(move_Speed, animFloatParam_Speed);
        mobiusGuardScript.agent.SetDestination(mobiusGuardScript.currentTarget.transform.position);
        mobiusGuardScript.OverrideAimingTarget();


    }
}
