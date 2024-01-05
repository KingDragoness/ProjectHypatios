using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "Fire", menuName = "Hypatios/BaseModifierEffectObject", order = 1)]
public class BaseModifierEffectObject : ScriptableObject
{

    public ModifierEffectCategory category;
    public List<ModifierEffectCategory> childCategories;
    public Sprite PerkSprite;
    [SerializeField] private string TitlePerk;
    [Tooltip("More like there's a status effect that can this at realtime rather very rigid perk upgrade like subway shortcut or soul bonus.")] public bool hasPerkUpgrade = false;
    [TextArea(3,6)] [SerializeField] private string DescriptionModifier;

    [FoldoutGroup("Essence")] public float baseValue = 0.1f;
    [FoldoutGroup("Essence")] public bool craftableEssence = false;
    [FoldoutGroup("Essence")] [ShowIf("craftableEssence")] public List<Recipe> requirementCrafting = new List<Recipe>();

    //[SerializeField] private LocalizedString loc_TitlePerk;
    //[SerializeField] private LocalizedString loc_DescriptionModifier;

    public string GetTitlePerk()
    {
        return TitlePerk;  //loc_TitlePerk.GetString(TitlePerk);
    }

    public string GetDescriptionPerk()
    {
        return DescriptionModifier; // loc_DescriptionModifier.GetString(DescriptionModifier);

    }


}
