﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "OnEnemyKilled", menuName = "Hypatios/GameEvents", order = 1)]

public class GameEvent : ScriptableObject
{

    private List<GameEventListener> listeners =
       new List<GameEventListener>();

    public System.Action Listeners;


    [Button("Raise")]
    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised();

        Listeners?.Invoke();
    }

    public void RegisterListener(GameEventListener listener)
    { listeners.Add(listener); }

    public void UnregisterListener(GameEventListener listener)
    { listeners.Remove(listener); }

}