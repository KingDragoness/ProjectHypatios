using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnActivateTriggerEvent : MonoBehaviour
{

    public UnityEvent triggerEvents;

    void Start()
    {
        triggerEvents?.Invoke();

    }


}
