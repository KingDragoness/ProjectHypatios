using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// for anything that is persistent (check 10 times per second)
/// </summary>
/// 

public class OnStatusAilmentCheck : MonoBehaviour
{

    public UnityEvent OnChecked;
    public UnityEvent OnFail;
    public BaseStatusEffectObject ailment;
    public bool MultipleTriggerOnSuccess = false;

    private float _time = 0.1f;
    private bool _hasSuccess = false;

    void Update()
    {
        _time -= Time.deltaTime;
        if (_time > 0f) return;
        CheckAilment();

        _time = 0.1f;
    }

    private void CheckAilment()
    {
        bool success = false;

        if (Hypatios.Player.IsStatusEffectGroup(ailment) == true)
        {
            success = true;
        }

        if (success)
        {
            if (MultipleTriggerOnSuccess)
            {
                OnChecked?.Invoke();
            }
            else
            {
                if (_hasSuccess != success)
                    OnChecked?.Invoke();
            }
        }
        else
        {
            if (MultipleTriggerOnSuccess)
            {
                OnFail?.Invoke();
            }
            else
            {
                if (_hasSuccess != success)
                    OnFail?.Invoke();
            }
        }

        _hasSuccess = success;
    }
}
