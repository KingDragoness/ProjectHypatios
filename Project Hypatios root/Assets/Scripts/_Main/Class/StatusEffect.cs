using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Kryz.CharacterStats;


public enum ModifierEffectCategory
{
    //Generic status 0 - 49
    Nothing = 0,
    Fire = 2,
    Poison = 3,
    Paralyze = 4,
    MovementBonus,
    MaxHitpointBonus,
    DamageBonus,
    MaxHPPercentage,
    ArmorRating,
    //Bonus Perks for Player 50 - 99
    RegenHPBonus = 50,
    KnockbackResistance,
    SoulBonus,
    DashCooldown,
    ShortcutDiscount,
    BossDamageBonus,
    Alcoholism,
    Recoil,
    RegenHPPercentage,
    Digestion,
    //Bonus Damage weapons for Player 100-120
    BonusDamageMelee = 99,
    BonusDamageGun = 100,
    BonusDamageShotgun = 102,
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


public abstract class BaseModifierEffect : MonoBehaviour
{
    [ShowInInspector] [ReadOnly] public Entity target;
    public float EffectTimer = 5f;
    public ModifierEffectCategory statusCategoryType;
    public float Value = 0.1f;
    public StatusEffectMono statusMono;
    public string SourceID = "PermanentPerk";

    public bool IsTiedToStatusMono
    {
        get
        {
            return statusMono != null;
        }
    }

    public abstract void ApplyEffect();

    internal void GenericApplyEffect()
    {
        var enemyScript = target as EnemyScript;
        var playerScript = target as CharacterScript;
        CleanupEffects();

        if (playerScript)
        {
            if (statusCategoryType == ModifierEffectCategory.MovementBonus)
                playerScript.speedMultiplier.AddModifier(new StatModifier(Value, StatModType.PercentAdd, this.gameObject));

            if (statusCategoryType == ModifierEffectCategory.MaxHitpointBonus)
                playerScript.Health.maxHealth.AddModifier(new StatModifier(Value, StatModType.Flat, this.gameObject));

            if (statusCategoryType == ModifierEffectCategory.MaxHPPercentage)
                playerScript.Health.maxHealth.AddModifier(new StatModifier(Value, StatModType.PercentMult, this.gameObject));

            if (statusCategoryType == ModifierEffectCategory.RegenHPBonus)
                playerScript.Health.healthRegen.AddModifier(new StatModifier(Value, StatModType.Flat, this.gameObject));

            if (statusCategoryType == ModifierEffectCategory.RegenHPPercentage)
                playerScript.Health.healthRegen.AddModifier(new StatModifier(Value, StatModType.PercentMult, this.gameObject));

            if (statusCategoryType == ModifierEffectCategory.KnockbackResistance)
                playerScript.Weapon.Recoil.knockbackResistance.AddModifier(new StatModifier(-Value, StatModType.Flat, this.gameObject));

            if (statusCategoryType == ModifierEffectCategory.Recoil)
                playerScript.Weapon.Recoil.baseRecoil.AddModifier(new StatModifier(Value, StatModType.PercentMult, this.gameObject));

            if (statusCategoryType == ModifierEffectCategory.BonusDamageMelee)
                playerScript.BonusDamageMelee.AddModifier(new StatModifier(Value, StatModType.Flat, this.gameObject));

            if (statusCategoryType == ModifierEffectCategory.BonusDamageGun)
                playerScript.BonusDamageGun.AddModifier(new StatModifier(Value, StatModType.Flat, this.gameObject));

            if (statusCategoryType == ModifierEffectCategory.DashCooldown)
                playerScript.dashCooldown.AddModifier(new StatModifier(Value, StatModType.Flat, this.gameObject));

            if (statusCategoryType == ModifierEffectCategory.ArmorRating)
                playerScript.Health.armorRating.AddModifier(new StatModifier(Value, StatModType.Flat, this.gameObject));

            if (statusCategoryType == ModifierEffectCategory.Digestion)
                playerScript.Health.digestion.AddModifier(new StatModifier(Value, StatModType.Flat, this.gameObject));

            var test1 = playerScript.BonusDamageMelee.Value; //prevent value bug
            test1 = playerScript.BonusDamageGun.Value;
            test1 = playerScript.Health.armorRating.Value;
        }
        else if (enemyScript)
        {
            if (statusCategoryType == ModifierEffectCategory.MovementBonus)
                enemyScript.Stats.MovementBonus.AddModifier(new StatModifier(Value, StatModType.Flat, this.gameObject));

            if (statusCategoryType == ModifierEffectCategory.MaxHitpointBonus)
                enemyScript.Stats.MaxHitpoint.AddModifier(new StatModifier(Value, StatModType.Flat, this.gameObject));

            if (statusCategoryType == ModifierEffectCategory.MaxHPPercentage)
                enemyScript.Stats.MaxHitpoint.AddModifier(new StatModifier(Value, StatModType.PercentMult, this.gameObject));

            // if (statusCategoryType == StatusEffectCategory.RegenHPBonus), not implemented

        }
    }

    private void CleanupEffects()
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
            playerScript.BonusDamageMelee.RemoveAllModifiersFromSource(gameObject);
            playerScript.BonusDamageGun.RemoveAllModifiersFromSource(gameObject);
            playerScript.Health.maxHealth.RemoveAllModifiersFromSource(gameObject);
            playerScript.Health.healthRegen.RemoveAllModifiersFromSource(gameObject);
            playerScript.Health.armorRating.RemoveAllModifiersFromSource(gameObject);
            playerScript.Health.digestion.RemoveAllModifiersFromSource(gameObject);
            playerScript.Weapon.Recoil.knockbackResistance.RemoveAllModifiersFromSource(gameObject);
            playerScript.Weapon.Recoil.baseRecoil.RemoveAllModifiersFromSource(gameObject);
            playerScript.dashCooldown.RemoveAllModifiersFromSource(gameObject);

        }
    }

    private void RemoveEffects()
    {
        CleanupEffects();

        if (target != null)
        {
            target.AllStatusInEffect.RemoveAll(x => x == null);
            target.AllStatusInEffect.Remove(this);
        }
    }

    public virtual void OnDestroy()
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
