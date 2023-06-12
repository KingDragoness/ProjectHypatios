using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerCustomEvent : MonoBehaviour
{

    public UnityEvent OnTriggerEvent;


    public void TriggerEvent()
    {
        OnTriggerEvent?.Invoke();
    }

}
