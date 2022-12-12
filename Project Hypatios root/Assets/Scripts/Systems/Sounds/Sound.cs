using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

[System.Serializable]
public class Sound
{
    [HorizontalGroup(LabelWidth = 44)] public string name;
    [HorizontalGroup(LabelWidth = 44)] public AudioClip clip;

    [FoldoutGroup("Show more")]
    [Range(1, 8)]
    public int sourceAmount;

    [FoldoutGroup("Show more")]
    [Range(0f, 1f)]
    public float volume;

    [FoldoutGroup("Show more")]
    [Range(.1f, 3f)]
    public float pitch;

    [FoldoutGroup("Show more")]
    public bool loop;

    [FoldoutGroup("Show more")]
    [ReadOnly]
    public List<AudioSource> sources = new List<AudioSource>();

    public static Sound CreateSound(AudioClip _clip, string _name = "horay", int _sourceAmount = 1, float _volume = 1, float _pitch = 1)
    {
        Sound s1 = new Sound();
        s1.clip = _clip;
        s1.name = _name;
        s1.sourceAmount = _sourceAmount;
        s1.volume = _volume;
        s1.pitch = _pitch;

        return s1;
    }

    public AudioSource source
    {
        get { return sources[0]; }
    }

    public AudioSource GetAnySource()
    {
        //first check not playing
        {
            AudioSource audioNotPlaying = sources.Find(x => x.isPlaying == false);

            if (audioNotPlaying != null) return audioNotPlaying;
        }

        //second find by time
        {
            var audioByTime = sources.OrderBy(x => x.time).ToArray();
            var audio1 = audioByTime[0];

            if (audio1 != null) return audio1;
        }

        //nyerah
        return source;
    }
}
