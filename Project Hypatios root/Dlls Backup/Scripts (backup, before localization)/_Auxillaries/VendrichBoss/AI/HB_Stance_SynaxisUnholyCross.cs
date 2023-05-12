using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Stance_SynaxisUnholyCross", menuName = "Hypatios/Vendrich AI/Stance_SynaxisUnholyCross", order = 1)]
public class HB_Stance_SynaxisUnholyCross : HB_AIPackage
{

    public float Time_CallingHeaven = 4f;
    public float Time_SpawnSynaxis = 5f;

    public override void Run(MechHeavenblazerEnemy _mech)
    {
        _mech.timerSynaxisFire += Time.deltaTime;

        if (_mech.timerSynaxisFire > Time_CallingHeaven && _mech.has_spawned_synaxisHeaven == false)
        {
            _mech.spawn_Synaxis.transform.position = Hypatios.Player.transform.position;
            _mech.DEBUG_SpawnHeavenSynaxis();
            _mech.has_spawned_synaxisHeaven = true;
        }
        
        if (_mech.timerSynaxisFire > Time_SpawnSynaxis && _mech.has_spawned_synaxisUnholy == false)
        {
            _mech.DEBUG_SpawnSynaxis();
            _mech.has_spawned_synaxisUnholy = true;
        }

        base.Run(_mech);
    }


    public override void OnChangedToThis(MechHeavenblazerEnemy _mech)
    {
        _mech.AnimatorPlayer.PlayAnimation(clip, 0.5f);

        base.OnChangedToThis(_mech);
    }

    public override void NotRun(MechHeavenblazerEnemy _mech)
    {
        _mech.timerSynaxisFire = 0f;
        _mech.has_spawned_synaxisHeaven = false;
        _mech.has_spawned_synaxisUnholy = false;
        base.NotRun(_mech);
    }

    public override int GetWeightDecision(MechHeavenblazerEnemy _mech)
    {
        int weight = 5;

        if (_mech.has_spawned_synaxisHeaven == true)
        {
            weight = -10;
        }

        return weight;
    }

}
