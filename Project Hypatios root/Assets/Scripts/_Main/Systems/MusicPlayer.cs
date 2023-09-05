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
    private AudioSource newMusicAudio;
    private AudioSource oldMusicAudio;
    private bool isTransitioning = false;
    private float _transitTime = 2f;
    private float _transitionClock = 2f;
    private float _originalVolume = 0f;
    private float _targetVolume = 0f;

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

    private void Update()
    {
        if (isTransitioning)
        {
            float f = (_transitionClock / _transitTime);
            float f1 = 1 - f;
            _transitionClock -= Time.deltaTime;

            oldMusicAudio.volume = f * _originalVolume;
            newMusicAudio.volume = f1 * _targetVolume;

            if (_transitionClock <= 0)
            {
                ChangePrimaryAudioSource();
                isTransitioning = false;
            }
        }
    }

    private void ChangePrimaryAudioSource()
    {
        musicSource = newMusicAudio;
        newMusicAudio = null;
        Destroy(oldMusicAudio);
    }

    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }

    public void TransitionMusic(AudioClip musicClip, float transitionTime = 2f, float targetVolume = 1f)
    {
        if (newMusicAudio != null)
        {
            Destroy(newMusicAudio);
        }

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = musicClip;
        newSource.loop = true;
        newSource.playOnAwake = true;
        newSource.volume = 0f;
        newSource.outputAudioMixerGroup = musicSource.outputAudioMixerGroup;

        oldMusicAudio = musicSource;
        newMusicAudio = newSource;
        _originalVolume = oldMusicAudio.volume;
        _targetVolume = targetVolume;
        newSource.Play();

        isTransitioning = true;
        _transitTime = transitionTime;
        _transitionClock = transitionTime;
    }
}
