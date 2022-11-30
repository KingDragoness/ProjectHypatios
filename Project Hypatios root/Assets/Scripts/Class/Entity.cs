using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Entity : MonoBehaviour
{

    [FoldoutGroup("Base")] [SerializeField] private Bounds boundingBox;
    [FoldoutGroup("Base")] [ShowInInspector] private List<BaseStatusEffect> _allStatusInEffect = new List<BaseStatusEffect>();
    public List<BaseStatusEffect> AllStatusInEffect { get => _allStatusInEffect; }
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
        if (CheckDuplicateBySimilarEffect(StatusEffectCategory.Fire))
        {
            var effect = GetStatusEffect(StatusEffectCategory.Fire);
            effect.EffectTimer = 5f;
            return;
        }

        var FireParticle = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.FireEffect, false);
        var particleFX = FireParticle.GetComponent<ParticleFXResizer>();
        FireParticle.transform.position = OffsetedBoundWorldPosition;
        FireParticle.transform.localEulerAngles = Vector3.zero;

        particleFX.ResizeParticle(OffsetedBoundScale.magnitude);
        var statusObject = CreateGenericStatusEffect(StatusEffectCategory.Fire, -1f, 5f);
        var fireStatus = statusObject.gameObject.AddComponent<FireStatus>();
        fireStatus.damageType = FireStatus.DamageType.Fire;
        FireParticle.transform.SetParent(statusObject.transform);
    }

    [FoldoutGroup("Debug")]
    [Button("Poison")]
    public virtual void Poison()
    {
        if (CheckDuplicateBySimilarEffect(StatusEffectCategory.Poison))
        {
            var effect = GetStatusEffect(StatusEffectCategory.Poison);
            effect.EffectTimer = 5f;
            return;
        }

        var FireParticle = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.PoisonEffect, false);
        var particleFX = FireParticle.GetComponent<ParticleFXResizer>();
        FireParticle.transform.position = OffsetedBoundWorldPosition;
        FireParticle.transform.localEulerAngles = Vector3.zero;

        particleFX.ResizeParticle(OffsetedBoundScale.magnitude);
        var statusObject = CreateGenericStatusEffect(StatusEffectCategory.Poison, -1f, 5f);
        var poisonStatus = statusObject.gameObject.AddComponent<FireStatus>();
        poisonStatus.damageType = FireStatus.DamageType.Poison;
        FireParticle.transform.SetParent(statusObject.transform);

    }
    #endregion

    #region Status Effect Utilities
    private BaseStatusEffect GetStatusEffect(StatusEffectCategory _statusCategory)
    {
        return _allStatusInEffect.Find(x => x.statusCategoryType == _statusCategory);
    }

    private BaseStatusEffect GetStatusEffect(StatusEffectCategory _statusCategory, GameObject _source)
    {
        return _allStatusInEffect.Find(x => x.source == _source);
    }

    private bool CheckDuplicates(StatusEffectCategory _statusCategory, GameObject _source)
    {
        return _allStatusInEffect.Find(x => x.source == _source && x.statusCategoryType == _statusCategory);
    }

    private bool CheckDuplicateBySimilarEffect(StatusEffectCategory _statusCategory)
    {
        return _allStatusInEffect.Find(x => x.statusCategoryType == _statusCategory);
    }

    private bool CheckDuplicateBySimilarSource(GameObject _source)
    {
        return _allStatusInEffect.Find(x => x.source == _source);
    }

    public GenericStatus GetGenericEffect(StatusEffectCategory _statusCategory, GameObject _source)
    {
        return _allStatusInEffect.Find(x => x.source == _source && x.statusCategoryType == _statusCategory) as GenericStatus;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="_source">Source must same as the one created from CreateStatusEffect</param>
    public void RemoveAllEffectsBySource(GameObject _source)
    {
        _allStatusInEffect.RemoveAll(x => x == null);
        var allEffects = _allStatusInEffect.FindAll(x => x.source == _source);

        foreach (var effect in allEffects)
            Destroy(effect.gameObject);

        _allStatusInEffect.RemoveAll(x => x == null);
    }


    [FoldoutGroup("Debug")]
    [Button("Create Status Effect")]
    private GenericStatus CreateGenericStatusEffect(StatusEffectCategory _statusCategory,
        float _value,
        float _effectTimer = 1f,
        GameObject _source = null)
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
    public GenericStatus CreatePersistentStatusEffect(StatusEffectCategory _statusCategory, float _value, GameObject _source, bool allowDuplicate = false)
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
    public GenericStatus CreateTimerStatusEffect(StatusEffectCategory _statusCategory, float _value, float _timer = 1, GameObject _source = null, bool allowDuplicate = false)
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
