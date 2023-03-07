using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Paradox_ChamberVisit : MonoBehaviour
{

    public enum Condition
    {
        And,
        Any
    }

    [Tooltip("Only apply to eventKey")] public Condition conditionType;
    public List<string> eventKey = new List<string>();
    public List<string> disqualifyEventKey = new List<string>();
    public int triggerAfterVisit = 2;
    public UnityEvent OnConditionMet;
    public UnityEvent OnFailed;
    public GameObject targetObject;
    public ChamberLevel chamberObject;

    private void Start()
    {
        bool success = IsSuccess();

        if (success)
        {
            if (targetObject != null)
            {
                OnConditionMet?.Invoke();
                targetObject.gameObject.SetActive(true);
            }
        }
        else
        {
            if (targetObject != null)
            {
                targetObject.gameObject.SetActive(false);
                OnFailed?.Invoke();

            }
        }
    }

    public bool IsSuccess()
    {
        var saveData = ChamberLevelController.GetSaveData(chamberObject);

        if (saveData == null)
            return false;

        if (saveData.timesVisited < triggerAfterVisit)
            return false;

        foreach (var eventKey in disqualifyEventKey)
        {
            if (Hypatios.Game.Check_ParadoxEvent(eventKey))
                return false;
        }

        bool keyExist = false;

        if (conditionType == Condition.Any)
        {
            foreach (var eventKey in eventKey)
            {
                if (Hypatios.Game.Check_ParadoxEvent(eventKey))
                {
                    keyExist = true;
                }
            }
        }
        else if (conditionType == Condition.And)
        {
            keyExist = true;

            foreach (var eventKey in eventKey)
            {
                if (!Hypatios.Game.Check_ParadoxEvent(eventKey))
                {
                    keyExist = false;
                }
            }
        }

        if (eventKey.Count == 0)
            keyExist = true;

        if (keyExist)
            return true;

        return false;
    }


}
