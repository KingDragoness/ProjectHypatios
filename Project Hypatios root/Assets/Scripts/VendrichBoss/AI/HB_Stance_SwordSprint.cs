using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Animancer;

[CreateAssetMenu(fileName = "Stance_SwordSprint", menuName = "Hypatios/Vendrich AI/Stance_SwordSprint", order = 1)]

public class HB_Stance_SwordSprint : HB_AIPackage
{

    public ClipTransition turnLeft;
    public ClipTransition turnRight;
    public ClipTransition flying;
    public float chaseSpeed = 6f;
    public float distanceToTarget = 10f;
    public float rotateSpeed = 40f;
    public float maxY_Fly = 4f;


    public override void Run(MechHeavenblazerEnemy _mech)
    {

        var playerPos = Hypatios.Player.transform.position;
        playerPos.y = 0;
        bool isBehind = false;
        bool tooHigh = false;
        bool isPlayerOutOfBound = false;

        Vector3 relativePos = _mech.transform.InverseTransformPoint(_mech.swordChase_Target);
        if (relativePos.z < 0) isBehind = true;

        float dist = Vector3.Distance(_mech.transform.position, _mech.swordChase_Target);

        //looping until near the player
        if (dist > distanceToTarget)
        {
            if (_mech.RefreshChangeStageTime < 0.1f)
            {
                _mech.DEBUG_AddAIRefreshTime(0.1f);
            }
        }
        else
        {
            _mech.initiate_Attack = true;
            //enforce change
        }

        if (_mech.transform.position.y > maxY_Fly)
        {
            tooHigh = true;
        }

        {
            Vector3 vPos = Hypatios.Player.transform.position;
            vPos.y = _mech.PatrolRegion.transform.position.y;

            if (_mech.PatrolRegion.IsInsideOcclusionBox(vPos) == false)
            {
                _mech.swordChase_Target = _mech.PatrolRegion.GetClosestPoint(vPos);
                isPlayerOutOfBound = true;
            }
            else
            {
                _mech.swordChase_Target = playerPos;
            }
        }

        if (isBehind == false)
        {
            var step = chaseSpeed * Time.deltaTime;
            _mech.transform.position = Vector3.MoveTowards(_mech.transform.position, _mech.swordChase_Target, step);

            if (tooHigh == false)
            {
                if (_mech.AnimatorPlayer.IsPlayingClip(clip.Clip) == false)
                    _mech.AnimatorPlayer.PlayAnimation(clip, 0.4f);
            }

        }
        else if (isBehind == true && tooHigh == false)
        {
            if (_mech.AnimatorPlayer.IsPlayingClip(turnLeft.Clip) == false)
                _mech.AnimatorPlayer.PlayAnimation(turnLeft, 0.4f);
        }
        

        if (tooHigh == true)
        {
            Vector3 down = _mech.transform.position + Vector3.down * 2f;

            var step = chaseSpeed * Time.deltaTime;
            _mech.transform.position = Vector3.MoveTowards(_mech.transform.position, _mech.swordChase_Target, step);

            if (_mech.AnimatorPlayer.IsPlayingClip(flying.Clip) == false)
                _mech.AnimatorPlayer.PlayAnimation(flying, 0.4f);
        }

        {
            Vector3 dir = _mech.swordChase_Target - _mech.transform.position;

            if (isPlayerOutOfBound)
            {
                dir = playerPos - _mech.transform.position;
            }

            dir.y = 0;
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            _mech.transform.rotation = Quaternion.RotateTowards(_mech.transform.rotation, rotation, Time.deltaTime * rotateSpeed);
        }

        base.Run(_mech);
    }

    public override void OnChangedToThis(MechHeavenblazerEnemy _mech)
    {
        _mech.initiate_Attack = false;
        _mech.timer_SwordAttack = 0f;

        base.OnChangedToThis(_mech);
    }

    public override int GetWeightDecision(MechHeavenblazerEnemy _mech)
    {
        int value = 0;

        float dist = Vector3.Distance(_mech.transform.position, Hypatios.Player.transform.position);
        if (dist < distanceToTarget)
            return 0;

        return 9999;
    }
}
