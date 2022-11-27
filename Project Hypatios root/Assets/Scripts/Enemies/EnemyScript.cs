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

    public System.Action OnDied;

    public virtual void Awake()
    {
        Hypatios.Enemy.RegisterEnemy(this);
        _stats = _baseStat.Stats.Clone();
        _stats.Initialize();
        OnDied += Died;

    }
    public virtual void OnDestroy()
    {
        Hypatios.Enemy.DeregisterEnemy(this);
    }


    private void Died()
    {
        Hypatios.Enemy.OnEnemyDied?.Invoke(this);
    }

    public virtual void Attacked(DamageToken token)
    {

    }

    //Currently only implemented in level 7
    public virtual void OnCreated()
    {

    }



}
