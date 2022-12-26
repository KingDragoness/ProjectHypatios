using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using System.Linq;

public class SeaverScarab : EnemyScript
{
    public float minimumDistance = 1.5f;
    public float deathTimer = 5f;
    public NavMeshAgent agent;


    [ReadOnly] [SerializeField] bool playerCanBeReached = false;

    private void Start()
    {
        agent.speed /= Hypatios.ExtraAttackSpeedModifier();
    }

    private void Update()
    {
        AttackMode();
    }

    public void OverrideTarget(Entity t, Alliance currentAlliance)
    {
        currentTarget = t;
        Stats.MainAlliance = currentAlliance;
    }


    public override void Attacked(DamageToken token)
    {
        Stats.CurrentHitpoint -= token.damage;
        if (token.originEnemy == this) return;
        base.Attacked(token);

        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }
        else if (token.origin == DamageToken.DamageOrigin.Player)
        {
            DamageOutputterUI.instance.DisplayText(token.damage);
        }
    }

    private void AttackMode()
    {
        deathTimer -= Time.deltaTime;

        if (deathTimer < 0 && Stats.IsDead == false)
        {
            Die();
        }
        if (isAIEnabled == false) return;
        if (Stats.IsDead == true) return;
        if (currentTarget == null) return;

        float dist = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (playerCanBeReached)
            agent.SetDestination(currentTarget.transform.position);
        else
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(currentTarget.transform.position, out hit, 5.0f, NavMesh.AllAreas))
            {
                Vector3 result = hit.position;
                agent.SetDestination(hit.position);
            }

            if (NavMesh.FindClosestEdge(currentTarget.transform.position, out hit, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            if ((Mathf.RoundToInt(Time.time * 10) % 10) == 0)
            {
                float chanceKillSelf = Random.Range(0f, 1f);

                if (chanceKillSelf < 0.01f)
                {
                    Die();

                }

            }
        }


        if ((Mathf.RoundToInt(Time.time * 5) % 2) == 0)
        {
            CheckCalculate();
        }

        if (dist < minimumDistance)
        {
            Die();
        }
    }

    private void CheckCalculate()
    {
        NavMeshPath navMeshPath = new NavMeshPath();

        if (agent.CalculatePath(currentTarget.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            playerCanBeReached = true;

        }
        else
        {
            playerCanBeReached = false;
        }

    }

    public override void Die()
    {
        if (Stats.IsDead) return;
        Stats.IsDead = true; //stack overflow crash!

        var explosion1 = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.ExplosionSeaver, true, transform.position, transform.rotation);
        explosion1.transform.position = transform.position;
        explosion1.transform.rotation = transform.rotation; //Instantiate(explosion, transform.position, transform.rotation);
        var killzoneComp = explosion1.GetComponentInChildren<KillZone>();
        killzoneComp.originEnemy = this;
        killzoneComp.DamageEnemy(); //There's no other fucking option
        explosion1.gameObject.SetActive(true);
        Destroy(gameObject);
    }

}
