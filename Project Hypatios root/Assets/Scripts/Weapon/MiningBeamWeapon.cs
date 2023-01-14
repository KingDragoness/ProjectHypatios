using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MiningBeamWeapon : GunScript
{

    [FoldoutGroup("Mining Weapon")] public LineRenderer laserTracer;
    [FoldoutGroup("Mining Weapon")] public ParticleSystem laserSparks;

    private float cooldownDamage = 0.25f; //Prevent overloading memory

    private void OnEnable()
    {
        isFiring = false;
    }

    public override void Update()
    {
        base.Update();

        if (isFiring)
        {
            if (!audioFire.isPlaying) audioFire.Play();
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
            float multiplierDamage1 = (5f - (distance / 10f)) / 5f;
            float damageDist = (damage * 4f + variableDamage) * multiplierDamage1;
            damageDist = Mathf.Clamp(damageDist, 1, 9999);

            damageToken.damage = damageDist; damageToken.repulsionForce = repulsionForce;
            damageReceiver.Attacked(damageToken);
            StartCoroutine(SetCrosshairHitActive());
        }
    }

    private void FixedUpdate()
    {

        if (isFiring)
        {
            if (laserTracer.gameObject.activeSelf == false) laserTracer.gameObject.SetActive(true);

            Vector3[] points = new Vector3[2];
            points[0] = muzzle1.gameObject.transform.position;
            points[1] = currentHit.point;
            laserTracer.SetPositions(points);
            laserSparks.gameObject.transform.position = currentHit.point;
            laserSparks.transform.rotation = Quaternion.LookRotation(currentHit.normal);
            laserSparks.Play();
        }
        else
        {
            if (laserTracer.gameObject.activeSelf == true) laserTracer.gameObject.SetActive(false);
            laserSparks.Stop();
        }
    }


    public override void FireWeapon()
    {
        float spreadX = 0;  Random.Range(-spread, spread);
        float spreadY = 0;  Random.Range(-spread, spread);
        Vector3 raycastDir = new Vector3(cam.transform.forward.x + spreadX, cam.transform.forward.y + spreadY, cam.transform.forward.z);
        RaycastHit hit;


        if (Physics.Raycast(cam.transform.position, raycastDir, out hit, 50f, layerMask, QueryTriggerInteraction.Ignore))
        {
            currentHit = hit;

        }
        else
        {
            currentHit.point = (raycastDir *100f) + cam.transform.position;
        }
    }
}
