using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class AIMod_AssaultInvader : FortWar_AIModule
{

    public enum State
    {
        FIRE,
        CHASE,
        RETREAT,
        REPOSITION
    }

    public State currentState = State.CHASE;
    public float repositionRange = 3f;

    #region INPUTS 


    #endregion

    #region OUTPUTS


    #endregion


    /// <summary>
    /// 3 states: FIRE, CHASE, RETREAT
    /// </summary>
    public override void Run()
    {
        if (currentState == State.FIRE)
        {
            State_Fire();
        }
        else if (currentState == State.CHASE)
        {
            State_Chase();
        }
        else if (currentState == State.RETREAT)
        {
            State_Retreat();
        }
        else if (currentState == State.REPOSITION)
        {
            State_Reposition();
        }
    }

    private void ResumeMovement()
    {

        BotScript.Agent.updateRotation = true;
    }

    private void LockMovement()
    {
        BotScript.Agent.SetDestination(transform.position);
        BotScript.Agent.updateRotation = false;
    }

    private Vector3 SampleReposition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * repositionRange;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 25f, 1);
        Vector3 finalPosition = hit.position;

        return finalPosition;
    }

    public void State_Fire()
    {
        LockMovement();

        Vector3 dir = BotScript.GetCurrentTarget().position - transform.position;
        dir.y = 0;
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        BotScript.transform.rotation = rotation;
    }

    public void State_Chase()
    {
        ResumeMovement();

        BotScript.Agent.destination = BotScript.GetCurrentTarget().position;
    }

    public void State_Retreat()
    {
        ResumeMovement();


    }

    public void State_Reposition()
    {
        ResumeMovement();

        BotScript.Agent.destination = SampleReposition();

    }
}
