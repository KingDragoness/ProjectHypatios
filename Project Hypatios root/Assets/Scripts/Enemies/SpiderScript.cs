using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Sirenix.OdinInspector;

public class SpiderScript : EnemyScript
{



    NavMeshAgent enemyAI;
    Vector3 targetPos;

    bool isAttacking = false;
    bool isWalking = false;

    [FoldoutGroup("References")] public ParticleSystem laserCharging;
    [FoldoutGroup("References")] public ParticleSystemRenderer laserChargingRenderer;
    [FoldoutGroup("References")] public GameObject laser;
    [FoldoutGroup("References")] public GameObject blueLaser;
    [FoldoutGroup("References")] public GameObject normalEffectMode;
    [FoldoutGroup("References")] public GameObject playerEffectMode;
    [FoldoutGroup("References")] public GameObject eyeLocation;
    [FoldoutGroup("References")] public ParticleSystem deadPS;
    [FoldoutGroup("References")] public GameObject dissolveEffect;
    [FoldoutGroup("References")] public GameObject body;
    [FoldoutGroup("References")] public AudioSource audio_AboutAttack;
    [FoldoutGroup("References")] public AudioSource audio_Fire;
    [FoldoutGroup("References")] public AudioSource audio_Dead;
    [FoldoutGroup("References")] public List<Material> spiderMat;

    [Space]
    [FoldoutGroup("Stats")] [ColorUsage(false, true)] public Color PlayerColor;
    [FoldoutGroup("Stats")] [ColorUsage(false, true)] public Color NormalColor;
    [FoldoutGroup("Stats")] public float attackRange = 30f;
    [FoldoutGroup("Stats")] public float attackTime;
    [FoldoutGroup("Stats")] public float attackRecharge;
    [FoldoutGroup("Stats")] public float lockBeforeAttackTime;
    [FoldoutGroup("Stats")] public float dieHeight;
    [FoldoutGroup("Stats")] public bool haveSeenPlayer = false;
    [FoldoutGroup("Stats")] public bool onSpawnShouldReady = true;
    [FoldoutGroup("Stats")] public float spawnChance_Ammo = 0.17f;
    [FoldoutGroup("Stats")] public float damping = 2f;

    float curLockTime = 0f;
    float nextAttackTime = 0f;
    float count = 0f;
    float colorSet = 1f;
    float dissolve = -1f;

    [ShowInInspector] [ReadOnly] bool canLookAtTarget = false;
    bool isCharging = false;
    bool hasShot = false;
    bool hasTargetted = false;
    Vector3 lockPos;
    bool hasInstanced = false;
    Vector3 colorVector;
    bool isDie = false;
    float afterDeathTime = 0f;
    bool dissolved = false;

    public SpawnHeal spawnHeal;
    public SpawnAmmo spawnAmmo;

    [ShowInInspector] [ReadOnly] private Entity currentTarget;
    private float timerReady = 3f;
    private bool ready = true;


