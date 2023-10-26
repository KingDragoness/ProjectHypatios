using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRandomizedPitch : MonoBehaviour
{

    public AudioSource audioSource;
    public float pitchRandomRange = 0.04f;

    private float originalPitch;

    private void Start()
    {
        originalPitch = audioSource.pitch;
        audioSource.pitch = originalPitch + Random.Range(-pitchRandomRange / 2f, pitchRandomRange / 2f);
    }

 
}
