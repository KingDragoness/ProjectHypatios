using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class MechizDroneMonster : EnemyScript
{

    public enum State
    {
        Idle,
        Flying,
        Attack
    }


    public State aiState;
    public UnityEvent OnKilledEvent;

    [FoldoutGroup("References")] public Transform target;
    [FoldoutGroup("References")] public GameObject face;
    [FoldoutGroup("References")] public GameObject smoke;
    [FoldoutGroup("References")] public GameObject muzzleFire;
    [FoldoutGroup("References")] public Animator animator_weaponTurret;
    [FoldoutGroup("References")] public Animator animator_faceHurt;
    [FoldoutGroup("Audios")] public AudioSource audio_FireWeapon;
    [FoldoutGroup("Audios")] public AudioSource audio_Dead;
    [FoldoutGroup("Param")] public float rotateSpeed = 10f;
    [FoldoutGroup("Param")] public float hoverSpeed = 5f;
    [FoldoutGroup("Param")] public float moveSpeed = 10f;

    [ProgressBar(0, "maxHitpoint")]
    public float hitpoint = 211;
    public float maxHitpoint = 211;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void Attacked(DamageToken token)
    {
        hitpoint -= token.damage;
        DamageOutputterUI.instance.DisplayText(token.damage);
        animator_faceHurt.SetTrigger("Hurt");
        base.Attacked(token);
    }


    private void Update()
    {
        if (hitpoint < 0)
        {
            Die();
            return;
        }

        UpdateAIState();
        ErrorCheckState();
    }

    private float timer_ErrorStateCheck = 2f;

    private void ErrorCheckState()
    {
        timer_ErrorStateCheck -= Time.deltaTime;

        if (timer_ErrorStateCheck < 0)
        {
            Vector3 v = transform.position;

            if (v.y < 0)
            {
                v.y = 10;
                transform.position = v;
            }

            timer_ErrorStateCheck = 2f;
        }
    }

    public override void Die()
    {
        if (!Stats.IsDead)
        {
            Destroy(gameObject, 5f);
            face.gameObject.SetActive(false);
            smoke.gameObject.SetActive(true);
            audio_Dead.Play();
            OnKilledEvent?.Invoke();
            OnDied?.Invoke();
            rb.useGravity = true;
        }

        Stats.IsDead = true;
    }

    #region AI Update
    private void UpdateAIState()
    {
        AIDecision();

        if (aiState == State.Flying)
        {
            Flying();
        }
        else if (aiState == State.Attack)
        {
            Attack();
        }
    }

    private float timer_AIDecisionMaking = 2f;

    private void AIDecision()
    {
        timer_AIDecisionMaking -= Time.deltaTime;

        if (timer_AIDecisionMaking < 0)
        {
            State prevAttackPattern = aiState;

            float chanceAI = Random.Range(0f, 1f);

            if (chanceAI > 0.4f)
            {
                aiState = State.Flying;
            }
            else //if (chanceAI > 0.2f)
            {
                aiState = State.Attack;
            }

            timer_AIDecisionMaking = 2f;
        }
    }


    private void Flying()
    {
        muzzleFire.gameObject.SetActive(false);
        audio_FireWeapon.gameObject.SetActive(false);
        animator_weaponTurret.SetBool("FireMode", false);

        Vector3 targetPos = target.position;

        var q = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotateSpeed * Time.deltaTime);

        rb.AddForce(transform.forward * 100 * moveSpeed);
        rb.AddForce(Vector3.up * 50 * hoverSpeed);

    }

    private void Attack()
    {
        muzzleFire.gameObject.SetActive(true);
        audio_FireWeapon.gameObject.SetActive(true);
        animator_weaponTurret.SetBool("FireMode", true);

        Vector3 targetPos = target.position;

        var q = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotateSpeed * Time.deltaTime);
    }

    #endregion

}