    // Start is called before the first frame update
    void Start()
    {
        colorSet = 1f;
        currentTarget = Hypatios.Enemy.FindEnemyEntity(Stats.MainAlliance);
        enemyAI = GetComponent<NavMeshAgent>();
        colorVector = new Vector3(1f, 1f, 1f);
        spiderMat = body.GetComponent<Renderer>().materials.ToList();
        //if (spiderMat.Count >= 2) spiderMat[2].SetVector("_ColorSet", colorVector);
        foreach (Material m in spiderMat)
        {
            m.SetFloat("_dissolve", dissolve);
        }
        spawnHeal = GetComponent<SpawnHeal>();
        spawnAmmo = GetComponent<SpawnAmmo>();

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
            if (Mathf.RoundToInt(Time.time) % 5 == 0)
                ScanForEnemies();

            enemyAI.updateRotation = false;

            if (currentTarget != null) ProcessAI();
            else ScanForEnemies();

            UpdateVisuals();
        }

        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }
     }

    private void UpdateVisuals()
    {
        if (Stats.MainAlliance == Alliance.Player)
        {
            if (!playerEffectMode.gameObject.activeSelf)
            {
                laserChargingRenderer.material.SetColor("_EmissionColor", PlayerColor);
                spiderMat[2].SetVector("_EmissionColor", PlayerColor);
                playerEffectMode.gameObject.SetActive(true);
            }

            if (normalEffectMode.gameObject.activeSelf)
                normalEffectMode.gameObject.SetActive(false);

        }

        if (Stats.MainAlliance != Alliance.Player)
        {
            if (!normalEffectMode.gameObject.activeSelf)
            {
                laserChargingRenderer.material.SetColor("_EmissionColor", NormalColor);
                spiderMat[2].SetVector("_EmissionColor", NormalColor);
                normalEffectMode.gameObject.SetActive(true);
            }

            if (playerEffectMode.gameObject.activeSelf)
                playerEffectMode.gameObject.SetActive(false);
        }
    }

    private void ProcessAI()
    {
        var posOffsetLook = currentTarget.OffsetedBoundPosition;
        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        Debug.DrawLine(transform.position, posOffsetLook);

        if (!isDie && distance < attackRange)
        {

            if (Physics.Raycast(eyeLocation.transform.position, posOffsetLook - eyeLocation.transform.position, out RaycastHit hit, distance))
            {
                if (hit.transform.tag == "Player" | Hypatios.Enemy.CheckTransformIsAnEnemy(hit.transform, Stats.MainAlliance))
                {
                    haveSeenPlayer = true;
                    Debug.DrawLine(eyeLocation.transform.position, posOffsetLook - eyeLocation.transform.position);
                }

            }
        }

        if (haveSeenPlayer && !isDie)
        {
            ManageSpiderRotation();


            Ray ray = new Ray(eyeLocation.transform.position, posOffsetLook - eyeLocation.transform.position);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.transform.tag == "Player" | Hypatios.Enemy.CheckTransformIsAnEnemy(hit.transform, Stats.MainAlliance))
                {
                    canLookAtTarget = true;
                }
                else
                {
                    canLookAtTarget = false;
                }
            }


            if (!canLookAtTarget)
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

    [FoldoutGroup("Debug")] [Button("Enforce scan target")]
    private void ScanForEnemies()
    {
        float distPlayer = Vector3.Distance(Hypatios.Player.transform.position, transform.position);
        float f_valueChoosingPlayerAllies = Mathf.Clamp(distPlayer * 0.03f, 0.3f, 0.9f); //distance is 20 then 0.6, distance is 33 then 1 (limit)
        currentTarget = Hypatios.Enemy.FindEnemyEntity(Stats.MainAlliance, transform.position, chanceSelectAlly: f_valueChoosingPlayerAllies);
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
                targetPos = currentTarget.transform.position;
            }

            if (count >= attackTime)
            {
                
                hasTargetted = false;
                var points = new Vector3[2];
                points[0] = eyeLocation.transform.position;

                var currentLaser = laser;
                if (Stats.MainAlliance == Alliance.Player) currentLaser = blueLaser;
                GameObject laserLine = Instantiate(currentLaser, eyeLocation.transform.position, Quaternion.identity);
                points[1] = targetPos;
                var lr = laserLine.GetComponent<LineRenderer>();
                lr.SetPositions(points);
                hasShot = true;

                Ray ray = new Ray(eyeLocation.transform.position, targetPos - eyeLocation.transform.position);
                if (Physics.SphereCast(ray, .2f, out RaycastHit hit, 100f))
                {
                    DamageToken token = new DamageToken();
                    token.damage = Stats.BaseDamage.Value + Random.Range(0, Stats.VariableDamage.Value);
                    token.originEnemy = this;
                    if (Stats.MainAlliance != Alliance.Player) token.origin = DamageToken.DamageOrigin.Enemy; else token.origin = DamageToken.DamageOrigin.Ally;
                    token.healthSpeed = 10f;
                    token.shakinessFactor = 1f;

                    UniversalDamage.TryDamage(token, hit.transform, transform);

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
            enemyAI.SetDestination(currentTarget.transform.position);
    }

    public override void Attacked(DamageToken token)
    {
        haveSeenPlayer = true;
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


        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }
        else
        {
            if (token.origin == DamageToken.DamageOrigin.Player | token.origin == DamageToken.DamageOrigin.Ally) DamageOutputterUI.instance.DisplayText(token.damage);
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
        //if (spiderMat.Count >= 2) spiderMat[2].SetVector("_ColorVector", colorVector);

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
            OnDied?.Invoke();
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
