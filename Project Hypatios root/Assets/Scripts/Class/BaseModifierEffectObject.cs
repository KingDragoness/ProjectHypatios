using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Fire", menuName = "Hypatios/BaseModifierEffectObject", order = 1)]
public class BaseModifierEffectObject : ScriptableObject
{

    public ModifierEffectCategory category;
    public Sprite PerkSprite;
    public string TitlePerk;
    [Tooltip("More like there's a status effect that can this at realtime rather very rigid perk upgrade like subway shortcut or soul bonus.")] public bool hasPerkUpgrade = false;
    [TextArea(3,6)] public string DescriptionModifier;

}
