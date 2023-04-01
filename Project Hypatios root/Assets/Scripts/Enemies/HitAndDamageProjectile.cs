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
    public bool isAllowIndicator = false;
    public bool useObjectPooler = false;
    [ShowIf("useObjectPooler")] public bool reuseInactive = true;
    [Tooltip("For player-made explosions")] [ShowIf("allowHitEnemy")] public bool shouldOverrideAsPlayerOrigin = false;
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
        if (!allowHitEnemy && other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            return;
        
        AttemptDamage(other);

    }


    private void AttemptDamage(Collider other)
    {
        DamageToken token = new DamageToken();
        token.damage = Damage;
        token.origin = DamageToken.DamageOrigin.Environment;
        if (allowHitEnemy) token.origin = DamageToken.DamageOrigin.Player;
        token.healthSpeed = DamageSpeedOverride;
        token.originEnemy = enemyOrigin;
        token.isBurn = isBurn;
        if (isAllowIndicator) token.allowPlayerIndicator = true;

        UniversalDamage.TryDamage(token, other.transform, transform);
    }

    private void Update()
    {
        timerDead -= Time.deltaTime;
    }

    public void SpawnSomething()
    {
        GameObject gameObject1 = null;

        if (useObjectPooler == false)
            gameObject1 = Instantiate(prefabSpawnOnImpact, transform.position, Quaternion.identity);
        else
        {
            gameObject1 = Hypatios.ObjectPool.SummonObject(prefabSpawnOnImpact, 2, 30, reuseInactive, spawnPos: transform.position);
        }
        gameObject1.gameObject.SetActive(true);

        if (shouldOverrideAsPlayerOrigin)
        {
            var allKillzones = gameObject1.GetComponentsInChildren<KillZone>();

            foreach (var killzone in allKillzones)
                killzone.origin = DamageToken.DamageOrigin.Player;
        }
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
            AttemptDamage(collision.collider);

            Destroy(this.gameObject);
        }
    }

  
}
