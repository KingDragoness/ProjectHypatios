using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class HitAndDamageProjectile : MonoBehaviour
{

    public bool killByImpact = false;
    public bool isBurn = false;
    public bool allowHitEnemy = false;
    public EnemyScript enemyOrigin;
    public UnityEvent OnHitImpact;
    public float preventKillAboveTimer = 0f;

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
        if (allowHitEnemy) token.origin = DamageToken.DamageOrigin.Player;
        token.healthSpeed = DamageSpeedOverride;
        token.originEnemy = enemyOrigin;
        token.isBurn = isBurn;

        UniversalDamage.TryDamage(token, other.transform, transform);


    }

    private void Update()
    {
        timerDead -= Time.deltaTime;
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

            if (timerDead > preventKillAboveTimer)
                return;

            if (!allowHitEnemy && collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                return;
            }

            OnHitImpact?.Invoke();
            Destroy(this.gameObject);
        }
    }

  
}
