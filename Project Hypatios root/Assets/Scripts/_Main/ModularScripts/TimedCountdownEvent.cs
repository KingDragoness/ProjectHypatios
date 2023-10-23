using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedCountdownEvent : MonoBehaviour
{

    public float CooldownTimer = 10;
    public UnityEvent OnCooldownTrigger;
    public bool useTimescale = true;

    private float f_currentCooldown = 0;
    private bool isDone = false;

    private void Update()
    {
        if (useTimescale)
        {
            f_currentCooldown += Time.deltaTime;
        }
        else
        {
            f_currentCooldown += Time.unscaledDeltaTime;
        }

        if (f_currentCooldown >= CooldownTimer && !isDone)
        {
            OnCooldownTrigger?.Invoke();
            isDone = true;
        }
        else
        {
        }
    }

    public void ResetTimer()
    {
        f_currentCooldown = 0f;
        isDone = false;
    }


}
