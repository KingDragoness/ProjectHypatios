using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Preferably only one off trigger like
/// dialogue or level event.
/// </summary>
public class gEventListener_OnStatusAilment : MonoBehaviour
{

    public BaseStatusEffectObject statusAilment;
    public UnityEvent OnAilment;

    private bool hasStarted = false;

    private void OnEnable()
    {
        if (hasStarted == false) return;
        Hypatios.Event.OnStatusAilment.d_Listeners += AilmentAdd;
    }

    private void Start()
    {
        if (!hasStarted)
        {
            Hypatios.Event.OnStatusAilment.d_Listeners += AilmentAdd;
        }

        hasStarted = true;
    }

    private void OnDisable()
    {
        Hypatios.Event.OnStatusAilment.d_Listeners -= AilmentAdd;
    }


    private void AilmentAdd(BaseStatusEffectObject obj)
    {
        if (statusAilment != obj) return;

        OnAilment?.Invoke();
    }

}
