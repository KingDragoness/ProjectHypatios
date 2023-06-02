using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MAIB_Patrol : MobiusAIBehaviour
{


    public float move_Speed = 1f;
    public float rotation_Speed = 15f;
    public float animFloatParam_Speed = 0.4f;
    public float multiplier_SurvivalIndex = 0.5f;
    [FoldoutGroup("Random Patrols")] public float distanceRandomSphere = 11f;
    [FoldoutGroup("Random Patrols")] public float cooldownFindNewEscapePoint = 5;
    [FoldoutGroup("Random Patrols")] public float distThresholdLimit = 5f;

    [ReadOnly] public bool stayPostAtAllCost = false;
    [ReadOnly] public int priorityPost_Normal = 10;
    [ReadOnly] public int priorityPost_atAllCost = 100;
    [ReadOnly] public MobiusPostGuard currentPost;
    private float dist;
    private float cooldown = 5f;
    private Vector3 escapePos = Vector3.zero;

    public override int CalculatePriority()
    {
        int priority = 0;
        if (currentPost != null)
        {
            if (stayPostAtAllCost)
            {
                priority =  priorityPost_atAllCost + Mathf.RoundToInt(mobiusGuardScript.survivalEngageLevel * multiplier_SurvivalIndex) + basePriority;
            }
            else
            {
                priority = priorityPost_Normal + Mathf.RoundToInt(mobiusGuardScript.survivalEngageLevel * multiplier_SurvivalIndex) + basePriority;
            }

            //if player is behind the turret, then abandon quickly.
            if (currentPost.isTurretPost)
            {
                float dist = Vector3.Distance(mobiusGuardScript.currentTarget.transform.position, currentPost.transform.position);
                Vector3 relativePos = currentPost.transform.InverseTransformPoint(mobiusGuardScript.currentTarget.transform.position);

                if (relativePos.z < 0 && dist < currentPost.dist_PlayerToAbandon)
                {
                    priority += currentPost.post_threatenMultiplier;
                }

                if (currentPost.IsPlayerInTurretArea(mobiusGuardScript.currentTarget.transform.position))
                {
                    priority += -100;
                }
            }

            return priority;
        }
        else
        {
            return basePriority;
        }
        return base.CalculatePriority();
    }

    public override bool IsConditionFulfilled()
    {
        return base.IsConditionFulfilled();
    }

    public override void OnBehaviourActive()
    {
        mobiusGuardScript.Set_DisableAiming();
        mobiusGuardScript.HideRifleModel();
        OnEnableBehaviour?.Invoke();

    }

    public override void OnBehaviourDisable()
    {
        mobiusGuardScript.animator.SetFloat("Speed", 0f);
        mobiusGuardScript.AnimatorPlayer.DisableAnimancer();
        if (currentPost != null)
        {
            currentPost.OnUnguarded();
        }
        OnDisableBehaviour?.Invoke();
        base.OnBehaviourDisable();
    }

    public override void Execute()
    {
        if (mobiusGuardScript.currentTarget == null) return;

        if (currentPost != null)
        {
            mobiusGuardScript.Set_DisableAiming();
            mobiusGuardScript.HideRifleModel();
            dist = Vector3.Distance(mobiusGuardScript.OffsetedBoundWorldPosition, currentPost.transform.position);

            if (dist < currentPost.dist_StartAnimation)
            {
                currentPost.OnGuard();
                mobiusGuardScript.agent.updateRotation = false;
                mobiusGuardScript.AnimatorPlayer.PlayAnimation(currentPost.idleAnimation, 0.5f);
                mobiusGuardScript.transform.rotation = Quaternion.Lerp(mobiusGuardScript.transform.rotation, currentPost.transform.rotation, Time.deltaTime * rotation_Speed);
            }
            else
            {
                if (currentPost.IsGuarded == false) currentPost.OnUnguarded();
            }


            mobiusGuardScript.agent.updateRotation = true;
            mobiusGuardScript.Set_StartMoving(move_Speed, animFloatParam_Speed);
            mobiusGuardScript.agent.SetDestination(currentPost.transform.position);
        }
        else
        {
            mobiusGuardScript.Set_EnableAiming();
            mobiusGuardScript.ShowRifleModel();

            RandomPatrol();
        }

    }

    private void RandomPatrol()
    {
        cooldown -= Time.deltaTime;
        float dist = Vector3.Distance(transform.position, escapePos);

        if (!mobiusGuardScript.IsMoving && cooldown > 0.1f)
        {
            cooldown = 0.1f;
        }

        if (dist < distThresholdLimit && cooldown > 0.1f)
        {
            cooldown = 0.1f;
        }

        if (cooldown < 0f)
        {
            escapePos = IsopatiosUtility.RandomNavSphere(mobiusGuardScript.transform.position, distanceRandomSphere, -1);
            cooldown = cooldownFindNewEscapePoint;
        }

        float paramSpeed = mobiusGuardScript.agent.velocity.magnitude * animFloatParam_Speed;
        if (paramSpeed > animFloatParam_Speed) paramSpeed = animFloatParam_Speed;
        mobiusGuardScript.Set_StartMoving(move_Speed, paramSpeed);
        mobiusGuardScript.agent.SetDestination(escapePos);
    }
}
