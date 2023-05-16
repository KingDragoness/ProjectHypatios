using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using System.Linq;

public class MercenaryEnemy : EnemyScript
{

    public enum AIState
    {
        Idle,
        Move,
        Escape, //move backward, prevent getting killed from explosions 
        Fire,
        Melee
    }

    public AIState state;
    public float CooldownAIRefresh = 0.1f;
    public float CooldownWeapon = 3f;
    public float CooldownMelee = 2f;
    public float CooldownFindNewEscapePoint = 5;
    public float MoveToTargetDistance = 29f;
    public float AngleLimitAttack = 16f;
    public float RotationSpeed = 10f;
    public float MinimumDistance = 5f;
    public float ProjectileTooCloseDistance = 5f;
    public float Anim_SpeedMultplierValue = 0.1f;
    [FoldoutGroup("Merc")] public Animator anim;
    [FoldoutGroup("Merc")] public ParticleSystem laserCharge;
    [FoldoutGroup("Merc")] public GameObject corpse;
    [FoldoutGroup("Merc")] public GameObject meleeAttackSphere;
    [FoldoutGroup("Merc")] public Transform outOrigin;
    [FoldoutGroup("Merc")] public MissileChameleon missilePrefab;
    [FoldoutGroup("Audios")] public AudioSource audio_Fire;
    [FoldoutGroup("Audios")] public AudioSource audio_walking;
    [FoldoutGroup("Audios")] public float walk_PitchLimit = 2f;
    [FoldoutGroup("Audios")] public float walk_PitchMin = 1f;
    [FoldoutGroup("Audios")] public float walk_PitchMult = 2.5f;

    private NavMeshAgent agent;
    private float _timerDetection = 0.1f;
    private float _timerWeaponMissile = 3f;
    private float _timerMelee = 2f;
    private Vector3 escapePos = Vector3.zero;
    private float _escapeCooldown = 0.2f;
    private bool isCharging = false;

    public bool IsMoving => agent.velocity.magnitude > 0.1f;

    private void Start()
    {
        currentTarget = Hypatios.Enemy.FindEnemyEntity(Stats.MainAlliance);
        agent = GetComponent<NavMeshAgent>();

    }

    private void Update()
    {
        if (Time.timeScale <= 0) return;

        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }
        if (isAIEnabled == false) return;

        _timerDetection -= Time.deltaTime;
        _escapeCooldown -= Time.deltaTime;

        if (Mathf.RoundToInt(Time.time) % 5 == 0)
            ScanForEnemies();

        if (currentTarget != null) AI_Detection();

        if (_timerDetection <= 0f)
        {
            UpdateAIStates();
            _timerDetection = CooldownAIRefresh;
        }

        Movement();
        AnimationUpdate();

