using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class ModularTurretGun : MonoBehaviour
{

    public float damage = 10f;
    public float healthSpeed = 25;
    public float bulletPerSecond = 10;
    public DamageToken.DamageOrigin originToken;
    public Alliance alliance;
    [Range(0f,1f)]
    public float chanceFire = 0.5f;
    [FoldoutGroup("References")] public Transform outWeaponTransform;
    [FoldoutGroup("References")] public GameObject flashWeapon;
    [FoldoutGroup("References")] public GameObject tracerLaser;
    public LayerMask layermaskWeapon;

    private EnemyScript targetEnemy;
    private float nextAttackTime = 0f;
    private bool isHittingSomething = false;
    private bool isHittingTarget = false;

    private void Update()
    {
        if (Time.timeScale <= 0) return;
        FindTarget();


      

    }

    private void FixedUpdate()
    {
        if (targetEnemy == null) return;

        if (Time.time >= nextAttackTime)
        {
            float random1 = Random.Range(0f, 1f);

            if (random1 < chanceFire)
                FireTurret();

            nextAttackTime = Time.time + 1f / bulletPerSecond + 0.05f;
        }
    }

    private void FindTarget()
    {
        targetEnemy = Hypatios.Enemy.FindEnemyEntity(alliance, transform.position) as EnemyScript;
    }

    private void FireTurret()
    {
        RaycastHit hit;
        isHittingSomething = false;
        isHittingTarget = false;

        if (Physics.Raycast(outWeaponTransform.position, outWeaponTransform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layermaskWeapon, QueryTriggerInteraction.Ignore))
        {
            isHittingSomething = true;

            if (hit.collider.gameObject.IsParentOf(targetEnemy.transform.gameObject))
            {
                isHittingTarget = true;
            }
            else if (hit.collider.gameObject == targetEnemy.transform.gameObject)
            {
                isHittingTarget = true;
            }
        }

        if (isHittingTarget == false)
        {
            float random1 = Random.Range(0f, 1f);

            if (random1 < 0.1f)
                return;
        }

        if (hit.collider != null)
        {
            HitTarget(hit);
        }


    }

    private void HitTarget(RaycastHit hit)
    {

        DamageToken token = new DamageToken();
        token.damage = damage;
        token.origin = originToken;
        token.healthSpeed = healthSpeed;

        var spark = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.BulletSparksEnemy, true);

        spark.transform.position = hit.point;
        spark.transform.rotation = Quaternion.LookRotation(hit.normal);
        flashWeapon.gameObject.SetActive(true);

        UniversalDamage.TryDamage(token, hit.transform, transform);


        var points = new Vector3[2];
        points[0] = outWeaponTransform.transform.position;
        var currentLaser = tracerLaser;
        GameObject laserLine = Instantiate(currentLaser, outWeaponTransform.transform.position, Quaternion.identity);

        {
            points[1] = hit.point;
            var lr = laserLine.GetComponent<LineRenderer>();
            lr.SetPositions(points);
        }
    }

}
