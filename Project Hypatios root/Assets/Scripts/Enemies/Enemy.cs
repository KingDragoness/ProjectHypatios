using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Base class for all enemies

public class Enemy : MonoBehaviour
{
    public delegate void OnEnemyKilled(Enemy mySelf);

    public static event OnEnemyKilled onKilled;

    protected static void IAmDead(Enemy mySelf)
    {
        onKilled?.Invoke(mySelf);
    }

    public virtual void Attacked(float damage, float repulsionForce = 1f)
    {

    }

}
