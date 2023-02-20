using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSoundSourceSpawnerModular : MonoBehaviour
{

    public AudioClip audioClip;


    public void PlayAudioSource()
    {
        if (soundManagerScript.instance.IsSoundExists(audioClip.name) == false)
            soundManagerScript.instance.Temp_NewSound(audioClip, audioClip.name, _sourceAmount: 5);

        soundManagerScript.instance.Play($"{audioClip.name}");
    }

}
