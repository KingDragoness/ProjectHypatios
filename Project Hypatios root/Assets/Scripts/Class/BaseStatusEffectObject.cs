using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Fire", menuName = "Hypatios/BaseStatusEffect", order = 1)]
public class BaseStatusEffectObject : ScriptableObject
{

    public StatusEffectCategory category;
    public Sprite PerkSprite;
    public string TitlePerk;

}
