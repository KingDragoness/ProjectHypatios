using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Animancer;

[CreateAssetMenu(fileName = "Stance_SwordAttack", menuName = "Hypatios/Vendrich AI/Stance_SwordAttack", order = 1)]
public class HB_Stance_SwordAttack : HB_AIPackage
{

    public float sword_PointHit_Y = 0.1f;

    public override void Run(MechHeavenblazerEnemy _mech)
    {

        _mech.timer_SwordAttack += Time.deltaTime;
        _mech.initiate_Attack = false;

        if ((clip.Clip.length - 0.1f) > _mech.timer_SwordAttack)
        {
            if (_mech.RefreshChangeStageTime < 0.1f)
            {
                _mech.DEBUG_AddAIRefreshTime(0.1f);
            }
        }

        if (_mech.lastSword_PointOfHit.transform.position.y < sword_PointHit_Y
            && _mech.has_hit_lastSword == false)
        {
            _mech.has_hit_lastSword = true;
            _mech.DEBUG_SpawnLastSwordBomb();
        }
    }

    public override void OnChangedToThis(MechHeavenblazerEnemy _mech)
    {
        _mech.AnimatorPlayer.PlayAnimation(clip, 0.3f);
        _mech.has_hit_lastSword = false;
        base.OnChangedToThis(_mech);
    }

    public override void NotRun(MechHeavenblazerEnemy _mech)
    {
        _mech.timer_SwordAttack = 0f;
        _mech.has_hit_lastSword = false;
        base.NotRun(_mech);
    }

    public override int GetWeightDecision(MechHeavenblazerEnemy _mech)
    {
        int value = 0;

        if (_mech.initiate_Attack)
        {
            value = 1000;
        }

        return value;
    }
}
