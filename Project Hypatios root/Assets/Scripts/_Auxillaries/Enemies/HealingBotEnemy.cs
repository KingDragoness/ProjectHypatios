using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class HealingBotEnemy : EnemyScript
{

    public float BoltHealAmount = 40f;
    public float limitAngle = 33f;
    public float cooldownWeapon = 3f;
    public Transform outWeapon;
    public GameObject corpse;
    [FoldoutGroup("References")] public GameObject laser;
    [FoldoutGroup("References")] public GameObject prefabHealParticle;
    public DummyEnemyTest dummyAI;

    private float _timerShootingBolt = 2f;

    private void Update()
    {
        if (Time.timeScale <= 0) return;

        if (Stats.CurrentHitpoint <= 0f)
        {
            dummyAI.enabled = false;
            Die();
            return;
        }

        if (isAIEnabled == false)
        {
            dummyAI.disableBehavior = true;
            return;
        }
        else
        {
            if (dummyAI.disableBehavior)
                dummyAI.disableBehavior = false;
        }

        if (Mathf.RoundToInt(Time.time) % 5 == 0)
            ScanForEnemies(overrideAlliance: Alliance.Player);

        if (currentTarget == null) return;
        dummyAI.target = currentTarget.transform;

        UpdateShootHealingBolts();

    }

    private void UpdateShootHealingBolts()
    {
        _timerShootingBolt -= Time.deltaTime;

        if (_timerShootingBolt < 0f)
        {
            _timerShootingBolt = cooldownWeapon;
        }
        else return;

        RaycastHit hit;

        damageReceiver dmgReceiver = null;
        outWeapon.transform.LookAt(currentTarget.OffsetedBoundWorldPosition);

        if (Physics.Raycast(outWeapon.transform.position, outWeapon.transform.forward, out hit, 100f, Hypatios.Enemy.baseSolidLayer, QueryTriggerInteraction.Ignore))
        {
            var damageReceiver = hit.collider.GetComponent<damageReceiver>();

            if (damageReceiver != null)
            {
                dmgReceiver = damageReceiver;
               
            }
        }
        else
        {
            hit.point = outWeapon.transform.position + outWeapon.transform.forward * 100f;
        }

        if (dmgReceiver != null)
            FireLaser(dmgReceiver, hit);
    }

    private void FireLaser(damageReceiver damageReceiver, RaycastHit hit)
    {
        var points = new Vector3[2];
        points[0] = eyeLocation.transform.position;
        points[1] = hit.point;
        var currentLaser = laser;
        GameObject laserLine = Instantiate(currentLaser, eyeLocation.transform.position, Quaternion.identity);

        var enemy = damageReceiver.enemyScript;
        float _heal = BoltHealAmount;

        if (enemy.Stats.CurrentHitpoint + BoltHealAmount > enemy.Stats.MaxHitpoint.Value)
        {
            float maxToHeal = enemy.Stats.MaxHitpoint.Value - enemy.Stats.CurrentHitpoint;
            maxToHeal = Mathf.Clamp(maxToHeal, 0f, enemy.Stats.MaxHitpoint.Value);
            _heal = maxToHeal;
        }

        damageReceiver.enemyScript.Heal(_heal);

        var lr = laserLine.GetComponent<LineRenderer>();
        lr.SetPositions(points);

        {
            var HealParticle = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.HealEffect, false);
            var particleFX = HealParticle.GetComponent<ParticleFXResizer>();
            HealParticle.transform.position = damageReceiver.enemyScript.OffsetedBoundWorldPosition;
            HealParticle.transform.localEulerAngles = Vector3.zero;


            particleFX.ResizeParticle(damageReceiver.enemyScript.OffsetedBoundScale.magnitude);
        }
    }

    public override void Attacked(DamageToken token)
    {
        _lastDamageToken = token;

        hasSeenPlayer = true;
        Stats.CurrentHitpoint -= token.damage;


        if (Stats.IsDead == false)
            if (token.origin == DamageToken.DamageOrigin.Player | token.origin == DamageToken.DamageOrigin.Ally) DamageOutputterUI.instance.DisplayText(token.damage);

        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }
    }

    public override void Die()
    {
        {
            var corpse1 = Instantiate(corpse, transform.position, transform.rotation);
            corpse1.gameObject.SetActive(true);
            corpse1.transform.position = transform.position;
            corpse1.transform.rotation = transform.rotation;
        }
        Destroy(gameObject);
        OnSelfKilled?.Invoke();
    }
}
