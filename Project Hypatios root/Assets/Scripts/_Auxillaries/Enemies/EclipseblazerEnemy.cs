using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System.Linq;
using Animancer;

public class EclipseblazerEnemy : EnemyScript
{

    public enum Stance
    {
        Introduction,
        Idle,
        Patrol,
        Deathsweeper = 10,
        Chain_Attack,
        Sword_Attack,
        Mirror_Attack,
        Sunken_Spear, //if there is time, merely just copying mirror attack's AI
        GroundStomp
    }


    [FoldoutGroup("Base Parameters")] public float healingSpeed = 5f;
    [FoldoutGroup("Base Parameters")] public float flyingSpeed = 15f;
    [FoldoutGroup("Eclipseblazer")] public List<EclipseBlaz_AIModule> all_AIModules = new List<EclipseBlaz_AIModule>();
    [FoldoutGroup("Eclipseblazer")] public Stance currentStance;
    [FoldoutGroup("References")] public Animator animator;
    [FoldoutGroup("References")] public AnimancerPlayer AnimatorPlayer;

    private Stance prevStance;
    private bool _bossFightStarted = false;
    private float _timeToChangeDecision = 1f;

    public override void Awake()
    {
        base.Awake();

        foreach(var module in all_AIModules)
        {
            module.eclipseblazer = this;
        }
    }

    private void Update()
    {
        if (Time.timeScale <= 0) return;

        RegenerateHealth();
        UpdateAI();

        if (_bossFightStarted)
        {
            DecisionMaking();
        }
    }

    [FoldoutGroup("DEBUG")] [Button("DEBUG_TriggerBossFight")]
    public void TriggerBossFight()
    {
        if (_bossFightStarted) return;
        _bossFightStarted = true;
        currentStance = Stance.Idle;
    }

    [FoldoutGroup("DEBUG")]
    [Button("DEBUG_Modify_DecisionTime")]
    public void Modify_DecisionTime(float time)
    {
        _timeToChangeDecision = time;
    }

    #region AI

    private void DecisionMaking()
    {
        _timeToChangeDecision -= Time.deltaTime;

        if (_timeToChangeDecision <= 0f)
        {
            var module = Get_RandomAIModule();
            currentStance = module.stance;

            _timeToChangeDecision = module.minimumDuration;
        }
    }

    private void UpdateAI()
    {
        var prevStanceClass = GetModule(prevStance);
        var currentStanceClass = GetModule(currentStance);

        if (prevStance != currentStance)
        {
            JustEnterStance();
            if (prevStanceClass != null) prevStanceClass.OnExitState();
        }

        if (currentStanceClass != null)
        {
            currentStanceClass.Run();
        }

        prevStance = currentStance;
    }

    private void JustEnterStance()
    {
        var newStance = GetModule(currentStance);

        if (newStance == null) return;
        newStance.OnEnterAnimation(AnimatorPlayer);
        newStance.OnEnterState();

        if (newStance.objectToSpawn != null)
        {
            var newObject = Instantiate(newStance.objectToSpawn);
            newObject.gameObject.SetActive(true);

        }
    }

    public EclipseBlaz_AIModule Get_RandomAIModule()
    {
        int output = 0;

        var totalWeight = GetTotalWeight();
        int rndWeightValue = Random.Range(1, totalWeight + 1);

        //Checking where random weight value falls
        var processedWeight = 0;
        int index1 = 0;
        foreach (var entry in all_AIModules)
        {
            if (entry.ignoreSelection) continue;

            processedWeight += entry.GetWeight();
            if (rndWeightValue <= processedWeight)
            {
                output = index1;
                break;
            }
            index1++;
        }

        return all_AIModules[output];
    }

    private int GetTotalWeight()
    {
        int result = 0;

        foreach (var entry1 in all_AIModules)
        {
            if (entry1.ignoreSelection) continue;
            result += entry1.GetWeight();
        }


        return result;
    }

    #endregion

    private EclipseBlaz_AIModule GetModule (Stance _targetedStance)
    {
        return all_AIModules.Find(x => x.stance == _targetedStance);
    }

    public void RegenerateHealth()
    {
        if (Stats.CurrentHitpoint < Stats.MaxHitpoint.Value)
        {
            Stats.CurrentHitpoint += Time.deltaTime * healingSpeed;
        }
        
        if (Stats.CurrentHitpoint < -1f)
        {
            Stats.CurrentHitpoint = 100f;
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
        //impossible to die.
    }
}
