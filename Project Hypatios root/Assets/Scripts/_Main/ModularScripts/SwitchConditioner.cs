using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchConditioner : MonoBehaviour
{
    

    public string switchName = "Switch 1";
    private bool triggered = false;

    public bool Triggered { get => triggered; }

    public void Switch()
    {
        triggered = true;
    }

    public void Clear()
    {
        triggered = false;
    }

    [ContextMenu("Toggle switch")]
    public void Toggle()
    {
        triggered = !triggered;
    }

    public void Randomize()
    {
        float f = Random.Range(0f, 1f);

        if (f > 0.5f) triggered = true;
        else triggered = false;
    }

}
