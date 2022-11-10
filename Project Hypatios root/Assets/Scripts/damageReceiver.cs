using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DamageToken
{
    public enum DamageOrigin
    {
        Player,
        Enemy,
        Environment
    }

    public float damage = 1;
    public float repulsionForce = 1;
    public DamageOrigin origin = DamageOrigin.Player;
}

public class damageReceiver : MonoBehaviour
{
    public Enemy enemyScript;
    public bool isCriticalHit = false;
    [Tooltip("> 1 for weak spots. < 1 for resistant spots.")]
    public float multiplier = 1f;

    [Space]
    public UnityEvent OnHit;

    public void Attacked(DamageToken token)
    {
        token.damage *= multiplier;

        if (enemyScript != null) enemyScript.Attacked(token);

        if (isCriticalHit && token.origin == DamageToken.DamageOrigin.Player)
        {
            soundManagerScript.instance.Play("bingo");
        }

        //Debug.Log(gameObject.name);
        if (FPSMainScript.instance.currentGamemode != FPSMainScript.CurrentGamemode.Elena)
            FPSMainScript.instance.RuntimeTutorialHelp("ENEMY", "Ammo conservation is critical. Shooting enemy at weak spots can deal additional damage. Try to hit them at weak spots!", "FirstEnemyHit");

        OnHit?.Invoke();
    }
}
