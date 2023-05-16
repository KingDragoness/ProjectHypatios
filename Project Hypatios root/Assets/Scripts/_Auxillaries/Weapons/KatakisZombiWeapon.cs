using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class KatakisZombiWeapon : GunScript
{

    public float meleeRange = 5f;
    public int ammoConsumeKatakis = 2;
    public float CooldownBurst = 1f;
    public float TimeExecuteBurst = 0.3f;
    public Transform outPosZombi;
    public Transform explosionPorkos;
    public CooldownTimeTriggerEvent TriggerBurst;
    [FoldoutGroup("Audios")] public AudioSource audio_HitSword;
    [FoldoutGroup("Audios")] public AudioSource audio_burst;

    public override void FireInput()
    {

        //Firing melee, it will not consume any ammo when attack
        //but cannot attack if ammo is zero

        if (Hypatios.Input.Fire1.triggered && curAmmo > 0 && !isReloading)
        {

            if (Time.time >= nextAttackTime)
            {
                anim.SetTrigger("shooting");
                FireWeapon();
                nextAttackTime = Time.time + 1f / bulletPerSecond;
            }
        }


        if (Hypatios.Input.Fire2.triggered && curAmmo >= ammoConsumeKatakis && !isReloading)
        {

            if (Time.time >= nextAttackTime)
            {
                anim.SetTrigger("burst");
                TriggerBurst.ExecuteTrigger(TimeExecuteBurst);
                nextAttackTime = Time.time + CooldownBurst;
                curAmmo -= ammoConsumeKatakis;
            }
        }
        else if (Hypatios.Input.Fire2.triggered && !isReloading)
        {
            CustomReload();
        }
    }

    private void CustomReload()
    {
        OnReloadStart?.Invoke();
        anim.SetTrigger("reload");
        isReloading = true;


        if (audioReload != null)
        {
            audioReload.Play();
        }
    }

    public override void FireWeapon()
    {

        if (audioFire != null)
        {
            audioFire.Play();
        }

        {
            Vector3 offsetVelocity = Hypatios.Player.dir * 2f;

            GameObject clawParticle = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.ClawAttack, false, _pos: transform.position);

            if (clawParticle != null)
            {
                clawParticle.transform.position = outPosZombi.position + offsetVelocity;
                clawParticle.transform.rotation = outPosZombi.transform.rotation;
                clawParticle.gameObject.SetActive(true);
                clawParticle.DisableObjectTimer(0.8f);
            }
        }

        Hypatios.Player.Weapon.Recoil.RecoilFire();
        Vector3 raycastDir = new Vector3(cam.transform.forward.x, cam.transform.forward.y, cam.transform.forward.z);
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, raycastDir, out hit, meleeRange, Hypatios.Player.Weapon.defaultLayerMask, QueryTriggerInteraction.Ignore))
        {
            var damageReceiver = hit.transform.gameObject.GetComponentThenChild<damageReceiver>();

            if (damageReceiver != null)
            {
                DamageEnemy(damageReceiver);
            }
        }

    }

    public void BurstProjectile()
    {
        if (audio_burst != null)
        {
            audio_burst.Play();
        }

        {
            Vector3 offsetVelocity = outPosZombi.position + Hypatios.Player.dir * 2f;


            var prefab = Instantiate(explosionPorkos, offsetVelocity, outPosZombi.rotation);
            prefab.gameObject.SetActive(true);

        }
    }

    public void DamageEnemy(damageReceiver damageReceiver)
    {
        if (damageReceiver.enemyScript == null)
            return;
        var token = new DamageToken();
        token.damage = damage + Random.Range(0, variableAdditionalDamage);
        token.repulsionForce = 0.1f;
        if (Hypatios.Player.BonusDamageMelee.Value != 0) token.damage *= Hypatios.Player.BonusDamageMelee.Value;
        damageReceiver.Attacked(token);
        if (audio_HitSword) audio_HitSword.Play();
    }

}
