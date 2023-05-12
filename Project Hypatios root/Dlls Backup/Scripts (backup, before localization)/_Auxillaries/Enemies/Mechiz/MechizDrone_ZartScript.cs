using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechizDrone_ZartScript : MonoBehaviour
{

    public Chamber_Level6 chamberScript;
    public MechizDroneMonster mechizDrone;

    private void Start()
    {
        mechizDrone.target = FindTarget();

        if (mechizDrone.target == null)
        {
            mechizDrone.hitpoint = -1;
        }
    }

    private Transform FindTarget()
    {
        int ix = Random.Range(0, chamberScript.allCustomers.Count);
        var enemy = chamberScript.allCustomers[ix];

        return enemy.transform;
    }

}
