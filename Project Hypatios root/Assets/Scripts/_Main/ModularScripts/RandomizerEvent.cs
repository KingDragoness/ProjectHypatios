using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RandomizerEvent : MonoBehaviour
{

    [Range(0f,1f)]
    public float chanceSpawn = 0;
    public UnityEvent OnSpawn;
    public UnityEvent OnFailed;


    private bool shouldSpawn = false;

    private void Start()
    {
        Randomized();
    }

    public void Randomized()
    {
        var random = Hypatios.GetRandomChance();
        var seed = Hypatios.GetSeed();


        if (Hypatios.Game.TotalRuns == 0)
        {
            OnFailed?.Invoke();
            return;
        }

        if (random < chanceSpawn)
        {
            shouldSpawn = true;
            OnSpawn?.Invoke();
        }
        else
        {
            OnFailed?.Invoke();
        }
    }


}
