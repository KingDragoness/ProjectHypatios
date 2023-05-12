using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MAIB_Ragdoll : MobiusAIBehaviour
{

    public float velocityThreshold = 1f;
    public float CooldownCheck = 0.1f;
    public float ExtraCooldownOnEnabled = 2f;
    [ReadOnly] public float rbVelocity;
    private float _timerCheckWakeup = 0.1f;

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
        _timerCheckWakeup += ExtraCooldownOnEnabled;
    }

    public override void OnBehaviourDisable()
    {
        _timerCheckWakeup = 0f;
    }

    public override void Execute()
    {
        _timerCheckWakeup -= Time.deltaTime;
        if (_timerCheckWakeup < 0f)
        {
            bool allowWakeup = false;
            rbVelocity = mobiusGuardScript.mainRagdollRigidbody.velocity.magnitude;

            if (IsopatiosUtility.CheckNavMeshWalkable(mobiusGuardScript.transform.position, 2f, out Vector3 result) &&
                rbVelocity < velocityThreshold)
            {
                allowWakeup = true;
            }

            if (allowWakeup)
            {
                mobiusGuardScript.Wakeup();

            }

            _timerCheckWakeup = CooldownCheck;
        }

        mobiusGuardScript.Set_StopMoving();
    }
}
