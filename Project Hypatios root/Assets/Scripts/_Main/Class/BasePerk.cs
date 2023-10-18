using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "MaxUpgradePerk", menuName = "Hypatios/BasePerks", order = 1)]
public class BasePerk : ScriptableObject
{

    public string PerkID = "";
    public int MAX_LEVEL = 1;
    public Sprite PerkSprite;
    public string ReaperDialogue;
    public string DescriptionPerk;
    public string TitlePerk;
    public float pricingPerValue = 1f;
    public bool TemporaryPerkOverLimit = false;
    public bool NoTemporaryPerk = false;
    public bool NoPermanentPerk = false;
    public float Commonness = 100; //100 is very common generate
    public ModifierEffectCategory category;


    public bool CheckLevelMaxed()
    {
        HypatiosSave.PerkDataSave perkDataClass = null;

        if (Hypatios.Player == null)
        {
            perkDataClass = FPSMainScript.savedata.AllPerkDatas;
        }
        else
        {
            perkDataClass = Hypatios.Player.PerkData;
        }

        if (category == ModifierEffectCategory.MaxHitpointBonus)
        {
            return false; //the only perk which player can upgrade forever
        }
        else if (category == ModifierEffectCategory.RegenHPBonus)
        {
            return perkDataClass.Perk_LV_RegenHitpointUpgrade >= MAX_LEVEL;
        }
        else if (category == ModifierEffectCategory.SoulBonus)
        {
            return perkDataClass.Perk_LV_Soulbonus >= MAX_LEVEL;
        }
        else if (category == ModifierEffectCategory.KnockbackResistance)
        {
            return perkDataClass.Perk_LV_KnockbackRecoil >= MAX_LEVEL;
        }
        else if (category == ModifierEffectCategory.Recoil)
        {
            return perkDataClass.Perk_LV_WeaponRecoil >= MAX_LEVEL;
        }
        else if (category == ModifierEffectCategory.DashCooldown)
        {
            return perkDataClass.Perk_LV_DashCooldown >= MAX_LEVEL;
        }
        else if (category == ModifierEffectCategory.ShortcutDiscount)
        {
            return perkDataClass.Perk_LV_ShortcutDiscount >= MAX_LEVEL;
        }
        else if (category == ModifierEffectCategory.BonusDamageMelee)
        {
            return perkDataClass.Perk_LV_IncreaseMeleeDamage >= MAX_LEVEL;
        }
        else if (category == ModifierEffectCategory.BonusDamageGun)
        {
            return perkDataClass.Perk_LV_IncreaseGunDamage >= MAX_LEVEL;
        }

        return false;

    }

    [FoldoutGroup("Quick Menu")]
    [Button("MaxHP")]
    public void Description_MaxHPUpgrade(float Value)
    {
        PerkID = "MaxHPUpgrade";
        MAX_LEVEL = 999;
        category = ModifierEffectCategory.MaxHitpointBonus;
        ReaperDialogue = $"\"Vitality upgrade by 7 HP. Make yourself tougher.\"";
        DescriptionPerk = "+7 Maximum Hitpoint";
        TemporaryPerkOverLimit = true;
        Commonness = 100;
        TitlePerk = "[MAX HP UPGRADE]";
    }

    [FoldoutGroup("Quick Menu")]
    [Button("RegenHP")]
    public void Description_RegenHPUpgrade(float Value)
    {
        PerkID = "RegenHPUpgrade";
        MAX_LEVEL = 20;
        category = ModifierEffectCategory.RegenHPBonus;
        ReaperDialogue = $"\"Regenerate hitpoint upgrades by +0.1 HP per second.\"";
        DescriptionPerk = "+0.1 HP per second";
        TemporaryPerkOverLimit = true;
        Commonness = 60;
        TitlePerk = "[REGEN HP UPGRADE]";
    }

    [FoldoutGroup("Quick Menu")]
    [Button("SoulBonus")]
    public void Description_SoulBonusUpgrade(float Value)
    {
        PerkID = "SoulBonusUpgrade";
        MAX_LEVEL = 5;
        category = ModifierEffectCategory.SoulBonus;
        ReaperDialogue = $"\"Increases your luck of finding souls.\"";
        DescriptionPerk = "+0.08 HP per second"; //needs custom description
        TemporaryPerkOverLimit = false;
        Commonness = 6;
        TitlePerk = "[SOUL BONUS UPGRADE]";
    }

