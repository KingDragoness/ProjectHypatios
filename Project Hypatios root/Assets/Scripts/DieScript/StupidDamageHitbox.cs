using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StupidDamageHitbox : MonoBehaviour
{

    [Tooltip("For Enemy attacks")]
    public EnemyScript originEnemy;
    public int damage = 10;
    public DamageToken.DamageOrigin originDamage = DamageToken.DamageOrigin.Enemy;

    private void OnTriggerEnter(Collider other)
    {
        damageReceiver damageReceiver = other.gameObject.GetComponent<damageReceiver>();


        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth h = other.gameObject.GetComponent<PlayerHealth>();

            if (h != null)
            {
                h.takeDamage(damage);
                Hypatios.UI.SpawnIndicator.Spawn(transform);

            }
        }

        if (damageReceiver != null)
        {
            var token = new DamageToken(); token.origin = originDamage; token.damage = damage; if (originEnemy != null) token.originEnemy = originEnemy;

            damageReceiver.Attacked(token);
        }
    }
}
