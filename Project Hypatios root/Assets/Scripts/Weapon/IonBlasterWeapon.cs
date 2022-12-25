using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class IonBlasterWeapon : GunScript
{
    [FoldoutGroup("Ion Weapon")] public LineRenderer laserTracer;
    [FoldoutGroup("Ion Weapon")] public ParticleSystem chargingIonParticle;
    [FoldoutGroup("Ion Weapon")] public ThrowProjectileEnemy throwProjectile;
    [FoldoutGroup("Ion Weapon")] public Transform origin;
    [FoldoutGroup("Audios")] public float minPitchLaser = 0.2f;
    [FoldoutGroup("Audios")] public float maxPitchLaser = 3f;
    [FoldoutGroup("Audios")] public AudioSource audioLaserLoop;

    private float _currentChargeTime = 0;
    private bool isChargeReady = false;

    public override void Update()
    {
        base.Update();

        if (isFiring)
        {
            _currentChargeTime += Time.deltaTime;
            if (!audioLaserLoop.isPlaying) audioLaserLoop.Play();

            float endTime = (1f / bulletPerSecond + 0.05f);
            float a = _currentChargeTime / endTime;
            a = Mathf.Clamp(a, 0f, 1f);
            float pitch = Mathf.Lerp(minPitchLaser, maxPitchLaser, a);
            audioLaserLoop.pitch = a;
            chargingIonParticle.Play();

            if (_currentChargeTime > endTime)
            {
                isChargeReady = true;
            }
        }
        else
        {
            if (audioLaserLoop.isPlaying) audioLaserLoop.Stop();
            if (isChargeReady)
            {
                if (curAmmo > 0) LaunchProjectile();
                isChargeReady = false;
            }
            _currentChargeTime = 0f;
            chargingIonParticle.Stop();

        }
    }

    public override void FireInput()
    {
        if (Input.GetButton("Fire1") && curAmmo > 0 && !isReloading)
        {
            if (!isFiring)
            {
                isFiring = true;
            }

            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + 1f / bulletPerSecond + 0.05f;
            }
        }
        else
        {
            if (isFiring)
            {
                isFiring = false;
            }

        }

        if (Input.GetButtonUp("Fire1") && bulletPerSecond > 5 && curAmmo > 0 && !isReloading)
        {
            LaunchProjectile();
        }
    }

    public void LaunchProjectile()
    {
        gunRecoil.RecoilFire();

        audioFire.Play();
        throwProjectile.FireProjectile(origin.forward);
        anim.SetTrigger("shooting");
        curAmmo--;

    }

    public override void FireWeapon()
    {
        base.FireWeapon();
    }


}
