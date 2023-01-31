using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Sirenix.OdinInspector;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(FW_Targetable))]

public abstract class Enemy_FW_Bot : EnemyScript
{

    public List<FortWar_AIModule> allAIModules = new List<FortWar_AIModule>();
    public FortWar_AIModule currentModule;
    public FW_Targetable myUnit;

    [FoldoutGroup("Base")] public UnityEvent OnBotKilled;
    [FoldoutGroup("Base")] public UnityEvent OnPlayerKill;
    [FoldoutGroup("Base")] public GameObject botCorpse;
    [FoldoutGroup("AI System")] public Transform target;

    [SerializeField] private NavMeshAgent _agent;
    protected Chamber_Level7 _chamberScript;

    public NavMeshAgent Agent { get => _agent; }

    public delegate void OnAITick();
    public delegate void OnAIFixedTick();
    public event OnAITick onAITick;
    public event OnAIFixedTick onAIFixedTick;
    private float _tickTimer = 0.1f;
    private int _tick = 0;
    private const float TICK_MAX = 0.1f;


    private void OnValidate()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public virtual void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _chamberScript = Chamber_Level7.instance;
        _chamberScript.RegisterUnit(myUnit);
    }

    public Transform GetCurrentTarget()
    {
        return target;
    }

    public virtual void Update()
    {
        if (Time.timeScale == 0) return;
        if (Time.realtimeSinceStartup < 2f) return;
        if (isAIEnabled == false) return;
        if (_chamberScript.currentStage != Chamber_Level7.Stage.Ongoing) return;
        if (currentModule != null) currentModule.Run();

        {
            _tickTimer -= Time.deltaTime;

            if (_tickTimer < 0)
            {
                _tick++;
                onAITick?.Invoke();
                _tickTimer = TICK_MAX + Random.Range(0,0.1f);
            }
        }
    }

    public async void AI_Tick()
    {

    }

    public virtual void FixedUpdate()
    {
        {
           

        }
    }

    public override void Attacked(DamageToken token)
    {
        Stats.CurrentHitpoint -= token.damage;
        if (!Stats.IsDead && token.origin == DamageToken.DamageOrigin.Player)
            DamageOutputterUI.instance.DisplayText(token.damage);

        if (Stats.CurrentHitpoint < 0)
        {
            Die();

            if (token.origin == DamageToken.DamageOrigin.Player)
            {
                OnPlayerKill?.Invoke();
            }
        }

        base.Attacked(token);

    }

    public override void Die()
    {
        _chamberScript.DeregisterUnit(myUnit);

        if (botCorpse)
        {
            var corpse1 = Instantiate(botCorpse, transform.position, transform.rotation);
            corpse1.gameObject.SetActive(true);
        }
        OnDied?.Invoke();
        Destroy(gameObject);
        Stats.IsDead = true;
    }



}
