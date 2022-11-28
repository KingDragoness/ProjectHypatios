using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;



public abstract class EnemyScript : Entity
{

    [FoldoutGroup("Base")] [HideInEditorMode] [ShowInInspector] private EnemyStats _stats;
    [FoldoutGroup("Base")] [HideInPlayMode] [SerializeField] [InlineEditor] private BaseEnemyStats _baseStat;
    public EnemyStats Stats { get => _stats; }
    public string EnemyName { get => _baseStat.name; }
    [FoldoutGroup("AI")] [ShowInInspector] [ReadOnly] internal Entity currentTarget;
    [FoldoutGroup("AI")] [ShowInInspector] [ReadOnly] internal bool hasSeenPlayer = false;
    [FoldoutGroup("AI")] [ShowInInspector] [ReadOnly] internal bool canLookAtTarget = false;
    [FoldoutGroup("AI")] [SerializeField] internal Transform eyeLocation;

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

    public virtual void Hack()
    {
        Stats.MainAlliance = Alliance.Player;
        ScanForEnemies();
    }

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

    private void Died()
    {
        Hypatios.Enemy.OnEnemyDied?.Invoke(this);
        Stats.IsDead = true;
    }

    [FoldoutGroup("Debug")]
    [Button("Enforce scan target")]
    public virtual void ScanForEnemies()
    {
        float distPlayer = Vector3.Distance(Hypatios.Player.transform.position, transform.position);
        float f_valueChoosingPlayerAllies = Mathf.Clamp(distPlayer * 0.03f, 0.3f, 0.9f); //distance is 20 then 0.6, distance is 33 then 1 (limit)
        currentTarget = Hypatios.Enemy.FindEnemyEntity(Stats.MainAlliance, transform.position, chanceSelectAlly: f_valueChoosingPlayerAllies);
    }

    public virtual void AI_Detection()
    {
        var posOffsetLook = currentTarget.OffsetedBoundPosition;
        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        Debug.DrawLine(transform.position, posOffsetLook);
        canLookAtTarget = false;

        if (!Stats.IsDead)
        {

            if (Physics.Raycast(eyeLocation.transform.position, posOffsetLook - eyeLocation.transform.position, out RaycastHit hit, distance))
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


    public virtual void Attacked(DamageToken token)
    {

    }

    //Currently only implemented in level 7
    public virtual void OnCreated()
    {

    }



}
