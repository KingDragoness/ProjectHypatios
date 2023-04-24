using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public class AIMod_CaptureCP : FortWar_AIModule
{

    public Vector3 targetPosition = new Vector3();
    public bool destinationValid;

    private FW_ControlPoint GetFirstUncapturedCP()
    {
        return Chamber_Level7.instance.GetCurrentCP();
    }

    private const float CALCULATION_TIMER = 0.5f;
    private float _recalcTimer = 0.5f;

    public override void Run()
    {
        _recalcTimer -= Time.deltaTime;

        if (_recalcTimer <= 0)
        {
            _recalcTimer = CALCULATION_TIMER;
            return;
        }


        targetPosition = GetFirstUncapturedCP().transform.position;
        BotScript.Agent.updateRotation = true;
        BotScript.Agent.stoppingDistance = 2f;
        destinationValid = BotScript.Agent.SetDestination(targetPosition);

    }

    public override void OnChangedState(FortWar_AIModule currentModule)
    {
        if (currentModule == this)
        {


        }
    }

}
