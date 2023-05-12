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
    private float cooldownFire = 0.1f;

    public override void Start()
    {
        base.Start();
    }


    public override void Update()
    {
       

        base.Update();

        if (Time.timeScale == 0) return;

        cooldownFire -= Time.deltaTime;


        if (isAutomatic == false)
        {
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
                if (isChargeReady && cooldownFire < 0f)
                {
                    if (curAmmo > 0) LaunchProjectile();
                    isChargeReady = false;
                }
                _currentChargeTime = 0f;
                chargingIonParticle.Stop();

            }
        }
    }


    public override void FireInput()
    {


        if (isAutomatic == false)
        {
            if (Hypatios.Input.Fire1.WasReleasedThisFrame() && bulletPerSecond > 5 && curAmmo > 0 && !isReloading && IsRecentlyPaused())
            {
                if (cooldownFire < 0f)
                {
                    LaunchProjectile();
                }
            }


            if (Hypatios.Input.Fire1.IsPressed() && curAmmo > 0 && !isReloading)
            {
                if (!isFiring)
                {
                    isFiring = true;
                }

            }
            else
            {
                if (isFiring)
                {
                    isFiring = false;
                }

            }
        }
        else //For Aktion
        {

            if (Hypatios.Input.Fire1.IsPressed() && curAmmo > 0 && !isReloading)
            {
                if (!isFiring)
                {
                    isFiring = true;
                    anim.SetBool("isFiring", isFiring);
                }

                if (Time.time >= nextAttackTime)
                {
                    LaunchProjectile();
                    nextAttackTime = Time.time + 1f / bulletPerSecond + 0.05f;
                }
            }
            else
            {
                if (isFiring)
                {
                    isFiring = false;
                    anim.SetBool("isFiring", isFiring);
                }

            }

        }
    }

    public void LaunchProjectile()
    {
        var weapon = GetWeaponItem();
        var weaponSave = GetWeaponItemSave();
        bool enablePerfectAccuracy = false;
        if (weapon.IsAttachmentExists("CrosshitGrip", weaponSave.allAttachments) == true)
        {
            enablePerfectAccuracy = true;
        }

        if (enablePerfectAccuracy)
        {
            origin.transform.position = cam.transform.position;
            origin.transform.rotation = cam.transform.rotation;
        }
        else
        {
            origin.transform.position = cam.transform.position;

            Vector3 target = cam.transform.position + (cam.transform.forward * 5f);
            target.x += Random.Range(-spread * 5f, spread * 5f);
            target.z += Random.Range(-spread * 5f, spread * 5f);
            Vector3 dir = (target * 1f) - cam.transform.position;
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            origin.transform.rotation = rotation;
        }

        gunRecoil.RecoilFire();
        cooldownFire = 1f / bulletPerSecond + 0.05f;
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
