using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Stance_Laser", menuName = "Hypatios/Vendrich AI/Stance_Laser", order = 1)]
public class HB_Stance_Laser : HB_AIPackage
{

    public float laserRotateSpeed = 6f;



    public override void Run(MechHeavenblazerEnemy _mech)
    {
        var target = Hypatios.Player.transform;

        {

        }

        //raycast from laser origin
        Vector3 origin = _mech.laser_Origin.position;
        bool isHittingSomething = false;
        bool isHittingTarget = false;

        RaycastHit hit;

        {
            Vector3 dir = target.position - _mech.laser_Origin.position;
            Quaternion rotation = Quaternion.LookRotation(dir);
            _mech.laser_Origin.rotation = Quaternion.RotateTowards(_mech.laser_Origin.rotation, rotation, Time.deltaTime * laserRotateSpeed);
        }

        if (Physics.Raycast(origin, _mech.laser_Origin.forward, out hit, 1000f, Hypatios.Enemy.baseSolidLayer, QueryTriggerInteraction.Ignore))
        {
            isHittingSomething = true;
        }

        if (isHittingSomething)
        {
            EnableLaserSystem(_mech, true);
            Vector3[] v3 = new Vector3[2];
            v3[0] = origin;
            v3[1] = hit.point;
            _mech.laser_Sparks.transform.position = hit.point;
            _mech.laser_LineRendr.SetPositions(v3);
        }
        else
        {
            EnableLaserSystem(_mech, false);

        }


        base.Run(_mech);
    }

    private void EnableLaserSystem(MechHeavenblazerEnemy _mech, bool enable)
    {
        if (_mech.laser_LineRendr.gameObject.activeSelf != enable)
            _mech.laser_LineRendr.gameObject.SetActive(enable);
        if (_mech.laser_Sparks.gameObject.activeSelf != enable)
            _mech.laser_Sparks.gameObject.SetActive(enable);
    }

    public override void NotRun(MechHeavenblazerEnemy _mech)
    {
        EnableLaserSystem(_mech, false);

        base.NotRun(_mech);
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
