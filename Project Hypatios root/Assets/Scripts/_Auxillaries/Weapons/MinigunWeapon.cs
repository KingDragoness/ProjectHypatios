using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility ;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MinigunWeapon : GunScript
{

    [FoldoutGroup("Minigun Weapon")] public float Range = 100f;
    [FoldoutGroup("Minigun Weapon")] public float RevTime = 3f;
    [FoldoutGroup("Minigun Weapon")] public float DefaultRevTime = 3f;
    [FoldoutGroup("Minigun Weapon")] [Range(0f,0.2f)] public float IntervalPerBarrel = 0.1f;
    [FoldoutGroup("Minigun Weapon")] public int BarrelTotal = 5;
    [FoldoutGroup("Minigun Weapon")] public AutoMoveAndRotate BarrelRotateScript;
    [FoldoutGroup("Minigun Weapon")] public float BarrelRotation = 700f;
    [FoldoutGroup("Audios")] public float minPitchRev = 0.2f;
    [FoldoutGroup("Audios")] public float maxPitchRev = 1f;
    [FoldoutGroup("Minigun Weapon")] public AudioSource RevvingAudio;

    private IEnumerator weaponFireCoroutine;
    private float _readyFireTimer = 3f;
    private bool isRevving = false;
    private bool initialized = false;

    private void OnEnable()
    {
        isFiring = false;
        isRevving = false;
        if (initialized) RefreshWeaponStat();
    }

    public override void Start()
    {
        base.Start();
        initialized = true;
        RefreshWeaponStat();
    }

    private void RefreshWeaponStat()
    {
        var weapon = GetWeaponItem();
        var weaponSave = GetWeaponItemSave();
        bool fasterRevving = false;
        if (weapon.IsAttachmentExists("RapidBarrel", weaponSave.allAttachments) == true)
        {
            fasterRevving = true;
        }

        if (fasterRevving)
        {
            RevTime = 1f;
        }
        else
        {
            RevTime = DefaultRevTime;
        }

    }

    public bool IsWeaponReadyFire()
    {
        if (_readyFireTimer < RevTime) return false;
        return true;

    }

    public override void Update()
    {
        base.Update();

        if (Time.timeScale == 0) return;

        if (isRevving)
        {
            if (_readyFireTimer < RevTime) _readyFireTimer += Time.deltaTime;
            if (!RevvingAudio.isPlaying) RevvingAudio.Play();

        }
        else
        {
            if (_readyFireTimer > 0) _readyFireTimer -= Time.deltaTime ;
            if (RevvingAudio.pitch <= minPitchRev) RevvingAudio.Stop();
        }


        float f = Mathf.Clamp(_readyFireTimer, 0, RevTime);
        f /= RevTime;
        float a = Mathf.Lerp(minPitchRev, maxPitchRev, f);
        float b = Mathf.Lerp(0f, BarrelRotation, f);
        RevvingAudio.pitch = a;
        var v3 = BarrelRotateScript.rotateDegreesPerSecond.value;
        v3.z = b;
        BarrelRotateScript.rotateDegreesPerSecond.value = v3;
    }

    public override void FireInput()
    {
        if (Hypatios.Input.Fire1.IsPressed() && curAmmo > 0 && isAutomatic && !isReloading && IsWeaponReadyFire())
        {
            if (!isFiring)
            {
                isFiring = true;
                anim.SetBool("isFiring", isFiring);
            }

            if (Time.time >= nextAttackTime)
            {
                FireWeapon();
                nextAttackTime = Time.time + 1f / bulletPerSecond + 0.03f;
                curAmmo--;
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
        if (Hypatios.Input.Fire1.triggered && curAmmo > 0 && !isAutomatic && !isReloading && IsWeaponReadyFire())
        {

            if (Time.time >= nextAttackTime)
            {
                anim.SetTrigger("shooting");
                FireWeapon();
                nextAttackTime = Time.time + 1f / bulletPerSecond;
                curAmmo--;
            }
        }
        if (Hypatios.Input.Fire2.IsPressed() && !isReloading)
        {
            isRevving = true;
        }
        else
        {
            isRevving = false;
        }
    }

    public override void FireWeapon()
    {
        gunRecoil.RecoilFire();

        if (audioFire != null)
        {
            audioFire.Play();
        }

        if (weaponFireCoroutine != null) StopCoroutine(weaponFireCoroutine);
        weaponFireCoroutine = FireBarrel();
        StartCoroutine(weaponFireCoroutine);
        muzzle1.Emit(1);
        
    }

    IEnumerator FireBarrel()
    {
        for (int i = 0; i < BarrelTotal; i++)
        {
            var damageToken = new DamageToken();
            damageToken.damageType = DamageToken.DamageType.Ballistic;

            var points = new Vector3[2];
            points[0] = bulletShooter.transform.position;
            GameObject trace = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.UseableBulletDustTracer, true);// Instantiate(bulletTracer, bulletShooter.transform.position, Quaternion.identity);
            RaycastHit hit;
            float spreadX = Random.Range(-spread, spread);
            float spreadY = Random.Range(-spread, spread);
            Vector3 raycastDir = new Vector3(cam.transform.forward.x + spreadX, cam.transform.forward.y + spreadY, cam.transform.forward.z);


            if (Physics.Raycast(cam.transform.position, raycastDir, out hit, 1000f, Hypatios.Player.Weapon.defaultLayerMask, QueryTriggerInteraction.Ignore))
            {
                var damageReceiver = hit.collider.gameObject.GetComponentThenChild<damageReceiver>();
                float variableDamage = Random.Range(0, variableAdditionalDamage);

                if (damageReceiver != null)
                {
                    damageToken.damage = damage * Hypatios.Player.BonusDamageGun.Value + variableDamage; damageToken.repulsionForce = repulsionForce;
                    UniversalDamage.TryDamage(damageToken, damageReceiver.transform, transform);
                    HandleCrosshairActive(damageReceiver);
                }

                points[1] = hit.point;//cam.transform.position + cam.transform.forward * hit.distance;
                trace.GetComponent<LineRenderer>().SetPositions(points);
                if (hit.transform.gameObject.layer != 13 &&
                        hit.transform.gameObject.layer != 12)
                {
                    GameObject bulletHole = Hypatios.ObjectPool.SummonObject(bulletImpact, 10, IncludeActive: true);
                    if (bulletHole != null)
                    {
                        bulletHole.transform.position = hit.point + hit.normal * .0001f;
                        bulletHole.transform.rotation = Quaternion.LookRotation(hit.normal);
                        //bulletHole.DisableObjectTimer(2f);
                        bulletHole.transform.SetParent(hit.collider.gameObject.transform);
                    }

                }

                GameObject bulletSpark_ = Hypatios.ObjectPool.SummonObject(bulletSparks, 10, IncludeActive: true);
                if (bulletSpark_ != null)
                {
                    bulletSpark_.transform.position = hit.point;
                    bulletSpark_.transform.rotation = Quaternion.LookRotation(hit.normal);
                    bulletSpark_.DisableObjectTimer(2f);
                }
                //Instantiate(bulletSparks, hit.point, Quaternion.LookRotation(hit.normal));
                //Destroy(bulletSpark_, 4f);

                currentHit = hit;
            }
            else
            {
                points[1] = cam.ViewportToWorldPoint(new Vector3(.5f + spreadX, .5f + spreadY, 100f));
                trace.GetComponent<LineRenderer>().SetPositions(points);
            }

            yield return new WaitForSeconds(IntervalPerBarrel);
        }
    }

}
