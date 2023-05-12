using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

public class AnimationPlayerScript : MonoBehaviour
{

    public ClipTransition clip;
    public AnimancerPlayer animancerPlayer;

    public void PlayAnimation()
    {
        animancerPlayer.PlayAnimation(clip);
    }

}
