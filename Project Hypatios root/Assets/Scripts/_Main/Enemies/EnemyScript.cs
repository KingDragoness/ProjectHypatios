using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;



public abstract class EnemyScript : Entity
{

    [FoldoutGroup("Base")] [HideInEditorMode] [ShowInInspector] private EnemyStats _stats;
    [FoldoutGroup("Base")] [HideInPlayMode] [SerializeField] [InlineEditor] private BaseEnemyStats _baseStat;
    [FoldoutGroup("Optional")] public UnityEvent OnDamaged; //called from derived enemy classes
    [FoldoutGroup("Optional")] public UnityEvent OnSelfKilled; //called from derived enemy classes
    [FoldoutGroup("Optional")] public UnityEvent OnPlayerDetected1; //called from derived enemy classes


    public EnemyStats Stats { get => _stats; }
    public Sprite EnemySprite { get => _baseStat.enemySprite; }
    public string EnemyName { get => _baseStat.name; }
    public RaycastHit HitDetection { get => _hitDetection;  }
    public Transform EyeLocation { get => eyeLocation; }
    public float LastTimeSeenPlayer { get => _lastTimeSeenPlayer; set => _lastTimeSeenPlayer = value; }

    [FoldoutGroup("AI")] [ShowInInspector] [ReadOnly] public Entity currentTarget;
    [FoldoutGroup("AI")] [ShowInInspector] public bool hasSeenPlayer = false;
    [FoldoutGroup("AI")] [ShowInInspector] [ReadOnly] private bool _canLookAtTarget = false;
    [FoldoutGroup("AI")] [ShowInInspector] [SerializeField] public bool isAIEnabled = true;
    [FoldoutGroup("AI")] [SerializeField] protected internal Transform eyeLocation;
    [FoldoutGroup("AI")] [SerializeField] protected Transform debug_FirstPassHit;
    [FoldoutGroup("AI")] [SerializeField] protected Transform debug_SecondPassHit;
    [FoldoutGroup("AI")] [SerializeField] public bool onSpawnShouldReady = true;
    [FoldoutGroup("AI")] [SerializeField] protected float _timerReady = 0f;

    public bool canLookAtTarget
    {
        get
        {
             return _canLookAtTarget;
        }
    }

    protected DamageToken _lastDamageToken;
    protected float _lastTimeSeenPlayer = 0f;

    public System.Action OnDied;
    public System.Action OnPlayerDetected;

    private RaycastHit _hitDetection;
    private bool _hasTriggeredDetection = false;

    public virtual void Awake()
    {
        Hypatios.Enemy.RegisterEnemy(this);
        if (eyeLocation == null) eyeLocation = transform;
        _stats = _baseStat.Stats.Clone();
        _stats.Initialize();
        OnDied += Died;

    }

    public virtual string Debug_AdditionalString()
    {
        return "";
    }

    public virtual void OnDestroy()
    {
        Hypatios.Enemy.DeregisterEnemy(this);
    }


    #region Status

    [HorizontalGroup("Status", order: 101)] [Button("Paralyze")]
    /// <summary>
    /// Disables enemy's AI.
    /// </summary>
    public virtual void Paralyze() 
    {
        if (isAIEnabled == true)
        {
            var ParalyzeParticle = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.ParalyzeEffect, false);
            var particleFX = ParalyzeParticle.GetComponent<ParticleFXResizer>();
            var pos = OffsetedBoundWorldPosition;
            pos.y += OffsetedBoundScale.y;
            ParalyzeParticle.transform.position = pos;
            ParalyzeParticle.transform.localEulerAngles = Vector3.zero;
            particleFX.ResizeParticle(OffsetedBoundScale.magnitude);
            ParalyzeParticle.transform.SetParent(transform);
        }

