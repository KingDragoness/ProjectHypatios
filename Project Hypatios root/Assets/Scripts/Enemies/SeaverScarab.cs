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

    private Transform player;

    private void Start()
    {
        if (player == null) player = FindObjectOfType<CharacterScript>().transform;
    }

    [ReadOnly] [SerializeField] bool playerCanBeReached = false;

    private void Update()
    {
        AttackMode();
    }

    public override void Attacked(DamageToken token)
    {
        Stats.CurrentHitpoint -= token.damage;

        base.Attacked(token);

        if (Stats.CurrentHitpoint <= 0f)
        {
            Explode();
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
            Explode();
        }

        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.transform.position);

        if (playerCanBeReached)
            agent.SetDestination(player.transform.position);
        else
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(player.transform.position, out hit, 5.0f, NavMesh.AllAreas))
            {
                Vector3 result = hit.position;
                agent.SetDestination(hit.position);
                Debug.Log($"Target: {result}");
            }

            if (NavMesh.FindClosestEdge(player.transform.position, out hit, NavMesh.AllAreas))
            {
                Debug.Log("Found closest edge at: " + hit.position);
                agent.SetDestination(hit.position);
            }

            if ((Mathf.RoundToInt(Time.time * 10) % 10) == 0)
            {
                float chanceKillSelf = Random.Range(0f, 1f);

                if (chanceKillSelf < 0.01f)
                {
                    Explode();

                }

            }
        }


        if ((Mathf.RoundToInt(Time.time * 10) % 2) == 0)
        {
            CheckCalculate();
        }

        if (dist < minimumDistance)
        {
            Explode();
        }
    }

    private void CheckCalculate()
    {
        NavMeshPath navMeshPath = new NavMeshPath();

        if (agent.CalculatePath(player.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            playerCanBeReached = true;

        }
        else
        {
            playerCanBeReached = false;
        }

    }

    private void Explode()
    {
        var explosion1 = Instantiate(explosion, transform.position, transform.rotation);

        explosion1.gameObject.SetActive(true);
        Destroy(gameObject);
    }

}
