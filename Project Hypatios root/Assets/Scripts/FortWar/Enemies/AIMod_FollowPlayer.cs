using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMod_FollowPlayer : FortWar_AIModule
{

    public float MustFollowRange = 10f;

    private characterScript player;

    private void Start()
    {
        player = FindObjectOfType<characterScript>();
    }

    public float DistancePlayer
    {
        get { return Vector3.Distance(transform.position, player.transform.position); }
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

    public override void OnChangedState(FortWar_AIModule currentModule)
    {
        if (currentModule == this)
        {

        }
    }

}
