using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdEnemy : Enemy
{

    public enum StateAI
    {
        Pursue,
        Attack,
        Idle
    }

    public DummyEnemyTest dummyAI;
    public StateAI stateAI;
    public Animator animator;
    public ThrowProjectileEnemy throwProjectileEnemy;
    public float fireRate = 1.2f;
    public float maxAttackRange = 30;
    public float acquisationRange = 65;
    [Space]
    public float maxHealth;
    public float curHealth;
    public float damage;
    public float variableDamage = 1;
    public float spawnChance_Ammo = 0.21f;

    [Header("Audio")]
    public AudioSource audio_BirdAttack;
    public AudioSource audio_Die;
    public AudioSource audio_Idle;

    [Header("Objects")]
    public GameObject head;
    public GameObject birdCorpse;
    public GameObject aliveState;

    private Transform player;
    private bool isDead = false;
    private bool hasSeen = false;

    private Vector3 lookPos;
    float afterDeathTime = 0f;
    public SpawnAmmo spawnAmmo;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spawnAmmo = GetComponent<SpawnAmmo>();

        curHealth = maxHealth;
    }

    private void Update()
    {
        if (curHealth <= 0f)
        {
            dummyAI.enabled = false;
            Die();
        }
        else
        {
            AliveState();
            EvaluateMyLife();
        }
    }

    private float timerFire = 0;
    private float evaluateChoiceTimer = 5;

    private void EvaluateMyLife()
    {
        evaluateChoiceTimer += Time.deltaTime;

        if (evaluateChoiceTimer > 4)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            audio_Idle.Play();

            if (hasSeen)
            {
                if (dist <= maxAttackRange)
                {
                    stateAI = StateAI.Attack;
                }
                else
                {
                    stateAI = StateAI.Pursue;
                }
            }
            else
            {
                if (dist <= acquisationRange)
                {
                    hasSeen = true;
                }

                stateAI = StateAI.Idle;
            }
            evaluateChoiceTimer = 0;
        }
    }

    public void AliveState()
    {
        if (stateAI == StateAI.Pursue)
        {
            dummyAI.Player = player.transform;
            head.transform.LookAt(player);
            dummyAI.enabled = true;
            animator.SetBool("Attack", false);
        }
        else if (stateAI == StateAI.Attack)
        {
            dummyAI.enabled = false;
            animator.SetBool("Attack", true);
            head.transform.LookAt(player);

            Vector3 offset = player.transform.position + new Vector3(0, 3, 0);
            transform.LookAt(offset);

            timerFire += Time.deltaTime;

            if (timerFire > fireRate)
            {
                audio_BirdAttack.PlayOneShot(audio_BirdAttack.clip);
                var projectile1 = throwProjectileEnemy.FireProjectile();
                projectile1.Damage = damage + Random.Range(0, variableDamage);
                timerFire = 0;
            }
        }
        else if (stateAI == StateAI.Idle)
        {
            dummyAI.enabled = false;
            head.transform.LookAt(player);
        }
    }

    public override void Attacked(DamageToken token)
    {
        curHealth -= damage;


        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddRelativeForce(Vector3.forward * -1 * 100 * token.repulsionForce);
        }
        else
        {
            transform.position += Vector3.back * 0.05f * token.repulsionForce;
        }

        hasSeen = true;

        if (curHealth <= 0f)
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
        if (isDead == false)
        {
            LootDrop();
            birdCorpse.gameObject.SetActive(true);
            aliveState.gameObject.SetActive(false);
            audio_Die.Play();
            Debug.Log("Dies.");
            isDead = true;
        }

        afterDeathTime += Time.deltaTime;

        if (afterDeathTime >= 5f)
        {
            IAmDead(this);
            Destroy(gameObject);
        }
    }

    private void LootDrop()
    {
        spawnAmmo.SpawnAmmoCapsule(spawnChance_Ammo);

        float randomChanceMoreSoul = Random.Range(0f, 1f);
        spawnAmmo.SpawnSoulCapsule();
        spawnAmmo.SpawnSoulCapsule();

        if (randomChanceMoreSoul > 0.6f)
        {
            spawnAmmo.SpawnSoulCapsule();
            spawnAmmo.SpawnSoulCapsule();

        }
        if (randomChanceMoreSoul > 0.95f)
        {
            spawnAmmo.SpawnSoulCapsule();
            spawnAmmo.SpawnSoulCapsule();
            spawnAmmo.SpawnSoulCapsule();

        }
    }
}
