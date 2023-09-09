using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Animancer;


public class EclipseBlaz_AIModule : MonoBehaviour
{

    [ReadOnly] public EclipseblazerEnemy eclipseblazer;
    public EclipseblazerEnemy.Stance stance;
    public bool ignoreSelection = false;
    [SerializeField] private int weight = 40;
    [FoldoutGroup("Show more")] public float minimumDuration = 5f;
    [FoldoutGroup("Show more")] public ClipTransition idleAnimation;
    [FoldoutGroup("Show more")] public GameObject objectToSpawn;

    public virtual int GetWeight()
    {
        return weight;
    }

    public virtual void Run()
    {

    }


    public void OnEnterAnimation(AnimancerPlayer AnimatorPlayer)
    {
        AnimatorPlayer.PlayAnimation(idleAnimation, 1f);
    }

}
