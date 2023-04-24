using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

//You can modify this
public class CharStatButton : MonoBehaviour
{

    public PlayerRPGUI playerRPGUI;
    public ModifierEffectCategory category1;
    public Text statLabel;


    public void HighlightPerk()
    {
        playerRPGUI.HighlightStat(this);
    }

    public void DehighlightPerk()
    {
        playerRPGUI.DehighlightStat();
        Hypatios.UI.CloseAllTooltip();
    }


    private void OnEnable()
    {
        PerkInitialize(category1);
    }

    public void ForceRefresh()
    {
        PerkInitialize(category1);

    }

    private void PerkInitialize(ModifierEffectCategory category)
    {
        float value = 0;
        string s = "";
        HypatiosSave.PerkDataSave PerkData = Hypatios.Player.PerkData;

        if (category == ModifierEffectCategory.MaxHitpointBonus)
        {
            value = PlayerPerk.GetValue_MaxHPUpgrade(PerkData.Perk_LV_MaxHitpointUpgrade);
            s = $"{value} HP";
        }
        else if (category == ModifierEffectCategory.RegenHPBonus)
        {
            value = PlayerPerk.GetValue_RegenHPUpgrade(PerkData.Perk_LV_RegenHitpointUpgrade);
            if (value == 0) s = $"{value} HP/s";
            else if (value > 0) s = $"{value} HP/s";
            else s = $"-{value} HP/s";
        }
        else if (category == ModifierEffectCategory.KnockbackResistance)
        {
            value = PlayerPerk.GetValue_KnockbackResistUpgrade(PerkData.Perk_LV_KnockbackRecoil);
            s = $"-{value}";
        }
        else if (category == ModifierEffectCategory.Recoil)
        {
            value = PlayerPerk.GetValue_RecoilUpgrade(PerkData.Perk_LV_WeaponRecoil);
            s = $"-{value}";
        }
        else if (category == ModifierEffectCategory.BonusDamageMelee)
        {
            value = PlayerPerk.GetValue_BonusMeleeDamage(PerkData.Perk_LV_IncreaseMeleeDamage);
            s = $"{value*100}%";
        }
        else if (category == ModifierEffectCategory.BonusDamageGun)
        {
            value = PlayerPerk.GetValue_BonusGunDamage(PerkData.Perk_LV_IncreaseGunDamage);
            s = $"{value * 100}%";
        }
        else if (category == ModifierEffectCategory.DashCooldown)
        {
            value = PlayerPerk.GetValue_Dashcooldown(PerkData.Perk_LV_DashCooldown);
            s = $"{value}s";
        }
        else if (category == ModifierEffectCategory.SoulBonus)
        {
            value = Hypatios.Player.GetNetSoulBonusPerk();
            s = $"Lv {value}/5";
        }
        else if (category == ModifierEffectCategory.ShortcutDiscount)
        {
            value = Hypatios.Player.GetNetShortcutPerk();
            s = $"Lv {value}/5";
        }
        else if (category == ModifierEffectCategory.Alcoholism)
        {
            value = Mathf.RoundToInt(Hypatios.Player.Health.alcoholMeter);
            s = $"{value}/100%";
        }

        statLabel.text = s;
    }

}
