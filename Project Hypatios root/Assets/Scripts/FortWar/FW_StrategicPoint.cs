using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class FW_StrategicPoint : MonoBehaviour
{
    
    public enum Type
    {
        InvaderCPArea,
        DefenderPoint
    }

    public int controlPoint = 0; //0 = final
    public RandomSpawnArea randomSpawnArea;
    public Type type;

    [ReadOnly] public Enemy_FW_Bot currentBot;

    private void Awake()
    {
        Chamber_Level7.instance.strategicPoints.Add(this);
    }

    public void AssignBot(Enemy_FW_Bot bot)
    {
        currentBot = bot;
    }

    public void Unassign()
    {
        currentBot = null;
    }

    public bool IsOccupied { get { return currentBot;} }


    [Button("Toggle RandomSpawnArea")]

    private void ToggleRandomSpawn()
    {
        if (randomSpawnArea.DEBUG_DrawGizmos == true)
            randomSpawnArea.DEBUG_DrawGizmos = false;
        else
            randomSpawnArea.DEBUG_DrawGizmos = true;
    }


    public bool IsClearedPoint()
    {
        var cp = Chamber_Level7.instance.controlPoint.Find(x => x.CPNumber == controlPoint);
        if (cp == null) return true;

        if (cp.isCaptured) return true;
        else return false;
    }

    public bool IsCurrentCP()
    {
        var cp = Chamber_Level7.instance.controlPoint.Find(x => x.isCaptured == false);

        //Debug.Log(cp.CPNumber);
        if (cp.CPNumber == controlPoint)
        {
            return true;
        }
        else
        {
            return false;
        }

        return cp;
    }

}
