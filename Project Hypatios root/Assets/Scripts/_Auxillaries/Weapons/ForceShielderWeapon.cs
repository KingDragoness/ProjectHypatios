using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;


public class ForceShielderWeapon : GunScript
{

    [FoldoutGroup("Force Shielder")] public GameObject shieldProtect;
    [FoldoutGroup("Force Shielder")] public Transform attachTarget;
    [FoldoutGroup("Force Shielder")] public damageReceiver damageReceiver;

    public override void OnEnable()
    {
        base.OnEnable();

        isFiring = false;
        shieldProtect.transform.SetParent(null);
    }

    public override void Start()
    {
        base.Start();
        damageReceiver.m_MyEvent.AddListener(TakeDamage);
    }

    private void OnDestroy()
    {
        Destroy(shieldProtect.gameObject);
        DeactivateArmor();

    }

    private void OnDisable()
    {
        shieldProtect.gameObject.SetActive(false);
        attachTarget.gameObject.SetActive(false);
        DeactivateArmor();

    }

    public void TakeDamage(float damage)
    {
        int ammoToRemove = Mathf.RoundToInt(damage*0.2f);
        curAmmo -= ammoToRemove;

        if (curAmmo <= 0)
        {
            curAmmo = 0;
        }
    }

    bool b = false;

    private void ActivateArmor()
    {
        if (Hypatios.Player.IsStatusEffect(ModifierEffectCategory.ArmorRating, "Weapon.ForceShielder") == false)
        {
            Hypatios.Player.CreatePersistentStatusEffect(ModifierEffectCategory.ArmorRating, GetFinalValue("Armor"), "Weapon.ForceShielder");
            Hypatios.Player.CreatePersistentStatusEffect(ModifierEffectCategory.KnockbackResistance, GetFinalValue("KnockbackResist"), "Weapon.ForceShielder");

        }

    }

    private void DeactivateArmor()
    {

        if (Hypatios.Player.IsStatusEffect(ModifierEffectCategory.ArmorRating, "Weapon.ForceShielder") == true)
        {
            Hypatios.Player.RemoveAllEffectsBySource("Weapon.ForceShielder");
        }
    }

    public override void Update()
    {
        base.Update();

        if (isFiring)
        {
            if (!audioFire.isPlaying) audioFire.Play();

            Vector3 posTarget = attachTarget.transform.position;
            Quaternion rotTarget = attachTarget.transform.rotation;
            if (posTarget.y > attachTarget.transform.position.y)
            {

            }
            shieldProtect.transform.position = posTarget;
            shieldProtect.transform.rotation = rotTarget;

            ActivateArmor();
        }
        else
        {
            if (audioFire.isPlaying) audioFire.Stop();
            DeactivateArmor();
        }
    }

    public override void FireWeapon()
    {

    }

    private void FixedUpdate()
    {

        if (isFiring)
        {
            shieldProtect.gameObject.SetActive(true);
            attachTarget.gameObject.SetActive(true);


        }
        else
        {
            shieldProtect.gameObject.SetActive(false);
            attachTarget.gameObject.SetActive(false);
        }
    }
}
