using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

//Invader bot test
public class Enemy_FW_BotTest : Enemy_FW_Bot
{

    public enum State
    {
        AssaultInvader,
        CaptureCP
    }

    [FoldoutGroup("Modules")] public AIMod_AssaultInvader Mod_Assault;
    [FoldoutGroup("Modules")] public AIMod_CaptureCP Mod_CaptureCP;
    [FoldoutGroup("BotTest")] public State currentState = State.CaptureCP;

    [FoldoutGroup("Sensors")] public FW_AI_SensorEnemy sensor;

    public override void Awake()
    {
        base.Awake();
        onAITick += RefreshAI;
    }


    public override void Update()
    {
        ConditionalChecks();
        StateSet();
        base.Update();
    }

    public override void FixedUpdate()
    {
        //RefreshAI();
    }

    private void StateSet()
    {
        if (currentState == State.AssaultInvader)
        {
            currentModule = Mod_Assault;
        }
        else if(currentState == State.CaptureCP)
        {
            currentModule = Mod_CaptureCP;
        }
        
    }

    private void ConditionalChecks()
    {

    }

    private int _timeSinceEnemyLastSeen = 80; //1 tick = 0.1s

    private void RefreshAI()
    {
        var botsInSight = sensor.GetBotsInSight(myUnit.AllianceEnemy()).ToList();
        bool anyBotsInSight = false; if (botsInSight.Count > 0) anyBotsInSight = true;
        bool isTargetBlocked = false;

        if (target != null)
        {
            isTargetBlocked = sensor.IsTargetBlocked(target);
        }

       
        if (anyBotsInSight)
        {
            _timeSinceEnemyLastSeen = 80;
            target = botsInSight[0].transform;

            if (isTargetBlocked == false)
            {
                Mod_Assault.currentState = AIMod_AssaultInvader.State.FIRE;
            }
            else
            {
                Mod_Assault.currentState = AIMod_AssaultInvader.State.REPOSITION;
            }

            currentState = State.AssaultInvader;
        }
        else
        {
            Mod_Assault.currentState = AIMod_AssaultInvader.State.CHASE;
            _timeSinceEnemyLastSeen--;
        }

        if (anyBotsInSight == false && _timeSinceEnemyLastSeen < 0f)
        {
            currentState = State.CaptureCP;
        }

    }
}
