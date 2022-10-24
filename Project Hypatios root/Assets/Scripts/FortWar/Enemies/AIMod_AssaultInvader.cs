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
    [FoldoutGroup("Weapons")] public LayerMask weapon_WeaponLayer;
    [FoldoutGroup("Weapons")] public float laser_Damage = 20;
    [FoldoutGroup("Weapons")] public GameObject damageSpark;

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

    private float cooldownAttack = 0.15f;

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
            
            cooldownAttack = 0.15f;
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

        if (Physics.Raycast(weapon_OriginFire.transform.position, dir, out hit, 100f, weapon_WeaponLayer))
        {
            var damageReceiver = hit.collider.gameObject.GetComponent<damageReceiver>();
            var health = hit.collider.gameObject.GetComponent<health>();

            //laser_lineRendr.SetPosition(0, laser_PointerOrigin.transform.position);
            //laser_lineRendr.SetPosition(1, hit.point);
            //laser_PointerTarget.transform.position = hit.point;

            if (damageReceiver != null)
                LaserAttack(damageReceiver);

            if (health != null)
            {
                float chance = Random.Range(0f, 1f);

                if (chance < 0.5f) LaserAttack(health);
            }

            SparkFX(hit.point);
            Debug.DrawRay(weapon_OriginFire.transform.position, dir * hit.distance, Color.green);
        }
    }

    private void SparkFX(Vector3 pos)
    {
        var damageSpark1 = Instantiate(damageSpark);
        damageSpark1.transform.position = pos;
        damageSpark1.gameObject.SetActive(true);
    }

    private void LaserAttack(damageReceiver damageReceiver)
    {
        var token = new DamageToken();
        token.damage = laser_Damage;
        token.origin = DamageToken.DamageOrigin.Enemy;
        damageReceiver.Attacked(token);

    }

    private void LaserAttack(health health)
    {
        health.takeDamage(Mathf.RoundToInt(laser_Damage));
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
