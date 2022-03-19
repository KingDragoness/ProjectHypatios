using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class damageReceiver : MonoBehaviour
{
    public Enemy enemyScript;
    public bool isCriticalHit = false;
    [Tooltip("> 1 for weak spots. < 1 for resistant spots.")]
    public float multiplier = 1f;

    [Space]
    public UnityEvent OnHit;

    public void Attacked(float damage, float repulsionForce = 1)
    {
        if (enemyScript != null) enemyScript.Attacked(damage * multiplier, repulsionForce);

        if (isCriticalHit)
        {
            soundManagerScript.instance.Play("bingo");
        }


        FPSMainScript.instance.RuntimeTutorialHelp("ENEMY", "Ammo conservation is critical. Shooting enemy at weak spots can deal additional damage. Try to hit them at weak spots!", "FirstEnemyHit");

        OnHit?.Invoke();
    }
}
