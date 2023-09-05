using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer_Transition : MonoBehaviour
{

    public AudioClip musicClip;
    public float transitionTime = 2f;
    public float targetVolume = 1f;

    public void ChangeMusic()
    {
        MusicPlayer.Instance.TransitionMusic(musicClip, transitionTime, targetVolume);
    }

}
