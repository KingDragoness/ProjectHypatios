using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Linq;
using Sirenix.OdinInspector;

public class B0MBScript : EnemyScript
{
    public cameraScript cam;
    public GameObject body;

    public float chaseDistance;
    public float distanceToExplode;
    public float damage;
    public float moveSpeed;
    public float chaseSpeed;
    public float explosionRadius;
    public float explosionForce;

    public float highDamage = 50f;
    public float midDamage = 25f;
    public float lowDamage = 10f;
    public float highDistance = 2f;
    public float midDistance = 5f;
    public float lowDistance = 10f;

    [Space]
    [Header("Audios")]
    public AudioSource Audio_Death;
    public AudioSource Audio_Chasing;

    [Space]
    NavMeshAgent enemyAI;
    Vector3 currentPos;

    public float attackRange;
    float distance;
    //public Renderer render;
    public Material mat1;
    float afterDeathTime = -1.5f;
    public ParticleSystem aura;
    public ParticleSystem spawnParticle;
    Animator anim;
    bool hasInstanced = false;
    bool gonnaExplode = false;
    float colorChange;
    public LayerMask playerMask;

    public List<Material> bombMat;
    GameObject[] bomb;
    public bool haveSeenPlayer;

    // Start is called before the first frame update
    void Start()
    {
        colorChange = 5f;
        chaseSpeed = moveSpeed + 3f;
        anim = transform.GetChild(0).gameObject.GetComponent<Animator>();
        enemyAI = GetComponent<NavMeshAgent>();
        currentTarget = Hypatios.Enemy.FindEnemyEntity(Stats.MainAlliance);
        enemyAI.speed = moveSpeed;
        bombMat = body.GetComponent<Renderer>().materials.ToList();
        foreach (Material m in bombMat)
        {
            m.SetFloat("_ColorChange", 5f);
        }

        if (cam == null)
        {
            cam = FindObjectOfType<cameraScript>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isAIEnabled == false) return;

        if (Mathf.RoundToInt(Time.time) % 5 == 0)
            ScanForEnemies();

        if (currentTarget != null) ProcessAI();
        else ScanForEnemies();


    }

    private void ProcessAI()
    {
        currentPos = currentTarget.transform.position;
        distance = Vector3.Distance(transform.position, currentPos);


        if (!Stats.IsDead && distance < attackRange)
        {
            AI_Detection();
        }


        if (haveSeenPlayer)
        {
            bomb = GameObject.FindGameObjectsWithTag("bomb");
            foreach (GameObject b in bomb)
            {
                if (Vector3.Distance(transform.position, b.transform.position) < attackRange)
                {
                    b.GetComponent<B0MBScript>().haveSeenPlayer = true;
                }
            }

            if (Audio_Chasing.isPlaying == false)
                Audio_Chasing.Play();

            enemyAI.SetDestination(currentPos);

            if (distance < chaseDistance)
            {
                moveSpeed = chaseSpeed;
            }
            if (distance < distanceToExplode || Stats.CurrentHitpoint <= 0f)
            {
                gonnaExplode = true;
            }

            if (gonnaExplode)
            {
                Die();
            }

            Audio_Chasing.pitch = 1.1f;
        }
    }

 


    public override void Attacked(DamageToken token)
    {
        haveSeenPlayer = true;
        DamageOutputterUI.instance.DisplayText(token.damage);
        Stats.CurrentHitpoint -= token.damage;
    }



    public override void Die()
    {
        anim.SetBool("gonnaExplode", true);
        enemyAI.SetDestination(transform.position);
        Audio_Chasing.gameObject.SetActive(false);

        if (Audio_Death.isPlaying == false)
            Audio_Death.Play();

        DamageToken token = new DamageToken();
        token.damage = damage;
        token.origin = DamageToken.DamageOrigin.Enemy;
        token.originEnemy = this;

        if (colorChange >= -.5f)
        {
            colorChange -= Time.deltaTime * 8f;
            foreach (Material m in bombMat)
            {
                m.SetFloat("_ColorChange", colorChange);
            }
            
        }
        else
        {
            var explosion = Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.ExplosionAll, false);
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider c in colliders)
            {
                Rigidbody obj = c.GetComponent<Rigidbody>();
                if (obj != null)
                {
                    obj.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }

                PlayerHealth character = c.GetComponent<PlayerHealth>();
                if (distance < highDistance)
                {
                    token.damage = highDamage;
                    if (character != null) cam.ShakeCam(.45f, 0.3f);
                }
                else if (distance < midDistance)
                {
                    token.damage = midDamage;
                    if (character != null) cam.ShakeCam(.3f, 0.15f);
                }
                else if (distance < lowDistance)
                {
                    token.damage = lowDamage;
                    if (character != null) cam.ShakeCam(.3f, 0.05f);
                }

                UniversalDamage.TryDamage(token, c.transform, transform);

            }

            OnDied?.Invoke();
            Destroy(gameObject);
        }
    }
}
