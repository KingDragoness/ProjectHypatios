using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightSniper : MonoBehaviour
{

    public float damage;
    public float variableDamage = 1;
    public Transform bulletShooter;
    public ParticleSystem muzzle1;
    public GameObject bulletImpact;
    public GameObject bulletSparks;
    public GameObject laser;

    [Space]
    public float rotateSpeed = 3;
    public float CooldownFire = 0.2f;
    public Transform target;

    [Space]

    public AudioSource audioFire;

    private float cooldownFire_ = 0.1f;
    private float cooldownRandom_ = 1f;

    void Start()
    {
        
    }

    private float chance;

    // Update is called once per frame
    void Update()
    {
        Vector3 posError = transform.position;
        posError.x += Random.Range(-1, 1);
        posError.z += Random.Range(-1, 1);

        var q = Quaternion.LookRotation(target.position - posError);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotateSpeed * Time.deltaTime);

        if (cooldownRandom_ > 0)
        {
            cooldownRandom_ -= Time.deltaTime;
        }
        else
        {
            chance = Random.Range(0f, 1f);
            cooldownRandom_ = 0.3f + Random.Range(-0.05f, 0.3f);
        }

        if (cooldownFire_ > 0)
        {
            cooldownFire_ -= Time.deltaTime;
        }
        else
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {

                if (hit.transform.tag == "Player")
                {
                    Fire();
                    cooldownFire_ = CooldownFire;
                }
                else
                {

                    if (chance > Random.Range(0.8f, 0.92f))
                    {
                        Fire();
                        cooldownFire_ = CooldownFire;
                    }

                }
            }

        }
    }

    void Fire()
    {

        var points = new Vector3[2];
        points[0] = bulletShooter.transform.position;
        RaycastHit hit;

        if (audioFire != null)
            audioFire.Play();


        muzzle1.gameObject.SetActive(true);
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            int varDamageResult = Mathf.RoundToInt(Random.Range(-variableDamage, variableDamage));

            DamageToken token = new DamageToken();
            token.damage = (int)damage + varDamageResult;
            token.origin = DamageToken.DamageOrigin.Enemy;
            token.healthSpeed = 25f;

            UniversalDamage.TryDamage(token, hit.transform, transform);

            points[1] = hit.point;
            if (hit.transform.gameObject.layer != 12)
            {
                //GameObject bulletHole = Instantiate(bulletImpact, hit.point + hit.normal * .0001f, Quaternion.LookRotation(hit.normal));
                GameObject bulletSpark_ = Instantiate(bulletSparks, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bulletSpark_, 4f);
                //Destroy(bulletHole, 4f);
            }

        }


        GameObject laserLine = Instantiate(laser, bulletShooter.transform.position, Quaternion.identity);
        var lr = laserLine.GetComponent<LineRenderer>();
        var ls = laserLine.GetComponent<laserSmaller>();
        lr.SetPositions(points);
        ls.AssignLaserWidth(0.08f, 0.3f);

    }
}
