using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Fortification_WoodenPlank : EnemyScript
{

    [FoldoutGroup("Events")] public UnityEvent OnDieEvent;
    [FoldoutGroup("Audios")] public AudioSource destroyedWood;

    public override void Die()
    {
        destroyedWood.Play();
        Destroy(gameObject, 3f);
        OnDied?.Invoke();
        OnDieEvent?.Invoke();
    }

    public override void Attacked(DamageToken token)
    {
        if (token.origin == DamageToken.DamageOrigin.Player | token.origin == DamageToken.DamageOrigin.Ally)
            return;
        //DamageOutputterUI.instance.DisplayText(token.damage)
        Stats.CurrentHitpoint -= token.damage;


        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }
        else
        {
        }
    }
}
