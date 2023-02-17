using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Stance_AttackFiring", menuName = "Hypatios/Vendrich AI/Stance_AttackFiring", order = 1)]
public class HB_Stance_AttackFiring : HB_AIPackage
{

    public override void Run(MechHeavenblazerEnemy _mech)
    {
        {
            if (_mech.modularTurretGun.activeInHierarchy == false)
                _mech.modularTurretGun.SetActive(true);
            if (_mech.modularTurretGun1.activeInHierarchy == false)
                _mech.modularTurretGun1.SetActive(true);
        }
  

        _mech.ik_target_player = true;

        base.Run(_mech);
    }

    public override void NotRun(MechHeavenblazerEnemy _mech)
    {
        if (_mech.modularTurretGun.activeInHierarchy)
            _mech.modularTurretGun.SetActive(false);
        if (_mech.modularTurretGun1.activeInHierarchy)
            _mech.modularTurretGun1.SetActive(false);

        _mech.ik_target_player = false;
        base.NotRun(_mech);
    }

    public override void OnChangedToThis(MechHeavenblazerEnemy _mech)
    {
        _mech.AnimatorPlayer.PlayAnimation(clip, 0.5f);

        base.OnChangedToThis(_mech);
    }

    public override int GetWeightDecision(MechHeavenblazerEnemy _mech)
    {
        bool isBehind = false;

        if (_mech.currentTarget == null)
            return 0;

        {
            Vector3 turretRelative = _mech.transform.InverseTransformPoint(_mech.currentTarget.transform.position);
            if (turretRelative.z < 0) isBehind = true;

        }


        if (isBehind)
            return -40;
        else
            return 100;
    }

}
