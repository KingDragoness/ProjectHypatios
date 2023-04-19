using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Conditioner : MonoBehaviour
{
    public enum ConditionType
    {
        ChamberVisit = 0,
        ChamberCompleted,
        ParadoxEventChecked,
        TriviaCompleted = 20,
    }

    [System.Serializable]
    public class ConditionEvent
    {
        public ConditionType conditionType;


        public bool IsConditionChecked()
        {
            return false;
        }
    }
}
