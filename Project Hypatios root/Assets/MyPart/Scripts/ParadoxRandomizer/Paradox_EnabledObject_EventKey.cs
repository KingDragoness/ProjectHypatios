using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject targetObject;

    private void Start()
    {
        bool keyExist = false;

        if (conditionType == Condition.Any)
        {
            keyExist = false;

            foreach (var eventKey in eventKeyName)
            {
                if (FPSMainScript.instance.Check_ParadoxEvent(eventKey))
                {
                    keyExist = true;
                }
            }
        }
        else if (conditionType == Condition.Any)
        {
            keyExist = true;

            foreach (var eventKey in eventKeyName)
            {
                if (!FPSMainScript.instance.Check_ParadoxEvent(eventKey))
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
                    targetObject.gameObject.SetActive(true);
                }
            }
            else
            {
                if (targetObject != null)
                {
                    targetObject.gameObject.SetActive(false);
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
                }
            }
            else
            {
                if (targetObject != null)
                {
                    targetObject.gameObject.SetActive(true);
                }
            }
        }
    }
}
