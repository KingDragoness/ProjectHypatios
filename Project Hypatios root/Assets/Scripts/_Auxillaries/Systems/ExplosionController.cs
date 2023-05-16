using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{

    public float Damage = 25f;
    public List<KillZone> allDamageScripts = new List<KillZone>();

    private float[] originalDamageList;

    private void Awake()
    {
        originalDamageList = new float[allDamageScripts.Count];
    }

    private void OnEnable()
    {
        foreach(var damageScript in allDamageScripts)
        {
            damageScript.DamagePerSecond = Damage;
        }
    }

    public void ChangeEnemyOrigin(EnemyScript enemyScript)
    {
        foreach (var damageScript in allDamageScripts)
        {
            damageScript.originEnemy = enemyScript;
        }
    }
}
