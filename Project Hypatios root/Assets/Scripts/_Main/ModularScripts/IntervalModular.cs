using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntervalModular : MonoBehaviour
{

    public UnityEvent OnIntervalTrigger;
    public int interval = 1;

    private bool safetyCheck = false;

    private void Start()
    {
        if (interval <= 1)
        {
            interval = 2;
        }
    }

    private void Update()
    {
        if (Time.timeScale > 0 && Mathf.RoundToInt(Time.time * 10) % interval == 1) { safetyCheck = false; }

        if (Mathf.RoundToInt(Time.time * 10) % interval == 0 && safetyCheck == false)
        {
            OnIntervalTrigger?.Invoke();
            safetyCheck = true;

        }
    }

}
