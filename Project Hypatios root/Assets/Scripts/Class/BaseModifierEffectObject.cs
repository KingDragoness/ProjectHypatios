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
    [TextArea(3,6)] public string DescriptionModifier;

}
