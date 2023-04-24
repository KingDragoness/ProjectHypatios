using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Stance_IdleFlying", menuName = "Hypatios/Vendrich AI/Stance_IdleFlying", order = 1)]
public class HB_Stance_IdleFlying : HB_AIPackage
{

    public float distanceToTarget = 10f;
    public float walkSpeed = 6f;
    public float rotateSpeed = 6f;

    public override void Run(MechHeavenblazerEnemy _mech)
    {

        var target = _mech.patrolFly_Target;
        float dist = Vector3.Distance(_mech.transform.position, target);
        float distPlayer = Vector3.Distance(_mech.transform.position, Hypatios.Player.transform.position);

        if (dist < distanceToTarget)
        {
            _mech.patrolFly_Target = _mech.FlyRegion.GetAnyPositionInsideBox();

        }

        var step = walkSpeed * Time.deltaTime;
        _mech.transform.position = Vector3.MoveTowards(_mech.transform.position, target, step);

        {
            Vector3 dir = target - _mech.transform.position;
            dir.y = 0;
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            _mech.transform.rotation = Quaternion.RotateTowards(_mech.transform.rotation, rotation, Time.deltaTime * rotateSpeed);
        }

        base.Run(_mech);
    }

    public override void OnChangedToThis(MechHeavenblazerEnemy _mech)
    {
        _mech.AnimatorPlayer.PlayAnimation(clip, 0.5f);

        base.OnChangedToThis(_mech);
    }

    public override int GetWeightDecision(MechHeavenblazerEnemy _mech)
    {

        return 1;
    }

}
