using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Paradox_TalkEventKey : MonoBehaviour
{

    public enum Mode
    {
        DisabledWhenConditionMet,
        EnabledWhenConditionMet
    }

    public enum Condition
    {
        And,
        Any
    }

    public List<string> eventKeyName = new List<string>();
    public Condition conditionType;
    public Mode mode;
    public UnityEvent OnConditionMet;
    public UnityEvent OnFailed;

    private void Start()
    {
        bool keyExist = false;
        List<string> keyToSaves = new List<string>();

        if (conditionType == Condition.Any)
        {
            keyExist = false;

            foreach (var eventKey in eventKeyName)
            {
                string key = $"{eventKey}.0";

                if (Hypatios.Game.Check_ParadoxEvent(key)) continue;

                if (Hypatios.Game.Check_ParadoxEvent(eventKey))
                {
                    keyExist = true;
                    keyToSaves.Add(key);
                }
            }
        }
        else if (conditionType == Condition.And) //unfinish implementation
        {
            keyExist = true;

            foreach (var eventKey in eventKeyName)
            {
                string key = $"{eventKey}.0";

                if (!Hypatios.Game.Check_ParadoxEvent(eventKey))
                {
                    keyExist = false;
                    keyToSaves.Add(key);

                }
                else
                {
                }
            }

            if (keyExist == false)
                keyToSaves.Clear();
        }


        if (mode == Mode.EnabledWhenConditionMet)
        {
            if (keyExist) //success
            {
                OnConditionMet?.Invoke();

                foreach (var key in keyToSaves)
                    TriggerEvent(key);

            }
            else  //failed!
            {
                OnFailed?.Invoke();
            }
        }
        else if (mode == Mode.DisabledWhenConditionMet)
        {
            if (keyExist) //failed!
            {
                OnFailed?.Invoke();

            }
            else //success
            {
                OnConditionMet?.Invoke();

                foreach (var key in keyToSaves)
                    TriggerEvent(key);
            }
        }
    }

    public void TriggerEvent(string _eventKeyName)
    {
        bool keyExist = Hypatios.Game.Check_EverUsed(_eventKeyName);

        if (!keyExist)
        {
            Hypatios.Game.TryAdd_ParadoxEvent(_eventKeyName);
        }
    }
}