    [FoldoutGroup("Quick Menu")]
    [Button("KnockResist")]
    public void Description_KnockbackUpgrade(float Value)
    {
        PerkID = "KnockbackUpgrade";
        MAX_LEVEL = 10;
        category = ModifierEffectCategory.KnockbackResistance;
        ReaperDialogue = $"\"Decreases physical knockback and damage shake effect.\"";
        DescriptionPerk = "Decreases knockback effect by -0.15 point.";
        TemporaryPerkOverLimit = false;
        Commonness = 10;
        TitlePerk = "[KNOCKBACK RESISTANCE UPGRADE]";
    }

    [FoldoutGroup("Quick Menu")]
    [Button("Recoil")]
    public void Description_RecoilUpgrade(float Value)
    {
        PerkID = "RecoilUpgrade";
        MAX_LEVEL = 12;
        category = ModifierEffectCategory.Recoil;
        ReaperDialogue = $"\"Decreases gun recoil effect.\"";
        DescriptionPerk = "Decreases recoil effect by -6%";
        TemporaryPerkOverLimit = false;
        Commonness = 10;
        TitlePerk = "[RECOIL UPGRADE]";
    }

    [FoldoutGroup("Quick Menu")]
    [Button("Dash Cooldown")]
    public void Description_DashCooldown(float Value)
    {
        PerkID = "DashCooldownUpgrade";
        MAX_LEVEL = 5;
        category = ModifierEffectCategory.DashCooldown;
        ReaperDialogue = $"\"Decreases dash cooldown by -0.2 seconds.\"";
        DescriptionPerk = "-0.2 seconds dash cooldown";
        TemporaryPerkOverLimit = false;
        Commonness = 10;
        TitlePerk = "[DASH COOLDOWN UPGRADE]";
    }

    [FoldoutGroup("Quick Menu")]
    [Button("Melee Damage")]
    public void Description_BonusMeleeUpgrade(float Value)
    {
        PerkID = "MeleeDamageUpgrade";
        MAX_LEVEL = 10;
        category = ModifierEffectCategory.BonusDamageMelee;
        ReaperDialogue = $"\"Increases melee damage by +12%.\"";
        DescriptionPerk = "+12% increases melee damage";
        TemporaryPerkOverLimit = true;
        Commonness = 25;
        TitlePerk = "[MELEE DAMAGE UPGRADE]";
    }

    [FoldoutGroup("Quick Menu")]
    [Button("Shortcut Discount")]
    public void Description_ShortcutDiscount(float Value)
    {
        PerkID = "ShortcutDiscount";
        MAX_LEVEL = 5;
        category = ModifierEffectCategory.ShortcutDiscount;
        ReaperDialogue = $"\"Discount for shortcuts.\"";
        DescriptionPerk = "+4% increases melee ";
        TemporaryPerkOverLimit = false;
        Commonness = 10;
        TitlePerk = "[SHORTCUT DISCOUNT]";
    }

    [FoldoutGroup("Quick Menu")]
    [Button("Gun Damage")]
    public void Description_GunDamage(float Value)
    {
        PerkID = "GunDamage";
        MAX_LEVEL = 5;
        category = ModifierEffectCategory.BonusDamageGun;
        ReaperDialogue = $"\"Bonus damage for all guns.\"";
        DescriptionPerk = "+5% bonus gun damage ";
        TemporaryPerkOverLimit = false;
        Commonness = 10;
        TitlePerk = "[GUN DAMAGE]";
    }

