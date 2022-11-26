using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyContainer : MonoBehaviour
{

    [ReadOnly] [ShowInInspector] private List<EnemyScript> AllEnemies = new List<EnemyScript>();
    public System.Action<EnemyScript> OnEnemyDied;

    public void RegisterEnemy(EnemyScript enemy)
    {
        AllEnemies.Add(enemy);
    }

    public void DeregisterEnemy(EnemyScript enemy)
    {
        AllEnemies.Remove(enemy);
    }

}
