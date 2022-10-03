using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public class AIMod_CaptureCP : FortWar_AIModule
{

    private FW_ControlPoint GetFirstUncapturedCP()
    {
        return Chamber_Level7.instance.controlPoint.Find(x => x.isCaptured == false);
    }

    public override void Run()
    {
        if (BotScript.Agent.isStopped == true)
            BotScript.Agent.Resume();
        BotScript.Agent.updateRotation = true;
        BotScript.Agent.stoppingDistance = 2f;

        //test
        Vector3 targetCP = GetFirstUncapturedCP().transform.position;
        BotScript.Agent.destination = targetCP;
    }

   

}
