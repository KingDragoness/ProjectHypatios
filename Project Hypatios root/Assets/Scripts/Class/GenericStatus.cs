using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericStatus : BaseModifierEffect
{

    /// <summary>
    /// Please don't call this if you don't know what you're doing. Use Entity's CreateStatusEffect instead.
    /// </summary>
    /// <param name="_statusCategory"></param>
    /// <param name="_value"></param>
    /// <param name="_effectTimer"></param>
    /// <param name="_source"></param>
    /// <returns></returns>
    public static GenericStatus CreateStatusEffect
        (ModifierEffectCategory _statusCategory,
        float _value,
        float _effectTimer = 1f,
        string _sourceID = "Generic"
        )
    {
        GenericStatus status = new GameObject().AddComponent<GenericStatus>();
        status.statusCategoryType = _statusCategory;
        status.Value = _value;
        status.EffectTimer = _effectTimer;
        status.SourceID = _sourceID;
        status.gameObject.name = $"{ _statusCategory}_{_sourceID}";
        return status;
    }

    public override void ApplyEffect()
    {
        GenericApplyEffect();
    }

}
