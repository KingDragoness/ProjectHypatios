using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnDestroyEvent : MonoBehaviour
{

    public UnityEvent triggerEvents;


    private void OnDestroy()
    {
        triggerEvents?.Invoke();

    }
}
