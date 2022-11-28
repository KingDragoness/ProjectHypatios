using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using System.Linq;

public class SeaverScarab : EnemyScript
{
    public GameObject explosion;
    public float minimumDistance = 1.5f;
    public float deathTimer = 5f;
    public NavMeshAgent agent;


    [ReadOnly] [SerializeField] bool playerCanBeReached = false;

    private void Start()
    {

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

        if (deathTimer < 0)
        {
            Die();
        }

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
                Debug.Log($"Target: {result}");
            }

            if (NavMesh.FindClosestEdge(currentTarget.transform.position, out hit, NavMesh.AllAreas))
            {
                Debug.Log("Found closest edge at: " + hit.position);
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


        if ((Mathf.RoundToInt(Time.time * 10) % 2) == 0)
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
        var explosion1 = Instantiate(explosion, transform.position, transform.rotation);

        explosion1.gameObject.SetActive(true);
        Destroy(gameObject);
    }

}
