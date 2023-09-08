using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System.Linq;

public class EclipseblazerEnemy : EnemyScript
{

    [FoldoutGroup("Base Parameters")] public float healingSpeed = 5f;


    private void Update()
    {
        if (Time.timeScale <= 0) return;

        RegenerateHealth();
    }

    public void RegenerateHealth()
    {
        if (Stats.CurrentHitpoint < Stats.MaxHitpoint.Value)
        {
            Stats.CurrentHitpoint += Time.deltaTime * healingSpeed;
        }
        
        if (Stats.CurrentHitpoint < -1f)
        {
            Stats.CurrentHitpoint = 100f;
        }
    }

    public override void Attacked(DamageToken token)
    {
        if (token.originEnemy == this) return;
        _lastDamageToken = token;

        Stats.CurrentHitpoint -= token.damage;
        if (!Stats.IsDead)
            DamageOutputterUI.instance.DisplayText(token.damage);

        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }

        base.Attacked(token);
    }


    public override void Die()
    {
        //impossible to die.
    }
}
