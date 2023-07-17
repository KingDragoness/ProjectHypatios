using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class HackableGate_Enemy : EnemyScript
{

    public UnityEvent OnHacked; //open the door or something
    private bool hasBeenHacked = false;

    public override void Die()
    {
        //indestructible
    }

    private void Update()
    {
        if (Stats.MainAlliance == Alliance.Player)
        {
            if (hasBeenHacked == false)
            {
                OnHacked?.Invoke();
            }

            hasBeenHacked = true;
        }
    }
}
