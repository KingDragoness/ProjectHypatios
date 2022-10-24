using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class KatanaScript : BaseWeaponScript
{

    public float cooldownAttack = 1f;
    public float range = 5f;

    [FoldoutGroup("References")] public characterScript characterScript;
    [FoldoutGroup("References")] public health playerHealth;
    [FoldoutGroup("Audios")] public AudioSource audio_HitSword;

    float nextAttackTime = 0f;
    Camera cam;
    Recoil gunRecoil;

    private void Start()
    {
        weaponSystem = GameObject.FindGameObjectWithTag("GunHolder").GetComponent<WeaponManager>();

        if (characterScript == null) characterScript = FindObjectOfType<characterScript>();
        if (playerHealth == null) playerHealth = FindObjectOfType<health>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        gunRecoil = weaponSystem.gunRecoil;

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
            playerHealth.armorStrength = 2f;
            characterScript.speedMultiplier = 4f;
            anim.SetBool("Block", true);
        }
        else
        {
            playerHealth.armorStrength = 1f;
            characterScript.speedMultiplier = 8f;
            anim.SetBool("Block", false);
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

        if (Physics.Raycast(cam.transform.position, raycastDir, out hit, range, layerMask, QueryTriggerInteraction.Ignore))
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
        damageReceiver.Attacked(token);
        if (audio_HitSword) audio_HitSword.Play();
    }

}
