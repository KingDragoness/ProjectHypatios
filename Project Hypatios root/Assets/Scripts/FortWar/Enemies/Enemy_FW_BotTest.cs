using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System.Linq;

//Invader bot test
public class Enemy_FW_BotTest : Enemy_FW_Bot
{

    public enum State
    {
        AssaultInvader,
        CaptureCP,
        StrategicDefend,
        DefendCP,
        FollowPlayer
    }

    public enum Role
    {
        Capturer, //focus on capturing control point
        Defend //go to strategic area to defend
    }

    [FoldoutGroup("Modules")] public AIMod_AssaultInvader Mod_Assault;
    [FoldoutGroup("Modules")] public AIMod_CaptureCP Mod_CaptureCP;
    [FoldoutGroup("Modules")] public AIMod_InvaderStrategicDefend Mod_InvaderStrategy;
    [FoldoutGroup("Modules")] public AIMod_DefenderDefend Mod_DefendCP;
    [FoldoutGroup("Modules")] public AIMod_FollowPlayer Mod_FollowPlayer;
    [FoldoutGroup("BotTest")] public State currentState = State.CaptureCP;
    [FoldoutGroup("BotTest")] public Role currentRole = Role.Capturer;
    [FoldoutGroup("BotTest")] public bool isFollowingPlayer = false;

    [FoldoutGroup("Sensors")] public FW_AI_SensorEnemy sensor;
    [FoldoutGroup("Visuals")] public Transform v_target_Head;
    [FoldoutGroup("Visuals")] public Transform v_target_Weapon;
    [FoldoutGroup("Visuals")] public Transform v_FollowerLight;

    public override void Awake()
    {
        base.Awake();
        onAITick += RefreshAI;
    }

    public override void Start()
    {
        base.Start();
        float chance = Random.Range(0f, 1f);

        if (chance < 0.3f)
        {
            currentRole = Role.Defend;
        }

        gameObject.name += $"({currentRole})";
    }

   

    public override void Update()
    {
        ConditionalChecks();
        StateSet();
        UpdateVisuals();
        base.Update();
    }

    public void FollowPlayer()
    {
        if (isFollowingPlayer)
        {
            _chamberScript.RemoveFollower(this);
            isFollowingPlayer = false;

        }
        else
        {
            if (_chamberScript.AddFollower(this))
            {
                isFollowingPlayer = true;
            }
        }
    }

    public override void Die()
    {
        if (isFollowingPlayer)
            _chamberScript.RemoveFollower(this);

        base.Die();
    }

    public override void FixedUpdate()
    {
        //RefreshAI();
    }

    private void UpdateVisuals()
    {
        if (target != null)
        {
            Vector3 posTarget = GetCurrentTarget().position;
            Vector3 dir = transform.position - posTarget;
            dir.Normalize();
            v_target_Head.localPosition = dir * 6; v_target_Weapon.localPosition = dir * 6;

        }

        if (isFollowingPlayer)
            v_FollowerLight.gameObject.SetActive(true);
        else
            v_FollowerLight.gameObject.SetActive(false);
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
        else if (currentState == State.StrategicDefend)
        {
            currentModule = Mod_InvaderStrategy;
        }
        else if (currentState == State.DefendCP)
        {
            currentModule = Mod_DefendCP;
        }
        else if (currentState == State.FollowPlayer)
        {
            currentModule = Mod_FollowPlayer;
        }
    }

    private void ConditionalChecks()
    {

    }

    private int _timeSinceEnemyLastSeen = 80; //1 tick = 0.1s

    private void RefreshAI()
    {
        float distToPlayer = Mod_FollowPlayer.DistancePlayer;
        var botsInSight = sensor.GetBotsInSight(myUnit.AllianceEnemy()).ToList();
        bool anyBotsInSight = false; if (botsInSight.Count > 0) anyBotsInSight = true;
        bool isTargetBlocked = false;

        if (target != null)
        {
            isTargetBlocked = sensor.IsTargetBlocked(target);
        }

       
        if (anyBotsInSight)
        {
            _timeSinceEnemyLastSeen = 50;
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
            SetDefaultState();
        }


        if (isFollowingPlayer && distToPlayer > Mod_FollowPlayer.MustFollowRange && myUnit.Alliance == FW_Alliance.INVADER)
        {
            currentState = State.FollowPlayer;
        }
    }


    private void SetDefaultState()
    {
        if (myUnit.Alliance == FW_Alliance.INVADER)
        {
            if (isFollowingPlayer == false)
            {
                if (currentRole == Role.Capturer)
                {
                    currentState = State.CaptureCP;
                }
                else if (currentRole == Role.Defend)
                {
                    if (Mod_InvaderStrategy.IsStrategicPointFound())
                    {
                        currentState = State.StrategicDefend;
                    }
                    else if (Mod_InvaderStrategy.currentStrategicPoint == null)
                    {
                        currentState = State.CaptureCP;
                    }
                }
            }
            else
            {
                currentState = State.FollowPlayer;
            }
        }
        else
        {
            //Mod_DefendCP.IsStrategicPointFound();
            currentState = State.DefendCP;
        }
    }
}
