using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AIMod_InvaderStrategicDefend : FortWar_AIModule
{

    [ReadOnly] public FW_StrategicPoint currentStrategicPoint;

    [SerializeField]
    private bool cspExists = false;
    private Vector3 targetDefendPoint;

    private FW_StrategicPoint GetStrategicPoint()
    {
        var targeted = Chamber_Level7.instance.strategicPoints.Find(x => x.IsOccupied == false
        && x.IsCurrentCP() == true
        && x.type == FW_StrategicPoint.Type.InvaderCPArea);

        //pass 1
        if (targeted == null)
        {
            var allPoints = Chamber_Level7.instance.strategicPoints.FindAll(x => x.IsCurrentCP() == true
            && x.type == FW_StrategicPoint.Type.InvaderCPArea);

            if (allPoints.Count > 0)
            {
                targeted = allPoints[Random.Range(0, allPoints.Count)];
            }
        }

        return targeted;
    }
    public override void OnChangedState(FortWar_AIModule currentModule)
    {
        if (currentModule == this)
        {

        }
    }

    public FW_ControlPoint GetMyCP()
    {
        return Chamber_Level7.instance.GetCurrentCP();
    }

    public bool IsStrategicPointFound()
    {
        var st1 = GetStrategicPoint();

        if (st1 != null) return true; else return false;
    }

    public override void Run()
    {
        if (BotScript.Agent.isStopped == true)
            BotScript.Agent.Resume();
        BotScript.Agent.updateRotation = true;

        {
            if (currentStrategicPoint != null)
            {
                if (currentStrategicPoint.IsClearedPoint())
                {
                    ClearStrategicPoint();
                }
            }

            if (currentStrategicPoint == null)
            {
                currentStrategicPoint = GetStrategicPoint();
            }
        }

        if (currentStrategicPoint == null)
        {
            if (cspExists) targetDefendPoint = GetMyCP().areaCP.GetAnyPositionInsideBox();
            Vector3 targetCP1 = targetDefendPoint;
            BotScript.Agent.destination = targetCP1;
            cspExists = false;

            return;
        }


        float dist = Vector3.Distance(transform.position, targetDefendPoint);

        if (dist < 2.1f) { targetDefendPoint = currentStrategicPoint.randomSpawnArea.GetAnyPositionInsideBox(); }

        //test
        Vector3 targetCP = targetDefendPoint;
        currentStrategicPoint.AssignBot(BotScript);
        cspExists = true;
        BotScript.Agent.destination = targetCP;
    }

    private void ClearStrategicPoint()
    {
        currentStrategicPoint.Unassign();
        currentStrategicPoint = null;
    }

}
