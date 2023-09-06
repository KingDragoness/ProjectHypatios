using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public class DeathSweeperEnemy : EnemyScript
{

    public enum StateAI
    {
        Pursue,
        Attack,
        Idle
    }

    public enum AttackState
    {
        Charging,
        Attacking,
        Cooldown
    }

    public StateAI currentStateAI;
    [ReadOnly] [ShowInInspector] private AttackState _attackState;
    public Animator animator;
    public DummyEnemyTest dummyAI;
    [FoldoutGroup("AI-DeathSweep")] public float maxAttackRange = 30;
    [FoldoutGroup("AI-DeathSweep")] public float acquisationRange = 65;
    [FoldoutGroup("AI-DeathSweep")] public float rotationSpeed = 7.5f;
    [FoldoutGroup("Weapons")] public float chargeSweeperTime = 6f;
    [FoldoutGroup("Weapons")] public float ultraLaserTime = 6f;
    [FoldoutGroup("Weapons")] public float cooldownSweeper = 4f; //after UltraLaserTime
    [FoldoutGroup("References")] public GameObject corpse;
    [FoldoutGroup("References")] public ParticleSystem particle_LaserCharging;
    [FoldoutGroup("References")] public LineRenderer line_UltraLaser;

    [FoldoutGroup("Sounds")] public AudioSource audio_LaserCharging;
    [FoldoutGroup("Sounds")] public AudioSource audio_LaserAttack;
    [FoldoutGroup("Sounds")] public AudioSource audio_Idle;

    public const float ATTACK_FIRE_RATE = 0.25f;

    private float evaluateChoiceTimer = 2;
    private float _attack_FireRate = 0.25f;
    private float _attack_chargeLaserTime = 0f;
    private float _attack_ClockTimer = 0f;
    private float _attack_Cooldown = 0f;
    private StateAI previousState;

    private void Start()
    {
        currentTarget = Hypatios.Enemy.FindEnemyEntity(Stats.MainAlliance);
        particle_LaserCharging.Stop();
        line_UltraLaser.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }
        else
        {
            if (isAIEnabled == false) return;

            if (Mathf.RoundToInt(Time.time) % 5 == 0)
                ScanForEnemies();

            AliveState();
            EvaluateMyLife();
        }

    }


    public void AliveState()
    {
        if (currentTarget == null) return;
        dummyAI.target = currentTarget.transform;

        if (currentStateAI == StateAI.Pursue)
        {
            dummyAI.disableBehavior = false;
            dummyAI.Rigidbody.isKinematic = false;
            animator.SetBool("Attack", false);
        }

        if (currentStateAI == StateAI.Attack)
        {
            dummyAI.disableBehavior = true;
            dummyAI.Rigidbody.isKinematic = true;
            animator.SetBool("Attack", true);

            Vector3 relativePos = currentTarget.transform.position - transform.position;

            Quaternion toRotation = Quaternion.LookRotation(relativePos);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            Attacking();
        }
        else
        {
            NotAttackingState();
        }

        if (currentStateAI == StateAI.Idle)
        {
            dummyAI.Rigidbody.isKinematic = false;
            dummyAI.disableBehavior = false;
        }

        previousState = currentStateAI;
    }

    private void NotAttackingState()
    {
        particle_LaserCharging.Stop();
        _hasCharged = false;
        if (line_UltraLaser.gameObject.activeSelf == true) line_UltraLaser.gameObject.SetActive(false);
        audio_LaserCharging.Stop();
        audio_LaserAttack.Stop();
    }

    private bool _hasCharged = false;

    private void Attacking()
    {
        if (previousState != StateAI.Attack)
        {
            //resets charge Time
            _attack_chargeLaserTime = chargeSweeperTime;
            _attack_ClockTimer = ultraLaserTime;
            _attack_Cooldown = cooldownSweeper;
            _attackState = AttackState.Charging;
            _hasCharged = false;
        }

        if (_attackState == AttackState.Charging)
        {
            _attack_chargeLaserTime -= Time.deltaTime;
            if (audio_LaserCharging.isPlaying == false) audio_LaserCharging.Play();

            if (_hasCharged == false)
            {
                particle_LaserCharging.Emit(1);
                _hasCharged = true;
            }

            if (_attack_chargeLaserTime <= 0)
            {
                //fire
                _attackState = AttackState.Attacking;
            }
        }
        else
        {
            particle_LaserCharging.Stop();
            audio_LaserCharging.Stop();
            _hasCharged = false;
        }

        if (_attackState == AttackState.Attacking)
        {
            _attack_FireRate -= Time.deltaTime;
            _attack_ClockTimer -= Time.deltaTime;
            if (line_UltraLaser.gameObject.activeSelf == false) line_UltraLaser.gameObject.SetActive(true);
            if (audio_LaserAttack.isPlaying == false) audio_LaserAttack.Play();

            DrawLaser();

            if (_attack_FireRate <= 0f)
            {
                //deal damage
                DealDamage();
                _attack_FireRate = ATTACK_FIRE_RATE;
            }

            if (_attack_ClockTimer <= 0f)
            {
                _attackState = AttackState.Cooldown;
            }
        }
        else
        {
            if (line_UltraLaser.gameObject.activeSelf == true) line_UltraLaser.gameObject.SetActive(false);
            audio_LaserAttack.Stop();

        }

        if (_attackState == AttackState.Cooldown)
        {
            _attack_Cooldown -= Time.deltaTime;

            if (_attack_Cooldown <= 0f)
            {
                //reset
                _attack_chargeLaserTime = chargeSweeperTime;
                _attack_ClockTimer = ultraLaserTime;
                _attack_Cooldown = cooldownSweeper;
                _attackState = AttackState.Charging;
                _hasCharged = false;
            }
        }
    }

    private void DrawLaser()
    {
        var targetPos = currentTarget.OffsetedBoundWorldPosition;
        var points = new Vector3[2];
        points[0] = eyeLocation.transform.position;


        Ray ray = new Ray(eyeLocation.transform.position, targetPos - eyeLocation.transform.position);
        if (Physics.SphereCast(ray, .2f, out RaycastHit hit, 100f))
        {
            {
                points[1] = hit.point;
            }
        }
        else
        {
            points[1] = eyeLocation.forward * 100f;
        }

        line_UltraLaser.SetPositions(points);
    }

    private void DealDamage()
    {
        if (currentTarget == null) return; //no target detected :(

        var targetPos = currentTarget.OffsetedBoundWorldPosition;
       

        Ray ray = new Ray(eyeLocation.transform.position, targetPos - eyeLocation.transform.position);
        if (Physics.SphereCast(ray, .2f, out RaycastHit hit, 100f))
        {
            DamageToken token = new DamageToken();
            token.damage = Stats.BaseDamage.Value + Random.Range(0, Stats.VariableDamage.Value);
            token.originEnemy = this;
            if (Stats.MainAlliance != Alliance.Player) token.origin = DamageToken.DamageOrigin.Enemy; else token.origin = DamageToken.DamageOrigin.Ally;
            token.healthSpeed = 60f;
            token.shakinessFactor = 1f;

            UniversalDamage.TryDamage(token, hit.transform, transform);
        }
        else
        {
        }


    }

    private void EvaluateMyLife()
    {
        evaluateChoiceTimer += Time.deltaTime;

        if (currentTarget == null) return;

        if (evaluateChoiceTimer > 2)
        {
            float randomSound = Random.Range(0f, 5f);
            float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (randomSound < 1f) audio_Idle.Play();
            AI_Detection();

            if (hasSeenPlayer)
            {
                if (dist <= maxAttackRange)
                {
                    currentStateAI = StateAI.Attack;
                }
                else
                {
                    currentStateAI = StateAI.Pursue;
                }
            }
            else
            {

                currentStateAI = StateAI.Idle;
            }
            evaluateChoiceTimer = 0;
        }
    }

    public override void Attacked(DamageToken token)
    {
        if (token.originEnemy == this) return;
        _lastDamageToken = token;

        Stats.CurrentHitpoint -= token.damage;
        if (!Stats.IsDead)
            DamageOutputterUI.instance.DisplayText(token.damage);

        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }

        base.Attacked(token);
    }

    public override void Die()
    {
        if (Stats.IsDead) return;

        OnDied?.Invoke();
        Destroy(gameObject);
        var corpse1 = Instantiate(corpse, transform.position, transform.rotation);
        corpse1.gameObject.SetActive(true);
        corpse1.transform.position = transform.position;
        corpse1.transform.rotation = transform.rotation;
        Stats.IsDead = true;
    }
}
