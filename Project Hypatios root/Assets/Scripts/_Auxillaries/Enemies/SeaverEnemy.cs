using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using System.Linq;

public class SeaverEnemy : EnemyScript
{


    public Transform spawnScarab;
    public Animator anim;
    public GameObject scarabPrefab;
    [FoldoutGroup("Prefabs")] public GameObject corpse;
    [FoldoutGroup("Audios")] public AudioSource audio_Fire;
    public float velocityAnimationMinimum = .8f;

    private NavMeshAgent agent;
    private float cooldownAttack = 5f;
    private float _timerAttack = 5f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    public override void Attacked(DamageToken token)
    {
        if (token.damageType == DamageToken.DamageType.Explosion)
        {
            token.damage *= 0.2f;
        }
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


        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }

        if (!Stats.IsDead && token.origin == DamageToken.DamageOrigin.Player)
        {
            DamageOutputterUI.instance.DisplayText(token.damage);
        }

        base.Attacked(token);
    }

    public override void Die()
    {
        Destroy(gameObject);
        var corpse1 = Instantiate(corpse);
        corpse1.gameObject.SetActive(true);
        corpse1.transform.position = transform.position;
        corpse1.transform.rotation = transform.rotation;
        OnDied?.Invoke();
        Stats.IsDead = true;

    }

    private void Update()
    {
        if (isAIEnabled == false) return;


        if (Mathf.RoundToInt(Time.time) % 5 == 0)
            ScanForEnemies();

        if (currentTarget != null)
        {
            Attack();
            Movement();
        }
    }

    private void Attack()
    {
        _timerAttack -= Time.deltaTime;

        if (_timerAttack < 0)
        {
            float chance = Random.Range(0f, 1f);
            float hitpointLowChance = 0;

            if (Stats.CurrentHitpoint < (Stats.MaxHitpoint.Value / 2f))
                hitpointLowChance += 0.1f;
            if (Stats.CurrentHitpoint < (Stats.MaxHitpoint.Value / 3f))
                hitpointLowChance += 0.2f;

            if (chance + hitpointLowChance < 0.4f)
            {
                SpawnScarab();
            }

            _timerAttack = cooldownAttack;
        }
    }

    private void Movement()
    {
        agent.SetDestination(currentTarget.transform.position);

        if (agent.velocity.magnitude > velocityAnimationMinimum)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);

        }
    }

    [FoldoutGroup("Debug")]
    [Button("Spawn Scarab")]
    public void SpawnScarab()
    {
        var scarab1 = Instantiate(scarabPrefab, spawnScarab.position, spawnScarab.rotation);
        audio_Fire.Play();
        scarab1.SetActive(true);
        var scarabScript = scarab1.GetComponent<SeaverScarab>();
        scarabScript.OverrideTarget(currentTarget, Stats.MainAlliance);
    }

}
