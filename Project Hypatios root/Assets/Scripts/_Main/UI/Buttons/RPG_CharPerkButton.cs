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
    public PlayerStatusUI playerStatusUI;
    public Type type;
    public BaseModifierEffectObject statusEffect;
    public BaseStatusEffectObject baseStatusEffectGroup;
    public BaseModifierEffect attachedStatusEffectGO;
    public Image icon;
    public float value = 0;

    public void HighlightPerk()
    {
        if (playerRPGUI != null) playerRPGUI.HighlightPerk(this);
        if (playerStatusUI != null) playerStatusUI.HighlightPerk(this);
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
        if (playerRPGUI != null) playerRPGUI.DehighlightPerk();
        if (playerStatusUI != null) playerStatusUI.DehighlightPerk();
        Hypatios.UI.CloseAllTooltip();

    }

}
