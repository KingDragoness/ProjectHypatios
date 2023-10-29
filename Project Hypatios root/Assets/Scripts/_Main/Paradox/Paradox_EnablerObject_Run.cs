﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Paradox_EnablerObject_Run : MonoBehaviour
{

    [Tooltip("Make it same value for min/max for equivalcne")]
    public int activateMinRange = 0;
    public int activateMaxRange = 2;
    public GameObject targetObject;
    private int currentRun = 0;

    private void Start()
    {
        CheckCondition();

    }

    [Button("Sanity Check")]
    public void SanityCheck()
    {
        CheckCondition();
    }

    public void CheckCondition()
    {
        currentRun = Hypatios.Game.TotalRuns;

        if (activateMinRange == activateMaxRange)
        {
            if (currentRun == activateMinRange)
            {
                if (targetObject != null)
                {
                    targetObject.gameObject.SetActive(true);
                }

            }
            else
            {
                if (targetObject != null)
                    targetObject.gameObject.SetActive(false);
            }
        }
        else
        {
            if (currentRun >= activateMinRange && currentRun <= activateMaxRange)
            {
                if (targetObject != null)
                {
                    targetObject.gameObject.SetActive(true);
                }

            }
            else
            {
                if (targetObject != null)
                    targetObject.gameObject.SetActive(false);
            }
        }
    }

}
