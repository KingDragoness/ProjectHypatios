using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class AIMod_AssaultInvader : FortWar_AIModule
{

    public enum State
    {
        FIRE,
        CHASE,
        RETREAT,
        REPOSITION
    }

    public State currentState = State.CHASE;
    public float repositionRange = 3f;
    [FoldoutGroup("Weapons")] public GameObject weapon_DebugFlashFire;
    [FoldoutGroup("Weapons")] public GameObject weapon_OriginFire;
    [FoldoutGroup("Weapons")] public float laser_Damage = 20;
    [FoldoutGroup("Weapons")] public float laser_Cooldown = 0.2f;
    #region INPUTS 


    #endregion

    #region OUTPUTS


    #endregion


    /// <summary>
    /// 3 states: FIRE, CHASE, RETREAT
    /// </summary>
    public override void Run()
    {
        if (currentState == State.FIRE)
        {
            State_Fire();
        }
        else if (currentState == State.CHASE)
        {
            State_Chase();
        }
        else if (currentState == State.RETREAT)
        {
            State_Retreat();
        }
        else if (currentState == State.REPOSITION)
        {
            State_Reposition();
        }
    }

    private void ResumeMovement()
    {
        BotScript.Agent.stoppingDistance = 1f;
        BotScript.Agent.updateRotation = true;
    }

    private void LockMovement()
    {
        BotScript.Agent.SetDestination(transform.position);
        BotScript.Agent.updateRotation = false;
    }

    private Vector3 SampleReposition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * repositionRange;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 25f, 1);
        Vector3 finalPosition = hit.position;

        return finalPosition;
    }

    private float cooldownAttack = 0f;

    #region Attack Enemy
    public void State_Fire()
    {
        LockMovement();

        if (BotScript.GetCurrentTarget() == null)
        {
            return;
        }

        Vector3 posTarget = BotScript.GetCurrentTarget().position;

        Vector3 dir = posTarget - transform.position;
        dir.y = 0;
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        BotScript.transform.rotation = rotation;


        if (cooldownAttack > 0)
        {
            cooldownAttack -= Time.deltaTime;
        }
        else
        {
            float chanceHoldFire = Random.Range(0f, 1f);
            float dist = Vector3.Distance(posTarget, transform.position);

            float a = Mathf.Clamp((dist/100f), 0f, 0.5f);
            chanceHoldFire -= a;
            chanceHoldFire = Mathf.Clamp(chanceHoldFire, 0, 1f);

            if (chanceHoldFire < 0.4)
                FireWeapon();
            
            cooldownAttack = laser_Cooldown;
        }
    }

    private void FireWeapon()
    {
        weapon_DebugFlashFire.gameObject.SetActive(true);
        //raycast enemy

        Vector3 targetPos = BotScript.GetCurrentTarget().position; targetPos.y += 0.5f;
        { //inaccuracy
            targetPos.x += Random.Range(-1, 1f);
            targetPos.z += Random.Range(-1, 1f);
        }

        Vector3 dir = targetPos - weapon_OriginFire.transform.position;


        RaycastHit hit;

        if (Physics.Raycast(weapon_OriginFire.transform.position, dir, out hit, 100f, Hypatios.Enemy.baseSolidLayer))
        {
            var token = new DamageToken();
            token.damage = laser_Damage;
            if (BotScript.Stats.MainAlliance != Alliance.Player) token.origin = DamageToken.DamageOrigin.Enemy; else token.origin = DamageToken.DamageOrigin.Ally;
            token.originEnemy = BotScript;
            UniversalDamage.TryDamage(token, hit.transform, BotScript.transform);

            SparkFX(hit.point, hit.normal);
            Debug.DrawRay(weapon_OriginFire.transform.position, dir * hit.distance, Color.green);
        }
    }

    private void SparkFX(Vector3 pos, Vector3 normal)
    {
        GameObject bulletSpark_ = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.BulletSparksEnemy, true);
        if (bulletSpark_ != null)
        {
            bulletSpark_.transform.position = pos;
            bulletSpark_.transform.rotation = Quaternion.LookRotation(normal);
            bulletSpark_.DisableObjectTimer(1f);
        }
    }


    #endregion
    private FW_ControlPoint GetFirstUncapturedCP()
    {
        return Chamber_Level7.instance.GetCurrentCP();
    }

    public void State_Chase()
    {
        ResumeMovement();

        if (BotScript.GetCurrentTarget() == null)
        {
            var targetPosition = GetFirstUncapturedCP().transform.position;
            BotScript.Agent.SetDestination(targetPosition);

            return;
        }

        BotScript.Agent.SetDestination(BotScript.GetCurrentTarget().position);

    }

    public void State_Retreat()
    {
        ResumeMovement();


    }

    public void State_Reposition()
    {
        ResumeMovement();

        BotScript.Agent.SetDestination(SampleReposition());

    }

    public override void OnChangedState(FortWar_AIModule currentModule)
    {
        if (currentModule == this)
        {

        }
    }
}
