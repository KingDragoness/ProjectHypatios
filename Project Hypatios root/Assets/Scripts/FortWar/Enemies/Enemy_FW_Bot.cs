using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using UnityEngine.Events;



[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(FW_Targetable))]

public abstract class Enemy_FW_Bot : Enemy
{

    public List<FortWar_AIModule> allAIModules = new List<FortWar_AIModule>();
    public FortWar_AIModule currentModule;
    public FW_Targetable myUnit;

    [FoldoutGroup("Base")] public float currentHitpoint = 263;
    [FoldoutGroup("Base")] public UnityEvent OnBotKilled;
    [FoldoutGroup("Base")] public UnityEvent OnPlayerKill;
    [FoldoutGroup("Base")] public GameObject botCorpse;
    [FoldoutGroup("AI System")] public Transform target;

    private NavMeshAgent _agent;
    protected Chamber_Level7 _chamberScript;

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
        if (_chamberScript.currentStage != Chamber_Level7.Stage.Ongoing) return;
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

    public override void Attacked(DamageToken token)
    {
        currentHitpoint -= token.damage;
        if (token.origin == DamageToken.DamageOrigin.Player) DamageOutputterUI.instance.DisplayText(token.damage);

        if (currentHitpoint < 0)
        {
            Die();

            if (token.origin == DamageToken.DamageOrigin.Player)
            {
                OnPlayerKill?.Invoke();
            }
        }

        base.Attacked(token);

    }

    public virtual void Die()
    {
        _chamberScript.DeregisterUnit(myUnit);

        if (botCorpse)
        {
            var corpse1 = Instantiate(botCorpse, transform.position, transform.rotation);
            corpse1.gameObject.SetActive(true);
        }
        Destroy(gameObject);
    }



}
