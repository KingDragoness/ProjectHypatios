using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Animancer;

[CreateAssetMenu(fileName = "Stance_DivineJudgement ", menuName = "Hypatios/Vendrich AI/Stance_DivineJudgement", order = 1)]
public class HB_Stance_DivineJudgement : HB_AIPackage
{

    public ClipTransition clipDivineFlyUp;
    public ClipTransition clipDivineFlyDown;
    public float Y_PosMaxDivine = 400f;
    public float Y_PosLandDivine = 0.5f;
    public float Y_PosAutoCorrectControl = 110f;
    public float moveSpeedUp = 20f;
    public float moveSpeedDown = 80f;
    public float moveTowardPlayerSpeed = 6f;


    public override void Run(MechHeavenblazerEnemy _mech)
    {
        var target = Hypatios.Player.transform.position;
        var step = moveTowardPlayerSpeed * Time.deltaTime;

        if (_mech.has_reached_divineMaxHeight == false)
        {
            _mech.transform.Translate(Vector3.up * Time.deltaTime * moveSpeedUp);
        }


        if (_mech.has_done_divineIntervention == false)
        {

            if (_mech.has_reached_divineMaxHeight == true)
            {
                _mech.transform.Translate(Vector3.down * Time.deltaTime * moveSpeedDown);
                if (_mech.transform.position.y > Y_PosAutoCorrectControl) 
                    _mech.transform.position = Vector3.MoveTowards(_mech.transform.position, target, step);
            }

            if (_mech.RefreshChangeStageTime < 1f)
            {
                _mech.DEBUG_AddAIRefreshTime(1f);
            }

            if (_mech.transform.position.y < Y_PosLandDivine && _mech.has_reached_divineMaxHeight)
            {
                //duar explosion
                _mech.DEBUG_SpawnDivineBomb();
                _mech.has_done_divineIntervention = true;
                _mech.AnimatorPlayer.PlayAnimation(clip, 0.5f);
            }

        }

        if (_mech.has_reached_divineMaxHeight == false)
        {
            if (_mech.transform.position.y > Y_PosMaxDivine)
            {
                _mech.has_reached_divineMaxHeight = true;
                _mech.AnimatorPlayer.PlayAnimation(clipDivineFlyDown, 0.5f);
            }
        }


        base.Run(_mech);
    }


    public override void OnChangedToThis(MechHeavenblazerEnemy _mech)
    {
        _mech.AnimatorPlayer.PlayAnimation(clipDivineFlyUp, 0.5f);

        base.OnChangedToThis(_mech);
    }

    public override void NotRun(MechHeavenblazerEnemy _mech)
    {
        _mech.has_done_divineIntervention = false;
        _mech.has_reached_divineMaxHeight = false;
        base.NotRun(_mech);
    }

    public override int GetWeightDecision(MechHeavenblazerEnemy _mech)
    {
        int weight = 3;

        if (_mech.has_done_divineIntervention == true)
        {
            weight = -99;
        }

        return weight;
    }

}
