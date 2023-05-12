using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionProbeUpdator : MonoBehaviour
{

    public ReflectionProbe reflectionProbe;
    public float CooldownUpdateProbe = 0.6f;


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
            timer = CooldownUpdateProbe;
        }
    }
}
