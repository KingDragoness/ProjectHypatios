using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class Conditioner : MonoBehaviour
{
    public enum ConditionType
    {
        ChamberVisit = 0, //Chamber visit
        ChamberCompleted, //Chamber completed
        ParadoxEventChecked, //Paradox event flag
        TriviaCompleted = 20, //Trivia complete check
        Randomization //Randomize 0f-1f
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
        [ShowIf("conditionType", ConditionType.TriviaCompleted)] public Trivia trivia;
        [ShowIf("conditionType", ConditionType.ParadoxEventChecked)] public string eventKeyName;
        [ShowIf("conditionType", ConditionType.ChamberCompleted)] public int chamberAtLeastComplete = 2;
        [ShowIf("conditionType", ConditionType.ChamberCompleted)] public int chamberAtMostComplete = 9999;
        [ShowIf("conditionType", ConditionType.Randomization)] [Range(0f,1f)] public float randomChance = 0.1f;

        public bool IsConditionChecked()
        {
            if (conditionType == ConditionType.TriviaCompleted)
            {
                return Hypatios.Game.Check_TriviaCompleted(trivia);
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
                float random = Hypatios.GetRandomChance();
                if (random < randomChance)
                    return true;

            }

            return false;
        }
    }

    public string Title = "";
    public List<ConditionEvent> AllConditions = new List<ConditionEvent>();
    public FulfillCondition conditionForTrue;
    [ShowIf("isStartImmediately", true)] public UnityEvent OnTriggerStart;
    [ShowIf("isStartImmediately", true)] [InfoBox("In prefab this is empty because easier debugging. Best used for scene objects.")] public UnityEvent OnTriggerFail;
    public bool isStartImmediately = false;

    private void Start()
    {
        if (isStartImmediately)
        {
            if (GetEvaluateResult())
            {
                OnTriggerStart?.Invoke();
            }
            else
            {
                OnTriggerFail?.Invoke();
            }
        }
    }

    public bool GetEvaluateResult()
    {
        if (conditionForTrue == FulfillCondition.OR)
        {
            foreach(var condition in AllConditions)
            {
                if (condition.IsConditionChecked())
                    return true;
            }
        }
        else if (conditionForTrue == FulfillCondition.AND)
        {
            foreach (var condition in AllConditions)
            {
                if (condition.IsConditionChecked() == false)
                    return false;
            }

            return true;
        }
        else if (conditionForTrue == FulfillCondition.NOT)
        {
            foreach (var condition in AllConditions)
            {
                if (condition.IsConditionChecked() == true)
                    return false;
            }

            return true;
        }

        return false;
    }
}
