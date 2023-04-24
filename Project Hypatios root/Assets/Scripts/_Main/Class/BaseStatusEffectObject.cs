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
        Hypatios.Player.PerkData.Temp_StatusEffect.Add(statusEffectDat);
        Hypatios.Player.ReloadStatEffects();
    }
}
