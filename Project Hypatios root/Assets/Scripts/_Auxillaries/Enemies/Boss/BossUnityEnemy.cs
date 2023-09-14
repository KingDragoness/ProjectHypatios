using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossUnityEnemy : EnemyScript
{

    public enum Stage
    {
        Idle,
        Battle,
        Death
    }

    public Stage currentStage = Stage.Idle;

    public void ChangeStage(Stage _stage)
    {
        if (currentStage == Stage.Idle && _stage != Stage.Idle)
        {

        }

        currentStage = _stage;

    }

    public override void Die()
    {
        throw new System.NotImplementedException();
    }

    public override void Attacked(DamageToken token)
    {
        float damageProcessed = token.damage;

        if (currentStage == Stage.Idle)
        {
            ChangeStage(Stage.Battle);
        }

        Stats.CurrentHitpoint -= damageProcessed;
        _lastDamageToken = token;


        if (Stats.CurrentHitpoint > 0f)
            DamageOutputterUI.instance.DisplayText(damageProcessed);
        else
        {
            Die();
        }

        base.Attacked(token);
    }
}
