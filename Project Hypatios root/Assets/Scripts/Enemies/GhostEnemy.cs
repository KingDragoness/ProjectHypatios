using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class GhostEnemy : EnemyScript
{
    public Transform player;
    public float playerVelocityThreshold = 5;
    NavMeshAgent enemyAI;

    private Vector3 lastPos;
    private Vector3 velocity;
    private float cooldownPlayerMove = 3f;

    private void Start()
    {
        enemyAI = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (lastPos != player.position)
        {
            velocity = player.position - lastPos;
            velocity /= Time.deltaTime;
            lastPos = player.position;
        }

        if (velocity.magnitude < playerVelocityThreshold)
        {
            cooldownPlayerMove -= Time.deltaTime;
        }
        else
        {
            cooldownPlayerMove = 3f;
        }

        if (cooldownPlayerMove <= 0f)
        {
            enemyAI.SetDestination(player.position);
        }

    }
}
