using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class AIMod_FollowPlayer : FortWar_AIModule
{

    public float MustFollowRange = 10f;
    public bool destinationValid;

    [ReadOnly] [SerializeField] bool playerCanBeReached = false;

    private CharacterScript player;

    private void Start()
    {
        player = Hypatios.Player;
    }

    public float DistancePlayer
    {
        get { return Vector3.Distance(transform.position, player.transform.position); }
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
     
        BotScript.Agent.updateRotation = true;
        BotScript.Agent.stoppingDistance = 4f;

        Vector3 target = player.transform.position + new Vector3(0, 0, 1);


        destinationValid = BotScript.Agent.SetDestination(target);
        CheckCalculate();

        if (playerCanBeReached == false)
        {
            //destinationValid = BotScript.Agent.SetDestination(Hypatios.Enemy.CheckTargetClosestPosition(target));
        }
    }

    private void CheckCalculate()
    {
        NavMeshPath navMeshPath = new NavMeshPath();

        if (BotScript.Agent.CalculatePath(player.transform.position, navMeshPath))
        {
            playerCanBeReached = true;
        }
        else
        {
            playerCanBeReached = false;
        }

    }

    public override void OnChangedState(FortWar_AIModule currentModule)
    {
        if (currentModule == this)
        {

        }
    }

}
