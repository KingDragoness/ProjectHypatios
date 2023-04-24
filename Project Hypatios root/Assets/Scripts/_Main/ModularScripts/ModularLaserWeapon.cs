using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ModularLaserWeapon : MonoBehaviour
{
    [Tooltip("Optional.")] public EnemyScript enemyScript; //optional
    [FoldoutGroup("References")] public LineRenderer laser_LineRendr;
    [FoldoutGroup("References")] public Transform laser_Sparks;
    [FoldoutGroup("References")] public Transform laser_Origin;
    public float LaserDamage = 5f;
    public float LaserPerAttack = 0.2f;
    public float VariableCooldown = 0f;

    private float _timerLaserFX = 0.2f;
    private bool allowLaser = false;
    private RaycastHit currentHit;

    private void Update()
    {
        if (_timerLaserFX > 0)
        {
            _timerLaserFX -= Time.deltaTime;
            allowLaser = false;
        }
        else 
        {
            allowLaser = true;
        }

        Vector3 origin = laser_Origin.position;
        bool isHittingSomething = false;
        RaycastHit hit;

        if (Physics.Raycast(origin, laser_Origin.forward, out hit, 1000f, Hypatios.Enemy.baseSolidLayer, QueryTriggerInteraction.Ignore))
        {
            isHittingSomething = true;
        }

        if (isHittingSomething)
        {
            if (laser_Sparks.gameObject.activeSelf == false) laser_Sparks.gameObject.SetActive(true);
            Vector3[] v3 = new Vector3[2];
            v3[0] = origin;
            v3[1] = hit.point;
            laser_Sparks.transform.position = currentHit.point;
            laser_LineRendr.SetPositions(v3);

        }
        else
        {
            if (laser_Sparks.gameObject.activeSelf == true) laser_Sparks.gameObject.SetActive(false);
            Vector3[] v3 = new Vector3[2];
            v3[0] = origin;
            v3[1] = laser_Origin.forward * 1000f;
            laser_LineRendr.SetPositions(v3);

        }
    }

    private void FixedUpdate()
    {
        if (allowLaser == false)
            return;


        Vector3 origin = laser_Origin.position;
        RaycastHit hit;
        var damageToken = new DamageToken();
        damageToken.originEnemy = enemyScript;
        damageToken.origin = DamageToken.DamageOrigin.Enemy;

        if (Physics.Raycast(origin, laser_Origin.forward, out hit, 1000f, Hypatios.Enemy.baseSolidLayer, QueryTriggerInteraction.Ignore))
        {
            EnemyScript _hitEnemy = null;
            bool isHitSelf = false;
            {
                _hitEnemy = hit.collider.GetComponentInParent<EnemyScript>();
                if (_hitEnemy != null)
                {
                    if (_hitEnemy == enemyScript)
                        isHitSelf = true;
                }
            }

            if (isHitSelf == false)
            {
                float multiplierDamage1 = 1f;
                float damageDist = (LaserDamage * LaserPerAttack) * multiplierDamage1;
                damageDist = Mathf.Clamp(damageDist, 1, 9999);

                damageToken.damage = damageDist;


                UniversalDamage.TryDamage(damageToken, hit.collider.transform, transform);
            }


        }



        currentHit = hit;
        allowLaser = false;
        _timerLaserFX = LaserPerAttack + Random.Range(0f, VariableCooldown);
    }

}
