using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class MobiusGuard : Enemy
{

    public enum AIState
    {
        Standby,
        Patrol,
        StandAttack,
        ChasePlayer,
        UnderAttacked
    }

    public float hitpoint = 177;
    public UnityEvent OnDiedEvent;

    [FoldoutGroup("Parameter AI")] public AIState aiState;
    [FoldoutGroup("Parameter AI")] public float speed_Strafing = 2f;
    [FoldoutGroup("Parameter AI")] public float speed_StandAttack = 0.1f;
    [FoldoutGroup("Parameter AI")] public float randomStrafingDist = 3f;
    [FoldoutGroup("Parameter AI")] public float stunDamageTime = 1.1f;

    [FoldoutGroup("References")] public Animator animator;
    [FoldoutGroup("References")] public SpawnHeal spawnHeal;
    [FoldoutGroup("References")] public Transform targetPlayer;
    [FoldoutGroup("References")] public Transform outEye;
    [FoldoutGroup("References")] public Transform outWeaponTransform;
    [FoldoutGroup("References")] public MobiusLookWeaponLimit mobiusLookWeaponLimit;
    [FoldoutGroup("Weapon")] public GameObject sparkBullet;
    [FoldoutGroup("Weapon")] public GameObject flashWeapon;
    [FoldoutGroup("Weapon")] public LayerMask layermaskWeapon;
    [FoldoutGroup("Weapon")] public AudioSource audio_FireGun;
    [FoldoutGroup("Weapon")] public AudioSource audio_Flyby;
    [FoldoutGroup("Weapon")] public List<AudioClip> audioClipsAudioFlyby;
    [FoldoutGroup("Weapon")] public float cooldown_WeaponFire = 0.4f;
    [FoldoutGroup("Weapon")] public int damage_WeaponFire = 5;

    private Vector3 strafingTargetPos;
    private float timer_Stunned = 1.1f;
    private NavMeshAgent navMeshAgent;
    private bool ableSeePlayer = false;
    private bool hasDied = false;

    private void Start()
    {
        if (targetPlayer == null) targetPlayer = FindObjectOfType<characterScript>().transform;
        mobiusLookWeaponLimit.player = targetPlayer;
        strafingTargetPos = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {

        if (hitpoint < 0)
        {
            if (!hasDied)
            {
                OnDiedEvent?.Invoke();
                Destroy(gameObject, 5f);
                spawnHeal.SpawnHealCapsule(1);
            }
            hasDied = true;
            hitpoint = 0;
        }

        if (hasDied)
        {
            animator.SetBool("Dead", true);
            animator.ResetTrigger("Damaged");
            return;
        }

        UpdateEnemyState();
        AnimationStateUpdate();
        Attack();
        CheckICanSeePlayer();
    }

    public override void Attacked(DamageToken token)
    {
        timer_Stunned = stunDamageTime;
        aiState = AIState.UnderAttacked;
        animator.SetTrigger("Damaged");

        hitpoint -= token.damage;
        base.Attacked(token);
        DamageOutputterUI.instance.DisplayText(token.damage);
    }

    #region State

    private void UpdateEnemyState()
    {
        if (aiState == AIState.Standby)
        {
            State_Idle();
        }
        else if (aiState == AIState.Patrol)
        {
            State_Patrol();
        }
        else if (aiState == AIState.StandAttack)
        {
            State_StandAttack();
        }
        else if (aiState == AIState.ChasePlayer)
        {
            State_Chase();
        }
        else if (aiState == AIState.UnderAttacked)
        {
            State_Damaged();
        }
    }

    private void State_Damaged()
    {
        timer_Stunned -= Time.deltaTime;

        if (timer_Stunned < 0)
        {
            aiState = AIState.StandAttack;
            animator.ResetTrigger("Damaged");
            timer_Stunned = stunDamageTime;
        }
    }

    private void State_Idle()
    {

    }

    private float cooldown_FindingStrafPos = 3f;

    private void State_Patrol()
    {
        navMeshAgent.speed = speed_Strafing;
        cooldown_FindingStrafPos += Time.deltaTime;

        if (cooldown_FindingStrafPos > 3f)
        {
            Vector3 posRandom = new Vector3();
            posRandom.x += Random.Range(-randomStrafingDist, randomStrafingDist);
            posRandom.z += Random.Range(-randomStrafingDist, randomStrafingDist);

            strafingTargetPos = transform.position + posRandom;
            cooldown_FindingStrafPos = 0;
        }

        navMeshAgent.SetDestination(strafingTargetPos);

        if (ableSeePlayer)
        {
            aiState = AIState.ChasePlayer;
        }
    }

    private float cooldown_CheckMove = 1f;


    private void State_StandAttack()
    {
        navMeshAgent.speed = speed_StandAttack;
        navMeshAgent.SetDestination(targetPlayer.position);

        cooldown_CheckMove += Time.deltaTime;

        if (cooldown_CheckMove > 0.2f)
        {
            Vector3 cameraRelative = transform.InverseTransformPoint(targetPlayer.position);

            if (cameraRelative.z > 1.5f)
            {

            }
            else
            {
                aiState = AIState.ChasePlayer;
            }

            if (!ableSeePlayer)
            {
                aiState = AIState.ChasePlayer;

            }

            cooldown_CheckMove = 0;
        }


    }

    private void State_Chase()
    {
        navMeshAgent.speed = speed_Strafing;
        navMeshAgent.SetDestination(targetPlayer.position);


        cooldown_CheckMove += Time.deltaTime;

        if (cooldown_CheckMove > 1f)
        {
            Vector3 cameraRelative = transform.InverseTransformPoint(targetPlayer.position);

            if (cameraRelative.z > 0 && ableSeePlayer)
            {
                aiState = AIState.StandAttack;

            }
            else
            {
            }

            cooldown_CheckMove = 0;
        }

    }

    #endregion

    private void AnimationStateUpdate()
    {
        if (navMeshAgent.velocity.magnitude > 1f)
        {
            animator.SetFloat("Movespeed", 0.2f);
        }
        else
        {
            animator.SetFloat("Movespeed", 0f);

        }

        if (aiState == AIState.StandAttack)
        {
            animator.SetBool("Attacking", true);
        }
        else
        {
            animator.SetBool("Attacking", false);

        }
    }

    private float cooldown_FireAttack = 0.2f;

    private void CheckICanSeePlayer()
    {
        RaycastHit hit1;
        Vector3 targetDir = targetPlayer.position - outEye.position;
        targetDir.Normalize();

        bool PlayerSee = false;

        if (Physics.Raycast(outEye.position, targetDir, out hit1, Mathf.Infinity, layermaskWeapon))
        {
            Debug.DrawRay(outEye.position, targetDir * hit1.distance, Color.red);

            if (hit1.transform.gameObject.IsParentOf(targetPlayer.gameObject))
            {
                PlayerSee = true;
            }
            else if (hit1.transform.gameObject == targetPlayer.gameObject)
            {
                PlayerSee = true;
            }
        }

        ableSeePlayer = PlayerSee;
    }

    private void Attack()
    {
        if (aiState != AIState.StandAttack
            && aiState != AIState.ChasePlayer)
        {
            return;
        }

        RaycastHit hit;

        bool isHittingSomething = false;
        bool isHittingPlayer = false;

        if (Physics.Raycast(outWeaponTransform.position, outWeaponTransform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layermaskWeapon))
        {
            isHittingSomething = true;
            Debug.DrawRay(outWeaponTransform.position, outWeaponTransform.TransformDirection(Vector3.forward) * hit.distance, Color.green);

            if (hit.transform.gameObject.IsParentOf(targetPlayer.gameObject))
            {
                isHittingPlayer = true;
            }
            else if (hit.transform.gameObject == targetPlayer.gameObject)
            {
                isHittingPlayer = true;
            }
        }


        cooldown_FireAttack += Time.deltaTime;

        if (cooldown_FireAttack > cooldown_WeaponFire)
        {
            if (isHittingPlayer)
            {
                Fire(hit);
            }
            else
            {
                float chanceFire = Random.Range(0f, 1f);
                float hitDist = Vector3.Distance(hit.point, targetPlayer.position);

                //chanceFire = chanceFire + Mathf.Clamp(-hitDist * 0.1f, 0f, 1f);


                if (chanceFire > 0.9f && ableSeePlayer)
                {
                    Fire(hit);
                }
                else if (chanceFire > 0.99f && !ableSeePlayer)
                {
                    Fire(hit);
                }
            }
        }
    }

    private void Fire(RaycastHit hit)
    {
        if (hit.collider == null)
        {
            return;
        }

        var spark = Instantiate(sparkBullet);
        spark.transform.position = hit.point;
        spark.transform.rotation = Quaternion.LookRotation(hit.normal);
        flashWeapon.gameObject.SetActive(true);
        Destroy(spark.gameObject, 3f);

        characterScript charScript = hit.collider.GetComponent<characterScript>();

        if (charScript != null)
        {
            charScript.heal.takeDamage(damage_WeaponFire, 25f);
        }

        audio_Flyby.clip = audioClipsAudioFlyby[Random.Range(0, audioClipsAudioFlyby.Count)];
        audio_Flyby.Play();
        audio_FireGun.pitch = Random.Range(1.1f, 1.4f);
        audio_FireGun.Play();
        cooldown_FireAttack = 0f;
    }


}
