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

    public enum DamageType
    {
        Generic = 0,
        Ballistic = 1,
        Poison,
        Fire,
        WeakLaser = 10,
        MiningLaser = 11,
        Melee = 50,
        Explosion = 100,
    }

    public float damage = 1;
    public float repulsionForce = 1;
    public float shakinessFactor = 0.5f;
    public float healthSpeed = 10f;
    public bool isBurn = false;
    public bool isPoison = false;
    public bool allowPlayerIndicator = false;
    public EnemyScript originEnemy;
    public DamageType damageType = DamageType.Generic;
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

            if (token.allowPlayerIndicator)
            {
                Hypatios.Player.Weapon.ActivateCrosshairHit();
            }
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

           
            if (token.isBurn && !health.character.IsStatusEffect(StatusEffectCategory.Fire)) health.character.Burn();
            if (token.isPoison && !health.character.IsStatusEffect(StatusEffectCategory.Poison)) health.character.Poison();

        }
    }
}

public class damageReceiver : MonoBehaviour
{
    public EnemyScript enemyScript;
    public Destructibles destructibleScript;
    public bool isCriticalHit = false;
    [Tooltip("> 1 for weak spots. < 1 for resistant spots.")]
    public float multiplier = 1f;

    [Space]
    public UnityEvent OnHit;

    public void Attacked(DamageToken token)
    {
        token.damage *= multiplier;

        if (enemyScript != null)
        {
            enemyScript.Attacked(token);
            if (token.isBurn && !enemyScript.IsStatusEffect(StatusEffectCategory.Fire) && token.originEnemy != enemyScript) enemyScript.Burn();
            if (token.isPoison && !enemyScript.IsStatusEffect(StatusEffectCategory.Poison) && token.originEnemy != enemyScript) enemyScript.Poison();
        }

        if (destructibleScript != null)
        {
            destructibleScript.Damage(token);
        }

        if (isCriticalHit && token.origin == DamageToken.DamageOrigin.Player)
        {
            soundManagerScript.instance.Play("bingo");
        }

        //Debug.Log(gameObject.name);


        OnHit?.Invoke();
    }
}
