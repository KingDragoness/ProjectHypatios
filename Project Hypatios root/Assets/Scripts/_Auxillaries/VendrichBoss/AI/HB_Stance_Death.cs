using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Stance_Death", menuName = "Hypatios/Vendrich AI/Stance_Death", order = 1)]
public class HB_Stance_Death : HB_AIPackage
{
    public override void Run(MechHeavenblazerEnemy _mech)
    {

        base.Run(_mech);
    }

    public override void OnChangedToThis(MechHeavenblazerEnemy _mech)
    {
        _mech.AnimatorPlayer.PlayAnimation(clip, 0.5f);
        base.OnChangedToThis(_mech);
    }

    public override void NotRun(MechHeavenblazerEnemy _mech)
    {
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
