using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using System.Linq;

public class SeaverEnemy : EnemyScript
{

    [ProgressBar(0, "maxHitpoint")]
    public float hitpoint = 900;
    public float maxHitpoint = 900;

    public Transform spawnScarab;
    public Animator anim;
    public GameObject scarabPrefab;
    [FoldoutGroup("Prefabs")] public GameObject corpse;
    [FoldoutGroup("Audios")] public AudioSource audio_Fire;
    public float velocityAnimationMinimum = .8f;

    private NavMeshAgent agent;
    private float cooldownAttack = 5f;
    private float _timerAttack = 5f;
    private Transform player;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null) player = FindObjectOfType<CharacterScript>().transform;
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
        else if (token.origin == DamageToken.DamageOrigin.Player)
        {
            DamageOutputterUI.instance.DisplayText(token.damage);
        }

        base.Attacked(token);
    }

    private void Die()
    {
        Destroy(gameObject);
        var corpse1 = Instantiate(corpse);
        corpse1.gameObject.SetActive(true);
        corpse1.transform.position = transform.position;
        corpse1.transform.rotation = transform.rotation;
    }

    private void Update()
    {
        Attack();
        Movement();
    }

    private void Attack()
    {
        _timerAttack -= Time.deltaTime;

        if (_timerAttack < 0)
        {
            float chance = Random.Range(0f, 1f);
            float hitpointLowChance = 0;

            if (hitpoint < (maxHitpoint/2f))
                hitpointLowChance += 0.1f;
            if (hitpoint < (maxHitpoint / 4f))
                hitpointLowChance += 0.1f;

            if (chance < (0.4f + hitpointLowChance))
            {
                SpawnScarab();
            }

            _timerAttack = cooldownAttack;
        }
    }

    private void Movement()
    {
        agent.SetDestination(player.transform.position);

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
    }

}
