using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class Monster_ZombieMobius : Enemy
{

    public List<AnimatorOverrideController> variantControllers;
    public float hitpoint = 245;
    public Animator animator;
    public float speed = 10;
    public float distanceToAttack = 5f;
    public GameObject hitbox;
    public Transform target;
    public Transform playerTarget;

    private bool isAttacking = false;
    private bool isDead = false;

    private float durationAttack = 4f;
    private NavMeshAgent NavMeshAgent;
    private SpawnHeal SpawnHeal;

    private void Start()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        SpawnHeal = GetComponent<SpawnHeal>();
        NavMeshAgent.speed = speed + Random.Range(-1, 2f);
        hitbox.gameObject.SetActive(false);

        float random = Random.Range(0f, 1f);

        if (random < 0.5f)
        {
            animator.runtimeAnimatorController = variantControllers[0];
        }
        else
        {
            animator.runtimeAnimatorController = variantControllers[1];

        }
    }

    public override void Attacked(DamageToken token)
    {
        hitpoint -= token.damage;
        base.Attacked(token);
        DamageOutputterUI.instance.DisplayText(token.damage);

    }
    public void SetTarget(Transform target1 = null)
    {
        if (target1 == null)
        {
            target = playerTarget;
        }
        else
        {
            target = target1;
        }
    }

    private void Update()
    {
        if (hitpoint < 0)
        {
            if (!isDead) SpawnHeal.SpawnHealCapsule(2);
            isDead = true;
        }

        if (isDead)
        {
            animator.SetBool("Dead", true);
            Destroy(gameObject, 5f);
        }

        else
        {
            if (!isAttacking)
            {
                NavMeshAgent.SetDestination(target.position);
            }
            else
            {
                durationAttack -= Time.deltaTime;

                if (durationAttack < 0)
                {
                    animator.ResetTrigger("Attack");
                    isAttacking = false;
                }
            }

            if (Vector3.Distance(transform.position, target.position) < distanceToAttack)
            {
                Attack1();
                //AIDecision();
            }
        }
    }

    private void AIDecision()
    {

    }

    [Button("Attack")]
    public void Attack1()
    {
        animator.SetTrigger("Attack");
        if (gameObject.activeSelf)
            StartCoroutine(SampahTimerAttack());
        durationAttack = 2f;
        isAttacking = true;
    }

    public IEnumerator SampahTimerAttack()
    {
        yield return new WaitForSeconds(0.5f);
        hitbox.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        hitbox.gameObject.SetActive(false);
    }
}
