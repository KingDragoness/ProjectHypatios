using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FW_AI_SensorEnemy : MonoBehaviour
{

    public Transform[] sensors;
    public float limitRange = 100f;
    public LayerMask layerMask;
    public Enemy_FW_BotTest botScript;

    private Chamber_Level7 _chamberScript;
    private List<Entity> allBots = new List<Entity>();

    public virtual void Awake()
    {
        _chamberScript = Chamber_Level7.instance;
    }

    public Entity[] GetBotsInSight(EnemyScript mySelf)
    {
        allBots.Clear();
        var ListAllUnits = Hypatios.Enemy.RetrieveAllEnemies(mySelf);//_chamberScript.RetrieveAllUnitsOfType(enemyForce); DEPRECATED, replaced with the new enemy system

        if (mySelf.Stats.MainAlliance != Alliance.Player)
        {
            float dist = Vector3.Distance(transform.position, Hypatios.Player.transform.position);

            if (CheckTargetVisibleOnSight(Hypatios.Player.transform) && dist < limitRange)
                allBots.Add(Hypatios.Player);
        }

        foreach (var enemy in ListAllUnits)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);

            if (CheckTargetVisibleOnSight(enemy.transform) && dist < limitRange)
                allBots.Add(enemy);
        }

        allBots = allBots.OrderBy(
        bot => Vector3.Distance(this.transform.position, bot.transform.position)).ToList();

        return allBots.ToArray();
    }

    public bool IsTargetBlocked(Transform t)
    {
        return !CheckTargetVisibleOnSight(t);
    }

    private bool CheckTargetVisibleOnSight(Transform t)
    {
        foreach (var s1 in sensors)
        {
            if (s1 == null) continue;

            Vector3 t1 = t.position; t1.y += 0.4f;
            Vector3 dir = t1 - s1.position;
            dir.Normalize();
            RaycastHit hit;

            if (Physics.Raycast(s1.position, dir, out hit, limitRange, layerMask))
            {
                if (hit.collider.gameObject.IsParentOf(t.gameObject) | hit.collider.gameObject == t.gameObject)
                {
                    //Debug.DrawRay(s1.position, dir * hit.distance, Color.blue);
                    return true;
                }
                else
                {
                    //Debug.DrawRay(s1.position, dir * hit.distance, Color.white);

                }
            }
        }

        return false;
    }

}
