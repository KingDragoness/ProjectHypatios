using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

public class AnimancerPlayer : MonoBehaviour
{
    [SerializeField] private AnimancerComponent _Animancer;
    public ClipTransition defaultClip;

    private void Start()
    {
        if (defaultClip != null)
        {
            PlayAnimation(defaultClip);
        }
    }

    public void PlayAnimation(ClipTransition _SeparateAnimation, float time = 0.25f)
    {
        _Animancer.Play(_SeparateAnimation, time);
    }

    public bool IsPlayingClip(AnimationClip clip)
    {
        return _Animancer.IsPlayingClip(clip);
    }

}
