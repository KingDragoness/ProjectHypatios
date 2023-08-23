using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Liquidator_SoundSystem : MonoBehaviour
{

    [System.Serializable]
    public class Song
    {
        public AudioClip clip;
        public float pitch = 1f;
        public float volume = 0.9f;
    }

    public List<Song> playlist = new List<Song>();
    public MusicPlayer musicPlayer;
    public AudioLowPassFilter audio_EQFilter;
    public AudioReverbFilter audio_ReverbFilter;
    [FoldoutGroup("EQ Audio")] public float posY_mainChamber = 120; //above this level will immediately 22,000 kHz
    [FoldoutGroup("EQ Audio")] public float posY_cutoffHighest = 100; //between main chamber to cutoff will quickly lerp from highestEQ to 22,000
    [FoldoutGroup("EQ Audio")] public float posY_basement;
    [FoldoutGroup("EQ Audio")] public float lowestEQ = 200f;
    [FoldoutGroup("EQ Audio")] public float highestEQ = 900f; //Casinos level


    private AudioClip[] previouslyPlayedMusic = new AudioClip[3];
    private float cooldown = 0.1f;

    private void Start()
    {
        PlayRandomMusic();
    }

    private void Update()
    {
        cooldown -= Time.deltaTime;

        if (cooldown > 0f)
        {
            return;
        }

        AudioHandling();
        if (musicPlayer.musicSource.isPlaying == false && musicPlayer.musicSource.enabled == true)
        {
            PlayRandomMusic();
        }
        cooldown = 0.1f;
    }

    public void AudioHandling()
    {
        float posY = Hypatios.Player.transform.position.y;
        float percentPos = (Hypatios.Player.transform.position.y - posY_basement) / (posY_cutoffHighest - posY_basement);
        float freq = Mathf.Lerp(lowestEQ, highestEQ, percentPos);

        if (posY >= posY_cutoffHighest)
        {
            percentPos = (Hypatios.Player.transform.position.y - posY_cutoffHighest) / (posY_mainChamber - posY_cutoffHighest);
            freq = Mathf.Lerp(highestEQ, 22000f, percentPos);
        }
        if (posY >= posY_mainChamber)
        {
            freq = 22000f;
        }

        audio_EQFilter.cutoffFrequency = freq;
    }

    [FoldoutGroup("DEBUG")][Button("Stop Music")]
    public void StopMusic()
    {
        musicPlayer.musicSource.Stop();
    }

    public void PlayRandomMusic()
    {
        int attempts = 0;
        bool success = false;
        Song selectedSong = null;

        while (success == false && attempts < 100)
        {
            selectedSong = playlist[Random.Range(0, playlist.Count)];
            if (HasMusicBeenPlayed(selectedSong.clip) == false)
            {
                success = true;
                break;
            }

            attempts++;
        }

        if (selectedSong == null)
        {
            Debug.LogError("Failed selected <color=green>song!</color>");
            return;
        }

        AudioClip[] newClipHistory = new AudioClip[3];
        newClipHistory[0] = selectedSong.clip;
        newClipHistory[1] = previouslyPlayedMusic[0];
        newClipHistory[2] = previouslyPlayedMusic[1];
        previouslyPlayedMusic = newClipHistory;

        musicPlayer.musicSource.pitch = selectedSong.pitch;
        musicPlayer.musicSource.volume = selectedSong.volume;
        musicPlayer.PlayMusic(selectedSong.clip);
    }

    public bool HasMusicBeenPlayed(AudioClip clip)
    {
        foreach(var playedMusic in previouslyPlayedMusic)
        {
            if (playedMusic == null)
                continue;

            if (playedMusic == clip)
                return true;
        }

        return false;
    }
}
