using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//modular, can be fire turret or missiles, etc
public class MAIB_FireWeapon : MobiusAIBehaviour
{


    public float multiplierPriority = 0.4f;

    public override int CalculatePriority()
    {
        if (mobiusGuardScript.canLookAtTarget)
        {
            return 10 + (int)(mobiusGuardScript.survivalEngageLevel * multiplierPriority) + basePriority;
        }
        else
        {
            return -10;
        }

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
