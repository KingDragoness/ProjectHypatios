using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class B0MBScript : EnemyScript
{
    public cameraScript cam;
    public GameObject body;

    public float chaseDistance;
    public float distanceToExplode;
    public float curHealth;
    public float maxHealth;
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
    GameObject player;
    Vector3 playerPos;

    public float attackRange;
    float distance;
    //public Renderer render;
    public Material mat1;
    float afterDeathTime = -1.5f;
    public ParticleSystem aura;
    public ParticleSystem spawnParticle;
    public ParticleSystem explosion;
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
        player = GameObject.FindGameObjectWithTag("Player");
        enemyAI.speed = moveSpeed;
        curHealth = maxHealth;
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
        playerPos = player.transform.position;
        distance = Vector3.Distance(transform.position, playerPos);
       

        if (distance <= attackRange && Physics.Raycast(transform.position, playerPos - transform.position, out RaycastHit hit, attackRange))
        {
            if (hit.transform.tag == "Player")
            {
                haveSeenPlayer = true;
            }    
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

            enemyAI.SetDestination(playerPos);

            if (distance < chaseDistance)
            {
                moveSpeed = chaseSpeed;
            }
            if (distance < distanceToExplode || curHealth <= 0f)
            {
                gonnaExplode = true;
            }

            if (gonnaExplode)
            {
                Explode();
            }

            Audio_Chasing.pitch = 1.1f;
        }  
    }

    public override void Attacked(DamageToken token)
    {
        haveSeenPlayer = true;
        DamageOutputterUI.instance.DisplayText(token.damage);
        curHealth -= token.damage;
    }


    void Explode()
    {
        anim.SetBool("gonnaExplode", true);
        enemyAI.SetDestination(transform.position);
        Audio_Chasing.gameObject.SetActive(false);

        if (Audio_Death.isPlaying == false)
            Audio_Death.Play();

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
            Instantiate(explosion, transform.position, transform.rotation);
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider c in colliders)
            {
                Rigidbody obj = c.GetComponent<Rigidbody>();
                if (obj != null)
                {
                    obj.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }

                PlayerHealth character = c.GetComponent<PlayerHealth>();
                if (character != null)
                {
                    if (distance < highDistance)
                    {
                        damage = highDamage;
                        cam.ShakeCam(.45f, 0.3f);
                    }
                    else if (distance < midDistance)
                    {
                        damage = midDamage;
                        cam.ShakeCam(.3f, 0.15f);
                    }
                    else if (distance < lowDistance)
                    {
                        damage = lowDamage;
                        cam.ShakeCam(.3f, 0.05f);
                    }
                    character.takeDamage((int)damage);
                }

            }

            OnDied?.Invoke();
            Destroy(gameObject);
        }
    }
}
