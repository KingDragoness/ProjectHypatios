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

    private void OnEnable()
    {
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
        Hypatios.Player.Health.armorStrength = 1f;

    }

    private void OnDisable()
    {
        shieldProtect.gameObject.SetActive(false);
        attachTarget.gameObject.SetActive(false);
        Hypatios.Player.Health.armorStrength = 1f;

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

            Hypatios.Player.Health.armorStrength = GetFinalValue("Armor");


        }
        else
        {
            if (audioFire.isPlaying) audioFire.Stop();

            Hypatios.Player.Health.armorStrength = 1f;

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
