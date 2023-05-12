using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// StatusEffectMono is mainly to store "BaseStatusEffectObject" (e.g: Bleeding, Curse of Slowing, Heavy Weight)
/// 
/// </summary>
public class StatusEffectMono : BaseModifierEffect
{

    public BaseStatusEffectObject statusEffect;

    /// <summary>
    /// Please don't call this if you don't know what you're doing. Use Entity's CreateStatusEffect instead.
    /// </summary>
    /// <param name="_statusCategory"></param>
    /// <param name="_value"></param>
    /// <param name="_effectTimer"></param>
    /// <param name="_source"></param>
    /// <returns></returns>
    public static StatusEffectMono CreateStatusEffect
        (BaseStatusEffectObject _statusEffect,
        float _value,
        float _effectTimer = 1f,
        string _sourceID = "Generic"
        )
    {
        StatusEffectMono status = new GameObject().AddComponent<StatusEffectMono>();
        status.Value = _value;
        status.EffectTimer = _effectTimer;
        status.SourceID = _sourceID;
        status.statusEffect = _statusEffect;
        status.gameObject.name = $"{ _statusEffect.GetDisplayText()}_{_sourceID}";
        return status;
    }

    public override void OnDestroy()
    {
        CharacterScript charScript = target as CharacterScript;
        if (charScript != null)
        {
            charScript.PerkData.Temp_StatusEffect.RemoveAll(x => x.ID == statusEffect.GetID());
        }
        base.OnDestroy();

    }

    public override void ApplyEffect()
    {
        GenericApplyEffect();
    }

}
