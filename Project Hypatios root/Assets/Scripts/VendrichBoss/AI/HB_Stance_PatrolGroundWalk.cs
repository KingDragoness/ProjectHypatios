using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Stance_PatrolGroundWalk", menuName = "Hypatios/Vendrich AI/Stance_PatrolGroundWalk", order = 1)]

public class HB_Stance_PatrolGroundWalk : HB_AIPackage
{

    public float distanceToTarget = 10f;
    public float aggroPlayerRange = 20f;
    public float walkSpeed = 6f;
    public float rotateSpeed = 6f;
    public float transitionSpeed = 1.5f;

    public override void Run(MechHeavenblazerEnemy _mech)
    {

        var target = _mech.patrolground_WalkTarget;
        float dist = Vector3.Distance(_mech.transform.position, target);
        float distPlayer = Vector3.Distance(_mech.transform.position, Hypatios.Player.transform.position);


        if (dist < distanceToTarget)
        {
            _mech.patrolground_WalkTarget = _mech.PatrolRegion.GetAnyPositionInsideBox();
      
        }
        if (aggroPlayerRange > distPlayer)
        {
            float chance = Random.Range(0f, 1f);

            Vector3 vPos = Hypatios.Player.transform.position;
            vPos.y = _mech.PatrolRegion.transform.position.y;

            if (_mech.PatrolRegion.IsInsideOcclusionBox(vPos))
            {
                _mech.patrolground_WalkTarget = vPos;
            }
            else
            {
            }
        }

        var step = walkSpeed * Time.deltaTime;
        _mech.transform.position = Vector3.MoveTowards(_mech.transform.position, target, step);

        {
            Vector3 dir = target - _mech.transform.position;
            dir.y = 0;
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            _mech.transform.rotation = Quaternion.RotateTowards(_mech.transform.rotation, rotation, Time.deltaTime * rotateSpeed);
        }

        if (_mech.mainAnimator)
        {
            _mech.mainAnimator.SetBool("Walking", true);
        }

        base.Run(_mech);
    }

    public override void OnChangedToThis(MechHeavenblazerEnemy _mech)
    {
        _mech.AnimatorPlayer.PlayAnimation(clip, transitionSpeed);

        base.OnChangedToThis(_mech);
    }

    public override int GetWeightDecision(MechHeavenblazerEnemy _mech)
    {

        return 1;
    }

}
