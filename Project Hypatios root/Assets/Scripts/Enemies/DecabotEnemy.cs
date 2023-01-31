using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using System.Linq;

public class DecabotEnemy : EnemyScript
{
    [System.Serializable]
    public class WeaponTurret
    {
        public Transform origin;
        public ParticleSystem laserCharging;
    }

    [FoldoutGroup("Weapons")] public MissileChameleon missilePrefab;
    [FoldoutGroup("Weapons")] public WeaponTurret[] weaponTurret;
    [FoldoutGroup("Weapons")] public float attackRange = 30f;
    [FoldoutGroup("Weapons")] public float attackTime = .2f;
    [FoldoutGroup("Weapons")] public float attackRecharge = .2f;
    [FoldoutGroup("Prefabs")] public GameObject corpse;


    [FoldoutGroup("Audios")] public AudioSource audio_AboutAttack;
    [FoldoutGroup("Audios")] public AudioSource audio_Fire;
    [FoldoutGroup("Audios")] public AudioSource audio_Dead;

    [FoldoutGroup("Movements")] public float damping = 2f;
    [FoldoutGroup("Movements")] public GameObject[] eyeLocations;


    [SerializeField] private SpawnHeal spawnHeal;
    [SerializeField] private SpawnAmmo spawnAmmo;
    private NavMeshAgent enemyAI;
    private bool canLookAtPlayer = false;
    private bool isCharging = false;
    private bool isAttacking = false;
    private bool isWalking = false;
    private bool isDie = false;
    private bool hasShot = false;
    private bool hasTargetted = false;
    private float count = 0f;
    private float nextAttackTime = 0f;
    private Vector3 targetPos;

    private void Start()
    {
        currentTarget = Hypatios.Enemy.FindEnemyEntity(Stats.MainAlliance);
        enemyAI = GetComponent<NavMeshAgent>();
        spawnHeal = GetComponent<SpawnHeal>();
        spawnAmmo = GetComponent<SpawnAmmo>();

    }

    public override void Attacked(DamageToken token)
    {
        Stats.CurrentHitpoint -= token.damage;


        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddRelativeForce(Vector3.forward * -1 * 100 * token.repulsionForce);
        }
        else
        {
            transform.position += Vector3.back * 0.05f * token.repulsionForce;
        }


        if (!Stats.IsDead && token.origin == DamageToken.DamageOrigin.Player)
            DamageOutputterUI.instance.DisplayText(token.damage);

        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }

    }

    public override void Die()
    {
        OnDied?.Invoke();
        Destroy(gameObject);
        var corpse1 = Instantiate(corpse, transform.position, transform.rotation);
        corpse1.gameObject.SetActive(true);
        corpse1.transform.position = transform.position;
        corpse1.transform.rotation = transform.rotation;
        Stats.IsDead = true;
    }

    private void Update()
    {
        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }
        if (isAIEnabled == false) return;

        if (Mathf.RoundToInt(Time.time) % 5 == 0)
            ScanForEnemies();

        if (currentTarget != null)
        {
            Movement();
            Weapons();
        }
    }

    private void Weapons()
    {


        if (!canLookAtPlayer)
        {

            isAttacking = false;
            isWalking = true;

            foreach (var turret1 in weaponTurret)
            {
                var em = turret1.laserCharging.emission;
                em.enabled = false;
            }
            count = 0;
            isCharging = false;
            //FindPosition();
        }
        else
        {
            isAttacking = true;
            isWalking = false;
            Attack();
        }

        if (isAttacking && !isDie)
        {
        }
    }


    void FindPosition()
    {

        if (isWalking)
            enemyAI.SetDestination(currentTarget.transform.position);
    }

    private void Movement()
    {
        ManageSpiderRotation();
        bool _canlookPlayer = false;

        foreach (var eyeLocation in eyeLocations)
        {
            Ray ray = new Ray(eyeLocation.transform.position, currentTarget.transform.position - eyeLocation.transform.position);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, Hypatios.Enemy.baseDetectionLayer))
            {
                if (hit.transform.tag == "Player")
                {
                    _canlookPlayer = true;
                    break;
                }
                else
                {
                }

                //at least its near the player
                float dist = Vector3.Distance(hit.point, currentTarget.transform.position);

                if (dist < 1.5f)
                {
                    _canlookPlayer = true;
                    break;
                }

            }
        }

         canLookAtPlayer = _canlookPlayer;

        if (!canLookAtPlayer)
        {
            enemyAI.updateRotation = true;

            FindPosition();
        }
        else
        {
            enemyAI.updateRotation = false;

            Vector3 posTarget = currentTarget.transform.position;

            Vector3 dir = posTarget - transform.position;
            dir.y = 0;
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = rotation;

            //enemyAI.SetDestination(transform.position);
        }

    }

    void ManageSpiderRotation()
    {
        var lookPos = currentTarget.transform.position - transform.position;

        if (Vector3.Distance(currentTarget.transform.position, transform.position) >= 6f)
            lookPos.y = Mathf.Clamp(lookPos.y, -15f, 15f);
        else
            lookPos.y = 0f;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }

    int index1 = 0;


    [Button("Fire missile")]
    public void FireMissile(Transform outOrigin)
    {
        GameObject prefabMissile = Instantiate(missilePrefab.gameObject, outOrigin.position, outOrigin.rotation);
        prefabMissile.gameObject.SetActive(true);
        var missile = prefabMissile.GetComponent<MissileChameleon>();
        missile.OverrideTarget(currentTarget, Stats.MainAlliance);
        audio_Fire.Play();
        hasShot = true;
    }


    void Attack()
    {

        if (Time.time < nextAttackTime)
        {
            return;
        }

        if (!isCharging)
        {
            foreach (var turret1 in weaponTurret)
            {
                if (turret1.laserCharging != null) turret1.laserCharging.Emit(1);
            }
            isCharging = true;
        }
     
        hasTargetted = false;

        for(int x = 0; x < weaponTurret.Length; x++)
        {
            if (index1 >= weaponTurret.Length)
            {
                index1 = 0;
                nextAttackTime = Time.time + attackRecharge * Hypatios.ExtraAttackSpeedModifier();
                break;
            }

            targetPos = currentTarget.transform.position;


            var turret1 = weaponTurret[index1];


            AI_Detection();

            if (canLookAtTarget)
            {
                FireMissile(turret1.origin);
            }
            else if (isAttacking)
            {
                FireMissile(turret1.origin);
            }

            nextAttackTime = Time.time + attackTime * Hypatios.ExtraAttackSpeedModifier();
            index1++;
            break;
        }

        isCharging = false;

    }

}
