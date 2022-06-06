using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class SpiderScript : Enemy
{

    public bool onSpawnShouldReady = true;

    [Space]
    public float maxHealth;
    public float curHealth;
    public float damage;
    public float variableDamage = 1;
    public float spawnChance_Ammo = 0.17f;

    Transform player;
    NavMeshAgent enemyAI;
    Vector3 targetPos;

    bool isAttacking = false;
    bool isWalking = false;

    public ParticleSystem laserCharging;
    public GameObject laser;
    public GameObject eyeLocation;
    public ParticleSystem deadPS;
    public GameObject dissolveEffect;
    public GameObject body;
    public AudioSource audio_AboutAttack;
    public AudioSource audio_Fire;
    public AudioSource audio_Dead;

    [Space]
    public float attackRange = 30f;
    public float attackTime;
    public float attackRecharge;
    public float lockBeforeAttackTime;
    float curLockTime = 0f;
    float nextAttackTime = 0f;
    float count = 0f;
    float colorSet = 1f;
    float dissolve = -1f;

    bool canLookAtPlayer = false;
    bool isCharging = false;
    bool hasShot = false;
    bool hasTargetted = false;
    Vector3 lockPos;
    bool hasInstanced = false;
    Vector3 colorVector;
    bool isDie = false;
    public float dieHeight;
    float afterDeathTime = 0f;
    bool dissolved = false;
    public List<Material> spiderMat;
    public bool haveSeenPlayer = false;

    public LayerMask playerMask;

    public SpawnHeal spawnHeal;
    public SpawnAmmo spawnAmmo;

    public SpawnIndicator spawn;

    private float timerReady = 3f;
    private bool ready = true;

    public float damping = 2f;

    // Start is called before the first frame update
    void Start()
    {
        colorSet = 1f;
        curHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyAI = GetComponent<NavMeshAgent>();
        colorVector = new Vector3(1f, 1f, 1f);
        spiderMat = body.GetComponent<Renderer>().materials.ToList();
        if (spiderMat.Count >= 2) spiderMat[2].SetVector("_ColorSet", colorVector);
        foreach (Material m in spiderMat)
        {
            m.SetFloat("_dissolve", dissolve);
        }
        spawnHeal = GetComponent<SpawnHeal>();
        spawnAmmo = GetComponent<SpawnAmmo>();
        spawn = FindObjectOfType<SpawnIndicator>();

        if (onSpawnShouldReady)
        {
            ready = true;
        }
        else
        {
            NavMeshAgent meshAgent = GetComponent<NavMeshAgent>();
            meshAgent.enabled = false;
            gameObject.AddComponent<Rigidbody>();
            ready = false;
        }

    }

    // Update is called once per frame
    void Update()
    {


        if (ready == false)
        {
            if (timerReady > 0)
            {
                timerReady -= Time.deltaTime;

            }
            else
            {
                if (isDie == false)
                {
                    Rigidbody rb = GetComponent<Rigidbody>();

                    if (rb != null)
                    {
                        Destroy(rb);
                    }
                    NavMeshAgent meshAgent = GetComponent<NavMeshAgent>();
                    meshAgent.enabled = true;
                }

                ready = true;
            }

        }
        else
        {

            enemyAI.updateRotation = false;
            float distance = Vector3.Distance(transform.position, player.position);
            Debug.DrawLine(transform.position, player.position);
            if (!isDie && distance < attackRange)
            {
                if (Physics.Raycast(transform.position, player.position - transform.position, out RaycastHit hit, distance))
                {
                    if (hit.transform.tag == "Player")
                    {
                        haveSeenPlayer = true;
                        Debug.DrawLine(transform.position, player.position - transform.position);
                    }

                }
            }

            if (haveSeenPlayer && !isDie)
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
                    audio_AboutAttack.pitch = 1;
                    audio_AboutAttack.volume = 0.3f;
                    FindPosition();
                    isAttacking = false;
                    var em = laserCharging.emission;
                    em.enabled = false;
                    count = 0;
                    isCharging = false;
                }
                else
                {
                    audio_AboutAttack.pitch = 0.3f;
                    audio_AboutAttack.volume = 0.2f;
                    enemyAI.SetDestination(transform.position);
                    isAttacking = true;
                    isWalking = false;
                }

                if (isAttacking && !isDie)
                {
                    FindPosition();
                    Attack();
                }
            }
        }

        if (curHealth <= 0f)
        {
            Die();
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
    
    void Attack()
    {

        if (Time.time >= nextAttackTime)
        {
            audio_AboutAttack.pitch = 1 + (count/2f * count/2f);
            audio_AboutAttack.volume = 1f;

            count += Time.deltaTime;
            if (!isCharging)
            {
                if (laserCharging != null) laserCharging.Emit(1);
                isCharging = true;
            }

            if (count <= attackTime - .15f)
            {
                targetPos = player.position;
            }

            if (count >= attackTime)
            {
                
                hasTargetted = false;
                var points = new Vector3[2];
                points[0] = eyeLocation.transform.position;

                GameObject laserLine = Instantiate(laser, eyeLocation.transform.position, Quaternion.identity);
                points[1] = targetPos;
                var lr = laserLine.GetComponent<LineRenderer>();
                lr.SetPositions(points);
                hasShot = true;

                Ray ray = new Ray(eyeLocation.transform.position, targetPos - eyeLocation.transform.position);
                if (Physics.SphereCast(ray, .2f, out RaycastHit hit, 100f))
                {
                    if (hit.transform.tag == "Player")
                    {
                        int varDamageResult = Mathf.RoundToInt(Random.Range(-variableDamage, variableDamage));
                        hit.transform.gameObject.GetComponent<health>().takeDamage((int)damage + varDamageResult);
                        if (spawn == null) spawn = FindObjectOfType<SpawnIndicator>();
                        spawn.Spawn(transform);
                    }
                    
                }
                audio_Fire.Play();
                nextAttackTime = Time.time + attackRecharge;
                isCharging = false;
                count = 0f;

            }
         
        }   
    }

    void FindPosition()
    {
        isWalking = true;
        isAttacking = false;
        if (isWalking)
            enemyAI.SetDestination(player.position);
    }

    public override void Attacked(float damage, float repulsionForce = 1f)
    {
        haveSeenPlayer = true;
        curHealth -= damage;


        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddRelativeForce(Vector3.forward * -1 * 100 * repulsionForce);
        }
        else
        {
            transform.position += Vector3.back * 0.05f * repulsionForce;
        }


        if (curHealth <= 0f)
        {
            Die();
        }
        else
        {
            DamageOutputterUI.instance.DisplayText(damage);
        }
    }

    void Die()
    {
        isAttacking = false;
        Destroy(laserCharging);
        audio_AboutAttack.volume = 0f;
        afterDeathTime += Time.deltaTime;
        if (enemyAI.baseOffset > dieHeight)
        {
            enemyAI.baseOffset -= Time.deltaTime * 3f;
        }

        if (!isDie)
        {
            LootDrop();

            NavMeshAgent meshAgent = GetComponent<NavMeshAgent>();
            meshAgent.enabled = false;
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();

            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }

            rb.mass = 10;
            rb.AddForce(Vector3.up * 300 * rb.mass);
            rb.AddForce(-transform.forward * 400 * rb.mass);
            rb.AddRelativeTorque(-transform.forward * 30 * rb.mass);
            audio_Dead.Play();

            isDie = true;
        }
        
        if (colorSet > 0f)
        {
            colorSet -= Time.deltaTime;
        }

        colorVector = new Vector3(colorSet, colorSet, colorSet);
        if (spiderMat.Count >= 2) spiderMat[2].SetVector("_ColorVector", colorVector);

        if (!hasInstanced)
        {
            deadPS.Emit(1);
            deadPS.Play();
            hasInstanced = true;
        }
        
        if (dissolve < 1f && afterDeathTime >= 1.5f)
        {
            if (!dissolved)
            {
                Instantiate(dissolveEffect, transform.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
                dissolved = true;
            }
            
            dissolve += Time.deltaTime;
        }
        foreach(Material m in spiderMat)
        {
            m.SetFloat("_dissolve", dissolve);
            m.SetFloat("_DissolveAmount", dissolve);
            m.SetFloat("_DissolveWidth", 0.05f);
        }
        if (dissolve >= 1f)
        {
            IAmDead(this);
            Destroy(gameObject);
        }
        
    }

    private void LootDrop()
    {
        spawnHeal.SpawnHealCapsule(5);
        spawnAmmo.SpawnAmmoCapsule(spawnChance_Ammo);

        float randomChanceMoreSoul = Random.Range(0f, 1f);
        spawnAmmo.SpawnSoulCapsule();

        if (randomChanceMoreSoul > 0.6f)
        {
            spawnAmmo.SpawnSoulCapsule();
        }
        if (randomChanceMoreSoul > 0.95f)
        {
            spawnAmmo.SpawnSoulCapsule();
        }
    }
}
