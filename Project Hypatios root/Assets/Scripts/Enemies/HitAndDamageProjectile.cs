using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class HitAndDamageProjectile : MonoBehaviour
{

    public bool killByImpact = false;
    public EnemyScript enemyOrigin;
    public UnityEvent OnHitImpact;

    public GameObject prefabSpawnOnImpact;

    public float Damage = 10;
    public float DamageSpeedOverride = 10;
    public float timerDead = 6f;
    public Rigidbody rigidbody;

    void Start()
    {
        Destroy(this.gameObject, timerDead);
    }

    private void OnTriggerEnter(Collider other)
    {
        DamageToken token = new DamageToken();
        token.damage = Damage;
        token.origin = DamageToken.DamageOrigin.Environment;
        token.healthSpeed = DamageSpeedOverride;
        token.originEnemy = enemyOrigin;

        UniversalDamage.TryDamage(token, other.transform, transform);


    }

    public void SpawnSomething()
    {
       var gameObject1 = Instantiate(prefabSpawnOnImpact, transform.position, Quaternion.identity);
        gameObject1.gameObject.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (killByImpact)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                return;
            }

            OnHitImpact?.Invoke();
            Destroy(this.gameObject);
        }
    }

  
}
