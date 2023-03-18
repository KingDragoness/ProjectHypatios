using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RetardNavPathDistCheck : MonoBehaviour
{

    public Transform[] pathToCheck;
    public NavMeshAgent agentTest;

    private void Update()
    {
        foreach(var t in pathToCheck)
        {
            PathCheck(t);
        }
    }

    private void PathCheck(Transform origin)
    {
        NavMeshPath navMeshPath = new NavMeshPath();

        if (agentTest.CalculatePath(origin.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            var length = navMeshPath.GetPathLength();
            origin.gameObject.name = $"Object ({Mathf.Round(length*1000)/1000}m)";
        }
        else
        {
            var length = navMeshPath.GetPathLength();
            origin.gameObject.name = $"Object FAIL! ({length}m)";
        }
    }
}
