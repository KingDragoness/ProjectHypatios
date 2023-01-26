using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class SmartPistolWeapon : GunScript
{

    public HitAndDamageProjectile spawnProjectile;
    public EnemyScript target;
    [FoldoutGroup("Smart Weapon")] public Transform origin;

    public override void Update()
    {
        base.Update();

        if (bulletPerSecond > 12)
        {
            isAutomatic = true;
        }
        else
        {
            isAutomatic = false;
        }

        if (Hypatios.Input.Fire2.IsPressed())
        {

        }
        else
        {

        }
    }

    public override void FireWeapon()
    {
        gunRecoil.RecoilFire();

        if (audioFire != null)
        {
            audioFire.Play();
        }

        var _newHit = GetHit();
        var entity = Hypatios.Enemy.FindEnemyEntityFromScreen(Alliance.Player, cam);

        EnemyScript newTarget = null;
        if (entity != null)
        {
            newTarget = entity as EnemyScript;
        }

        if (newTarget != null)
        {
            target = newTarget;
        }

        var damageToken = new DamageToken();
        damageToken.damageType = damageType;

        muzzle1.Emit(1);

        Vector3 posSpawn = cam.transform.position;
        posSpawn += cam.transform.forward * 0.1f;
        //create projectile and tracer
        var projectile1 = Instantiate(spawnProjectile, posSpawn, cam.transform.rotation);
        var bulletTracer1 = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.BulletDustTracer, true);
        projectile1.gameObject.SetActive(true);
        projectile1.Damage = damage * Hypatios.Player.BonusDamageGun.Value;
        projectile1.isAllowIndicator = true;
        var projectileScript = projectile1.GetComponent<SmartBulletProjectile>();

        if (target != null)
        {
            projectileScript.enemyTarget = target;
            projectileScript.SetLookImmediately();
            projectileScript.cachedTargetPos = _newHit.point;
        }
        else
        {

            projectileScript.cachedTargetPos = _newHit.point;

            if (_newHit.collider == null)
            {
                projectileScript.isDisableAutoTrack = true;
            }
        }

        var tracerScript = bulletTracer1.GetComponent<BulletTracerScript>();
        tracerScript.currentProjectile = projectile1.transform;
        tracerScript.transform.position = origin.position;
        tracerScript.ResetTracer();
    }

}
