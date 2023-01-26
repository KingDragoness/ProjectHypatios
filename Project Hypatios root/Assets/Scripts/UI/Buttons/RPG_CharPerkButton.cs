using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class RPG_CharPerkButton : MonoBehaviour
{

    public PlayerRPGUI playerRPGUI;
    public BaseStatusEffectObject statusEffect;
    public BaseStatusEffect attachedStatusEffectGO;
    public Image icon;
    public float value = 0;

    public void HighlightPerk()
    {
        playerRPGUI.HighlightPerk(this);
    }

    public void Refresh()
    {
        icon.sprite = statusEffect.PerkSprite;
    }

    public void DehighlightPerk()
    {
        playerRPGUI.DehighlightPerk();
        Hypatios.UI.CloseAllTooltip();

    }

    public static string GetDescription(StatusEffectCategory statusEffect, float value)
    {
        string s = "";

        if (statusEffect == StatusEffectCategory.MovementBonus)
        {
            s = $"{Mathf.RoundToInt(value *100)}%";
        }
        else if (statusEffect == StatusEffectCategory.BonusDamageMelee)
        {
            s = $"{Mathf.RoundToInt(value*100)}%";
        }
        else if (statusEffect == StatusEffectCategory.BonusDamageGun)
        {
            s = $"{Mathf.RoundToInt(value * 100)}%";
        }
        else if (statusEffect == StatusEffectCategory.RegenHPBonus)
        {
            float value_ = value;
            if (value_ == 0) s = $"{value_} HP/s";
            else if (value_ > 0) s = $"{value_} HP/s";
            else s = $"-{value_} HP/s";
        }
        else if (statusEffect == StatusEffectCategory.MaxHitpointBonus)
        {
            float value_ = value;
            if (value_ == 0) s = $"{value_} HP";
            else if (value_ > 0) s = $"+{value_} HP";
            else s = $"-{value_} HP";
        }
        else if (statusEffect == StatusEffectCategory.DashCooldown)
        {
            s = $"{value}s";
        }
        else if (statusEffect == StatusEffectCategory.Alcoholism)
        {
            s = $"{value}/100%";
        }
        else
        {
            s = $"{value}";
        }

        return s;
    }
}
