using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RetardGunWeapon : GunScript
{

    public override DamageToken GetDamageToken(RaycastHit hit)
    {
        var currentHit = hit;
        var collider = currentHit.collider;

        EnemyScript enemyScript = null;

        if (collider != null)
        {
            var damageReceiver = currentHit.collider.gameObject.GetComponentThenChild<damageReceiver>();
            if (damageReceiver != null)
            {
                enemyScript = damageReceiver.enemyScript;

            }

        }


        if (enemyScript != null)
        {
            var token = base.GetDamageToken(hit);

            if (enemyScript.Stats.IsDeadObject == false)
            {
                float iq = enemyScript.Stats.Intelligence.Value;
                float percent = 1f;
                
                if (enemyScript.Stats.Intelligence.Value > 50)
                {
                    percent -= ((iq * 0.01f) - 0.5f) * 2f;
                }
                else if (enemyScript.Stats.Intelligence.Value < 50)
                {
                    percent += (0.5f - (iq * 0.01f)) * 2f;
                }

                token.damage *= percent;

                return token;

            }

        }

        return base.GetDamageToken(hit);
    }

}
