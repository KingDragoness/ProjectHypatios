using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class RPG_CharPerkButton : MonoBehaviour
{

    public enum Type
    {
        TemporaryModifier,
        StatusEffect
    }

    public PlayerRPGUI playerRPGUI;
    public Type type;
    public BaseModifierEffectObject statusEffect;
    public BaseStatusEffectObject baseStatusEffectGroup;
    public BaseModifierEffect attachedStatusEffectGO;
    public Image icon;
    public float value = 0;

    public void HighlightPerk()
    {
        playerRPGUI.HighlightPerk(this);
    }

    public void Refresh()
    {
        if (type == Type.TemporaryModifier)
        {
            icon.sprite = statusEffect.PerkSprite;
        }
        else
        {
            icon.sprite = baseStatusEffectGroup.PerkSprite;
        }
    }

    public void DehighlightPerk()
    {
        playerRPGUI.DehighlightPerk();
        Hypatios.UI.CloseAllTooltip();

    }

    public static string GetDescription(ModifierEffectCategory statusEffect, float value)
    {
        string s = "";

        if (statusEffect == ModifierEffectCategory.MovementBonus)
        {
            s = $"{Mathf.RoundToInt(value *100)}%";
        }
        else if (statusEffect == ModifierEffectCategory.BonusDamageMelee)
        {
            s = $"{Mathf.RoundToInt(value*100)}%";
        }
        else if (statusEffect == ModifierEffectCategory.BonusDamageGun)
        {
            s = $"{Mathf.RoundToInt(value * 100)}%";
        }
        else if (statusEffect == ModifierEffectCategory.RegenHPBonus)
        {
            float value_ = value;
            if (value_ == 0) s = $"{value_} HP/s";
            else if (value_ > 0) s = $"{value_} HP/s";
            else s = $"{value_} HP/s";
        }
        else if (statusEffect == ModifierEffectCategory.MaxHitpointBonus)
        {
            float value_ = value;
            if (value_ == 0) s = $"{value_} HP";
            else if (value_ > 0) s = $"+{value_} HP";
            else s = $"{value_} HP";
        }
        else if (statusEffect == ModifierEffectCategory.MaxHPPercentage)
        {
            s = $"{Mathf.RoundToInt(value * 100)}%";
        }
        else if (statusEffect == ModifierEffectCategory.DashCooldown)
        {
            s = $"{value}s";
        }
        else if (statusEffect == ModifierEffectCategory.Alcoholism)
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
