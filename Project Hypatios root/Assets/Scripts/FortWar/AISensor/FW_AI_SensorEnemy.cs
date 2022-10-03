using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FW_AI_SensorEnemy : MonoBehaviour
{

    public Transform[] sensors;
    public float limitRange = 100f;
    public LayerMask layerMask;
    private Chamber_Level7 _chamberScript;

    public virtual void Awake()
    {
        _chamberScript = Chamber_Level7.instance;
    }

    public FW_Targetable[] GetBotsInSight(FW_Alliance enemyForce)
    {
        List<FW_Targetable> allBots = new List<FW_Targetable>();

        foreach(var enemy in _chamberScript.RetrieveAllUnitsOfType(enemyForce))
        {
            if (CheckTargetVisibleOnSight(enemy.transform))
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
        foreach(var s1 in sensors)
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
                    Debug.DrawRay(s1.position, dir * hit.distance, Color.blue);
                    return true;
                }
                else
                {
                    Debug.DrawRay(s1.position, dir * hit.distance, Color.white);

                }
            }
        }

        return false;
    }

}
