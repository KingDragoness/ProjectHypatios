using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using System.Linq;

public class DecabotEnemy : Enemy
{
    [System.Serializable]
    public class WeaponTurret
    {
        public Transform origin;
        public ParticleSystem laserCharging;
    }

    [ProgressBar(0, "maxHitpoint")]
    public float hitpoint = 900;
    public float maxHitpoint = 900;
    
    [FoldoutGroup("Weapons")] public float damage = 15;
    [FoldoutGroup("Weapons")] public float variableDamage = 1;
    [FoldoutGroup("Weapons")] public float spawnChance_Ammo = 0.17f;
    [FoldoutGroup("Weapons")] public GameObject eyeLocation;
    [FoldoutGroup("Weapons")] public float attackRange = 30f;
    [FoldoutGroup("Weapons")] public float attackTime = .2f;
    [FoldoutGroup("Weapons")] public float attackRecharge = .2f;
    [FoldoutGroup("Weapons")] public WeaponTurret[] weaponTurret;
    [FoldoutGroup("Prefabs")] public GameObject laser;
    [FoldoutGroup("Prefabs")] public GameObject corpse;


    [FoldoutGroup("Audios")] public AudioSource audio_AboutAttack;
    [FoldoutGroup("Audios")] public AudioSource audio_Fire;
    [FoldoutGroup("Audios")] public AudioSource audio_Dead;

    [FoldoutGroup("Movements")] public float damping = 2f;


    [SerializeField] private SpawnHeal spawnHeal;
    [SerializeField] private SpawnAmmo spawnAmmo;
    [SerializeField] private SpawnIndicator spawn;
    private Transform player;
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
        hitpoint = maxHitpoint;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyAI = GetComponent<NavMeshAgent>();
        spawnHeal = GetComponent<SpawnHeal>();
        spawnAmmo = GetComponent<SpawnAmmo>();
        spawn = FindObjectOfType<SpawnIndicator>();

    }

    public override void Attacked(DamageToken token)
    {
        hitpoint -= token.damage;


        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddRelativeForce(Vector3.forward * -1 * 100 * token.repulsionForce);
        }
        else
        {
            transform.position += Vector3.back * 0.05f * token.repulsionForce;
        }


        if (hitpoint <= 0f)
        {
            Die();
        }
        else
        {
            DamageOutputterUI.instance.DisplayText(token.damage);
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        corpse.gameObject.SetActive(true);
        corpse.transform.position = transform.position;
        corpse.transform.rotation = transform.rotation;
    }

    private void Update()
    {
        if (hitpoint <= 0f)
        {
            Die();
        }

        Movement();
        Weapons();
    }

    private void Weapons()
    {


        if (!canLookAtPlayer)
        {

            isAttacking = false;
            foreach (var turret1 in weaponTurret)
            {
                var em = turret1.laserCharging.emission;
                em.enabled = false;
            }
            count = 0;
            isCharging = false;
        }
        else
        {

            isAttacking = true;
            isWalking = false;
        }

        if (isAttacking && !isDie)
        {
            FindPosition();
            Attack();
        }
    }


    void FindPosition()
    {
        isWalking = true;
        isAttacking = false;
        if (isWalking)
            enemyAI.SetDestination(player.position);
    }

    private void Movement()
    {
        ManageSpiderRotation();

        Ray ray = new Ray(eyeLocation.transform.position, player.position - eyeLocation.transform.position);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            if (hit.transform.tag == "Player")
            {
                canLookAtPlayer = true;
            }
            else
            {
                canLookAtPlayer = false;
            }
        }

        if (!canLookAtPlayer)
        {
            FindPosition();
        }
        else
        {
            enemyAI.SetDestination(transform.position);
        }

    }

    void ManageSpiderRotation()
    {
        var lookPos = player.transform.position - transform.position;

        if (Vector3.Distance(player.transform.position, transform.position) >= 6f)
            lookPos.y = Mathf.Clamp(lookPos.y, -15f, 15f);
        else
            lookPos.y = 0f;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }

    int index1 = 0;

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
                nextAttackTime = Time.time + attackRecharge;
                break;
            }

            targetPos = player.position;


            var turret1 = weaponTurret[index1];

            {
                var points = new Vector3[2];
                points[0] = turret1.origin.transform.position;
                GameObject laserLine = Instantiate(laser, turret1.origin.transform.position, Quaternion.identity);
                points[1] = targetPos;
                var lr = laserLine.GetComponent<LineRenderer>();
                lr.SetPositions(points);
                audio_Fire.Play();
            }
            hasShot = true;

            Ray ray = new Ray(turret1.origin.transform.position, targetPos - turret1.origin.transform.position);
            if (Physics.SphereCast(ray, .2f, out RaycastHit hit, 100f))
            {
                if (hit.transform.tag == "Player")
                {
                    int varDamageResult = Mathf.RoundToInt(Random.Range(-variableDamage, variableDamage));
                    hit.transform.gameObject.GetComponent<health>().takeDamage((int)damage + varDamageResult);
                    if (spawn == null) spawn = FindObjectOfType<SpawnIndicator>();
                    spawn.Spawn(transform);
                }

                var damageReceiver = hit.collider.gameObject.GetComponent<damageReceiver>();

                if (damageReceiver != null)
                    LaserAttack(damageReceiver);
            }

            nextAttackTime = Time.time + attackTime;
            index1++;
            break;
        }

        isCharging = false;

    }

    private void LaserAttack(damageReceiver damageReceiver)
    {
        var token = new DamageToken();
        token.damage = damage + 10f + Random.Range(-variableDamage,damage);
        token.origin = DamageToken.DamageOrigin.Enemy;
        damageReceiver.Attacked(token);

    }

}
