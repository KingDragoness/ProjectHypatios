using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Entity : MonoBehaviour
{

    [FoldoutGroup("Base")] [SerializeField] private Bounds boundingBox;
    [FoldoutGroup("Base")] [ShowInInspector] private List<BaseModifierEffect> _allStatusInEffect = new List<BaseModifierEffect>();
    public List<BaseModifierEffect> AllStatusInEffect { get => _allStatusInEffect; }
    public List<StatusEffectMono> AllStatusMonos { get 
        {
            List<StatusEffectMono> _statMonos = new List<StatusEffectMono>();
            foreach (var status in _allStatusInEffect)
            {
                var _statusEffect = status as StatusEffectMono;
                if (_statusEffect != null)
                    _statMonos.Add(_statusEffect);
            }

            return _statMonos; 
        } 
    }
    public Bounds BoundingBox { get => boundingBox; }

    private Transform _containerEntityParent;

    public virtual void OnDrawGizmosSelected()
    {

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(0.3f, 1f, 0.4f, 1f);
        Gizmos.DrawWireCube(Vector3.zero + BoundingBox.center, BoundingBox.extents);
        Gizmos.color = new Color(0.1f, 0.8f, 0.1f, 0.04f);
        //Gizmos.DrawCube(Vector3.zero + boundingBox.center, boundingBox.extents);
    }

    public Vector3 OffsetedBoundWorldPosition
    {
        get
        {
            return transform.position + Vector3.Scale(BoundingBox.center, transform.localScale);
        }
    }

    public Vector3 OffsetedBoundScale
    {
        get
        {
            return Vector3.Scale(BoundingBox.extents, transform.localScale);
        }
    }

    #region Status

    [FoldoutGroup("Debug")] [Button("Burn")]
    public virtual void Burn()
    {
        if (CheckDuplicateBySimilarEffect(ModifierEffectCategory.Fire))
        {
            var effect = GetStatusEffect(ModifierEffectCategory.Fire);
            effect.EffectTimer = 5f;
            return;
        }

        var FireParticle = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.FireEffect, false);

        if (FireParticle == null) return;
        var particleFX = FireParticle.GetComponent<ParticleFXResizer>();
        FireParticle.transform.position = OffsetedBoundWorldPosition;
        FireParticle.transform.localEulerAngles = Vector3.zero;

        particleFX.ResizeParticle(OffsetedBoundScale.magnitude);
        var statusObject = CreateGenericStatusEffect(ModifierEffectCategory.Fire, -1f, 5f);
        var fireStatus = statusObject.gameObject.AddComponent<FireStatus>();
        fireStatus.damageType = FireStatus.DamageType.Fire;
        FireParticle.transform.SetParent(statusObject.transform);
    }

    [FoldoutGroup("Debug")]
    [Button("Poison")]
    public virtual void Poison()
    {
        if (CheckDuplicateBySimilarEffect(ModifierEffectCategory.Poison))
        {
            var effect = GetStatusEffect(ModifierEffectCategory.Poison);
            effect.EffectTimer = 5f;
            return;
        }

        var FireParticle = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.PoisonEffect, false);
        var particleFX = FireParticle.GetComponent<ParticleFXResizer>();
        FireParticle.transform.position = OffsetedBoundWorldPosition;
        FireParticle.transform.localEulerAngles = Vector3.zero;

        particleFX.ResizeParticle(OffsetedBoundScale.magnitude);
        var statusObject = CreateGenericStatusEffect(ModifierEffectCategory.Poison, -1f, 5f);
        var poisonStatus = statusObject.gameObject.AddComponent<FireStatus>();
        poisonStatus.damageType = FireStatus.DamageType.Poison;
        FireParticle.transform.SetParent(statusObject.transform);

    }

    [FoldoutGroup("Debug")]
    [Button("Heal")]
    public virtual void Heal(float healAmount)
    {
        //player use playerHealth

    }

    [FoldoutGroup("Debug")]
    [Button("Paralysis")]
    public virtual void Paralysis()
    {
        if (CheckDuplicateBySimilarEffect(ModifierEffectCategory.Paralyze))
        {
            var effect = GetStatusEffect(ModifierEffectCategory.Paralyze);
            effect.EffectTimer = 11f;
            return;
        }

        var ParalyzeParticle = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.ParalyzeEffect, false);
        var particleFX = ParalyzeParticle.GetComponent<ParticleFXResizer>();
        var pos = OffsetedBoundWorldPosition;
        pos.y += OffsetedBoundScale.y;
        ParalyzeParticle.transform.position = pos;
        ParalyzeParticle.transform.localEulerAngles = Vector3.zero;

        particleFX.ResizeParticle(OffsetedBoundScale.magnitude);
        var statusObject = CreateGenericStatusEffect(ModifierEffectCategory.Paralyze, -1f, 11f);
        var poisonStatus = statusObject.gameObject.AddComponent<FireStatus>();
        poisonStatus.damageType = FireStatus.DamageType.NOTHING;
        ParalyzeParticle.transform.SetParent(statusObject.transform);


    }
    #endregion

    #region Status Effect Utilities
    private BaseModifierEffect GetStatusEffect(ModifierEffectCategory _statusCategory)
    {
        return _allStatusInEffect.Find(x => x.statusCategoryType == _statusCategory);
    }

    public int GetTempStatusEffectByCount(ModifierEffectCategory _statusCategory)
    {
        var listStatuses = _allStatusInEffect.FindAll(x => x.statusCategoryType == _statusCategory && x.SourceID != "PermanentPerk");
        var childCategories = Hypatios.Assets.GetStatusEffect(_statusCategory).childCategories;
        foreach(var category in childCategories)
        {
            var tempList = _allStatusInEffect.FindAll(x => x.statusCategoryType == category && x.SourceID != "PermanentPerk");
            listStatuses.AddRange(tempList);
        }

        return listStatuses.Count;
    }
    private BaseModifierEffect GetStatusEffect(ModifierEffectCategory _statusCategory, string _source)
    {
        return _allStatusInEffect.Find(x => x.SourceID == _source);
    }

    private bool CheckDuplicates(ModifierEffectCategory _statusCategory, string _source)
    {
        return _allStatusInEffect.Find(x => x.SourceID == _source && x.statusCategoryType == _statusCategory);
    }


    private bool CheckDuplicateBySimilarEffect(ModifierEffectCategory _statusCategory)
    {
        return _allStatusInEffect.Find(x => x.statusCategoryType == _statusCategory);
    }

    private bool CheckDuplicateBySimilarSource(string _source)
    {
        return _allStatusInEffect.Find(x => x.SourceID == _source);
    }

    public GenericStatus GetGenericEffect(ModifierEffectCategory _statusCategory, string _source)
    {
        return _allStatusInEffect.Find(x => x.SourceID == _source && x.statusCategoryType == _statusCategory) as GenericStatus;
    }

    public bool IsStatusEffect(ModifierEffectCategory _statusCategory)
    {
        return _allStatusInEffect.Find(x => x.statusCategoryType == _statusCategory) != null;
    }

    public bool IsStatusEffect(ModifierEffectCategory _statusCategory, string _source)
    {
        return _allStatusInEffect.Find(x => x.statusCategoryType == _statusCategory && x.SourceID == _source) != null;
    }

    public bool IsStatusEffectGroup(BaseStatusEffectObject _statusEffectObject)
    {
        List<StatusEffectMono> allStatusEffectMonos = new List<StatusEffectMono>();
        foreach (var status in _allStatusInEffect)
        {
            var _statusEffect = status as StatusEffectMono;
            if (_statusEffect != null)
                allStatusEffectMonos.Add(_statusEffect);
        }

        return allStatusEffectMonos.Find(x => x.statusEffect == _statusEffectObject);
    }


    public StatusEffectMono GetStatusEffectGroup(BaseStatusEffectObject _statusEffectObject)
    {
        List<StatusEffectMono> allStatusEffectMonos = new List<StatusEffectMono>();
        foreach (var status in _allStatusInEffect)
        {
            var _statusEffect = status as StatusEffectMono;
            if (_statusEffect != null)
                allStatusEffectMonos.Add(_statusEffect);
        }

        return allStatusEffectMonos.Find(x => x.statusEffect == _statusEffectObject);
    }

    /// <summary>
    /// Remove player's ailments or effects
    /// </summary>
    public virtual void RemoveStatusEffectGroup(BaseStatusEffectObject _statusEffect)
    {
        _allStatusInEffect.RemoveAll(x => x == null);
        var allGenericStatus = _allStatusInEffect.FindAll(x => x is GenericStatus);
        StatusEffectMono targetMono = GetStatusEffectGroup(_statusEffect);

        if (targetMono == null)
            return;
        else {
            _allStatusInEffect.Remove(targetMono);
            Destroy(targetMono.gameObject);
        }

        foreach (var effect in allGenericStatus)
        {
            if (effect.IsTiedToStatusMono == false) continue;
            if (effect.statusMono == null) continue;
            if (effect.statusMono.statusEffect == _statusEffect)
            {
                _allStatusInEffect.Remove(effect);
                Destroy(effect.gameObject);
            }
        }
        _allStatusInEffect.RemoveAll(x => x == null);
    }




    /// <summary>
    /// 
    /// </summary>
    /// <param name="_source">Source must same as the one created from CreateStatusEffect</param>
    public void RemoveAllEffectsBySource(string _source)
    {
        _allStatusInEffect.RemoveAll(x => x == null);
        var allEffects = _allStatusInEffect.FindAll(x => x.SourceID == _source);

        foreach (var effect in allEffects)
        {
            _allStatusInEffect.Remove(effect);
            Destroy(effect.gameObject);
        
        }
        _allStatusInEffect.RemoveAll(x => x == null);
    }



    public StatusEffectMono CreateStatusEffectGroup(BaseStatusEffectObject _effectObject,
    float _effectTimer = 1f,
    string _source = "Generic")
    {
        StatusEffectMono statusMono = StatusEffectMono.CreateStatusEffect(_effectObject, 1f, _effectTimer, _source);
        if (_containerEntityParent == null) CreateEntityParent();
        statusMono.transform.SetParent(_containerEntityParent.transform);
        statusMono.target = this;
        statusMono.ApplyEffect();
        _allStatusInEffect.Add(statusMono);
        return statusMono;
    }


    [FoldoutGroup("Debug")]
    [Button("Create Status Effect")]
    private GenericStatus CreateGenericStatusEffect(ModifierEffectCategory _statusCategory,
        float _value,
        float _effectTimer = 1f,
        string _source = "Generic")
    {
        //if (CheckDuplicateBySimilarEffect(_statusCategory) == true) return;

        GenericStatus genericStatus = GenericStatus.CreateStatusEffect(_statusCategory, _value, _effectTimer, _source);
        if (_containerEntityParent == null) CreateEntityParent();        
        genericStatus.transform.SetParent(_containerEntityParent.transform);
        genericStatus.target = this;
        genericStatus.ApplyEffect();
        _allStatusInEffect.Add(genericStatus);
        return genericStatus;
    }

    /// <summary>
    /// Recommended for creating perks and weapon temporary's effect (e.g: shield block).
    /// </summary>
    /// <param name="_statusCategory"></param>
    /// <param name="_value"></param>
    /// <param name="_source">Must assign source! Or else it'll will duplicate for infinity.</param>
    /// <param name="allowDuplicate"></param>
    public GenericStatus CreatePersistentStatusEffect(ModifierEffectCategory _statusCategory, float _value, string _source, bool allowDuplicate = false)
    {
        if (allowDuplicate == false && CheckDuplicates(_statusCategory, _source))
            return GetStatusEffect(_statusCategory, _source) as GenericStatus;

        return CreateGenericStatusEffect(_statusCategory, _value, 99999f, _source);
    }

    /// <summary>
    /// Recommended for creating temporary status effect like burn, poison, etc.
    /// </summary>
    /// <param name="_statusCategory"></param>
    /// <param name="_value"></param>
    /// <param name="_timer"></param>
    /// <param name="_source"></param>
    /// <param name="allowDuplicate"></param>
    public GenericStatus CreateTimerStatusEffect(ModifierEffectCategory _statusCategory, float _value, float _timer = 1, string _source = "Generic", bool allowDuplicate = false)
    {
        if (allowDuplicate == false && CheckDuplicateBySimilarSource(_source))
            return GetStatusEffect(_statusCategory) as GenericStatus;

        return CreateGenericStatusEffect(_statusCategory, _value, _timer, _source);
    }


    private void CreateEntityParent()
    {
        _containerEntityParent = new GameObject().transform;
        _containerEntityParent.gameObject.name = "StatusInEffect";
        _containerEntityParent.transform.SetParent(this.transform);
        _containerEntityParent.transform.SetAsFirstSibling();
    }

    #endregion
}
