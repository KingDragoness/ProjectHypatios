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
    public StatusEffectCategory category;


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

        if (category == StatusEffectCategory.MaxHitpointBonus)
        {
            return false; //the only perk which player can upgrade forever
        }
        else if (category == StatusEffectCategory.RegenHPBonus)
        {
            return perkDataClass.Perk_LV_RegenHitpointUpgrade >= MAX_LEVEL;
        }
        else if (category == StatusEffectCategory.SoulBonus)
        {
            return perkDataClass.Perk_LV_Soulbonus >= MAX_LEVEL;
        }
        else if (category == StatusEffectCategory.KnockbackResistance)
        {
            return perkDataClass.Perk_LV_KnockbackRecoil >= MAX_LEVEL;
        }
        else if (category == StatusEffectCategory.DashCooldown)
        {
            return perkDataClass.Perk_LV_DashCooldown >= MAX_LEVEL;
        }
        else if (category == StatusEffectCategory.ShortcutDiscount)
        {
            return perkDataClass.Perk_LV_ShortcutDiscount >= MAX_LEVEL;
        }
        else if (category == StatusEffectCategory.BonusDamageMelee)
        {
            return perkDataClass.Perk_LV_IncreaseMeleeDamage >= MAX_LEVEL;
        }

        return false;

    }

    [FoldoutGroup("Quick Menu")]
    [Button("MaxHP")]
    public void Description_MaxHPUpgrade(float Value)
    {
        PerkID = "MaxHPUpgrade";
        MAX_LEVEL = 999;
        category = StatusEffectCategory.MaxHitpointBonus;
        ReaperDialogue = $"\"Vitality upgrade by 5 HP. Make yourself tougher.\"";
        DescriptionPerk = "+5 Maximum Hitpoint";
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
        category = StatusEffectCategory.RegenHPBonus;
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
        category = StatusEffectCategory.SoulBonus;
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
        category = StatusEffectCategory.KnockbackResistance;
        ReaperDialogue = $"\"Decreases gun recoil effect and damage shake effect.\"";
        DescriptionPerk = "Decreases recoil effect by -0.15 ";
        TemporaryPerkOverLimit = false;
        Commonness = 10;
        TitlePerk = "[KNOCKBACK RESISTANCE UPGRADE]";
    }

    [FoldoutGroup("Quick Menu")]
    [Button("Dash Cooldown")]
    public void Description_DashCooldown(float Value)
    {
        PerkID = "DashCooldownUpgrade";
        MAX_LEVEL = 5;
        category = StatusEffectCategory.DashCooldown;
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
        category = StatusEffectCategory.BonusDamageMelee;
        ReaperDialogue = $"\"Increases melee damage by +4%.\"";
        DescriptionPerk = "+4% increases melee damage";
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
        category = StatusEffectCategory.ShortcutDiscount;
        ReaperDialogue = $"\"Discount for shortcuts.\"";
        DescriptionPerk = "+4% increases melee ";
        TemporaryPerkOverLimit = false;
        Commonness = 10;
        TitlePerk = "[SHORTCUT DISCOUNT]";
    }

    public string GetDialogueTempPerk(float Value)
    {
        if (category == StatusEffectCategory.MaxHitpointBonus)
        {
            return $"\"Vitality increased by {Value} HP, remember only lasts 1 run.\"";
        }
        else if (category == StatusEffectCategory.RegenHPBonus)
        {
            return $"\"Regen increased by +{Value} HP per second, remember only lasts 1 run.\"";
        }
        else if (category == StatusEffectCategory.KnockbackResistance)
        {
            return $"\"Decreases knockback by -{Value * 100}%, remember only lasts 1 run.\"";
        }
        else if (category == StatusEffectCategory.DashCooldown)
        {
            return $"\"Decreases dash cooldown by -0.2 seconds, remember only lasts 1 run.\"";
        }
        else if (category == StatusEffectCategory.BonusDamageMelee)
        {
            return $"\"Increases melee damage by +{Value}%, remember only lasts 1 run.\"";
        }

        return "";
    }

    public string GetDescriptionTempPerk(float Value)
    {
        if (category == StatusEffectCategory.MaxHitpointBonus)
            return $"+{Value} Maximum Hitpoint. Only lasts 1 run.";
        else if (category == StatusEffectCategory.RegenHPBonus)
            return $"+{Value} HP per second. Only lasts 1 run.";
        else if (category == StatusEffectCategory.KnockbackResistance)
            return $"Decreases recoil effect by -{Value*100}%. Only lasts 1 run.";
        else if (category == StatusEffectCategory.DashCooldown)
            return $"-0.2 seconds dash cooldown. Only lasts 1 run.";
        else if (category == StatusEffectCategory.BonusDamageMelee)
            return $"+{Value * 100}% increases melee damage. Only lasts 1 run.";

        return "";
    }


    public static float GenerateValueForCustomPerk(StatusEffectCategory category)
    {
        float Value = 0;
        int run = 0;
        var perkClass = HypatiosSave.PerkDataSave.GetPerkDataSave();

        if (FPSMainScript.savedata != null)
            run = FPSMainScript.savedata.Game_TotalRuns;
        else
            run = 0;

        var hitpointNet = 100 + PlayerPerk.GetValue_MaxHPUpgrade(perkClass.Perk_LV_MaxHitpointUpgrade);

        if (category == StatusEffectCategory.MaxHitpointBonus)
        {
            float percent = Random.Range(0.35f, 0.5f);
            
            Value = Mathf.Clamp(percent * hitpointNet, 40, 1000);
            Value = Mathf.Round(Value);
        }
        else if (category == StatusEffectCategory.RegenHPBonus)
        {
            float percent = Random.Range(0.005f, 0.01f);

            Value = Mathf.Clamp(percent * hitpointNet, 0.1f, 100);
            Value = Mathf.Round(Value * 100f) / 100f;
        }
        else if (category == StatusEffectCategory.SoulBonus)
            Value = 1;
        else if (category == StatusEffectCategory.ShortcutDiscount)
            Value = 1;
        else if (category == StatusEffectCategory.KnockbackResistance)
        {
            Value = Random.Range(0.25f, 0.5f);
            Value = Mathf.Round(Value * 100) / 100;
        }
        else if (category == StatusEffectCategory.DashCooldown)
            Value = -0.2f;
        else if (category == StatusEffectCategory.BonusDamageMelee)
        {
            Value = Random.Range(0.15f, 0.3f);

            if (run > 3)
                Value += Random.Range(run * 0.005f, run * 0.01f);

            Value = Mathf.Clamp(Value, 0f, 2f);
            Value = Mathf.Round(Value * 100) / 100;
        }

        return Value;
    }
}
