using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// BaseStatusEffect can never be stack, only one instance
/// </summary>
[CreateAssetMenu(fileName = "Bleeding", menuName = "Hypatios/Status Effect", order = 1)]
public class BaseStatusEffectObject : ScriptableObject
{

    [InfoBox("If DisplayName is empty, it will use ID's")] [SerializeField] private string _displayName = "";
    [TextArea(3, 5)] [SerializeField] private string _description = "";
    public Sprite PerkSprite;
    [InfoBox("'Origin' left it blank, it will be filled using ID's (e.g: 'playerStatusEffect_bleeding')")] public List<PerkCustomEffect> allStatusEffects = new List<PerkCustomEffect>();
    [FoldoutGroup("Essence")] public bool craftableEssence = false;
    [FoldoutGroup("Essence")] [ShowIf("craftableEssence")] public List<Recipe> requirementCrafting = new List<Recipe>();
    [FoldoutGroup("Essence")] [ShowIf("craftableEssence")] public bool isNegativeAilment = false;
    public string Description { get => _description; }

    public string GetID()
    {
        return name;
    }

    public string GetDisplayText()
    {
        if (_displayName == "")
        {
            return name;
        }
        else
        {
            return _displayName;
        }
    }

    [FoldoutGroup("Debug")]
    [Button("Add Status")]
    public void AddStatusEffectPlayer(float Time = 9999f)
    {
        var statusEffectDat = new StatusEffectData();
        statusEffectDat.ID = GetID();
        statusEffectDat.Time = Time;
        if (Hypatios.Player.PerkData.Temp_StatusEffect.Find(x => x.ID == GetID()) == null)
        {
            if (allStatusEffects.Find(x => x.statusCategoryType == ModifierEffectCategory.Poison) != null)
            {
                Hypatios.Player.Poison();
            }
            Hypatios.Player.PerkData.Temp_StatusEffect.Add(statusEffectDat);
            Hypatios.Event.InvokeStatusEvent(this);
            Hypatios.Player.ReloadStatEffects();

         
        }
    }
}
