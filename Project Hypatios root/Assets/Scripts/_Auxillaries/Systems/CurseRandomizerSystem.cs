﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CurseRandomizerSystem : MonoBehaviour
{

    public int TriggerAfterRun = 5;
    [FoldoutGroup("Ailment Stats")] public float timePlayerGetFatigue = 600f;
    [FoldoutGroup("Ailment Stats")] public float unixTime_GetStagnationRage = 1500f; //25 minutes
    [FoldoutGroup("Ailment Stats")] [Range(0f, 1f)] public float chanceStagnationRage = 0.05f;
    [FoldoutGroup("Ailment Stats")] [Range(0f,1f)] public float chancePanicAttack = 0.1f;
    [FoldoutGroup("Ailment Stats")] [SerializeField] private float chanceDepressionAttack = 0.02f; //Now handled by a script
    [FoldoutGroup("Ailment Stats")] [Range(0f, 1f)] public float chanceBurnDegree = 0.1f;
    [FoldoutGroup("Ailment Stats")] public int tickThreshold_Degree2 = 200;
    [FoldoutGroup("Ailment Stats")] public int tickThreshold_Degree3 = 500;
    [FoldoutGroup("Ailment Stats")] public int tickThreshold_Degree4 = 1000;
    [FoldoutGroup("Status Effects")] [SerializeField] private BaseStatusEffectObject depression;
    [FoldoutGroup("Status Effects")] [SerializeField] private BaseStatusEffectObject panicAttack;
    [FoldoutGroup("Status Effects")] [SerializeField] private BaseStatusEffectObject chamberFatigue;
    [FoldoutGroup("Status Effects")] [SerializeField] private BaseStatusEffectObject antiDepressant;
    [FoldoutGroup("Status Effects")] [SerializeField] private BaseStatusEffectObject stagnationRage;
    [FoldoutGroup("Status Effects")] [SerializeField] private BaseStatusEffectObject fireRetardant;
    [FoldoutGroup("Status Effects")] [SerializeField] private BaseStatusEffectObject fire_degree2;
    [FoldoutGroup("Status Effects")] [SerializeField] private BaseStatusEffectObject fire_degree3;
    [FoldoutGroup("Status Effects")] [SerializeField] private BaseStatusEffectObject fire_degree4;
    [SerializeField] private float refreshTime;

    private bool isAntiDepressant = false;
    private bool isFireRetardant = false;
    private bool isFatigueChamber = false;
    private int _ticksPlayerGotBurned = 0;
    private float _timerRandomizer = 1f;
    private int _tick = 0;
    private int _totalEnemiesSeePlayer = 0;
    [FoldoutGroup("DEBUG")] [SerializeField] private float _timeInChamber = 0f;


    private void Start()
    {
        CheckPlayerCondition();
        CheckDepression();
    }

    private void CheckPlayerCondition()
    {
        isAntiDepressant = false;
        isFireRetardant = false;
        if (Hypatios.Player.IsStatusEffectGroup(antiDepressant)) isAntiDepressant = true;
        if (Hypatios.Player.IsStatusEffectGroup(fireRetardant)) isFireRetardant = true;


        isFatigueChamber = Hypatios.Chamber.chamberObject.isCausingFatigue;
        _totalEnemiesSeePlayer = Hypatios.Enemy.CountEnemyHasSeenMe();
    }

    private void Update()
    {
        int currentRun = Hypatios.Game.TotalRuns;

        if (currentRun < TriggerAfterRun)
        {
            return;
        }

        _timeInChamber += Time.deltaTime;
        if (_timerRandomizer > 0f)
        {
            _timerRandomizer -= Time.deltaTime;
            return;
        }
        CheckPlayerCondition();
        _tick++;
        _timerRandomizer = refreshTime;

        CheckFatigue();
        CheckBurning();
        CheckPanic();
        CheckRage();
    }

    #region Ailments
    private void CheckFatigue()
    {
        if (Hypatios.Player.IsStatusEffectGroup(chamberFatigue)) return;
        if (isFatigueChamber == false) return;
        if (isAntiDepressant) return;
        if (timePlayerGetFatigue > _timeInChamber) return;

        Fatigue();
    }

    private void CheckRage()
    {
        if (Hypatios.Player.IsStatusEffectGroup(stagnationRage)) return;
        if (isAntiDepressant) return;
        if (unixTime_GetStagnationRage > Hypatios.Game.UNIX_Timespan) return;
        if (_tick % 10 != 0) return;
        if (_timeInChamber < 60) return;
        float c = Random.Range(0f, 1f);
        if (c >= chanceStagnationRage) return;

        Rage();

    }

    /// <summary>
    /// Depression only occurs after run 10. 
    /// 10-50 runs: +0.2% per death
    /// 50-200 runs: +0.3%/death + 8%
    /// </summary>
    /// 
    [FoldoutGroup("DEBUG")] [Button("Sanity Check - Depression")]
    private void CheckDepression()
    {
        if (Hypatios.Player.IsStatusEffectGroup(depression)) return;
        if (isAntiDepressant) return;
        if (Hypatios.Game.TotalRuns < 10) return;

        {
            float baseChance = 0f;

            if (Hypatios.Game.TotalRuns < 50)
            {
                baseChance += 0.2f * (Hypatios.Game.TotalRuns - 10f) * 0.01f;
            }
            if (Hypatios.Game.TotalRuns >= 50)
            {
                baseChance += 0.3f * (Hypatios.Game.TotalRuns - 50f) * 0.01f;
            }
            if (baseChance >= 0.5f) baseChance = 0.5f;

            chanceDepressionAttack = baseChance;
        }

        float c = Random.Range(0f, 1f);
        if (c >= chanceDepressionAttack) return;

        Depression();
    }


    /// <summary>
    /// Panic only 60 seconds after level started and 
    /// at least 1 enemy sees the player. Check every 10s
    /// </summary>
    private void CheckPanic()
    {
        if (_tick % 10 != 0) return;
        if (Hypatios.Player.IsStatusEffectGroup(panicAttack)) return;
        if (isAntiDepressant) return;
        if (_timeInChamber < 60) return;
        if (_totalEnemiesSeePlayer <= 0) return;
        float c = Random.Range(0f, 1f);
        if (c >= chancePanicAttack) return;

        Panic();
    }

    private void CheckBurning()
    {
        if (Hypatios.Player.IsStatusEffect(ModifierEffectCategory.Fire) == false) return;
        if (isFireRetardant) return;
        float chance1 = Random.Range(0f, 1f);

        if (chance1 < chanceBurnDegree)
        {
            if (_ticksPlayerGotBurned > tickThreshold_Degree2 && Hypatios.Player.IsStatusEffectGroup(fire_degree2) == false)
            {
                BurningInjury(2);
            }
            if (_ticksPlayerGotBurned > tickThreshold_Degree3 && Hypatios.Player.IsStatusEffectGroup(fire_degree3) == false)
            {
                BurningInjury(3);
            }
            if (_ticksPlayerGotBurned > tickThreshold_Degree4 && Hypatios.Player.IsStatusEffectGroup(fire_degree4) == false)
            {
                BurningInjury(4);
            }
        }

        _ticksPlayerGotBurned++;

    }

    [FoldoutGroup("DEBUG")] [Button("Add Depression")]
    public void Depression()
    {
        depression.AddStatusEffectPlayer(9999f);
        DeadDialogue.PromptNotifyMessage_Mod("Aldrich suffers from depression.", 4f);
        Prompt_Tutorial();
    }

    [FoldoutGroup("DEBUG")] [Button("Add Fatigue")]
    public void Fatigue()
    {

        chamberFatigue.AddStatusEffectPlayer(3000f);
        DeadDialogue.PromptNotifyMessage_Mod("Aldrich suffers from chamber fatigue.", 4f);
        Prompt_Tutorial();
    }

    [FoldoutGroup("DEBUG")]
    [Button("Add Panic")]
    public void Panic()
    {
        panicAttack.AddStatusEffectPlayer(25f);
        DeadDialogue.PromptNotifyMessage_Mod("Aldrich suffers from panic attack.", 4f);
        Prompt_Tutorial();

    }

    [FoldoutGroup("DEBUG")]
    [Button("Add Rage")]
    public void Rage()
    {
        stagnationRage.AddStatusEffectPlayer(240f); //4 minutes
        DeadDialogue.PromptNotifyMessage_Mod("Aldrich becomes enraged with the lack of progression in the current, gaining 'Stagnation Rage' ailment.", 4f);
        Prompt_Tutorial();
    }

    private void Prompt_Tutorial()
    {
        Hypatios.Game.RuntimeTutorialHelp("Diseases and Ailments", "At random times, Aldrich can suffer from Depression, Fatigue, Rage and Panic Attack ailments. To prevent you need to find and consume anti-depressant pills.", "curse.ailments");

    }

    /// <summary>
    /// Set player status effect burning degree
    /// </summary>
    /// <param name="degree">2, 3, 4</param>
    [FoldoutGroup("DEBUG")]
    [Button("Add burning injury")]

    public void BurningInjury(int degree = 2)
    {
        if (degree == 2)
        {
            fire_degree2.AddStatusEffectPlayer(9999f);
            DeadDialogue.PromptNotifyMessage_Mod("Aldrich suffers from second-degree burn injury.", 4f);
        }
        else if (degree == 3)
        {
            fire_degree3.AddStatusEffectPlayer(9999f);
            DeadDialogue.PromptNotifyMessage_Mod("Aldrich suffers from third-degree burn injury.", 4f);
        }
        else if (degree == 4)
        {
            fire_degree4.AddStatusEffectPlayer(9999f);
            DeadDialogue.PromptNotifyMessage_Mod("Aldrich suffers from fourth-degree burn injury.", 4f);
        }

        Hypatios.Game.RuntimeTutorialHelp("Burn Injury", "Despite Aldrich's high endurance and damage resistance, fire can cause severe injury to Aldrich. Try not to get on fire too long.", "curse.burninjurydegree");
    }

    #endregion

}
