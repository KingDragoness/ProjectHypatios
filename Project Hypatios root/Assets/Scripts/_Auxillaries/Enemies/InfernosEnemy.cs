using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class InfernosEnemy : EnemyScript
{

    public enum Stage
    {
        Idle,
        Chase,
        Attack
    }

    [FoldoutGroup("References")] public Animator animator;
    [FoldoutGroup("References")] public GameObject corpse;
    [FoldoutGroup("References")] public GameObject startingModel;
    [FoldoutGroup("References")] public GameObject currentModel;
    [FoldoutGroup("References")] public GameObject fireAttackZone;
    [FoldoutGroup("References")] public ParticleSystem fireParticle;
    [FoldoutGroup("References")] public Transform posSpawn;
    [FoldoutGroup("References")] public ThrowProjectileEnemy projectileEnemy;
    [FoldoutGroup("References")] public SpawnAmmo ammoSpawn;
    public Stage currentStage;
    public float distAttack = 5f;
    public float distSeenPlayer = 25f;
    public float CooldownAttack = 1.5f;
    public float RangeAttackDamage = 5f;
    public int RangeAttackBurstMin = 3;
    public int RangeAttackBurstmax = 6;
    [FoldoutGroup("Audios")] public AudioSource audio_FireLaser;

    private NavMeshAgent agent;
    private IEnumerator _coroutineAttack;
    private float _timerAttack = 0f;
    private bool _isReady = false;

    private void Start()
    {
        currentTarget = Hypatios.Enemy.FindEnemyEntity(Stats.MainAlliance);
        agent = GetComponent<NavMeshAgent>();
        _timerAttack = CooldownAttack;

        if (onSpawnShouldReady)
        {
            _isReady = true;
        }
        else
        {
            startingModel.gameObject.SetActive(true);
            currentModel.gameObject.SetActive(false);
            NavMeshAgent meshAgent = GetComponent<NavMeshAgent>();
            meshAgent.enabled = false;
        }
    }

    #region Attacks

    [FoldoutGroup("Debug Inferno")] [Button("SplashAttack")]
    public void SplashAttack()
    {
        animator.SetTrigger("Attack");
        fireAttackZone.gameObject.SetActive(false);
        if (_coroutineAttack != null)
            StopCoroutine(_coroutineAttack);
        _coroutineAttack = SplashAttackFireMove();
        StartCoroutine(_coroutineAttack);
    }

    [FoldoutGroup("Debug Inferno")]
    [Button("RangeAttack")]
    public void RangeAttack()
    {
        animator.SetTrigger("Attack");
        if (_coroutineAttack != null)
            StopCoroutine(_coroutineAttack);
        _coroutineAttack = RangeAttackFireMove();
        StartCoroutine(_coroutineAttack);
    }

    IEnumerator RangeAttackFireMove()
    {
        int bursts = Random.Range(RangeAttackBurstMin, RangeAttackBurstmax);
        yield return new WaitForSeconds(0.2f);
        for (int x = 0; x < bursts; x++)
        {
            if (currentTarget == null) break;
            Vector3 randomPos = posSpawn.transform.position;
            randomPos.y = transform.position.y;
            Vector3 dir = (currentTarget.transform.position - randomPos).normalized;
            var projectile1 = projectileEnemy.FireProjectile(dir);
            projectile1.Damage = RangeAttackDamage + Random.Range(0, 1f);
            projectile1.transform.position = posSpawn.transform.position;
            projectile1.gameObject.SetActive(true);
            audio_FireLaser.PlayOneShot(audio_FireLaser.clip);
            yield return new WaitForSeconds(0.11f);
        }
    }

    IEnumerator SplashAttackFireMove()
    {
        yield return new WaitForSeconds(0.3f);
        fireAttackZone.gameObject.SetActive(true);
        fireParticle.Play();
    }

    public override void Attacked(DamageToken token)
    {

        if (token.originEnemy == this) return;
        if (token.originEnemy != null)
        {
            if (token.originEnemy.GetType() == typeof(InfernosEnemy)) return;
        }


        hasSeenPlayer = true;
        _lastDamageToken = token;

        if (token.damageType == DamageToken.DamageType.Fire)
        {
            Stats.CurrentHitpoint += token.damage;
        } else Stats.CurrentHitpoint -= token.damage;


        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddRelativeForce(Vector3.forward * -1 * 100 * token.repulsionForce);
        }
        else
        {
            transform.position += Vector3.back * 0.05f * token.repulsionForce;
        }


        if (!Stats.IsDead && token.origin == DamageToken.DamageOrigin.Player)
            DamageOutputterUI.instance.DisplayText(token.damage);

        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }

    }

    #endregion

    private void Update()
    {
        if (_isReady == false)
        {
            if (_timerReady > 0)
            {
                _timerReady -= Time.deltaTime;
            }
            else
            {
                startingModel.gameObject.SetActive(false);
                currentModel.gameObject.SetActive(true);
                NavMeshAgent meshAgent = GetComponent<NavMeshAgent>();
                meshAgent.enabled = true;
                _isReady = true;
            }
        }

        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }
        if (isAIEnabled == false) return;

        if (Mathf.RoundToInt(Time.time) % 5 == 0)
            ScanForEnemies(0f, 60f);

        if (currentTarget != null && _isReady)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (dist < distSeenPlayer)
                hasSeenPlayer = true;

            if (hasSeenPlayer == true)
                CombatMode();
        }

   
    }

    private void CombatMode()
    {
        float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
        bool allowAttack = false;

        if (_timerAttack > 0)
            _timerAttack -= Time.deltaTime;
        else
        {
            allowAttack = true;
            _timerAttack = CooldownAttack;
        }

        if (dist < distAttack)
            currentStage = Stage.Attack;
        else currentStage = Stage.Chase;

        if (currentStage == Stage.Attack)
        {
            HandleAttackMovement();
            LockMovement();
            if (allowAttack)
                SplashAttack();
        }
        else
        {
            float chance = Random.Range(0f, 1f);
            if (chance < 0.2f && allowAttack)
                RangeAttack();
            ResumeMovement();
            agent.SetDestination(currentTarget.transform.position);
        }
    }

    private void HandleAttackMovement()
    {
        Vector3 posTarget = currentTarget.transform.position;

        Vector3 dir = posTarget - transform.position;
        dir.y = 0;
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = rotation;
    }


    private void ResumeMovement()
    {
        agent.stoppingDistance = 3f;
        agent.updateRotation = true;
    }

    private void LockMovement()
    {
        agent.updateRotation = false;
    }

    public override void Burn()
    {
        return;
    }

    public override void Die()
    {
        if (Stats.IsDead) return;

        OnDied?.Invoke();
        OnSelfKilled?.Invoke();
        Destroy(gameObject);
        var corpse1 = Instantiate(corpse, transform.position, transform.rotation);
        corpse1.gameObject.SetActive(true);
        corpse1.transform.position = transform.position;
        corpse1.transform.rotation = transform.rotation;
        ammoSpawn.SpawnSoulCapsule();
        Stats.IsDead = true;

    }



}
