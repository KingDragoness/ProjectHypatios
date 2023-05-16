using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class CooldownTimeTriggerEvent : MonoBehaviour
{

    public UnityEvent OnTimeEvent;

    private float _timer = 0.1f;
    private bool b = false;

    private void Update()
    {
        if (b == false) return;
        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
            return;
        }
        OnTimeEvent?.Invoke();
        b = false;
    }

    public void ExecuteTrigger(float cooldown = 0.3f)
    {
        _timer = cooldown;
        b = true;
    }

}
