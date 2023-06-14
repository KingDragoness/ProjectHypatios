using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MAIB_Hurt : MobiusAIBehaviour
{

    public float TimeToRecover = 0.6f;
    public string triggerName = "Hit";

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
        mobiusGuardScript.animator.SetTrigger(triggerName);
    }

    public override void OnBehaviourDisable()
    {
    }

    public override void Execute()
    {
        _timer += Time.deltaTime;
        mobiusGuardScript.Set_StopMoving();

        if (_timer > TimeToRecover + 0.02f)
        {
            mobiusGuardScript.ChangeAIBehaviour_Type<MAIB_Idle>();
        }
    }
}

