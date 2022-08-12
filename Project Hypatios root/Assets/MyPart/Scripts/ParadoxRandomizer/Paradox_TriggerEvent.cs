using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paradox_TriggerEvent : MonoBehaviour
{
    public string eventKeyName = "COMPLETED.CHAMBER1";

    public void TriggerEvent()
    {
        bool keyExist = FPSMainScript.instance.Check_EverUsed(eventKeyName);

        if (!keyExist)
        {
            FPSMainScript.instance.TryAdd_ParadoxEvent(eventKeyName);
        }
    }

    [ContextMenu("Clear Event")]
    public void ClearEvent()
    {
        FPSMainScript.instance.Clear_ParadoxEvent(eventKeyName);
    }
}
