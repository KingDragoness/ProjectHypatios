using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMod_FollowPlayer : FortWar_AIModule
{

    private characterScript player;

    private void Start()
    {
        player = FindObjectOfType<characterScript>();
    }

    public override void Run()
    {
        if (BotScript.Agent.isStopped == true)
            BotScript.Agent.Resume();
        BotScript.Agent.updateRotation = true;
        BotScript.Agent.stoppingDistance = 4f;

       Vector3 target = player.transform.position;
        BotScript.Agent.destination = target;
    }

}
