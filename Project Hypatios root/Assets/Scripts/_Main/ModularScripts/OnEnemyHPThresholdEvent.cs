using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class OnEnemyHPThresholdEvent : MonoBehaviour
{
    
    [System.Serializable]
    public class HPEvent
    {
        public int hitpoint = 20000;
        public UnityEvent OnTriggerHP;
        public bool isTriggered = false;
    }

    public List<HPEvent> allHPEvents = new List<HPEvent>();
    public EnemyScript targetEnemy;

    private void Update()
    {
        if (Time.timeScale <= 0) return;
        if (targetEnemy == null) return;

        foreach(var trigger in allHPEvents)
        {
            if (trigger.isTriggered == true) continue;

            if (targetEnemy.Stats.CurrentHitpoint < trigger.hitpoint)
            {
                trigger.OnTriggerHP?.Invoke();
                trigger.isTriggered = true;
            }
        }

    }

}
