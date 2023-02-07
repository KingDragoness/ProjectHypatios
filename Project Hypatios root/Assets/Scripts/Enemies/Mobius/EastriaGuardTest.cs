using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EastriaGuardTest : EnemyScript
{
    public enum AttackPattern
    {
        Idle,
        Attack
    }



    public AttackPattern mode = AttackPattern.Idle;

    [FoldoutGroup("Prefabs")] public GameObject corpsePrefab;
    [FoldoutGroup("Prefabs")] public GameObject projectilePrefab;
    [FoldoutGroup("References")] public Transform[] rigOrigin;
    [FoldoutGroup("References")] public Transform fireOrigin;
    [FoldoutGroup("References")] public FollowObject followObject;
    [FoldoutGroup("AI")] public float attackRange = 20f;
    [FoldoutGroup("AI")] public float followPlayerRange = 40f;
    [FoldoutGroup("AI")] public float attackCooldown = 2f;
    [FoldoutGroup("AI")] public int totalShockPerAttack = 5;
    [FoldoutGroup("AI")] public float shockRateFire = 0.1f;
    [FoldoutGroup("Thruster")] public float VerticalThrustForce = -900f;
    [FoldoutGroup("Thruster")] public float ForwardThrustForce = 1000f;
    [FoldoutGroup("Sounds")] public AudioSource Audio_FireShockwave;
    [FoldoutGroup("Sounds")] public AudioSource Audio_Voice_ContactInbound;

    public float speed;

    private Rigidbody m_Rigidbody;

    private void Start()
    {
        OnPlayerDetected += SeenPlayer;
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public override void Attacked(DamageToken token)
    {
        _lastDamageToken = token;

        SeenPlayer();
        Stats.CurrentHitpoint -= token.damage;
        DamageOutputterUI.instance.DisplayText(token.damage);

        base.Attacked(token);
    }

    public override void OnCreated()
    {
        base.OnCreated();
    }

    private void Update()
    {

        if (Mathf.RoundToInt(Time.time) % 5 == 0)
            ScanForEnemies();

        if (currentTarget != null && isAIEnabled)
        {
            followObject.target = currentTarget.transform;
            // direction towards target
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (distance < followPlayerRange)
            {
                Vector3 relativePos = currentTarget.transform.position - (transform.position + new Vector3(0, 1.1f, 0));
                Vector3 dirToPlayer = relativePos;
                Quaternion toRotation = Quaternion.LookRotation(relativePos);
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, speed * Time.deltaTime);

                m_Rigidbody.AddForce(dirToPlayer * ForwardThrustForce * Time.deltaTime);
                m_Rigidbody.AddForce(Vector3.up * VerticalThrustForce * Time.deltaTime);
            }

            AI_Detection();
            if (hasSeenPlayer) RunAI();
        }

        if (Stats.CurrentHitpoint < 0)
        {
            Die();
        }
    }



    private void SeenPlayer()
    {
        if (hasSeenPlayer == false)
        {
            hasSeenPlayer = true;

            if (Audio_Voice_ContactInbound != null) Audio_Voice_ContactInbound.Play();
        }
        else
        {

        }
    }

    private float _cooldownAttack = 2f;

    private void RunAI()
    {
        mode = AttackPattern.Attack;


        if (mode == AttackPattern.Attack)
        {
            _cooldownAttack -= Time.deltaTime;

            if (_cooldownAttack < 0)
            {
                if (Audio_FireShockwave != null)
                {
                    Audio_FireShockwave.Play();
                    //Audio_FireShockwave.pitch = Random.Range(0.9f, 1.1f);
                }
                fireOrigin.LookAt(currentTarget.transform.transform);
                StartCoroutine(FireWeapon());
                _cooldownAttack = attackCooldown + Random.Range(0f, 0.1f);
            }
        }
    }

    private IEnumerator FireWeapon()
    {
        for(int x = 0; x < totalShockPerAttack; x++)
        {
            SpawnProjectile();
            yield return new WaitForSeconds(shockRateFire);
        }
    }

    private void SpawnProjectile()
    {
        var shock1 = Instantiate(projectilePrefab);
        shock1.transform.position = fireOrigin.position;
        shock1.transform.rotation = fireOrigin.rotation;
        shock1.gameObject.SetActive(true);
        Destroy(shock1.gameObject, 3f);
    }

    public override void Die()
    {
        var corpse1 = Instantiate(corpsePrefab);
        corpse1.transform.position = transform.position;
        corpse1.transform.rotation = transform.rotation;
        corpse1.gameObject.SetActive(true);
        CopyTransformRagdoll ragdollScript = corpse1.GetComponent<CopyTransformRagdoll>();
        ragdollScript.CopyRotationPosition(rigOrigin);
        OnDied?.Invoke();
        Stats.IsDead = true;

        Destroy(this.gameObject);
    }


}
