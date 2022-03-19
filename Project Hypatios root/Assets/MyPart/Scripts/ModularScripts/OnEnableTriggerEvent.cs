using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEnableTriggerEvent : MonoBehaviour
{
    public UnityEvent triggerEvents;

    void OnEnable()
    {
        triggerEvents?.Invoke();

    }

}
