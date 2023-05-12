using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAIB_Idle : MobiusAIBehaviour
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
        mobiusGuardScript.Set_DisableAiming();

    }

    public override void OnBehaviourDisable()
    {
        base.OnBehaviourDisable();
    }

    public override void Execute()
    {
        mobiusGuardScript.Set_StopMoving();


    }
}
