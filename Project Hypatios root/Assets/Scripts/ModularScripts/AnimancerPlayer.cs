using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

public class AnimancerPlayer : MonoBehaviour
{
    [SerializeField] private AnimancerComponent _Animancer;

    public void FadeSeparateAnimation(ClipTransition _SeparateAnimation, float time = 0.25f)
    {
        _Animancer.Play(_SeparateAnimation, time);
    }

}
