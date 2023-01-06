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

    private GameObject soundContainer;

    private void Awake()
    {
        instance = this;
        soundContainer = new GameObject("SoundContainer");
        soundContainer.transform.position = Vector3.zero;

        foreach (Sound s in sounds)
        {
            CreateNewSound(s);
        }
    }

    private void CreateNewSound(Sound s)
    {
        var go = new GameObject();
        s.sources.Add(go.AddComponent<AudioSource>());
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;
        s.source.outputAudioMixerGroup = baseSFXMixer;
        s.source.playOnAwake = false;

        for (int x = 0; x < s.sourceAmount; x++)
        {
            var go1 = new GameObject();
            var newSource = go1.AddComponent<AudioSource>();
            s.sources.Add(newSource);
            newSource.clip = s.clip;
            newSource.volume = s.volume;
            newSource.pitch = s.pitch;
            newSource.loop = s.loop;
            newSource.outputAudioMixerGroup = baseSFXMixer;
            newSource.playOnAwake = false;
            go1.gameObject.name = $"{s.clip.name}";
            go1.transform.SetParent(soundContainer.transform);

        }

        go.gameObject.name = $"{s.clip.name}";
        go.transform.SetParent(soundContainer.transform);
    }

    public void Temp_NewSound(AudioClip _clip, string _name = "horay", int _sourceAmount = 1, float _volume = 1, float _pitch = 1)
    {
        Sound sound1 = Sound.CreateSound(_clip, _name, _sourceAmount, _volume, _pitch);
        sounds.Add(sound1);
        CreateNewSound(sound1);
    }

    public void Play (string name)
    {
        Sound s = sounds.Find(x => x.name == name);
        s.source.Play();
        s.source.spatialBlend = 0f;
    }

    public void Play3D(string name, Vector3 position)
    {
        Sound s = sounds.Find(x => x.name == name);
        if (s == null) return;
        var source = s.GetAnySource();
        source.Play();
        source.spatialBlend = 1f;
        source.transform.position = position;
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
