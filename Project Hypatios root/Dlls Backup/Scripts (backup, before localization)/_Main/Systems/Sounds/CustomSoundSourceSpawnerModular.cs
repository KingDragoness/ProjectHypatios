using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSoundSourceSpawnerModular : MonoBehaviour
{

    public AudioClip audioClip;
    public Sound sound;


    public void PlayAudioSource()
    {
        if (soundManagerScript.instance.IsSoundExists(audioClip.name) == false)
            soundManagerScript.instance.Temp_NewSound(audioClip, audioClip.name, _sourceAmount: 5);

        soundManagerScript.instance.Play($"{audioClip.name}");
    }

    public void PlaySound()
    {
        if (soundManagerScript.instance.IsSoundExists(sound.name) == false)
            soundManagerScript.instance.Temp_NewSound(sound);

        soundManagerScript.instance.Play($"{sound.name}");
    }

    public void PlaySound3D()
    {
        if (soundManagerScript.instance.IsSoundExists(sound.name) == false)
            soundManagerScript.instance.Temp_NewSound(sound);

        soundManagerScript.instance.Play3D($"{sound.name}", transform.position);
    }


}
