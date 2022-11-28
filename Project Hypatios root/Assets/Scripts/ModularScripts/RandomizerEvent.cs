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
        int seed = Hypatios.Game.TotalRuns + SystemInfo.graphicsDeviceID;
        var RandomSys = new System.Random(seed);
        float random = (RandomSys.Next(0, 100)) / 100f;
        Debug.Log($"seed: {seed} [{random}]");

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
