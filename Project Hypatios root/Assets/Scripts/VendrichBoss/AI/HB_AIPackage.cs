using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class HB_AIPackage : ScriptableObject
{

    public enum Category
    {
        Intro,
        PatrolIdle,
        Attack,
        SpecialAttack
    }

    public Category category;
    public AnimationClip clip;

    public virtual void OnChangedToThis(MechHeavenblazerEnemy _mech)
    {

    }

    public virtual void Run(MechHeavenblazerEnemy _mech)
    {

    }

    public virtual void NotRun(MechHeavenblazerEnemy _mech)
    {

    }

    //this is for weighting available decisions, example:
    //if the player's health is low =
    //  Quickly focus on attacking (set value = 1000) rather patrol/idle/healing (set value = 10)
    public virtual int GetWeightDecision(MechHeavenblazerEnemy _mech)
    {
        return 0;
    }

}
