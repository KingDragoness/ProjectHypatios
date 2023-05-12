using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BirdEnemy : EnemyScript
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
    public float rotationSpeed = 7.5f;
    [Space]
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

    private bool isDead = false;

    private Vector3 lookPos;
    float afterDeathTime = 0f;
    public SpawnAmmo spawnAmmo;

    private void Start()
    {
        currentTarget = Hypatios.Enemy.FindEnemyEntity(Stats.MainAlliance);
        spawnAmmo = GetComponent<SpawnAmmo>();
    }

    private void Update()
    {
        if (Stats.CurrentHitpoint <= 0f)
        {
            dummyAI.enabled = false;
            Die();
        }
        else
        {
            if (isAIEnabled == false) return;

            if (Mathf.RoundToInt(Time.time) % 5 == 0)
                ScanForEnemies();

            AliveState();
            EvaluateMyLife();
        }


    }

 

    private float timerFire = 0;
    private float evaluateChoiceTimer = 2;

    private void EvaluateMyLife()
    {
        evaluateChoiceTimer += Time.deltaTime;

        if (currentTarget == null) return;

        if (evaluateChoiceTimer > 2)
        {
            float randomSound = Random.Range(0f, 5f);
            float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (randomSound < 1f) audio_Idle.Play();
            AI_Detection();

            if (hasSeenPlayer)
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

                stateAI = StateAI.Idle;
            }
            evaluateChoiceTimer = 0;
        }
    }

    public void AliveState()
    {
        if (currentTarget == null) return;

        if (stateAI == StateAI.Pursue)
        {
            head.transform.LookAt(currentTarget.transform);
            dummyAI.disableBehavior = false;
            dummyAI.Rigidbody.isKinematic = false;
            animator.SetBool("Attack", false);
        }
        else if (stateAI == StateAI.Attack)
        {
            dummyAI.disableBehavior = true;
            dummyAI.Rigidbody.isKinematic = true;
            animator.SetBool("Attack", true);
            head.transform.LookAt(currentTarget.transform);

            Vector3 relativePos = currentTarget.transform.position - transform.position;

            Quaternion toRotation = Quaternion.LookRotation(relativePos);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            timerFire += Time.deltaTime;

            if (timerFire > fireRate)
            {
                audio_BirdAttack.PlayOneShot(audio_BirdAttack.clip);
                var projectile1 = throwProjectileEnemy.FireProjectile();
                projectile1.Damage = damage + Random.Range(0, variableDamage);
                projectile1.enemyOrigin = this;
                timerFire = 0;
            }
        }
        else if (stateAI == StateAI.Idle)
        {
            dummyAI.Rigidbody.isKinematic = false;
            dummyAI.disableBehavior = false;
            head.transform.LookAt(currentTarget.transform);
        }
    }

    public override void Attacked(DamageToken token)
    {
        if (token.originEnemy == this) return;
        _lastDamageToken = token;

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

        hasSeenPlayer = true;

        if (!Stats.IsDead)
            DamageOutputterUI.instance.DisplayText(token.damage);
        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }

    }

    public override void Die()
    {
        if (isDead == false)
        {
            LootDrop();
            birdCorpse.gameObject.SetActive(true);
            aliveState.gameObject.SetActive(false);
            audio_Die.Play();
            Debug.Log("Dies.");
            isDead = true;
            Stats.IsDead = true;
        }

        afterDeathTime += Time.deltaTime;

        if (afterDeathTime >= 5f)
        {
            OnDied?.Invoke();
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