        {
            if (state == AIState.Fire)
            {
                AttackingUpdate();
            }
            else
            {
                _timerWeaponMissile = CooldownWeapon;

                if (hasSeenPlayer)
                {
                    laserCharge.loop = false;
                    laserCharge.Stop();
                    isCharging = false;
                }
            }

            if (state == AIState.Escape)
            {
                HandleEscape();
            }

            if (state == AIState.Melee)
            {
                _timerMelee -= Time.deltaTime;
                StartCoroutine( HandleMeleeAttack());
            }
            else
            {
                _timerMelee = CooldownMelee;
            }

          
        }
    }

    private void HandleEscape()
    {
        if (currentTarget == null) return;
        float dist = 9999f;
        dist = Vector3.Distance(OffsetedBoundWorldPosition, currentTarget.transform.position);

        if (dist > MinimumDistance)
        {
            _escapeCooldown = 0.2f;
        }

        if (_escapeCooldown < 0f)
        {
            escapePos = IsopatiosUtility.RandomNavSphere(transform.position, 8f, -1);
            _escapeCooldown = CooldownFindNewEscapePoint;
        }
    }

    private IEnumerator HandleMeleeAttack()
    {
        anim.SetTrigger("Melee");
        yield return new WaitForSeconds(0.4f);
        meleeAttackSphere.gameObject.SetActive(true);

    }

    #region Updates
    private void UpdateAIStates()
    {
        float dist = 9999f;
        float angle = 0f;
        float relativeZ = 0f;
        if (currentTarget != null)
        {
            Vector3 targetDir = currentTarget.transform.position - transform.position;
            dist = Vector3.Distance(OffsetedBoundWorldPosition, currentTarget.transform.position);
            angle = Vector3.Angle(targetDir, transform.forward);
            relativeZ = transform.InverseTransformPoint(currentTarget.OffsetedBoundWorldPosition).z;
        }

        if (canLookAtTarget == true && dist < MoveToTargetDistance 
            && Mathf.Abs(angle) < AngleLimitAttack && currentTarget != null)
        {
            state = AIState.Fire;
        }
        else
        {
            state = AIState.Move;
        }

        if (_timerMelee < 0f)
        {
            state = AIState.Move;
        }

        if (dist < MinimumDistance && relativeZ > 0.5f)
        {
            state = AIState.Melee;
        }
    }

    private void AnimationUpdate()
    {
        anim.SetFloat("Speed", agent.velocity.magnitude * Anim_SpeedMultplierValue);

        if (IsMoving)
        {
            float pitch = agent.velocity.magnitude;
            pitch *= walk_PitchMult;
            pitch = Mathf.Clamp(pitch, walk_PitchMin, walk_PitchLimit);
            audio_walking.pitch = pitch;
            if (audio_walking.isPlaying == false) audio_walking?.Play();
        }
        else
        {
            if (audio_walking.isPlaying == true) audio_walking.Stop();
        }
    }

    private void AttackingUpdate()
    {
        if (currentTarget == null) return;
        _timerWeaponMissile -= Time.deltaTime;

        if (!isCharging)
        {
            laserCharge.Play();
            laserCharge.loop = true;
            isCharging = true;
        }


        if (_timerWeaponMissile <= 0f)
        {
            outOrigin.LookAt(currentTarget.OffsetedBoundWorldPosition);
            bool tooClose = IsProjectileWillHitTooClose();

            if (tooClose == false)
            {
                LaunchMissile();
                _timerWeaponMissile = CooldownWeapon;
            }
            else
            {
                _timerWeaponMissile = 0.1f;
            }
        }
    }

    private void Movement()
    {
        if (state == AIState.Move)
        {
            agent.updateRotation = true;

            FindPosition();
        }
        else if (state == AIState.Escape)
        {
            agent.updateRotation = true;

            agent.SetDestination(escapePos);
            if (agent.isStopped) agent.Resume();

        }
        else if (state == AIState.Fire | state == AIState.Melee)
        {
            agent.updateRotation = false;
            agent.Stop();

            if (currentTarget == null)
            {
                return;
            }
            Vector3 posTarget = currentTarget.transform.position;

            Vector3 dir = posTarget - transform.position;
            dir.y = 0;
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * RotationSpeed);

        }
    }
    #endregion

    private bool IsProjectileWillHitTooClose()
    {
        RaycastHit hit;

        if (Physics.Raycast(outOrigin.transform.position, outOrigin.transform.forward, out hit, ProjectileTooCloseDistance + 1f, Hypatios.Enemy.baseSolidLayer, QueryTriggerInteraction.Ignore))
        {
            if (hit.distance < ProjectileTooCloseDistance)
            {
                return true;
            }
        }

        return false;
    }

    [Button("Launch Missile")]
    public void LaunchMissile()
    {
        GameObject prefabMissile = Instantiate(missilePrefab.gameObject, outOrigin.position, outOrigin.rotation);
        prefabMissile.gameObject.SetActive(true);
        var missile = prefabMissile.GetComponent<MissileChameleon>();
        missile.OverrideTarget(currentTarget, Stats.MainAlliance);
        audio_Fire.Play();
        isCharging = false;

    }

    void FindPosition()
    {
        if (currentTarget != null) agent.SetDestination(currentTarget.transform.position);
        if (agent.isStopped) agent.Resume();
    }


    public override void Attacked(DamageToken token)
    {
        int time1 = Hypatios.TimeTick;

        if (_lastDamageToken != null)
        {
            if (time1 <= _lastDamageToken.timeAttack) return;
        }

        if (token.damageType == DamageToken.DamageType.Explosion)
        {
            token.damage *= 0.2f;
        }
        _lastDamageToken = token;

        Stats.CurrentHitpoint -= token.damage;


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
