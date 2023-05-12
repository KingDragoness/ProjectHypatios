using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AudioSourcePausable : MonoBehaviour
{

    public AudioSource audioSource;

    private bool b = false;

    private void Update()
    {
        if (Time.timeScale <= 0)
        {
            if (!b)  audioSource.Pause();
            b = true;
        }
        else
        {
            if (b) audioSource.UnPause();
            b = false;

        }
    }
}
