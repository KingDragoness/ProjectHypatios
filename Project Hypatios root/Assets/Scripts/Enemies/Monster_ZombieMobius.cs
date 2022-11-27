﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Monster_ZombieMobius : EnemyScript
{

    public List<AnimatorOverrideController> variantControllers;
    public UnityEvent OnAttack;
    public UnityEvent OnAttackDone;
    public UnityEvent OnDead;
    public Animator animator;
    public float speed = 10;
    public float distanceToAttack = 5f;
    public GameObject hitbox;
    public GameObject visualAttack;
    public Transform target;
    public Transform playerTarget;
    [FoldoutGroup("Animations")] public float cooldownAttack = 1.95f;
    [FoldoutGroup("Animations")] public float hitboxStart = .2f;
    [FoldoutGroup("Animations")] public float hitboxLasts = .3f;

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
        if (token.originEnemy == this) return;
        if (Stats.IsDamagableBySameType == false && token.originEnemy is Monster_ZombieMobius) return;

        Stats.CurrentHitpoint -= token.damage;
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

    private float distanceToPlayer = 0;

    private void Update()
    {
        if (Stats.CurrentHitpoint < 0)
        {
            if (!isDead) SpawnHeal.SpawnHealCapsule(2);
            isDead = true;
        }

        if (isDead)
        {
            animator.SetBool("Dead", true);
            Destroy(gameObject, 5f);
            OnDead?.Invoke();
        }

        else
        {
            if (isAttacking == false)
            {
                NavMeshAgent.updateRotation = true;
                NavMeshAgent.SetDestination(target.position);
            }
            else
            {

                durationAttack -= Time.deltaTime;
                HandleRotation();

                if (durationAttack < 0)
                {
                    animator.ResetTrigger("Attack");
                    isAttacking = false;
                    OnAttackDone?.Invoke();
                    visualAttack.gameObject.SetActive(false);
                }
            }

            distanceToPlayer = Vector3.Distance(transform.position, target.position);

            if (distanceToPlayer < distanceToAttack && !isAttacking)
            {
                AttackPlayer();
            }
        }
    }

    private void HandleRotation()
    {
        NavMeshAgent.updateRotation = false;

        Vector3 posTarget = target.transform.position;

        Vector3 dir = posTarget - transform.position;
        dir.y = 0;
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = rotation;
    }

    private void AIDecision()
    {

    }

    private IEnumerator couroutineAttack;

    [Button("Attack")]
    public void AttackPlayer()
    {
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");
        OnAttack?.Invoke();
        if (couroutineAttack != null) StopCoroutine(couroutineAttack);
        couroutineAttack = SampahTimerAttack();
        StartCoroutine(couroutineAttack);

        durationAttack = cooldownAttack;
        isAttacking = true;
    }

    public IEnumerator SampahTimerAttack()
    {
        yield return new WaitForSeconds(hitboxStart);
        visualAttack.gameObject.SetActive(true);
        hitbox.gameObject.SetActive(true);
        yield return new WaitForSeconds(hitboxLasts);
        hitbox.gameObject.SetActive(false);
        yield return new WaitForSeconds(cooldownAttack - hitboxStart - hitboxLasts);

    }
}
