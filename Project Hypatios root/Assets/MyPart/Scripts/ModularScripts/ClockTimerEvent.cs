using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClockTimerEvent : MonoBehaviour
{

    public float cooldownTimer = 10;
    public UnityEvent OnCooldownTrigger;

    private float f_currentCooldown = 0;

    private void Update()
    {
        f_currentCooldown += Time.deltaTime;

        if (f_currentCooldown >= cooldownTimer)
        {
            OnCooldownTrigger?.Invoke();
            f_currentCooldown = 0;
        }
    }

}
