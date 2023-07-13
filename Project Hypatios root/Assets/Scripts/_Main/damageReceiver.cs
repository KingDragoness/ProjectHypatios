using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

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
        PlayerPunch = 101
    }

    public float damage = 1;
    public float repulsionForce = 1;
    public float shakinessFactor = 0.5f;
    public float healthSpeed = 10f;
    public bool isBurn = false;
    public bool isPoison = false;
    public bool allowPlayerIndicator = false;
    public EnemyScript originEnemy;
    public Vector3 originAttackPosition;
    public DamageType damageType = DamageType.Generic;
    public DamageOrigin origin = DamageOrigin.Player;

}

public class UniversalDamage
{
    public static void TryDamage(DamageToken token, Transform hit, Transform origin)
    {
        var damageReceiver = hit.gameObject.GetComponent<damageReceiver>();
        var health = hit.gameObject.GetComponent<PlayerHealth>();
        token.originAttackPosition = origin.position;

        if (damageReceiver != null)
        {
            damageReceiver.Attacked(token);

            if (token.allowPlayerIndicator && damageReceiver.enemyScript != null)
            {
                Hypatios.Player.Weapon.ActivateCrosshairHit();
            }
        }

        if (health != null)
        {
            if (token.origin != DamageToken.DamageOrigin.Player)
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
                    token.damage *= 0.3f; token.healthSpeed *= 0.4f;
                }
                if (Hypatios.Difficulty == Hypatios.GameDifficulty.Peaceful)
                {
                    token.damage *= 0f; token.healthSpeed *= 0.7f;
                }
            }

            Hypatios.UI.SpawnIndicator.Spawn(origin);
            health.takeDamage(Mathf.RoundToInt(token.damage), token.healthSpeed, token.shakinessFactor);

           
            if (token.isBurn && !health.character.IsStatusEffect(ModifierEffectCategory.Fire)) health.character.Burn();
            if (token.isPoison && !health.character.IsStatusEffect(ModifierEffectCategory.Poison)) health.character.Poison();

        }
    }
}

[System.Serializable]
public class MyFloatEvent : UnityEvent<float>
{
}

public class damageReceiver : MonoBehaviour
{

    public enum BodyType
    {
        Default,
        CriticalSpot,
        Head = 20,
        Chest,
        LowerBody,
        LeftArm,
        RightArm,
        LeftLeg,
        RightLeg
    }

    public EnemyScript enemyScript;
    public Destructibles destructibleScript;
    public BodyType bodyType;
    public bool isCriticalHit = false;
    [Tooltip("> 1 for weak spots. < 1 for resistant spots.")]
    public float multiplier = 1f;
    public bool isPrintDebug = false;

    [Space]
    public UnityEvent OnHit;
    [HideInInspector] public MyFloatEvent m_MyEvent;


    [Button("Manual trigger")]
    public void BlankAttack()
    {
        DamageToken token = new DamageToken();
        Attacked(token);
    }

    internal void Attacked(DamageToken token)
    {
        if (isPrintDebug) Debug.Log(token.damage);
        token.damage *= multiplier;

        if (enemyScript != null)
        {
            enemyScript.Attacked(token);
            if (token.isBurn && !enemyScript.IsStatusEffect(ModifierEffectCategory.Fire) && token.originEnemy != enemyScript) enemyScript.Burn();
            if (token.isPoison && !enemyScript.IsStatusEffect(ModifierEffectCategory.Poison) && token.originEnemy != enemyScript && enemyScript.Stats.UnitType != UnitType.Mechanical) enemyScript.Poison();
        }

        if (destructibleScript != null)
        {
            destructibleScript.Damage(token);
        }

        if (isCriticalHit && token.origin == DamageToken.DamageOrigin.Player)
        {
            soundManagerScript.instance.Play("bingo");
            DamageOutputterUI.instance.DisplayText("CRIT!");

        }

        if (token.origin == DamageToken.DamageOrigin.Player)
            Hypatios.UI.mainHUDScript.bossUI.currentEnemy = enemyScript;

        //Debug.Log(gameObject.name);

        m_MyEvent.Invoke(token.damage);
        OnHit?.Invoke();
    }
}
