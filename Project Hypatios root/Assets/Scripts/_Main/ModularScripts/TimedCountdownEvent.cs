using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedCountdownEvent : MonoBehaviour
{

    public float CooldownTimer = 10;
    public UnityEvent OnCooldownTrigger;

    private float f_currentCooldown = 0;
    private bool isDone = false;

    private void Update()
    {
        f_currentCooldown += Time.deltaTime;


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
        f_currentCooldown = CooldownTimer;
    }
}
