using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class soundManagerScript : MonoBehaviour
{
    public List<Sound> sounds = new List<Sound>();
    public AudioMixerGroup baseSFXMixer;

    public static soundManagerScript instance;

    private void Awake()
    {
        instance = this;

        foreach(Sound s in sounds)
        {
            var go = new GameObject();
            s.source = go.AddComponent<AudioSource>();
            s.source.clip  = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = baseSFXMixer;
            s.source.playOnAwake = false;

            go.gameObject.name = $"{s.clip.name}";
            go.transform.SetParent(this.transform);
        }
    }

    public void Play (string name)
    {
        Sound s = sounds.Find(x => x.name == name);
        s.source.Play();
    }

    public void PlayOneShot(string name)
    {
        Sound s = sounds.Find(x => x.name == name);
        s.source.PlayOneShot(s.clip);
    }


    public void Pause (string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        s.source.Pause();
    }
}
