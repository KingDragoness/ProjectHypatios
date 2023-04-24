using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;

public class Vendrich_SummonTestSubjectEnemy : EnemyScript
{

    public Animator anim;

    [FoldoutGroup("Prefabs")] public GameObject corpse;
    public float velocityAnimationMinimum = .8f;

    private NavMeshAgent agent;
    private float cooldownAttack = 5f;
    private float _timerAttack = 5f;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public override void Die()
    {
        Destroy(gameObject);
        var corpse1 = Instantiate(corpse);
        corpse1.gameObject.SetActive(true);
        corpse1.transform.position = transform.position;
        corpse1.transform.rotation = transform.rotation;
        OnDied?.Invoke();
        Stats.IsDead = true;
    }

    private void Update()
    {
        if (isAIEnabled == false) return;


        if (Mathf.RoundToInt(Time.time) % 5 == 0)
            ScanForEnemies();

        if (currentTarget != null)
        {
            Attack();
            Movement();
        }
    }

    private void Attack()
    {
        _timerAttack -= Time.deltaTime;

        if (_timerAttack < 0)
        {
            
            _timerAttack = cooldownAttack;
        }
    }


    private void Movement()
    {
        agent.SetDestination(currentTarget.transform.position);

        if (agent.velocity.magnitude > velocityAnimationMinimum)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);

        }
    }

}
