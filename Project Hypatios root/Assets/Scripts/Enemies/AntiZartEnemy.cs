using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

//Cut content
public class AntiZartEnemy : EnemyScript
{
    public enum Mode
    {
        ZartChamber,
        Default
    }

    [FoldoutGroup("AI")] public Mode mode = Mode.Default;
    [FoldoutGroup("AI")] [ShowIf("mode", Mode.Default)] public GameObject player;
    [FoldoutGroup("AI")] [ShowIf("mode", Mode.ZartChamber)] public Chamber_Level6 chamberScript;
    [FoldoutGroup("AI")] [ReadOnly] public Transform target;
    [FoldoutGroup("AI")] public RandomSpawnArea randomSpawnRegion;
    [FoldoutGroup("Weapons")] public LineRenderer laser_lineRendr;
    [FoldoutGroup("Weapons")] public float laser_Damage = 20;
    [FoldoutGroup("Weapons")] public float laser_RotateSpeed = 11f;
    [FoldoutGroup("Weapons")] public Transform laser_PointerTarget;
    [FoldoutGroup("Weapons")] public Transform laser_PointerOrigin;
    [FoldoutGroup("Weapons")] public LayerMask layermask;
    [FoldoutGroup("Weapons")] public GameObject damageSpark;
    public GameObject explosion;
    public float rotateSpeed = 26;
    public float speedVertical = 5f;

    private bool isDead = false;

    public override void Attacked(DamageToken token)
    {
        Stats.CurrentHitpoint -= token.damage;
        DamageOutputterUI.instance.DisplayText(token.damage);

        if (mode == Mode.ZartChamber)
            mode = Mode.Default;

        if (Stats.CurrentHitpoint < 0)
        {
            ZartDead();
        }


        base.Attacked(token);
    }

    public override void Die()
    {
        throw new System.NotImplementedException();
    }
    private void ZartDead()
    {
        if (isDead) return;

        gameObject.AddComponent<Rigidbody>();
        Destroy(gameObject, 5f);
        isDead = true;
        var explosion1 = Instantiate(explosion.gameObject);
        explosion1.transform.position = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        laser_lineRendr.useWorldSpace = true;
        currentMoveTarget = randomSpawnRegion.GetAnyPositionInsideBox();
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == Mode.Default)
        {
            AI_ZartDefault();
        }
        if (mode == Mode.ZartChamber)
        {
            AI_ZartChamber();
        }

        UpdateMoveEnemy();
    }

    private Vector3 currentMoveTarget;
    private float cooldownPosition = 2f;

    private void UpdateMoveEnemy()
    {
        cooldownPosition -= Time.deltaTime;
        var step = speedVertical * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, currentMoveTarget, step);

        var lookPos = currentMoveTarget - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotateSpeed);

        if (cooldownPosition < 0)
        {
            currentMoveTarget = randomSpawnRegion.GetAnyPositionInsideBox();
            cooldownPosition = 2f;
        }
    }

    private void AI_ZartDefault()
    {
        if (player != null)
            target = player.transform;

        if (target == null | isDead)
        {
            AI_Zart_Idle();
        }
        else
        {
            AI_Zart_Attack();
        }
    }

    private void AI_ZartChamber()
    {
        if (target == null)
        {
            var customer = chamberScript.GetCustomer();

            if (customer != null)
                target = customer.transform;
 
        }

        if (target == null | isDead)
        {
            AI_Zart_Idle();
        }
        else
        {
            AI_Zart_Attack();
        }
    }

    private float cooldownAttack = 0.5f;

    private void AI_Zart_Attack()
    {
        cooldownAttack -= Time.deltaTime;
        RaycastHit hit;
        laser_lineRendr.gameObject.SetActive(true);

        Vector3 vTarget = target.position + new Vector3(0, 0.2f, 0);
        //vTarget.y += Random.Range(-0.05f, 0.05f);

        var q = Quaternion.LookRotation(vTarget - laser_PointerOrigin.position);
        laser_PointerOrigin.rotation = Quaternion.RotateTowards(laser_PointerOrigin.rotation, q, laser_RotateSpeed * Time.deltaTime);

        if (Physics.Raycast(laser_PointerOrigin.position, laser_PointerOrigin.forward, out hit, 100f, layermask))
        {
            var damageReceiver = hit.collider.gameObject.GetComponent<damageReceiver>();
            var health = hit.collider.gameObject.GetComponent<PlayerHealth>();

            laser_lineRendr.SetPosition(0, laser_PointerOrigin.transform.position);
            laser_lineRendr.SetPosition(1, hit.point);

            laser_PointerTarget.transform.position = hit.point;

            if (damageReceiver != null && cooldownAttack < 0)
                LaserAttack(damageReceiver);

            if (health != null && cooldownAttack < 0)
                LaserAttack(health);
        }
        else
        {
            Vector3 laserFarEnd = laser_PointerOrigin.transform.position + laser_PointerOrigin.forward * 100f;

            laser_lineRendr.SetPosition(0, laser_PointerOrigin.transform.position);
            laser_lineRendr.SetPosition(1, laserFarEnd);

            laser_PointerTarget.transform.position = laserFarEnd;
        }

        if (cooldownAttack < 0)
        {
            cooldownAttack = 0.15f;
        }
    }

    private void LaserAttack(damageReceiver damageReceiver)
    {
        var token = new DamageToken();
        token.damage = laser_Damage;
        token.origin = DamageToken.DamageOrigin.Enemy;
        damageReceiver.Attacked(token);
        var damageSpark1 = Instantiate(damageSpark);
        damageSpark1.transform.position = damageReceiver.transform.position;
        damageSpark1.gameObject.SetActive(true);
    }


    private void LaserAttack(PlayerHealth health)
    {
        //health.takeDamage(Mathf.RoundToInt(laser_Damage));
        var damageSpark1 = Instantiate(damageSpark);
        damageSpark1.transform.position = health.transform.position;
        damageSpark1.gameObject.SetActive(true);
    }

    private void AI_Zart_Idle()
    {
        laser_lineRendr.gameObject.SetActive(false);
    }
}
