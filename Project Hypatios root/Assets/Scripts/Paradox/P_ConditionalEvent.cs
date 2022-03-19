using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class P_ConditionalEvent : MonoBehaviour
{
    [System.Serializable]
    public class ConditionEvent
    {
        public string key = "";
        public UnityEvent unityEvent;
    }


    public List<ConditionEvent> conditionEvent = new List<ConditionEvent>();
    [Space(30)]
    public ParadoxLevelScript paradoxLevel;


    private void FixedUpdate()
    {

        foreach (var ce in conditionEvent)
        {
            if (paradoxLevel.isPreviewing)
            {
                return;
            }    

            if (ce.key == paradoxLevel.GetValue())
            {
                ce.unityEvent?.Invoke();
            }
        }

    }

}
