using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class MobiusGuard : EnemyScript
{

    public enum AIState
    {
        Standby,
        Patrol,
        StandAttack,
        ChasePlayer,
        UnderAttacked
    }

    public UnityEvent OnDiedEvent;

    [FoldoutGroup("Parameter AI")] public AIState aiState;
    [FoldoutGroup("Parameter AI")] public float speed_Strafing = 2f;
    [FoldoutGroup("Parameter AI")] public float speed_StandAttack = 0.1f;
    [FoldoutGroup("Parameter AI")] public float randomStrafingDist = 3f;
    [FoldoutGroup("Parameter AI")] public float stunDamageTime = 1.1f;

    [FoldoutGroup("References")] public Animator animator;
    [FoldoutGroup("References")] public SpawnHeal spawnHeal;
    [FoldoutGroup("References")] public Transform outEye;
    [FoldoutGroup("References")] public Transform originWeapon;
    [FoldoutGroup("References")] public Transform outWeaponTransform;
    [FoldoutGroup("References")] public FollowObject followTargetIK;
    [FoldoutGroup("References")] public MobiusLookWeaponLimit mobiusLookWeaponLimit;
    [FoldoutGroup("Weapon")] public GameObject sparkBullet;
    [FoldoutGroup("Weapon")] public GameObject flashWeapon;
    [FoldoutGroup("Weapon")] public GameObject tracerLaser;
    [FoldoutGroup("Weapon")] public AudioSource audio_FireGun;
    [FoldoutGroup("Weapon")] public AudioSource audio_Flyby;
    [FoldoutGroup("Weapon")] public List<AudioClip> audioClipsAudioFlyby;
    [FoldoutGroup("Weapon")] public float cooldown_WeaponFire = 0.4f;
    [FoldoutGroup("Weapon")] public int damage_WeaponFire = 5;

    private Vector3 strafingTargetPos;
    private float timer_Stunned = 1.1f;
    private NavMeshAgent navMeshAgent;
    private bool ableSeePlayer = false;
    [ReadOnly] [SerializeField] bool playerCanBeReached = false;

    private void Start()
    {
        currentTarget = Hypatios.Enemy.FindEnemyEntity(Stats.MainAlliance);
        strafingTargetPos = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {

        if (Stats.CurrentHitpoint < 0)
        {
            if (Stats.IsDead == false) Die();
            Stats.IsDead = true;
            Stats.CurrentHitpoint = 0;
        }

        if (Mathf.RoundToInt(Time.time) % 5 == 0)
            ScanForEnemies();

        if (Time.timeScale == 0) return;

        if (Stats.IsDead)
        {
            animator.SetBool("Dead", true);
            animator.ResetTrigger("Damaged");
            return;
        }

        if (isAIEnabled == false) return;
        if (currentTarget == null) return;
        followTargetIK.target = currentTarget.transform;
        mobiusLookWeaponLimit.player = currentTarget.transform;

        UpdateEnemyState();
        AnimationStateUpdate();
        Attack();
        CheckICanSeePlayer();
    }


    public override void Die()
    {
        OnDiedEvent?.Invoke();
        OnDied?.Invoke();
        Destroy(gameObject, 5f);
        //spawnHeal.SpawnHealCapsule(1);
        Stats.IsDead = true;

    }

    public override void Attacked(DamageToken token)
    {
        _lastDamageToken = token;

        timer_Stunned = stunDamageTime;
        aiState = AIState.UnderAttacked;
        animator.SetTrigger("Damaged");

        Stats.CurrentHitpoint -= token.damage;
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
        //navMeshAgent.speed = speed_StandAttack;
        //navMeshAgent.SetDestination(currentTarget.transform.position);
        navMeshAgent.updateRotation = false;
        Vector3 posTarget = currentTarget.transform.position;

        Vector3 dir = posTarget - transform.position;
        dir.y = 0;
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = rotation;


        cooldown_CheckMove += Time.deltaTime;

        if (cooldown_CheckMove > 0.2f)
        {
            Vector3 cameraRelative = transform.InverseTransformPoint(currentTarget.transform.position);

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

        if (playerCanBeReached)
        {
            navMeshAgent.SetDestination(currentTarget.transform.position);
            navMeshAgent.updateRotation = true;
        }
        else
        {
            navMeshAgent.updateRotation = false;
            Vector3 posTarget = currentTarget.transform.position;

            Vector3 dir = posTarget - transform.position;
            dir.y = 0;
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = rotation;

        }

        if ((Mathf.RoundToInt(Time.time * 5) % 2) == 0)
        {
            CheckCalculate();
        }

        cooldown_CheckMove += Time.deltaTime;

        if (cooldown_CheckMove > 1f)
        {
            Vector3 cameraRelative = transform.InverseTransformPoint(currentTarget.transform.position);

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

    private void CheckCalculate()
    {
        NavMeshPath navMeshPath = new NavMeshPath();

        if (navMeshAgent.CalculatePath(currentTarget.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            playerCanBeReached = true;

        }
        else
        {
            playerCanBeReached = false;
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
        Vector3 targetDir = currentTarget.transform.position - outEye.position;
        targetDir.Normalize();

        bool PlayerSee = false;

        if (Physics.Raycast(outEye.position, targetDir, out hit1, Mathf.Infinity, Hypatios.Enemy.baseDetectionLayer))
        {
            Debug.DrawRay(outEye.position, targetDir * hit1.distance, Color.red);

            if (hit1.transform.gameObject.IsParentOf(currentTarget.transform.gameObject))
            {
                PlayerSee = true;
            }
            else if (hit1.transform.gameObject == currentTarget.transform.gameObject)
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

        if (Physics.Raycast(outWeaponTransform.position, outWeaponTransform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, Hypatios.Enemy.baseSolidLayer))
        {
            isHittingSomething = true;
            Debug.DrawRay(outWeaponTransform.position, outWeaponTransform.TransformDirection(Vector3.forward) * hit.distance, Color.green);

            if (hit.transform.gameObject.IsParentOf(currentTarget.transform.gameObject))
            {
                isHittingPlayer = true;
            }
            else if (hit.transform.gameObject == currentTarget.transform.gameObject)
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
                float hitDist = Vector3.Distance(hit.point, currentTarget.transform.position);

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

        DamageToken token = new DamageToken();
        token.damage = damage_WeaponFire;
        token.origin = DamageToken.DamageOrigin.Enemy;
        token.originEnemy = this;
        token.healthSpeed = 25f;

        var spark = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.BulletSparksEnemy, true);
        spark.transform.position = hit.point;
        spark.transform.rotation = Quaternion.LookRotation(hit.normal);
        flashWeapon.gameObject.SetActive(true);

        UniversalDamage.TryDamage(token, hit.transform, transform) ;

        audio_Flyby.clip = audioClipsAudioFlyby[Random.Range(0, audioClipsAudioFlyby.Count)];
        audio_Flyby.Play();
        audio_FireGun.pitch = Random.Range(1.1f, 1.4f);
        audio_FireGun.Play();
        cooldown_FireAttack = 0f;

        {

            var points = new Vector3[2];
            points[0] = originWeapon.transform.position;
            var currentLaser = tracerLaser;
            GameObject laserLine = Instantiate(currentLaser, originWeapon.transform.position, Quaternion.identity);

            {
                points[1] = hit.point;
                var lr = laserLine.GetComponent<LineRenderer>();
                lr.SetPositions(points);
            }
        }
    }


}
