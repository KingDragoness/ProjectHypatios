using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CurseRandomizerSystem : MonoBehaviour
{

    [FoldoutGroup("Ailment Stats")] public float timePlayerGetFatigue = 600f;
    [FoldoutGroup("Ailment Stats")] [Range(0f,1f)] public float chancePanicAttack = 0.1f;
    [FoldoutGroup("Ailment Stats")] [Range(0f, 1f)] public float chanceDepressionAttack = 0.02f;
    [SerializeField] private BaseStatusEffectObject depression;
    [SerializeField] private BaseStatusEffectObject panicAttack;
    [SerializeField] private BaseStatusEffectObject chamberFatigue;
    [SerializeField] private BaseStatusEffectObject antiDepressant;
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

        _ticksPlayerGotBurned++;

    }

    [FoldoutGroup("DEBUG")] [Button("Add Depression")]
    public void Depression()
    {
        depression.AddStatusEffectPlayer(9999f);
        DeadDialogue.PromptNotifyMessage_Mod("Aldrich suffers from depression.", 4f);
    }

    [FoldoutGroup("DEBUG")] [Button("Add Fatigue")]
    public void Fatigue()
    {

        chamberFatigue.AddStatusEffectPlayer(3000f);
        DeadDialogue.PromptNotifyMessage_Mod("Aldrich suffers from chamber fatigue.", 4f);
    }

    [FoldoutGroup("DEBUG")]
    [Button("Add Panic")]
    public void Panic()
    {

        panicAttack.AddStatusEffectPlayer(25f);
        DeadDialogue.PromptNotifyMessage_Mod("Aldrich suffers from panic attack.", 4f);
    }



    #endregion

}
