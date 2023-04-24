using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class SykosisGunWeapon : GunScript
{

    public ParticleSystem trailSykos;

    public override void FireWeapon()
    {
        base.FireWeapon();
        if (trailSykos.isPlaying == false)
            trailSykos.Play();

        ParalysisEnemy();
    }

    private void ParalysisEnemy()
    {
        var collider = currentHit.collider; if (collider == null) return;
        var damageReceiver = currentHit.collider.GetComponent<damageReceiver>(); if (damageReceiver == null) return;
        var enemyScript = damageReceiver.enemyScript; if (enemyScript == null) return;

        float paralysisChance = GetFinalValue("ParalysisChance");
        float random = Random.Range(0f, 1f);

        if (enemyScript.Stats.IsDeadObject)
            return;

        if (random < paralysisChance)
            enemyScript.Paralysis();



    }

}
