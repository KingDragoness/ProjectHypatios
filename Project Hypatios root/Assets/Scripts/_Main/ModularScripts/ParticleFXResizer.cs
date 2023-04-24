using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ParticleFXResizer : MonoBehaviour
{


    public bool pref_IncludeChildren;

    [Button("Resize Particle")]
    public void ResizeParticle(float ScalingValue)
    {
        var go = gameObject;

        ParticleSystem[] systems;
        if (pref_IncludeChildren)
            systems = go.GetComponentsInChildren<ParticleSystem>(true);
        else
            systems = go.GetComponents<ParticleSystem>();

        foreach (ParticleSystem ps in systems)
        {
            ps.transform.localScale = ps.transform.localScale * ScalingValue;
        }

    }

    [Button("Stop Particle")]

    public void StopParticle()
    {
        var go = gameObject;

        ParticleSystem[] systems;
        if (pref_IncludeChildren)
            systems = go.GetComponentsInChildren<ParticleSystem>(true);
        else
            systems = go.GetComponents<ParticleSystem>();

        foreach (ParticleSystem ps in systems)
        {
            ps.loop = false;
        }
    }

    [Button("Reset Particle")]

    public void ResetParticle()
    {
        var go = gameObject;

        ParticleSystem[] systems;
        if (pref_IncludeChildren)
            systems = go.GetComponentsInChildren<ParticleSystem>(true);
        else
            systems = go.GetComponents<ParticleSystem>();

        foreach (ParticleSystem ps in systems)
        {
            ps.loop = true;
        }
    }
}
