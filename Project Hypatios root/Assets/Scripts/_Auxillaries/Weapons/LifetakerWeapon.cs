using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LifetakerWeapon : GunScript
{

    [FoldoutGroup("Lifetaker")] public GameObject muzzleSoul;
    [FoldoutGroup("Lifetaker")] public ParticleSystem soulTakeParticle;

    private float cooldownDamage = 0.25f; //Prevent overloading memory

    private void OnEnable()
    {
        isFiring = false;
    }

    public override void Start()
    {
        soulTakeParticle.transform.parent = null;
        base.Start();
    }

    private void OnDestroy()
    {
        Destroy(soulTakeParticle.gameObject);
    }

    public override void Update()
    {
        base.Update();

        if (isFiring)
        {
            if (!audioFire.isPlaying) audioFire.Play();
            if (muzzleSoul.gameObject.activeSelf == false) muzzleSoul.gameObject.SetActive(true);
            damageReceiver damageReceiver = null;
            EnemyScript enemy = null;
            
            if (currentHit.transform != null) damageReceiver = currentHit.transform.gameObject.GetComponentThenChild<damageReceiver>();
            if (damageReceiver != null) enemy = damageReceiver.enemyScript;

            if (soulTakeParticle.isPlaying == false && enemy != null)
            {
                soulTakeParticle.transform.position = currentHit.point;
                soulTakeParticle.Play();
                soulTakeParticle.loop = true;
            }
            else if (enemy == null)
            {
                soulTakeParticle.Stop();
                soulTakeParticle.loop = false;
            }

            cooldownDamage -= Time.deltaTime;

            if (cooldownDamage < 0)
            {
                if (currentHit.collider != null)
                {
                    TryDamage();
                }
                cooldownDamage = 0.25f;
            }
        }
        else
        {
            if (audioFire.isPlaying) audioFire.Stop();
            if (muzzleSoul.gameObject.activeSelf == true) muzzleSoul.gameObject.SetActive(false);
            if (soulTakeParticle.isPlaying == true)
            {
                soulTakeParticle.Stop();
                soulTakeParticle.loop = false;
            }

            cooldownDamage = 0.25f;
        }
    }

    private void TryDamage()
    {
        gunRecoil.RecoilFire();
        var damageToken = new DamageToken();
        var damageReceiver = currentHit.transform.gameObject.GetComponentThenChild<damageReceiver>();
        float variableDamage = Random.Range(0, variableAdditionalDamage);
        damageToken.damageType = DamageToken.DamageType.MiningLaser;

        float distance = Vector3.Distance(transform.position, currentHit.point); //50f

        if (damageReceiver != null)
        {
            float multiplierDamage1 = 1f;
            float damageDist = (damage * Hypatios.Player.BonusDamageGun.Value * 4f + variableDamage) * multiplierDamage1; //4f is from [1 / 0.25 (cooldown) = 4]
            damageDist = Mathf.Clamp(damageDist, 1, 9999);

            damageToken.damage = damageDist; damageToken.repulsionForce = repulsionForce;
            UniversalDamage.TryDamage(damageToken, damageReceiver.transform, transform);

            //steal enemy HP
            EnemyScript enemy = damageReceiver.enemyScript;

            if (enemy != null)
            {
                Hypatios.Player.Health.Heal(Mathf.RoundToInt(damageDist * damageReceiver.multiplier /2f), instantHeal: true);
            }

            HandleCrosshairActive(damageReceiver);
        }
    }

    public override void FireWeapon()
    {
        float spreadX = 0; Random.Range(-spread, spread);
        float spreadY = 0; Random.Range(-spread, spread);
        Vector3 raycastDir = new Vector3(cam.transform.forward.x + spreadX, cam.transform.forward.y + spreadY, cam.transform.forward.z);
        RaycastHit hit;


        if (Physics.Raycast(cam.transform.position, raycastDir, out hit, 50f, Hypatios.Player.Weapon.defaultLayerMask, QueryTriggerInteraction.Ignore))
        {
            currentHit = hit;

        }
        else
        {
            currentHit = hit;
            currentHit.point = (raycastDir * 100f) + cam.transform.position;
        }

    }

}
