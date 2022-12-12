using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class CharStatButton : MonoBehaviour
{

    public PlayerRPGUI playerRPGUI;
    public StatusEffectCategory category1;
    public Text statLabel;


    public void HighlightPerk()
    {
        playerRPGUI.HighlightStat(this);
    }

    public void DehighlightPerk()
    {
        playerRPGUI.DehighlightStat();
    }


    private void OnEnable()
    {
        PerkInitialize(category1);
    }

    private void PerkInitialize(StatusEffectCategory category)
    {
        float value = 0;
        string s = "";
        HypatiosSave.PerkDataSave PerkData = Hypatios.Player.PerkData;

        if (category == StatusEffectCategory.MaxHitpointBonus)
        {
            value = PlayerPerk.GetValue_MaxHPUpgrade(PerkData.Perk_LV_MaxHitpointUpgrade);
            s = $"{value} HP";
        }
        else if (category == StatusEffectCategory.RegenHPBonus)
        {
            value = PlayerPerk.GetValue_RegenHPUpgrade(PerkData.Perk_LV_RegenHitpointUpgrade);
            if (value == 0) s = $"{value} HP/s";
            else if (value > 0) s = $"{value} HP/s";
            else s = $"-{value} HP/s";
        }
        else if (category == StatusEffectCategory.KnockbackResistance)
        {
            value = PlayerPerk.GetValue_KnockbackResistUpgrade(PerkData.Perk_LV_KnockbackRecoil);
            s = $"-{value}";
        }
        else if (category == StatusEffectCategory.BonusDamageMelee)
        {
            value = PlayerPerk.GetValue_BonusMeleeDamage(PerkData.Perk_LV_IncreaseMeleeDamage);
            s = $"{value*100}%";
        }
        else if (category == StatusEffectCategory.DashCooldown)
        {
            value = PlayerPerk.GetValue_Dashcooldown(PerkData.Perk_LV_DashCooldown);
            s = $"{value}s";
        }
        else if (category == StatusEffectCategory.SoulBonus)
        {
            value = Hypatios.Player.GetNetSoulBonusPerk();
            s = $"Lv {value}/5";
        }

        statLabel.text = s;
    }

}
