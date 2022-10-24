using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public class AIMod_CaptureCP : FortWar_AIModule
{

    public Vector3 targetPosition = new Vector3();

    private FW_ControlPoint GetFirstUncapturedCP()
    {
        return Chamber_Level7.instance.GetCurrentCP();
    }

    public override void Run()
    {
        targetPosition = GetFirstUncapturedCP().transform.position;
        BotScript.Agent.updateRotation = true;
        BotScript.Agent.stoppingDistance = 2f;
        BotScript.Agent.SetDestination(targetPosition);

    }

    public override void OnChangedState(FortWar_AIModule currentModule)
    {
        if (currentModule == this)
        {


        }
    }

}
