using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionProbeUpdator : MonoBehaviour
{

    public ReflectionProbe reflectionProbe;


    private float COOLDOWN_UPDATE_PROBE = 0.5f;
    private float timer = 0.1f;

    void Update()
    {

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            reflectionProbe.RenderProbe();
            timer = COOLDOWN_UPDATE_PROBE;
        }
    }
}
