﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paradox_TriggerEvent : MonoBehaviour
{
    public string eventKeyName = "COMPLETED.CHAMBER1";

    public void TriggerEvent()
    {
        bool keyExist = Hypatios.Game.Check_EverUsed(eventKeyName);

        if (!keyExist)
        {
            Hypatios.Game.TryAdd_ParadoxEvent(eventKeyName);
        }
    }

    [ContextMenu("Clear Event")]
    public void ClearEvent()
    {
        Hypatios.Game.Clear_ParadoxEvent(eventKeyName);
    }
}