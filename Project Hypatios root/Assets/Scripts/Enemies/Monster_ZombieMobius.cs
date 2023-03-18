using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Monster_ZombieMobius : EnemyScript
{

    [FoldoutGroup("Events")] public UnityEvent OnAttack;
    [FoldoutGroup("Events")] public UnityEvent OnAttackDone;
    [FoldoutGroup("Events")] public UnityEvent OnDead;
    [FoldoutGroup("References")] public CopyTransformRagdoll originRagdoll;
    [FoldoutGroup("References")] public CopyTransformRagdoll targetRagdoll;
    [FoldoutGroup("Parameters")] public float minDistChangeRandomPos = 3f;
    [FoldoutGroup("Parameters")] public float scanEnemyCooldown = 0.5f;

    public Animator animator;
    public float speed = 10;
    public float distanceToAttack = 5f;
    public GameObject hitbox;
    public GameObject visualAttack;
    public List<AnimatorOverrideController> variantControllers;

    [FoldoutGroup("Animations")] public float cooldownAttack = 1.95f;
    [FoldoutGroup("Animations")] public float hitboxStart = .2f;
    [FoldoutGroup("Animations")] public float hitboxLasts = .3f;
    [FoldoutGroup("Animations")] public float anim_MultiplierMoveSpeed = .33f;
    [FoldoutGroup("DEBUG")] public Transform DEBUG_TESTSTUPIDNAVMESH;
    [FoldoutGroup("DEBUG")] public bool DEBUG_ChecknavmeshPath = false;

    private bool isAttacking = false;
    private Vector3 _targetMove;

    private float durationAttack = 4f;
    private NavMeshAgent _navMeshAgent;
    private SpawnHeal SpawnHeal;
    private float distanceToPlayer = 0;

    public static int TotalZombieInScene = 0;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        TotalZombieInScene = 0;
    }

    private void Start()
    {
        TotalZombieInScene++;
        currentTarget = Hypatios.Enemy.FindEnemyEntity(Stats.MainAlliance);
        if (currentTarget != null) _targetMove = currentTarget.transform.position;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        SpawnHeal = GetComponent<SpawnHeal>();
        _navMeshAgent.speed = speed + Random.Range(-1, 2f);
        hitbox.gameObject.SetActive(false);

        float random = Random.Range(0f, 1f);

        if (random < 0.5f)
        {
            animator.runtimeAnimatorController = variantControllers[0];
            transform.localScale *= 1.1f;
        }
        else
        {
            animator.runtimeAnimatorController = variantControllers[1];

        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        TotalZombieInScene--;
    }

    #region Attack and Hitpoint
    public override void Attacked(DamageToken token)
    {
        if (token.originEnemy == this) return;
        if (Stats.IsDamagableBySameType == false && token.originEnemy is Monster_ZombieMobius) return;
        _lastDamageToken = token;

        Stats.CurrentHitpoint -= token.damage;
        base.Attacked(token);

        if (!Stats.IsDead && token.origin == DamageToken.DamageOrigin.Player)
            DamageOutputterUI.instance.DisplayText(token.damage);

    }
    public override void Die()
    {
       // SpawnHeal.SpawnHealCapsule(2);
        _navMeshAgent.Stop();
        animator.SetBool("Dead", true);
        Destroy(gameObject, 5f);
        OnDead?.Invoke();
        OnDied?.Invoke();

        {
            targetRagdoll.CopyRotationPosition(originRagdoll);
            targetRagdoll.gameObject.SetActive(true);
            originRagdoll.gameObject.SetActive(false);
        }

        Stats.IsDead = true;

    }

    #endregion

    public void SetTarget(Transform target1 = null)
    {
        if (target1 == null)
        {
            _targetMove = currentTarget.transform.position;
        }
        else
        {
            _targetMove = target1.position;
        }
    }

    #region Updates


    private void Update()
    {
        if (DEBUG_ChecknavmeshPath) Debug_DrawNavmeshPath();

        if (Time.timeScale <= 0) return;
        DEBUG_TESTSTUPIDNAVMESH.transform.position = _targetMove;
        if (Stats.CurrentHitpoint < 0)
        {
            if (Stats.IsDead == false) Die();
            Stats.IsDead = true;
            return;
        }

        if (isAIEnabled == false) return;

      

        CheckTarget();
        HandleState();
        HandleMovement();
        HandleAnimation();


    }

    private void Debug_DrawNavmeshPath()
    {

    }

    private float _cooldownCheckTarget = 0.5f;
    private float distTargetMove;

    private void ChangeRandomPatrol()
    {
        Vector3 randomPos = Hypatios.Player.transform.position;
        randomPos.x += Random.Range(-10f, 10f);
        randomPos.z += Random.Range(-10f, 10f);
        _targetMove = Hypatios.Enemy.SampleClosestPosition(randomPos, 30f);

        if (_targetMove.x > 99999f)
            _targetMove = new Vector3();
    }

    private void CheckTarget()
    {
        _cooldownCheckTarget -= Time.deltaTime;
        bool enforceCheckMove = false;
        distTargetMove = Vector3.Distance(transform.position, _targetMove);

        if (_cooldownCheckTarget < 0)
        {
            float additionalCooldown = 0;     

            if (Hypatios.Enemy.IsPlayerReachable)
            {
                ScanForEnemies(0.1f);
            }
            else
            {
                currentTarget = Hypatios.Enemy.Horde_GetPlayerAlly(Stats.MainAlliance);
            }

            float random = Random.Range(0f, 1f);

            if (TotalZombieInScene > 50)
            {
                additionalCooldown = Random.Range((TotalZombieInScene / 50f) * 0.5f, (TotalZombieInScene / 50f) * 1f);
                random -= Mathf.Clamp((TotalZombieInScene/50f) * 0.2f, 0f, .5f);
            }

            if (_navMeshAgent.IsAgentCanReachLocation(_targetMove) == false && random > 0.5f)
                enforceCheckMove = true;

           
            _cooldownCheckTarget = scanEnemyCooldown +additionalCooldown;
        }

        if (enforceCheckMove)
        {
            ChangeRandomPatrol();
        }

        //there is really no other target left, wander randomly
        if (currentTarget == null)
        {

            if (distTargetMove < minDistChangeRandomPos)
            {
                ChangeRandomPatrol();
            }

            return;
        }

        if (_navMeshAgent.IsAgentCanReachLocation(_targetMove) == false)
        {
            
        }

        _targetMove = currentTarget.transform.position;
    }

    private void HandleAnimation()
    {
        animator.SetFloat("MoveSpeed", _navMeshAgent.velocity.magnitude / anim_MultiplierMoveSpeed);
    }

    private void HandleMovement()
    {
        if (isAttacking == false)
        {
            _navMeshAgent.updateRotation = true;
            _navMeshAgent.SetDestination(_targetMove);
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

    }

    private void HandleRotation()
    {
        _navMeshAgent.updateRotation = false;

        Vector3 posTarget = _targetMove;

        Vector3 dir = posTarget - transform.position;
        dir.y = 0;
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = rotation;
    }

    private void HandleState()
    {
        bool currentTargetExist = false;
        if (currentTarget != null)
            currentTargetExist = true;

        if (currentTargetExist)
        {
            distanceToPlayer = Vector3.Distance(transform.position, _targetMove);

            if (distanceToPlayer < distanceToAttack && !isAttacking)
            {
                AttackPlayer();
            }
        }
    }

    #endregion



    private void AIDecision()
    {

    }

    #region Attacking

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

    #endregion
}
