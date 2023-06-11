using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Animancer;

public class AnimancerPlayer : MonoBehaviour
{
    [SerializeField] private AnimancerComponent _Animancer;
    [SerializeField] private HybridAnimancerComponent _HybridAnimancer;
    public ClipTransition defaultClip;

    private List<AnimationClip> loadedClips = new List<AnimationClip>();

    private void Start()
    {
        if (defaultClip.Clip != null)
        {
            PlayAnimation(defaultClip);
        }
    }

    [Button("DEBUG_PlayShit")]
    public void PlayShit(AnimationClip _clip)
    {
        var ct = new ClipTransition();
        ct.Clip = _clip;
        PlayAnimation(ct, 1f);


    }

    public void PlayAnimation(ClipTransition _SeparateAnimation, float time = 0.25f, FadeMode fadeMode = default)
    {
        if (_Animancer != null) _Animancer.Play(_SeparateAnimation, time, fadeMode);
        if (_HybridAnimancer != null)
        {
            _HybridAnimancer.Play(_SeparateAnimation, time, fadeMode);
            if (_SeparateAnimation.Clip != null)
            {
                if (loadedClips.Contains(_SeparateAnimation.Clip) == false)
                    loadedClips.Add(_SeparateAnimation.Clip);
            }
        }
    }
    public bool IsPlayingClip(AnimationClip clip)
    {
        if (_Animancer != null) return _Animancer.IsPlayingClip(clip);
        if (_HybridAnimancer != null) return _HybridAnimancer.IsPlayingClip(clip);

        return false;
    }

    //This is only just a band-fix
    //might need better solution in the future (HybridAnimancer)
    public void DisableAnimancer()
    {
        if (_Animancer != null) _Animancer.Stop();
        if (_HybridAnimancer != null)
        {
            _HybridAnimancer.Play(_HybridAnimancer.Controller, 0f);
            //_HybridAnimancer.Stop();
            //Debug.Log(_HybridAnimancer.GetCurrentAnimatorClipInfoCount(0));
            //foreach (var clip in loadedClips)
            //{
            //    _HybridAnimancer.States.Destroy(clip);
            //}
            //loadedClips.Clear();
            //_HybridAnimancer.Layers[0].SetWeight(1f);
            //_HybridAnimancer.Layers[0].TargetWeight = 1f;
            //Debug.Log(_HybridAnimancer.States.Count);
            //foreach (var state in _HybridAnimancer.States)
            //{
            //    if (state == null) continue;
            //    Debug.Log($"{state.DebugName}");
            //    state.SetWeight(1f);
            //    state.TargetWeight = 1f;
            //    state.IsPlaying = true;
            //}

        }
    }
}
