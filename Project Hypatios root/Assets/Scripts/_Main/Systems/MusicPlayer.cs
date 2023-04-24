using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{

    [System.Serializable]
    public class NewMusicLayer
    {
        public Trivia trivia;
        public AudioClip clip;
    }

    public AudioSource musicSource;
    public NewMusicLayer newMusic;
    public AudioClip b_side_Track;
    public bool playBSideUponStart = false;

    public static MusicPlayer Instance;

    private void Awake()
    {
        Instance = this;

        if (b_side_Track != null)
        {
            var chance = Random.Range(0f, 1f);

            if (chance > 0.5f)
            {
                musicSource.clip = b_side_Track;
                if (playBSideUponStart) musicSource.Play();
            }
        }


        if (newMusic.trivia != null)
        {
            if (Hypatios.Game.Check_TriviaCompleted(newMusic.trivia))
            {
                musicSource.clip = newMusic.clip;
                musicSource.Play();
            }
        }
    }

    public void PlayMusic(AudioClip audioClip)
    {
        musicSource.clip = audioClip;
        musicSource.Play();
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
