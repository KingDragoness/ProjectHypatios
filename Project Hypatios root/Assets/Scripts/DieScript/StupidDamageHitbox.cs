using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StupidDamageHitbox : MonoBehaviour
{

    [Tooltip("For Enemy attacks")]
    public Enemy originEnemy;
    public int damage = 10;
    public DamageToken.DamageOrigin originDamage = DamageToken.DamageOrigin.Enemy;

    private void OnTriggerEnter(Collider other)
    {
        damageReceiver damageReceiver = other.gameObject.GetComponent<damageReceiver>();


        if (other.gameObject.CompareTag("Player"))
        {
            health h = other.gameObject.GetComponent<health>();

            if (h != null)
            {
                h.takeDamage(damage);
                var spawn = FindObjectOfType<SpawnIndicator>();
                spawn.Spawn(transform);
            }
        }

        if (damageReceiver != null)
        {
            var token = new DamageToken(); token.origin = originDamage; token.damage = damage; if (originEnemy != null) token.originEnemy = originEnemy;

            damageReceiver.Attacked(token);
        }
    }
}
