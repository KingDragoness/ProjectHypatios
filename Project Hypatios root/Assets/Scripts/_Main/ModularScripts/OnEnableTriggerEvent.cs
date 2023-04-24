using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEnableTriggerEvent : MonoBehaviour
{
    public UnityEvent triggerEvents;
    public UnityEvent ondisableEvent;

    void OnEnable()
    {
        triggerEvents?.Invoke();

    }
    void OnDisable()
    {
        ondisableEvent?.Invoke();

    }
}