        isAIEnabled = false; 
    }

    [HorizontalGroup("Status")]
    [Button("Deparalyze")]
    /// <summary>
    /// Disables enemy's AI.
    /// </summary>
    public virtual void Deparalyze() { isAIEnabled = true; }

    [HorizontalGroup("Status")]
    [Button("Hack")]
    /// <summary>
    /// Turn enemy's faction to Player.
    /// </summary>
    public virtual void Hack()
    {
        Stats.MainAlliance = Alliance.Player;
        ScanForEnemies();
    }

    [HorizontalGroup("Status")]
    [Button("Frenzy")]
    /// <summary>
    /// Turn enemy's faction to Player.
    /// </summary>
    public virtual void Frenzy()
    {
        Stats.MainAlliance = Alliance.Rogue;
        ScanForEnemies();
    }

    public override void Heal(float healAmount)
    {
        _stats.CurrentHitpoint += healAmount;
    }


    /// <summary>
    /// Warp enemy to world position.
    /// </summary>
    /// <param name="targetPos"></param>
    public virtual void Warp(Vector3 targetPos)
    {
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (agent != null) agent.Warp(targetPos);
        else transform.position = targetPos;
    }


    public virtual void Revert()
    {
        _stats = _baseStat.Stats.Clone();
        _stats.Initialize();
    }


    public abstract void Die();

    /// <summary>
    /// Remember to assign _lastDamageToken!
    /// </summary>
    private void Died()
    {
        Hypatios.Enemy.OnEnemyDied?.Invoke(this, _lastDamageToken);
        Stats.IsDead = true;
    }

    #endregion

    #region Check Status

    public bool IsOnFire()
    {
        return AllStatusInEffect.Find(x => x.statusCategoryType == ModifierEffectCategory.Fire);
    }
    public bool IsPoisoned()
    {
        return AllStatusInEffect.Find(x => x.statusCategoryType == ModifierEffectCategory.Poison);
    }
    public bool IsParalyzed()
    {
        return !isAIEnabled;
    }

    #endregion

    #region AI

    [FoldoutGroup("Debug")]
    [Button("Enforce scan target")]
    public virtual void ScanForEnemies(float favorPlayer = 0f, float maxDistance = 1000f, float thresholdNearestAllyDist = 20f, Alliance overrideAlliance = Alliance.Null)
    {
        float distPlayer = Vector3.Distance(Hypatios.Player.transform.position, transform.position);
        float f_valueChoosingPlayerAllies = Mathf.Clamp(distPlayer * 0.03f, 0.3f, 0.9f); //distance is 20 then 0.6, distance is 33 then 1 (limit)
        if (favorPlayer > 0f)
        {
            f_valueChoosingPlayerAllies = favorPlayer;
        }

        if (overrideAlliance == Alliance.Null)
        {
            currentTarget = Hypatios.Enemy.FindEnemyEntity(Stats.MainAlliance, transform.position, chanceSelectAlly: f_valueChoosingPlayerAllies, maxDistance: maxDistance, enemySelf: this);
        }


        if (currentTarget != null && Stats.MainAlliance != Alliance.Player)
        {
            float distAlly = Vector3.Distance(currentTarget.transform.position, Hypatios.Player.transform.position);
            //if nearest ally distance to the player is over 20 then just target the player
            if (distAlly > thresholdNearestAllyDist)
            {
                currentTarget = Hypatios.Player;
            }
        }
        if (overrideAlliance != Alliance.Null)
        {
            currentTarget = Hypatios.Enemy.FindEnemyEntity(overrideAlliance, transform.position, chanceSelectAlly: f_valueChoosingPlayerAllies, maxDistance: maxDistance, enemySelf: this);
        }
    }


    //Must run this in update!
    public virtual void AI_Detection()
    {
        if (Time.timeScale <= 0) return;
        var posOffsetLook = currentTarget.OffsetedBoundWorldPosition;
        float distance = 9999f;//Vector3.Distance(eyeLocation.transform.position, currentTarget.OffsetedBoundWorldPosition) + 10f;
        Debug.DrawLine(eyeLocation.transform.position, posOffsetLook, Color.cyan);
        _canLookAtTarget = false;

        if (hasSeenPlayer)
        {
            if (_hasTriggeredDetection == false)
            {
                OnPlayerDetected?.Invoke();
                OnPlayerDetected1?.Invoke();
            }
            _hasTriggeredDetection = true;
        }

        if (!Stats.IsDead)
        {
            var mainDir = (posOffsetLook - eyeLocation.transform.position).normalized;
            if (Physics.Raycast(eyeLocation.transform.position, mainDir, out RaycastHit hit, distance, Hypatios.Enemy.baseDetectionLayer))
            {
                _hitDetection = hit;
                bool firstPassSucceed = false;
                bool secondPassSucceed = false;

                if (hit.transform.tag == "Player" | Hypatios.Enemy.CheckTransformIsAnEnemy(hit.transform, Stats.MainAlliance))
                {
                    firstPassSucceed = true;
                }

                var inverseDir = (eyeLocation.transform.position - hit.point).normalized;

                //secondpass
                RaycastHit secondHit;
                if (Physics.Raycast(hit.point + inverseDir, inverseDir, out secondHit, hit.distance + 1, Hypatios.Enemy.baseDetectionLayer, QueryTriggerInteraction.Ignore))
                {
                   
                }
                else
                {
                    secondHit.point = hit.point + (inverseDir * (hit.distance + 1f));
                }
                if (debug_FirstPassHit != null)
                {
                    debug_FirstPassHit.transform.position = hit.point + inverseDir;
                    debug_FirstPassHit.transform.rotation = Quaternion.LookRotation(inverseDir, Vector3.up);
                }
                Debug.DrawLine(eyeLocation.transform.position, hit.point, Color.red);
                Debug.DrawLine(hit.point, secondHit.point, Color.red);

                var secondDir = (hit.point - secondHit.point).normalized;
                float limitThreshold = 3f;
                float dist = Vector3.Distance(secondHit.point, eyeLocation.transform.position);
                if (debug_SecondPassHit != null)
                {
                    if (secondHit.collider != null)
                    {
                        debug_SecondPassHit.transform.position = secondHit.point;
                        debug_SecondPassHit.transform.rotation = Quaternion.LookRotation(secondDir, Vector3.up);
                    }
                    else
                    {
                        debug_SecondPassHit.transform.position = new Vector3(-999f, -999f, -9999f);
                    }
                }
                if (dist <= limitThreshold) secondPassSucceed = true;

                if (firstPassSucceed && secondPassSucceed)
                {            
                    hasSeenPlayer = true;
                    _canLookAtTarget = true;
                    Debug.DrawLine(eyeLocation.transform.position, posOffsetLook - eyeLocation.transform.position, Color.grey);
                }
            }
          }

        if (_canLookAtTarget == false)
        {
            _lastTimeSeenPlayer += Time.deltaTime;
        }
        else
        {
            _lastTimeSeenPlayer = 0f;
        }
    }

    #endregion


    public virtual void Attacked(DamageToken token)
    {

    }

    //Currently only implemented in level 7
    public virtual void OnCreated()
    {

    }



}
