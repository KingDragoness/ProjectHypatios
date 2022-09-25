using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AIMod_CaptureCP : FortWar_AIModule
{
    public override void Run()
    {
        if (BotScript.Agent.isStopped == true)
            BotScript.Agent.Resume();
        BotScript.Agent.updateRotation = true;

        //test
        Vector3 targetCP = Chamber_Level7.instance.controlPoint[0].transform.position;
        BotScript.Agent.destination = targetCP;
    }

   

}
