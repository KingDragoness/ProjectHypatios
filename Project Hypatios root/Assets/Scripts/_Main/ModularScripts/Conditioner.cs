using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

// "NOT" HAS NO FUCKING USE
public class Conditioner : MonoBehaviour
{
    public enum ConditionType
    {
        ChamberVisit = 0, //Chamber visit
        ChamberCompleted, //Chamber completed
        ParadoxEventChecked, //Paradox event flag
        TriviaCompleted = 20, //Trivia complete check
        Randomization, //Randomize 0f-1f
        EnemyClearance,
        DistanceCheck,
        ConditionerCheck,
        Switch,
        TriviaNotCompleted,
        Flags,
        SwitchNotCompleted,
        ItemOwned = 100
    }

    public enum FulfillCondition
    {
        OR,
        NOT,
        AND
    }

    [System.Serializable]
    public class ConditionEvent
    {
        public ConditionType conditionType;
        [ShowIf(nameof(IsTriviaConditioner))] public Trivia trivia;
        [ShowIf(nameof(IsSwitchConditioner))] public SwitchConditioner switchCondition;
        [ShowIf("conditionType", ConditionType.ParadoxEventChecked)] public string eventKeyName;
        [ShowIf("conditionType", ConditionType.ChamberCompleted)] public int chamberAtLeastComplete = 2;
        [ShowIf("conditionType", ConditionType.ChamberCompleted)] public int chamberAtMostComplete = 9999;
        [ShowIf("conditionType", ConditionType.Randomization)] [Range(0f,1f)] public float randomChance = 0.1f;
        [ShowIf("conditionType", ConditionType.EnemyClearance)] public List<EnemyScript> allEnemiesToClear = new List<EnemyScript>();
        [ShowIf("conditionType", ConditionType.DistanceCheck)] public Transform t1;
        [ShowIf("conditionType", ConditionType.DistanceCheck)] public Transform t2;
        [ShowIf("conditionType", ConditionType.DistanceCheck)] public float distMin = 5f;
        [ShowIf("conditionType", ConditionType.DistanceCheck)] public float distMax = 20f;
        [ShowIf("conditionType", ConditionType.ConditionerCheck)] public Conditioner conditioner;
        [ShowIf("conditionType", ConditionType.ConditionerCheck)] public bool dontTriggerConditioner;
        [ShowIf("conditionType", ConditionType.ItemOwned)] public ItemInventory itemNeeded;
        [ShowIf("conditionType", ConditionType.ItemOwned)] public int itemCount = 1;
        [ShowIf("conditionType", ConditionType.Flags)] public GlobalFlagSO flagEvent;
        [ShowIf("conditionType", ConditionType.Flags)] public bool dontTriggerFlag;

        public bool IsTriviaConditioner => conditionType == ConditionType.TriviaCompleted || conditionType == ConditionType.TriviaNotCompleted;
        public bool IsSwitchConditioner => conditionType == ConditionType.Switch || conditionType == ConditionType.SwitchNotCompleted;

        private float STORE_FUCKING_RANDOM_NUMBER = -1f;

        public bool IsConditionChecked()
        {
            if (conditionType == ConditionType.TriviaCompleted)
            {
                return Hypatios.Game.Check_TriviaCompleted(trivia);
            }
            else if (conditionType == ConditionType.TriviaNotCompleted)
            {
                return !Hypatios.Game.Check_TriviaCompleted(trivia);
            }
            else if (conditionType == ConditionType.ParadoxEventChecked)
            {
                return Hypatios.Game.Check_ParadoxEvent(eventKeyName);
            }
            else if (conditionType == ConditionType.ChamberCompleted)
            {
                int i = Hypatios.Chamber.GetChamberCompletion();
                if (i >= chamberAtLeastComplete && i <= chamberAtMostComplete)
                    return true;

            }
            else if (conditionType == ConditionType.Randomization)
            {
                if (STORE_FUCKING_RANDOM_NUMBER == -1f) STORE_FUCKING_RANDOM_NUMBER = Hypatios.GetRandomChance();
                if (STORE_FUCKING_RANDOM_NUMBER < randomChance)
                    return true;

            }
            else if (conditionType == ConditionType.EnemyClearance)
            {
                allEnemiesToClear.RemoveAll(x => x == null);

                foreach(var enemy in allEnemiesToClear)
                {
                    if (enemy == null) continue;
                    if (enemy.Stats.IsDead == false) return false;
                }

                return true;
            }
            else if (conditionType == ConditionType.DistanceCheck)
            {
                float dist = Vector3.Distance(t1.position, t2.position);

                if (distMin < dist && distMax > dist)
                    return true;
            }
            else if (conditionType == ConditionType.ConditionerCheck)
            {
                if (conditioner.IsTriggered && dontTriggerConditioner == false)
                    return true;

                if (conditioner.IsTriggered == false && dontTriggerConditioner == true)
                    return true;
            }
            else if (conditionType == ConditionType.ItemOwned)
            {
                if (Hypatios.Player.Inventory.Count(itemNeeded) >= itemCount)
                   return true;
            }
            else if (conditionType == ConditionType.Switch)
            {
                if (switchCondition.Triggered)
                    return true;
            }
            else if (conditionType == ConditionType.SwitchNotCompleted)
            {
                if (switchCondition.Triggered == false)
                    return true;
            }
            else if (conditionType == ConditionType.Flags)
            {
                if (Hypatios.Game.Check_FlagTriggered(flagEvent.GetID()) && dontTriggerFlag == false)
                    return true;

                if (Hypatios.Game.Check_FlagTriggered(flagEvent.GetID()) == false && dontTriggerFlag == true)
                    return true;
            }

            return false;
        }
    }

