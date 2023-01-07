using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{

    public AudioSource musicSource;
    public AudioClip b_side_Track;

    public static MusicPlayer Instance;

    private void Awake()
    {
        Instance = this;

        if (b_side_Track != null)
        {
            var chance = Random.Range(0f, 1f);

            if (chance > 0.5f)
                musicSource.clip = b_side_Track;
        }
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
