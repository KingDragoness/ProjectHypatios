using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class FireStatus : MonoBehaviour
{
    public enum DamageType
    {
        Fire,
        Poison,
        NOTHING
    }

    [ShowInInspector] [ReadOnly] public EnemyScript target;
    [ShowInInspector] [ReadOnly] public CharacterScript character;
    public DamageType damageType;
    public DamageToken.DamageOrigin origin = DamageToken.DamageOrigin.Environment;
    private DamageToken damageToken = new DamageToken();

    private GenericStatus genericStatus;

    private void Start()
    {
        genericStatus = GetComponent<GenericStatus>();
        target = genericStatus.target as EnemyScript;
        character = genericStatus.target as CharacterScript;

        if (target == null && character == null) return;

        damageToken.damage = 10;
        damageToken.healthSpeed = 50;
        damageToken.origin = origin;
        damageToken.damageType = DamageToken.DamageType.Fire;
        //target.Attacked(damageToken);
    }


    private float cooldown = 1f;

    private void Update()
    {
        //updates 1 time a second
        if (cooldown > 0f)
        {
            if (genericStatus.EffectTimer < 1f)
            {
                var ParticleFXResizer = gameObject.GetComponentInChildren<ParticleFXResizer>();
                ParticleFXResizer.StopParticle();

                if (target != null)
                {
                    if (genericStatus.statusCategoryType == ModifierEffectCategory.Paralyze)
                        target.isAIEnabled = true;
                }

            }
            else
            {
                var ParticleFXResizer = gameObject.GetComponentInChildren<ParticleFXResizer>();
                ParticleFXResizer.ResetParticle();

            }

            cooldown -= Time.deltaTime;
            return;
        }

        if (damageType == DamageType.Fire)
        {
            if (target != null) target.Attacked(damageToken);
            if (character != null) UniversalDamage.TryDamage(damageToken, character.transform, transform);
        }
        else if (damageType == DamageType.Poison)
        {
            if (target != null)
            {
                if (target.Stats.UnitType == UnitType.Biological)
                    target.Attacked(damageToken);
            }

            if (character != null) UniversalDamage.TryDamage(damageToken, character.transform, transform);

        }
        
        if (genericStatus.statusCategoryType == ModifierEffectCategory.Paralyze)
        {
            if (target != null)
            {
                target.isAIEnabled = false;
            }


        }
        cooldown = 1f;
    }

}
