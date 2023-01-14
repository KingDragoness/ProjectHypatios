using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Paradox_EnabledObject_EventKey : MonoBehaviour
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
    public GameObject targetObject;

    private void Start()
    {
        bool keyExist = false;

        if (conditionType == Condition.Any)
        {
            keyExist = false;

            foreach (var eventKey in eventKeyName)
            {
                if (Hypatios.Game.Check_ParadoxEvent(eventKey))
                {
                    keyExist = true;
                }
            }
        }
        else if (conditionType == Condition.And)
        {
            keyExist = true;

            foreach (var eventKey in eventKeyName)
            {
                if (!Hypatios.Game.Check_ParadoxEvent(eventKey))
                {
                    keyExist = false;
                }
            }
        }


        if (mode == Mode.EnabledWhenConditionMet)
        {
            if (keyExist)
            {
                if (targetObject != null)
                {
                    OnConditionMet?.Invoke();
                    targetObject.gameObject.SetActive(true);
                }
            }
            else
            {
                if (targetObject != null)
                {
                    targetObject.gameObject.SetActive(false);
                    OnFailed?.Invoke();

                }
            }
        }
        else if (mode == Mode.DisabledWhenConditionMet)
        {
            if (keyExist)
            {
                if (targetObject != null)
                {
                    targetObject.gameObject.SetActive(false);
                    OnFailed?.Invoke();

                }
            }
            else
            {
                if (targetObject != null)
                {
                    OnConditionMet?.Invoke();
                    targetObject.gameObject.SetActive(true);
                }
            }
        }
    }
}
