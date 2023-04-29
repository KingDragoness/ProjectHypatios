using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CurseRandomizerSystem : MonoBehaviour
{

    [FoldoutGroup("Ailment Stats")] public float timePlayerGetFatigue = 600f;
    [FoldoutGroup("Ailment Stats")] [Range(0f,1f)] public float chancePanicAttack = 0.1f;
    [FoldoutGroup("Ailment Stats")] [Range(0f, 1f)] public float chanceDepressionAttack = 0.02f;
    [FoldoutGroup("Ailment Stats")] [Range(0f, 1f)] public float chanceBurnDegree = 0.1f;
    [FoldoutGroup("Ailment Stats")] public int tickThreshold_Degree2 = 200;
    [FoldoutGroup("Ailment Stats")] public int tickThreshold_Degree3 = 500;
    [FoldoutGroup("Ailment Stats")] public int tickThreshold_Degree4 = 1000;
    [FoldoutGroup("Status Effects")] [SerializeField] private BaseStatusEffectObject depression;
    [FoldoutGroup("Status Effects")] [SerializeField] private BaseStatusEffectObject panicAttack;
    [FoldoutGroup("Status Effects")] [SerializeField] private BaseStatusEffectObject chamberFatigue;
    [FoldoutGroup("Status Effects")] [SerializeField] private BaseStatusEffectObject antiDepressant;
    [FoldoutGroup("Status Effects")] [SerializeField] private BaseStatusEffectObject fire_degree2;
    [FoldoutGroup("Status Effects")] [SerializeField] private BaseStatusEffectObject fire_degree3;
    [FoldoutGroup("Status Effects")] [SerializeField] private BaseStatusEffectObject fire_degree4;
    [SerializeField] private float refreshTime;

    private bool isAntiDepressant = false;
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
        if (Hypatios.Player.IsStatusEffectGroup(antiDepressant))
        {
            isAntiDepressant = true;
        }
        else isAntiDepressant = false;

        isFatigueChamber = Hypatios.Chamber.chamberObject.isCausingFatigue;
        _totalEnemiesSeePlayer = Hypatios.Enemy.CountEnemyHasSeenMe();
    }

    private void Update()
    {
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

    /// <summary>
    /// Depression only occurs after run 20. 2% per level transition
    /// </summary>
    private void CheckDepression()
    {
        if (Hypatios.Player.IsStatusEffectGroup(depression)) return;
        if (isAntiDepressant) return;
        if (Hypatios.Game.TotalRuns < 20) return;
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
        Hypatios.Game.RuntimeTutorialHelp("Diseases and Ailments", "At random times, Aldrich can suffer from Depression, Fatigue and Panic Attack ailments. To prevent you need to find and consume anti-depressant pills.", "curse.ailments");
    }

    [FoldoutGroup("DEBUG")] [Button("Add Fatigue")]
    public void Fatigue()
    {

        chamberFatigue.AddStatusEffectPlayer(3000f);
        DeadDialogue.PromptNotifyMessage_Mod("Aldrich suffers from chamber fatigue.", 4f);
        Hypatios.Game.RuntimeTutorialHelp("Diseases and Ailments", "At random times, Aldrich can suffer from Depression, Fatigue and Panic Attack ailments. To prevent you need to find and consume anti-depressant pills.", "curse.ailments");
    }

    [FoldoutGroup("DEBUG")]
    [Button("Add Panic")]
    public void Panic()
    {

        panicAttack.AddStatusEffectPlayer(25f);
        DeadDialogue.PromptNotifyMessage_Mod("Aldrich suffers from panic attack.", 4f);
        Hypatios.Game.RuntimeTutorialHelp("Diseases and Ailments", "At random times, Aldrich can suffer from Depression, Fatigue and Panic Attack ailments. To prevent you need to find and consume anti-depressant pills.", "curse.ailments");
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
