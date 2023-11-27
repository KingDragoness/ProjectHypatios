using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class OnTriggerCustomEvent : MonoBehaviour
{

    public UnityEvent OnTriggerEvent;

    [Button("Trigger Event")]
    public void TriggerEvent()
    {
        OnTriggerEvent?.Invoke();
    }

}
