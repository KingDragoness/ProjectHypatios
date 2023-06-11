using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Animancer;

public class HESS_PostIdle : MonoBehaviour
{

    [FoldoutGroup("Idle")] public ClipTransition idleAnimation;
    [FoldoutGroup("Pose Preview")] public Animator animator;
    public float dist_StartAnimation = 0.7f;



    [ContextMenu("RefreshPose")]
    public void RefreshPose()
    {
        idleAnimation.Clip.SampleAnimation(animator.gameObject, 0f);

    }

}
