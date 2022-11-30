using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPool : MonoBehaviour
{
    public UnityEvent triggerEvents;

    /// <summary>
    /// Executed on SummonObject/Particle! This will have same order as OnEnabled!
    /// </summary>
    public void OnReuseObject()
    {
        triggerEvents?.Invoke();

    }

}
