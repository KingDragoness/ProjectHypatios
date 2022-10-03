using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AIMod_DefenderDefend : FortWar_AIModule
{
    [ReadOnly] public FW_StrategicPoint currentStrategicPoint;

    private bool cspExists = false;
    private Vector3 targetDefendPoint;

    private FW_StrategicPoint GetStrategicPoint()
    {
        var targeted = Chamber_Level7.instance.strategicPoints.Find(x => x.IsOccupied == false 
        && x.IsCurrentCP() == true
        && x.type == FW_StrategicPoint.Type.DefenderPoint);

        //pass 1
        if (targeted == null)
        {
            var allPoints = Chamber_Level7.instance.strategicPoints.FindAll(x => x.IsCurrentCP() == true
            && x.type == FW_StrategicPoint.Type.DefenderPoint);

            if (allPoints.Count > 0)
            {
                targeted = allPoints[Random.Range(0, allPoints.Count)];
            }
        }

        return targeted;

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
                if (currentStrategicPoint.IsClearedPoint())
                    ClearStrategicPoint();

            if (currentStrategicPoint == null)
                currentStrategicPoint = GetStrategicPoint();

            if (currentStrategicPoint == null)
            {
                cspExists = false;
                return;
            }
        }

        if (cspExists == false)
        {
            targetDefendPoint = currentStrategicPoint.randomSpawnArea.GetAnyPositionInsideBox();
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
