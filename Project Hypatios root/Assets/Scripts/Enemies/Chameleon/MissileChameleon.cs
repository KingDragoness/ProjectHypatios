using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MissileChameleon : EnemyScript
{

    public float rotateSpeed = 100;
    public float moveSpeed = 100;
    public float collisionLimitVelocity = 6f;
    public float initialVelocityForce = 1000;
    public float timer = 10;
    public bool isSmart = false;
    [ShowIf("isSmart", true)] public float predictiveVelocityMultiplier = 1f;
    [ShowIf("isSmart", true)] public float directTargetMoveSpeed = 100;

    public Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (currentTarget == null) currentTarget = Hypatios.Player;
        rb.AddForce(transform.forward * initialVelocityForce * rb.mass);
    }

    public void OverrideTarget(Entity t, Alliance currentAlliance)
    {
        currentTarget = t;
        Stats.MainAlliance = currentAlliance;
    }

    public override void Attacked(DamageToken token)
    {
        if (token.origin == DamageToken.DamageOrigin.Enemy) return;

        Stats.CurrentHitpoint -= token.damage;
        base.Attacked(token);
        DamageOutputterUI.instance.DisplayText(token.damage);

        if (Stats.CurrentHitpoint < 0)
        {
            Dead(true);
        }

    }

    private void Update()
    {
        if (isAIEnabled == false) return;

        if (currentTarget == null){ Dead(true); return; }

        Vector3 pos = currentTarget.transform.position;

        if (isSmart && currentTarget is CharacterScript)
        {
            var player = Hypatios.Player;
            var rb = player.GetComponent<Rigidbody>();
            pos += rb.velocity * predictiveVelocityMultiplier;
        }

        var q = Quaternion.LookRotation(pos - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotateSpeed * Time.deltaTime);

        float mcc = 1f; //multiplier correction course

        if (isSmart)
        {
            Vector3 dir =  currentTarget.transform.position - transform.position;
            rb.AddForce(dir * directTargetMoveSpeed * rb.mass * Time.deltaTime);
        }

        rb.AddForce(transform.forward * moveSpeed * rb.mass * Time.deltaTime);

        timer -= Time.deltaTime;

        if (timer < 0 | currentTarget == null)
        {
            Dead(true);
        }

    }

    private void Dead(bool harmless = false)
    {
        if (Stats.IsDead) return;
        GameObject explosion = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.ExplosionAll, false);
        if (harmless)
            explosion = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.ExplosionHarmless, false);

        if (explosion != null)
        {
            explosion.transform.position = transform.position;
            explosion.transform.rotation = Quaternion.identity;
            explosion.gameObject.SetActive(true);
            explosion.DisableObjectTimer(5f);
        }

        Die();
    }

    public override void Die()
    {
        Destroy(gameObject);
        OnSelfKilled?.Invoke();
        Stats.IsDead = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (rb.velocity.magnitude > collisionLimitVelocity)
        {
            Dead();
        }
    }

}
