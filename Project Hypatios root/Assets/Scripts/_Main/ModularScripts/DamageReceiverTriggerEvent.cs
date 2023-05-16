using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class DamageReceiverTriggerEvent : MonoBehaviour
{

    public BaseEnemyStats enemyTarget;
    public UnityEvent OnEnemyTrigger;
    public bool isOneTimeOnly = false;

    private LayerMask enemyLayer = 12;
    private bool _triggered = false;
    private int _tickLastTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered && isOneTimeOnly == true)
        {
            return;
        }

        //Must not triggered at the same frame multiple times
        if (_tickLastTriggered == Hypatios.TimeTick)
            return;

        if (other.gameObject.layer == enemyLayer)
        {
            var damageReceiver = other.gameObject.GetComponentInChildren<damageReceiver>();
            if (damageReceiver != null)
            {
                var enemyScript = damageReceiver.enemyScript;
                if (enemyScript != null)
                {
                    if (enemyScript.EnemyName == enemyTarget.name)
                    {
                        OnEnemyTrigger?.Invoke();
                        _triggered = true;
                        _tickLastTriggered = Hypatios.TimeTick;
                    }
                }
            }
        }
     
    }
}