    public string GetDialogueTempPerk(float Value)
    {
        if (category == ModifierEffectCategory.MaxHitpointBonus)
        {
            return $"\"Vitality increased by {Value} HP, remember only lasts 1 run.\"";
        }
        else if (category == ModifierEffectCategory.RegenHPBonus)
        {
            return $"\"Regen increased by +{Value} HP per second, remember only lasts 1 run.\"";
        }
        else if (category == ModifierEffectCategory.KnockbackResistance)
        {
            return $"\"Decreases knockback by -{Value * 100}%, remember only lasts 1 run.\"";
        }
        else if (category == ModifierEffectCategory.Recoil)
        {
            return $"\"Decreases recoil by -{Value * 100}%, remember only lasts 1 run.\"";
        }
        else if (category == ModifierEffectCategory.DashCooldown)
        {
            return $"\"Decreases dash cooldown by -0.2 seconds, remember only lasts 1 run.\"";
        }
        else if (category == ModifierEffectCategory.BonusDamageMelee)
        {
            return $"\"Increases melee damage by +{Value * 100}%, remember only lasts 1 run.\"";
        }
        else if (category == ModifierEffectCategory.BonusDamageGun)
        {
            return $"\"Increases gun damage by +{Value * 100}%, remember only lasts 1 run.\"";
        }


        return "";
    }

    public string GetDescriptionTempPerk(float Value)
    {
        if (category == ModifierEffectCategory.MaxHitpointBonus)
            return $"+{Value} Maximum Hitpoint. Only lasts 1 run.";
        else if (category == ModifierEffectCategory.RegenHPBonus)
            return $"+{Value} HP per second. Only lasts 1 run.";
        else if (category == ModifierEffectCategory.KnockbackResistance)
            return $"Decreases knockback effect by -{Value*100}%. Only lasts 1 run.";
        else if (category == ModifierEffectCategory.Recoil)
            return $"Decreases recoil effect by -{Value * 100}%. Only lasts 1 run.";
        else if (category == ModifierEffectCategory.DashCooldown)
            return $"-0.2 seconds dash cooldown. Only lasts 1 run.";
        else if (category == ModifierEffectCategory.BonusDamageMelee)
            return $"+{Value * 100}% increases melee damage. Only lasts 1 run.";
        else if (category == ModifierEffectCategory.BonusDamageGun)
            return $"+{Value * 100}% increases gun damage. Only lasts 1 run.";
        return "";
    }


    public static float GenerateValueForCustomPerk(ModifierEffectCategory category)
    {
        float Value = 0;
        int run = 0;
        var perkClass = HypatiosSave.PerkDataSave.GetPerkDataSave();

        if (FPSMainScript.savedata != null)
            run = FPSMainScript.savedata.Game_TotalRuns;
        else
            run = 0;

        var hitpointNet = 100 + PlayerPerk.GetValue_MaxHPUpgrade(perkClass.Perk_LV_MaxHitpointUpgrade);

        if (category == ModifierEffectCategory.MaxHitpointBonus)
        {
            float percent = Random.Range(0.42f, 0.56f);
            
            Value = Mathf.Clamp(percent * hitpointNet, 40, 1000);
            Value = Mathf.Round(Value);
        }
        else if (category == ModifierEffectCategory.RegenHPBonus)
        {
            float percent = Random.Range(0.005f, 0.01f);

            Value = Mathf.Clamp(percent * hitpointNet, 0.1f, 100);
            Value = Mathf.Round(Value * 100f) / 100f;
        }
        else if (category == ModifierEffectCategory.SoulBonus)
            Value = 1;
        else if (category == ModifierEffectCategory.ShortcutDiscount)
            Value = 1;
        else if (category == ModifierEffectCategory.KnockbackResistance)
        {
            Value = Random.Range(0.4f, 0.8f);
            Value = Mathf.Round(Value * 100) / 100;
        }
        else if (category == ModifierEffectCategory.Recoil)
        {
            Value = Random.Range(0.2f, 0.4f);
            Value = Mathf.Round(Value * 100) / 100;
        }
        else if (category == ModifierEffectCategory.DashCooldown)
            Value = -0.2f;
        else if (category == ModifierEffectCategory.BonusDamageMelee)
        {
            Value = Random.Range(0.25f, 0.4f);

            if (run > 3)
                Value += Random.Range(run * 0.007f, run * 0.02f);

            Value = Mathf.Clamp(Value, 0f, 2f);
            Value = Mathf.Round(Value * 100) / 100;
        }
        else if (category == ModifierEffectCategory.BonusDamageGun)
        {
            Value = Random.Range(0.21f, 0.4f);

            if (run > 3)
                Value += Random.Range(run * 0.0075f, run * 0.022f);

            Value = Mathf.Clamp(Value, 0f, 2f);
            Value = Mathf.Round(Value * 100) / 100;
        }

        return Value;
    }
}
