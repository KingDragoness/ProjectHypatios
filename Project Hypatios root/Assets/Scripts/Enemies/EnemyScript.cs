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


    public EnemyStats Stats { get => _stats; }
    public string EnemyName { get => _baseStat.name; }

    [FoldoutGroup("AI")] [ShowInInspector] [ReadOnly] internal Entity currentTarget;
    [FoldoutGroup("AI")] [ShowInInspector] public bool hasSeenPlayer = false;
    [FoldoutGroup("AI")] [ShowInInspector] [ReadOnly] internal bool canLookAtTarget = false;
    [FoldoutGroup("AI")] [ShowInInspector] [SerializeField] internal bool isAIEnabled = true;
    [FoldoutGroup("AI")] [SerializeField] internal Transform eyeLocation;

    internal DamageToken _lastDamageToken;

    public System.Action OnDied;
    public System.Action OnPlayerDetected;

    public virtual void Awake()
    {
        Hypatios.Enemy.RegisterEnemy(this);
        if (eyeLocation == null) eyeLocation = transform;
        _stats = _baseStat.Stats.Clone();
        _stats.Initialize();
        OnDied += Died;

    }
    public virtual void OnDestroy()
    {
        Hypatios.Enemy.DeregisterEnemy(this);
    }


    #region Status

    [FoldoutGroup("Debug")]
    [Button("Paralyze")]
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

    [FoldoutGroup("Debug")]
    [Button("Deparalyze")]
    /// <summary>
    /// Disables enemy's AI.
    /// </summary>
    public virtual void Deparalyze() { isAIEnabled = true; }

    [FoldoutGroup("Debug")]
    [Button("Hack")]
    /// <summary>
    /// Turn enemy's faction to Player.
    /// </summary>
    public virtual void Hack()
    {
        Stats.MainAlliance = Alliance.Player;
        ScanForEnemies();
    }

    [FoldoutGroup("Debug")]
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
        return AllStatusInEffect.Find(x => x.statusCategoryType == StatusEffectCategory.Fire);
    }
    public bool IsPoisoned()
    {
        return AllStatusInEffect.Find(x => x.statusCategoryType == StatusEffectCategory.Poison);
    }
    public bool IsParalyzed()
    {
        return !isAIEnabled;
    }

    #endregion

    #region AI

    [FoldoutGroup("Debug")]
    [Button("Enforce scan target")]
    public virtual void ScanForEnemies(float favorPlayer = 0f, float maxDistance = 1000f, float thresholdNearestAllyDist = 20f)
    {
        float distPlayer = Vector3.Distance(Hypatios.Player.transform.position, transform.position);
        float f_valueChoosingPlayerAllies = Mathf.Clamp(distPlayer * 0.03f, 0.3f, 0.9f); //distance is 20 then 0.6, distance is 33 then 1 (limit)
        if (favorPlayer > 0f)
        {
            f_valueChoosingPlayerAllies = favorPlayer;
        }

        currentTarget = Hypatios.Enemy.FindEnemyEntity(Stats.MainAlliance, transform.position, chanceSelectAlly: f_valueChoosingPlayerAllies, maxDistance: maxDistance);

        if (currentTarget != null)
        {
            float distAlly = Vector3.Distance(currentTarget.transform.position, Hypatios.Player.transform.position);
            //if nearest ally distance to the player is over 20 then just target the player
            if (distAlly > thresholdNearestAllyDist)
            {
                currentTarget = Hypatios.Player;
            }
        }
    }


    public virtual void AI_Detection()
    {
        var posOffsetLook = currentTarget.OffsetedBoundWorldPosition;
        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        Debug.DrawLine(transform.position, posOffsetLook);
        canLookAtTarget = false;

        if (!Stats.IsDead)
        {

            if (Physics.Raycast(eyeLocation.transform.position, posOffsetLook - eyeLocation.transform.position, out RaycastHit hit, distance, Hypatios.Enemy.baseDetectionLayer))
            {
                if (hit.transform.tag == "Player" | Hypatios.Enemy.CheckTransformIsAnEnemy(hit.transform, Stats.MainAlliance))
                {
                    if (hasSeenPlayer == false) OnPlayerDetected?.Invoke();
                    hasSeenPlayer = true;
                    canLookAtTarget = true;
                    Debug.DrawLine(eyeLocation.transform.position, posOffsetLook - eyeLocation.transform.position);
                }


            }
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
