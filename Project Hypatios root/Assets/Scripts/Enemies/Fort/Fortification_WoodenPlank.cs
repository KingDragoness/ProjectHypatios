using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Fortification_WoodenPlank : EnemyScript
{

    [FoldoutGroup("Events")] public UnityEvent OnDieEvent;
    [FoldoutGroup("Audios")] public AudioSource destroyedWood;
    [FoldoutGroup("Audios")] public AudioSource damagedWood;
    public DissolveMaterialScript dissolveMat;
    public DissolveMaterialScript dissolveMat1;


    public override void Die()
    {
        Destroy(gameObject, 3f);
        destroyedWood.Play();
        OnDied?.Invoke();
        OnDieEvent?.Invoke();
        Stats.IsDead = true;
    }

    public override void Attacked(DamageToken token)
    {
        if ((token.origin == DamageToken.DamageOrigin.Player | token.origin == DamageToken.DamageOrigin.Ally) &&
                token.isBurn == false)
            return;
        //DamageOutputterUI.instance.DisplayText(token.damage)
        Stats.CurrentHitpoint -= token.damage;
        _lastDamageToken = token;
        damagedWood.Play();

        dissolveMat.currentTime = 1f -(Stats.CurrentHitpoint/Stats.MaxHitpoint.Value);
        dissolveMat1.currentTime = 1f - (Stats.CurrentHitpoint / Stats.MaxHitpoint.Value);

        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }
        else
        {
        }
    }
}
