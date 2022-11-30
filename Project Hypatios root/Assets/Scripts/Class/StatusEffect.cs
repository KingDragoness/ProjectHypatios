using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Kryz.CharacterStats;


public enum StatusEffectCategory
{
    //Generic status 0 - 49
    Nothing = 0,
    Fire = 2,
    Poison = 3,
    Paralyze = 4,
    MovementBonus,
    MaxHitpointBonus,
    DamageBonus,
    //Bonus Perks for Player 50 - 99
    RegenHPBonus = 50,
    KnockbackResistance,
    SoulBonus,
    DashCooldown,
    ShortcutDiscount,
    BossDamageBonus,
    //Bonus Damage weapons for Player 100-120
    BonusDamagePistol = 100,
    BonusDamageShotgun = 101,
    BonusDamageMelee = 102,
    BonusDamageRifle = 103,
    BonusDamageExotics = 104
}

/// <summary> 
/// Example:
/// GenericStatus.cs, modify targeted character stat
/// FireStatus.cs, deals 7 damage per second for 10 seconds (All, unstackable)
/// PoisonStatus.cs, deals 10 damage per second for 5 seconds (Biological only, unstackable)
/// ParalyzeStatus.cs, temporarily disables enemy for 5 seconds (Stuns enemy, unstackable)
/// </summary>


public abstract class BaseStatusEffect : MonoBehaviour
{
    [ShowInInspector] [ReadOnly] public Entity target;
    public float EffectTimer = 5f;
    public StatusEffectCategory statusCategoryType;
    public float Value = 0.1f;
    public string SourceID = "PermanentPerk";


    public abstract void ApplyEffect();

    internal void GenericApplyEffect()
    {
        var enemyScript = target as EnemyScript;
        var playerScript = target as CharacterScript;


        if (playerScript)
        {
            if (statusCategoryType == StatusEffectCategory.MovementBonus)
                playerScript.speedMultiplier.AddModifier(new StatModifier(Value, StatModType.PercentAdd, this.gameObject));

            if (statusCategoryType == StatusEffectCategory.MaxHitpointBonus)
                playerScript.Health.maxHealth.AddModifier(new StatModifier(Value, StatModType.Flat, this.gameObject));

            if (statusCategoryType == StatusEffectCategory.RegenHPBonus)
                playerScript.Health.healthRegen.AddModifier(new StatModifier(Value, StatModType.Flat, this.gameObject));
        }
        else if (enemyScript)
        {
            if (statusCategoryType == StatusEffectCategory.MovementBonus)
                enemyScript.Stats.MovementBonus.AddModifier(new StatModifier(Value, StatModType.Flat, this.gameObject));

            if (statusCategoryType == StatusEffectCategory.MaxHitpointBonus)
                enemyScript.Stats.MaxHitpoint.AddModifier(new StatModifier(Value, StatModType.Flat, this.gameObject));

            // if (statusCategoryType == StatusEffectCategory.RegenHPBonus), not implemented

        }
    }
    private void RemoveEffects()
    {
        var enemyScript = target as EnemyScript;
        var playerScript = target as CharacterScript;

        if (enemyScript != null)
        {
            enemyScript.Stats.MovementBonus.RemoveAllModifiersFromSource(gameObject);
            enemyScript.Stats.BaseDamage.RemoveAllModifiersFromSource(gameObject);
            enemyScript.Stats.Luck.RemoveAllModifiersFromSource(gameObject);
            enemyScript.Stats.Intelligence.RemoveAllModifiersFromSource(gameObject);
            enemyScript.Stats.MovementBonus.RemoveAllModifiersFromSource(gameObject);
            enemyScript.Stats.MaxHitpoint.RemoveAllModifiersFromSource(gameObject);

        }

        if (playerScript != null)
        {
            playerScript.speedMultiplier.RemoveAllModifiersFromSource(gameObject);
            playerScript.Health.maxHealth.RemoveAllModifiersFromSource(gameObject);
            playerScript.Health.healthRegen.RemoveAllModifiersFromSource(gameObject);

        }
    }

    private void OnDestroy()
    {
        RemoveEffects();
    }

    private float cooldown = 1f;

    public virtual void Update()
    {
        if (EffectTimer < 9999)
        {
            EffectTimer -= Time.deltaTime;
            if (EffectTimer < 0)
                Destroy(gameObject);
        }

        //updates 1 time a second
        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
            return;
        }

        cooldown = 1f;


    }

}
