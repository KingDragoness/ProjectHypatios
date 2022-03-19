using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{

    public AudioSource musicSource;

    public static MusicPlayer Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Instance = this;
    }

    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }
}