    public string Title = "";
    public List<ConditionEvent> AllConditions = new List<ConditionEvent>();
    public FulfillCondition conditionForTrue;
    [FoldoutGroup("Events")] [ShowIf("isStartImmediately", true)] public UnityEvent OnTriggerStart;
    [FoldoutGroup("Events")] [ShowIf("isStartImmediately", true)] [InfoBox("OnTriggerFail: In prefab this is empty because easier debugging. Best used for scene objects.")] public UnityEvent OnTriggerFail;
    [FoldoutGroup("Events")] public UnityEvent OnTriggerFunction;
    [FoldoutGroup("Events")] public UnityEvent OnTriggerFunctionFailed;
    public bool isStartImmediately = false;
    public bool constantTriggerCheck = false;
    public bool printOutput = false;

    private bool _isTriggered = false;

    public bool IsTriggered { get => _isTriggered;  }

    private void Start()
    {
        if (isStartImmediately)
        {
            if (GetEvaluateResult())
            {
                _isTriggered = true;
                OnTriggerStart?.Invoke();
            }
            else
            {
                OnTriggerFail?.Invoke();
            }
        }
    }

    [Button("Sanity Check - Intro")]
    public void Test_SanityCheck_Intro()
    {
        if (GetEvaluateResult())
        {
            _isTriggered = true;
            OnTriggerStart?.Invoke();
        }
        else
        {
            OnTriggerFail?.Invoke();
        }
    }

    [Button("Sanity Check")]
    public void Test_SanityCheck()
    {
        TriggerFunction();
    }

    public void TriggerFunction()
    {
        if (GetEvaluateResult())
        {
            OnTriggerFunction?.Invoke();
            _isTriggered = true;
        }
        else
        {
            OnTriggerFunctionFailed?.Invoke();
        }
    }

    private void Update()
    {
        if (Time.timeScale <= 0) return;
        if (constantTriggerCheck == false) return;
        if (_isTriggered == true) return;

        TriggerFunction();

    }

    public bool GetEvaluateResult()
    {
        bool result = false;
        string output_str = $"{Title} ";


        if (conditionForTrue == FulfillCondition.OR)
        {
            result = false;

            foreach (var condition in AllConditions)
            {
                output_str += $" {condition.conditionType}";
                output_str += $": {condition.IsConditionChecked()}";

                if (condition.IsConditionChecked())
                {
                    result = true;
                    if (printOutput) Debug.Log($"{condition.conditionType}: Success.");
                    break;
                }

                output_str += $" | ";
            }
        }
        else if (conditionForTrue == FulfillCondition.AND)
        {
            result = true;

            foreach (var condition in AllConditions)
            {
                output_str += $" {condition.conditionType}";
                output_str += $": {condition.IsConditionChecked()}";

                if (condition.IsConditionChecked() == false)
                {
                    result = false;
                    if (printOutput) Debug.Log($"{condition.conditionType}: Fail.");
                    break;
                }

                output_str += $" | ";
            }

        }
        else if (conditionForTrue == FulfillCondition.NOT)
        {
            foreach (var condition in AllConditions)
            {
                output_str += $" {condition.conditionType}";
                output_str += $": {condition.IsConditionChecked()}";

                if (condition.IsConditionChecked() == true)
                {
                    result = false;
                    break;
                }

                output_str += $" | ";
            }

        }

        if (printOutput) Debug.Log($"{output_str} [FINAL: {result}]");
        return result;
    }
}
