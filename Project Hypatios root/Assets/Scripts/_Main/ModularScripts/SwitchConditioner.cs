using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class SwitchConditioner : MonoBehaviour
{
    

    public string switchName = "Switch 1";
    public UnityEvent OnSwitch;
    
    private bool triggered = false;

    public bool Triggered { get => triggered; }

    public void Switch()
    {
        triggered = true; Check();

    }

    public void Clear()
    {
        triggered = false; Check();

    }

    [ContextMenu("Toggle switch")]
    public void Toggle()
    {
        triggered = !triggered; Check();

    }

    public void Randomize()
    {
        float f = Random.Range(0f, 1f);

        if (f > 0.5f) triggered = true;
        else triggered = false;

        Check();
    }

    public void Check()
    {
        if (triggered)
        {
            OnSwitch?.Invoke();
        }
    }

}
