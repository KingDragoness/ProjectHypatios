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
    public bool keepFiringIfNoDetection = false;
    [Tooltip("Prevent back-face collider problem.")] public bool useSecondPass = false;
    public DamageToken.DamageOrigin originToken;
    [Tooltip("Optional for enemyScript that have modular gun turret.")] public EnemyScript mySelf; 
    public Alliance alliance;
    [Range(0f,1f)]
    public float chanceFire = 0.5f;
    [FoldoutGroup("References")] public Transform outWeaponTransform;
    [FoldoutGroup("References")] public GameObject flashWeapon;
    [FoldoutGroup("References")] public GameObject tracerLaser;

    private Entity targetEnemy;
    private float nextAttackTime = 0f;
    private bool isHittingSomething = false;
    private bool isHittingTarget = false;
    private bool isTargetingSelf = false;
    private bool secondpass_HitCeiling = false;

    private void Update()
    {
        if (Time.timeScale <= 0) return;
        FindTarget();


      

    }

    private void FixedUpdate()
    {
        if (targetEnemy == null && keepFiringIfNoDetection == false) return;

        if (Time.time >= nextAttackTime)
        {
            float random1 = Random.Range(0f, 1f);

            if (random1 < chanceFire)
                FireTurret();

            nextAttackTime = Time.time + 1f / bulletPerSecond + 0.02f;
        }
    }

    private void FindTarget()
    {
        targetEnemy = Hypatios.Enemy.FindEnemyEntity(alliance, transform.position);
    }

    public bool IsHittingTargetEnemy(GameObject go)
    {
        if (targetEnemy == null)
            return false;

        if (go.IsParentOf(targetEnemy.gameObject))
            return true;

        if (go == targetEnemy.gameObject)
            return true;

        return false;
    }

    private void FireTurret()
    {
        RaycastHit hit;
        isHittingSomething = false;
        isHittingTarget = false;
        isTargetingSelf = false;


        //first pass
        if (Physics.Raycast(outWeaponTransform.position, outWeaponTransform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, Hypatios.Enemy.baseSolidLayer, QueryTriggerInteraction.Ignore))
        {
            isHittingSomething = true;

            if (IsHittingTargetEnemy(hit.collider.gameObject))
            {
                isHittingTarget = true;
            }
           

            if (mySelf != null)
            {
                if (hit.collider.gameObject.IsParentOf(mySelf.transform.gameObject))
                {
                    isTargetingSelf = true;
                }
            }
        }

        //second pass
        if (useSecondPass)
        {
            RaycastHit secondHit;
            if (Physics.Raycast(hit.point, hit.normal, out secondHit, hit.distance + 1, Hypatios.Enemy.baseSolidLayer, QueryTriggerInteraction.Ignore))
            {
                hit = secondHit;
                secondpass_HitCeiling = true;
            }
        }

        if (isHittingTarget == false && keepFiringIfNoDetection == false)
        {
            float random1 = Random.Range(0f, 1f);

            if (random1 < 0.1f)
                return;
        }

        if (isTargetingSelf == false)
        {
            HitTarget(hit);
        }

      
    }

    private void HitTarget(RaycastHit hit)
    {

        if (hit.collider != null)
        {
            DamageToken token = new DamageToken();
            token.damage = damage;
            token.origin = originToken;
            token.healthSpeed = healthSpeed;
            token.shakinessFactor = 0.2f;
            token.damageType = DamageToken.DamageType.Ballistic;

            var spark = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.BulletSparksEnemy, true);

            spark.transform.position = hit.point;
            spark.transform.rotation = Quaternion.LookRotation(hit.normal);
            flashWeapon.gameObject.SetActive(true);

            UniversalDamage.TryDamage(token, hit.transform, transform);
        }

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
