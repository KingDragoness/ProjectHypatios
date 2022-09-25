using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public enum FW_Alliance
{
    INVADER,
    DEFENDER
}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(FW_Targetable))]

public abstract class Enemy_FW_Bot : Enemy
{

    public List<FortWar_AIModule> allAIModules = new List<FortWar_AIModule>();
    public FortWar_AIModule currentModule;
    public FW_Targetable myUnit;

    [FoldoutGroup("AI System")] public Transform target;

    private NavMeshAgent _agent;
    private Chamber_Level7 _chamberScript;

    public NavMeshAgent Agent { get => _agent; }

    public delegate void OnAITick();
    public delegate void OnAIFixedTick();
    public event OnAITick onAITick;
    public event OnAIFixedTick onAIFixedTick;
    private float _tickTimer = 0.1f;
    private int _tick = 0;
    private const float TICK_MAX = 0.1f;

    public virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _chamberScript = Chamber_Level7.instance;
    }

    public virtual void Start()
    {
        _chamberScript.RegisterUnit(myUnit);
    }

    public Transform GetCurrentTarget()
    {
        return target;
    }

    public virtual void Update()
    {
        if (Time.realtimeSinceStartup < 2f) return;
        if (currentModule != null) currentModule.Run();

        {
            _tickTimer -= Time.deltaTime;

            if (_tickTimer < 0)
            {
                _tick++;
                onAITick?.Invoke();
                _tickTimer = TICK_MAX;
            }
        }
    }

    public virtual void FixedUpdate()
    {
        {
           

        }
    }

  

}
