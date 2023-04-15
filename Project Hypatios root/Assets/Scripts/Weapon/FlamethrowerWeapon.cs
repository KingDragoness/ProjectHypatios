using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class FlamethrowerWeapon : GunScript
{

    [FoldoutGroup("Flamethrower Weapon")] public ParticleSystem fireParticle;
    [FoldoutGroup("Flamethrower Weapon")] public GameObject fireLight;
    [FoldoutGroup("Flamethrower Weapon")] public KillZone splashDamageScript;

    private float cooldownDamage = 0.1f; //Prevent overloading memory

    private void OnEnable()
    {
        isFiring = false;
        fireParticle.Stop();
        currentHit = new RaycastHit();
    }

    public override void Update()
    {
        base.Update();

        if (isFiring)
        {
            if (muzzle1.gameObject.activeSelf == false) muzzle1.gameObject.SetActive(true);
            if (!audioFire.isPlaying) audioFire.Play();
            cooldownDamage -= Time.deltaTime;
            splashDamageScript.DamagePerSecond = damage * 0.15f * Hypatios.Player.BonusDamageGun.Value;

            if (cooldownDamage < 0)
            {
                if (currentHit.collider != null)
                {
                    TryDamage();
                }
                cooldownDamage = 0.1f;
            }
        }
        else
        {
            if (audioFire.isPlaying) audioFire.Stop();
            muzzle1.gameObject.SetActive(false);

            cooldownDamage = 0.1f;
        }
    }

    private void TryDamage()
    {
        gunRecoil.RecoilFire();
        var damageToken = new DamageToken();
        var damageReceiver = currentHit.collider.gameObject.GetComponentThenChild<damageReceiver>();
        float variableDamage = Random.Range(0, variableAdditionalDamage);

        float distance = Vector3.Distance(transform.position, currentHit.point); //50f

        if (damageReceiver != null)
        {
            float multiplierDamage1 = (5f - (distance / 10f)) / 5f;
            float damageDist = (damage * 4f + variableDamage) * multiplierDamage1;
            damageDist = Mathf.Clamp(damageDist, 1, 9999);

            damageToken.damage = damageDist * Hypatios.Player.BonusDamageGun.Value; damageToken.repulsionForce = repulsionForce;
            damageToken.origin = DamageToken.DamageOrigin.Player;
            damageToken.isBurn = true;
            damageToken.damageType = DamageToken.DamageType.Fire;
            UniversalDamage.TryDamage(damageToken, currentHit.collider.transform, transform);
            StartCoroutine(SetCrosshairHitActive());
        }
    }
    IEnumerator SetCrosshairHitActive()
    {
        crosshairHit.gameObject.SetActive(true);
        MainGameHUDScript.Instance.audio_CrosshairClick.Play();
        yield return new WaitForSeconds(.2f);
        crosshairHit.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {

        if (isFiring)
        {
            fireParticle.Play();
            fireLight.gameObject.SetActive(true);
        }
        else
        {
            fireParticle.Stop();
            fireLight.gameObject.SetActive(false);
        }
    }


    public override void FireWeapon()
    {
        float spreadX = 0; Random.Range(-spread, spread);
        float spreadY = 0; Random.Range(-spread, spread);
        Vector3 raycastDir = new Vector3(cam.transform.forward.x + spreadX, cam.transform.forward.y + spreadY, cam.transform.forward.z);
        RaycastHit hit;


        if (Physics.Raycast(cam.transform.position, raycastDir, out hit, 12f, Hypatios.Player.Weapon.defaultLayerMask, QueryTriggerInteraction.Ignore))
        {
            currentHit = hit;

        }
        else
        {
            currentHit.point = (raycastDir * 100f) + cam.transform.position;
        }
    }
}
