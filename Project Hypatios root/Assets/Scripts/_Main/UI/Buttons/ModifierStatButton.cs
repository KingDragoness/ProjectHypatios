using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class ModifierStatButton : MonoBehaviour
{

    public Text label_ModifierName;
    public Text label_Value;
    public Text label_Description;
    public Image icon;
    public ModifierEffectCategory category1;

    private BaseModifierEffectObject baseModifierClass;

    private void OnEnable()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        baseModifierClass = Hypatios.Assets.AllModifierEffects.Find(x => x.category == category1);
        int countModifiers = Hypatios.Player.GetTempStatusEffectByCount(category1);

        if (baseModifierClass.hasPerkUpgrade == true && countModifiers > 0)
        {
            label_Value.text = $"{GetBaseValueStr(category1)} -> {GetFinalValueStr(category1)}";
        }
        else
        {
            label_Value.text = $"{GetBaseValueStr(category1)}";
        }
        label_ModifierName.text = baseModifierClass.TitlePerk;
        if (countModifiers == 0)
        {
            label_Description.text = baseModifierClass.DescriptionModifier;
        }
        else
        {
            label_Description.text = $"{baseModifierClass.DescriptionModifier} <size=13>(There are {countModifiers} modifiers affecting this stat)</size>";

        }
        icon.sprite = baseModifierClass.PerkSprite;

    }

    private string GetFinalValueStr(ModifierEffectCategory category)
    {
        string s = FormatizeValue(Hypatios.Player.GetCharFinalValue(category), category);
        return s;
    }

    public static string FormatizeValue(float value, ModifierEffectCategory category)
    {
        string s = "";

        if (category == ModifierEffectCategory.MaxHitpointBonus)
        {
            s = $"{value}";
        }
        else if (category == ModifierEffectCategory.RegenHPBonus)
        {
            if (value == 0) s = $"{value} HP/s";
            else if (value > 0) s = $"{value} HP/s";
            else s = $"-{value} HP/s";
        }
        else if (category == ModifierEffectCategory.KnockbackResistance)
        {
            s = $"{value}x";
        }
        else if (category == ModifierEffectCategory.Recoil)
        {
            s = $"{value}x";
        }
        else if (category == ModifierEffectCategory.BonusDamageMelee)
        {
            s = $"{value * 100}%";
        }
        else if (category == ModifierEffectCategory.BonusDamageGun)
        {
            s = $"{value * 100}%";
        }
        else if (category == ModifierEffectCategory.DashCooldown)
        {
            s = $"{value}s";
        }
        else if (category == ModifierEffectCategory.SoulBonus)
        {
            s = $"Lv {value}/5";
        }
        else if (category == ModifierEffectCategory.ShortcutDiscount)
        {
            s = $"Lv {value}/5";
        }
        else if (category == ModifierEffectCategory.Alcoholism)
        {
            s = $"{value}/100%";
        }
        else if (category == ModifierEffectCategory.MovementBonus)
        {
            s = $"{value}m/s";
        }
        else if (category == ModifierEffectCategory.ArmorRating)
        {
            s = $"{value * 100}%";
        }

        return s;
    }

    private string GetBaseValueStr(ModifierEffectCategory category)
    {
        float value = 0;
        string s = "";
        HypatiosSave.PerkDataSave PerkData = Hypatios.Player.PerkData;

        if (category == ModifierEffectCategory.MaxHitpointBonus)
        {
            value = Hypatios.Player.GetCharBaseValue(category) + PlayerPerk.GetValue_MaxHPUpgrade(PerkData.Perk_LV_MaxHitpointUpgrade);
        }
        else if (category == ModifierEffectCategory.RegenHPBonus)
        {
            value = Hypatios.Player.GetCharBaseValue(category) + PlayerPerk.GetValue_RegenHPUpgrade(PerkData.Perk_LV_RegenHitpointUpgrade);      
        }
        else if (category == ModifierEffectCategory.KnockbackResistance)
        {
            value = Hypatios.Player.GetCharBaseValue(category) - PlayerPerk.GetValue_KnockbackResistUpgrade(PerkData.Perk_LV_KnockbackRecoil);
        }
        else if (category == ModifierEffectCategory.Recoil)
        {
            value = Hypatios.Player.GetCharBaseValue(category) - PlayerPerk.GetValue_RecoilUpgrade(PerkData.Perk_LV_WeaponRecoil);
        }
        else if (category == ModifierEffectCategory.BonusDamageMelee)
        {
            value = Hypatios.Player.GetCharBaseValue(category) + PlayerPerk.GetValue_BonusMeleeDamage(PerkData.Perk_LV_IncreaseMeleeDamage);
        }
        else if (category == ModifierEffectCategory.BonusDamageGun)
        {
            value = Hypatios.Player.GetCharBaseValue(category) + PlayerPerk.GetValue_BonusGunDamage(PerkData.Perk_LV_IncreaseGunDamage);
        }
        else if (category == ModifierEffectCategory.DashCooldown)
        {
            value = PlayerPerk.GetValue_Dashcooldown(PerkData.Perk_LV_DashCooldown);
        }
        else if (category == ModifierEffectCategory.SoulBonus)
        {
            value = Hypatios.Player.GetCharBaseValue(category) + Hypatios.Player.GetNetSoulBonusPerk();
        }
        else if (category == ModifierEffectCategory.ShortcutDiscount)
        {
            value = Hypatios.Player.GetCharBaseValue(category) + Hypatios.Player.GetNetShortcutPerk();
        }
        else if (category == ModifierEffectCategory.Alcoholism)
        {
            value = Hypatios.Player.GetCharBaseValue(category) + Mathf.RoundToInt(Hypatios.Player.Health.alcoholMeter);
        }
        else if (category == ModifierEffectCategory.MovementBonus)
        {
            value = Hypatios.Player.GetCharBaseValue(category);
        }
        else if (category == ModifierEffectCategory.ArmorRating)
        {
            value = Hypatios.Player.GetCharBaseValue(category);
        }

        return FormatizeValue(value, category);
    }
}
