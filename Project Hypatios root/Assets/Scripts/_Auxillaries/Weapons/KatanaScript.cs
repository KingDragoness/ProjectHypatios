using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class KatanaScript : BaseWeaponScript
{

    public float cooldownAttack = 1f;
    public float range = 5f;

    [FoldoutGroup("References")] public PlayerHealth playerHealth;
    [FoldoutGroup("Audios")] public AudioSource audio_HitSword;

    float nextAttackTime = 0f;
    Camera cam;
    Recoil gunRecoil;

    private void Start()
    {
        weaponSystem = Hypatios.Player.Weapon;
        if (playerHealth == null) playerHealth = Hypatios.Player.Health;
        cam = Hypatios.MainCamera;

        gunRecoil = weaponSystem.Recoil;

    }

    bool b = false;

    private void ActivateArmor()
    {
        if (Hypatios.Player.IsStatusEffect(ModifierEffectCategory.ArmorRating, "Weapon.Katana") == false)
        {
            Hypatios.Player.CreatePersistentStatusEffect(ModifierEffectCategory.ArmorRating, 2f, "Weapon.Katana");
        }

    }

    private void DeactivateArmor()
    {

        if (Hypatios.Player.IsStatusEffect(ModifierEffectCategory.ArmorRating, "Weapon.Katana") == true)
        {
            Hypatios.Player.RemoveAllEffectsBySource("Weapon.Katana");
        }
    }


    private void Update()
    {
        if (Time.timeScale <= 0) return;

        if (Input.GetButtonDown("Fire1"))
        {
            FireWeapon();
        }

        if (Input.GetButton("Fire2"))
        {
            ActivateArmor();
            if (!b) Hypatios.Player.CreatePersistentStatusEffect(ModifierEffectCategory.MovementBonus, -0.5f, "Katana"); 
            anim.SetBool("Block", true);
            b = true;
        }
        else
        {
            DeactivateArmor();
            if (b) Hypatios.Player.RemoveAllEffectsBySource("Katana");
            //characterScript.speedMultiplier.Value = 8f;
            anim.SetBool("Block", false);
            b = false;
        }
    }

    public override void FireWeapon()
    {
        if (Time.time < nextAttackTime)
        {
            return;
        }

        gunRecoil.RecoilFire();

        Vector3 raycastDir = new Vector3(cam.transform.forward.x, cam.transform.forward.y, cam.transform.forward.z);
        RaycastHit hit;
        anim.SetTrigger("melee");
        nextAttackTime = Time.time + cooldownAttack;

        if (Physics.Raycast(cam.transform.position, raycastDir, out hit, range, Hypatios.Player.Weapon.defaultLayerMask, QueryTriggerInteraction.Ignore))
        {
            var damageReceiver = hit.transform.gameObject.GetComponentThenChild<damageReceiver>();

            if (damageReceiver != null)
            {
                DamageEnemy(damageReceiver);
            }
        }

    }

    public void DamageEnemy(damageReceiver damageReceiver)
    {
        var token = new DamageToken();
        token.damage = damage + Random.Range(0, variableAdditionalDamage);
        token.repulsionForce = 0.1f;
        UniversalDamage.TryDamage(token, damageReceiver.transform, transform);
        if (audio_HitSword) audio_HitSword.Play();
    }

}
