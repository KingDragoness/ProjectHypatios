using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Stance_Intro", menuName = "Hypatios/Vendrich AI/Stance_Intro", order = 1)]

public class HB_Stance_Intro : HB_AIPackage
{

    public override void Run(MechHeavenblazerEnemy _mech)
    {
        
        if (_mech.mainAnimator)
        {

        }

        base.Run(_mech);
    }

    public override void NotRun(MechHeavenblazerEnemy _mech)
    {
        _mech.AnimatorPlayer.PlayAnimation(clip, 0.5f);
        base.NotRun(_mech);
    }

    public override int GetWeightDecision(MechHeavenblazerEnemy _mech)
    {
        if (Hypatios.Player.Health.curHealth < 50)
        {
            return 100;
        }
        
        return 1;
    }

}
