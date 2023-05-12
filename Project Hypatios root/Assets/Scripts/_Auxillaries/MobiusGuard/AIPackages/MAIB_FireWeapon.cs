using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAIB_FireWeapon : MobiusAIBehaviour
{
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
        base.OnBehaviourDisable();
    }

    public override void Execute()
    {
        if (mobiusGuardScript.IsAiming == false)
            mobiusGuardScript.IsAiming = true;

        mobiusGuardScript.Set_StopMoving();
        mobiusGuardScript.OverrideAimingTarget();
    }
}
