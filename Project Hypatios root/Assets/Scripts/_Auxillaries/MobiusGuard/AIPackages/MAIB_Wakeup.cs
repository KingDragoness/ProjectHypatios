using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAIB_Wakeup : MobiusAIBehaviour
{

    public float TimeToIdle = 4.44f;

    private float _timer = 0f;

    public override int CalculatePriority()
    {
        return basePriority;
    }

    public override bool IsConditionFulfilled()
    {
        return base.IsConditionFulfilled();
    }

    public override void OnBehaviourActive()
    {
        _timer = 0f;
    }

    public override void OnBehaviourDisable()
    {
    }

    public override void Execute()
    {
        _timer += Time.deltaTime;
        mobiusGuardScript.Set_StopMoving();

        if (_timer > TimeToIdle + 0.02f)
        {
            mobiusGuardScript.ChangeAIBehaviour_Type<MAIB_Idle>();
        }
    }
}
