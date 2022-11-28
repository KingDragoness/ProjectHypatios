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
        Environment,
        Ally
    }

    public float damage = 1;
    public float repulsionForce = 1;
    public float shakinessFactor = 0.5f;
    public float healthSpeed = 10f;
    public EnemyScript originEnemy;
    public DamageOrigin origin = DamageOrigin.Player;
}

public class UniversalDamage
{
    public static void TryDamage(DamageToken token, Transform hit, Transform origin)
    {
        var damageReceiver = hit.gameObject.GetComponent<damageReceiver>();
        var health = hit.gameObject.GetComponent<PlayerHealth>();

        if (damageReceiver != null)
        {
            damageReceiver.Attacked(token);
        }

        if (health != null)
        {
            if (Hypatios.Difficulty == Hypatios.GameDifficulty.Brutal)
            {
                token.damage *= 1.6f; token.healthSpeed *= 1.5f;
            }
            if (Hypatios.Difficulty == Hypatios.GameDifficulty.Normal)
            {
                token.damage *= 0.8f; token.healthSpeed *= 0.9f;
            }
            if (Hypatios.Difficulty == Hypatios.GameDifficulty.Casual)
            {
                token.damage *= 0.6f; token.healthSpeed *= 0.8f;
            }
            if (Hypatios.Difficulty == Hypatios.GameDifficulty.Peaceful)
            {
                token.damage *= 0f; token.healthSpeed *= 0.7f;
            }

            Hypatios.UI.SpawnIndicator.Spawn(origin);
            health.takeDamage(Mathf.RoundToInt(token.damage), token.healthSpeed, token.shakinessFactor);

        }
    }
}

public class damageReceiver : MonoBehaviour
{
    public EnemyScript enemyScript;
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
        if (Hypatios.Game.currentGamemode != FPSMainScript.CurrentGamemode.Elena)
            Hypatios.Game.RuntimeTutorialHelp("ENEMY", "Ammo conservation is critical. Shooting enemy at weak spots can deal additional damage. Try to hit them at weak spots!", "FirstEnemyHit");

        OnHit?.Invoke();
    }
}
