using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//modular, can be fire turret or missiles, etc
public class MAIB_FireWeapon : MobiusAIBehaviour
{

    public UnityEvent OnEnableBehaviour;
    public UnityEvent OnDisableBehaviour;


    public override int CalculatePriority()
    {
        if (mobiusGuardScript.canLookAtTarget)
        {
            return 10 + (int)mobiusGuardScript.survivalEngageLevel;
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
        OnEnableBehaviour?.Invoke();
    }

    public override void OnBehaviourDisable()
    {
        base.OnBehaviourDisable();
        OnDisableBehaviour?.Invoke();
    }

    public override void Execute()
    {
        if (mobiusGuardScript.IsAiming == false)
            mobiusGuardScript.IsAiming = true;

        mobiusGuardScript.Set_StopMoving();
        mobiusGuardScript.OverrideAimingTarget();
    }
}
