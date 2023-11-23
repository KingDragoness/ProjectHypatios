using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAIB_Escape : MobiusAIBehaviour
{

    public float minimumRange = 3f;
    public float nearPlayerRadius = 12f;
    public int minimumRange_priority = -50;
    [Space]
    public float move_Speed = 2f;
    public float animFloatParam_Speed = 0.9f;
    public float multiplier_FloatParam_Speed = 0.2f;
    public float distanceRandomSphere = 11f;
    public float cooldownFindNewEscapePoint = 5;
    public float distThresholdLimit = 5f;
    public List<WeaponItem> dangerousWeapons = new List<WeaponItem>();
    public RandomSpawnArea escapeAreaFallback;

    private float cooldown = 5f;
    private Vector3 escapePos = Vector3.zero;

    public override int CalculatePriority()
    {
        float dist = 999f;

        if (mobiusGuardScript.currentTarget != null)
        {
            dist = Vector3.Distance(mobiusGuardScript.transform.position, mobiusGuardScript.currentTarget.transform.position);
        }

        int priority = -(int)mobiusGuardScript.survivalEngageLevel + basePriority;

        if (IsWeaponDangerous())
        {
            priority += 20;
        }
        if (minimumRange > dist)
        {
            return minimumRange_priority;
        }

        return priority;


    }

    public bool IsWeaponDangerous()
    {
        if (Hypatios.Player.Weapon.currentGunHeld != null)
        {
            if (dangerousWeapons.Find(x => x.nameWeapon == Hypatios.Player.Weapon.currentGunHeld.weaponName) != null)
            {
                return true;
            }
        }

        return false;
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
        base.OnBehaviourDisable();
        OnDisableBehaviour?.Invoke();

    }

    public override void Execute()
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
            var activeGuards = MobiusGuardEnemy.AllActiveGuards;
            if (activeGuards.Count > 0)
            {
                var randomGuard = activeGuards[Random.Range(0, activeGuards.Count)];
                escapePos = IsopatiosUtility.RandomNavSphere(randomGuard.transform.position, 5f, -1);
                cooldown += 2f;
            }
            else
            {
                escapePos = IsopatiosUtility.RandomNavSphere(mobiusGuardScript.transform.position, distanceRandomSphere, -1);
            }
            cooldown = cooldownFindNewEscapePoint;
        }

        //if escape pos too near with the player, rapidly search again
        float distPlayer_esc = 999f;

        if (mobiusGuardScript.currentTarget != null)
        {
            distPlayer_esc = Vector3.Distance(escapePos, mobiusGuardScript.currentTarget.transform.position);
        }

        int tries = 0;

        while(tries < 5 && distPlayer_esc < nearPlayerRadius)
        {
            escapePos = IsopatiosUtility.RandomNavSphere(mobiusGuardScript.transform.position, distanceRandomSphere, -1);
            distPlayer_esc = Vector3.Distance(escapePos, mobiusGuardScript.currentTarget.transform.position);
            tries++;
        }

        float paramSpeed = mobiusGuardScript.agent.velocity.magnitude * multiplier_FloatParam_Speed;
        if (paramSpeed > animFloatParam_Speed) paramSpeed = animFloatParam_Speed;
        mobiusGuardScript.Set_StartMoving(move_Speed, paramSpeed);
        mobiusGuardScript.agent.SetDestination(escapePos);

    }
}
