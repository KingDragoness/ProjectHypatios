using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MechizMonsterRobot : Enemy
{

    public enum AttackPattern
    {
        Idle,
        Flying, //or missile mode also ok
        GroundSaw
    }
    

    [ProgressBar(0, "maxHitpoint")]
    public float hitpoint = 21000;
    public float maxHitpoint = 21000;

    [FoldoutGroup("AI")] public AttackPattern attackPattern;

    //Reuse asset
    [FoldoutGroup("Prefabs")] public MissileChameleon missilePrefab;
    [FoldoutGroup("Prefabs")] public GameObject nonDamageExplosion;
    [FoldoutGroup("Audios")] public AudioSource audio_RocketLaunch;
    [FoldoutGroup("Audios")] public AudioSource audio_ChangeMode;
    [FoldoutGroup("Audios")] public AudioSource audio_Sawmill;

    [FoldoutGroup("Param")] public float saw_movingSpeed = 3f;
    [FoldoutGroup("Param")] public float saw_rotateSpeed = 10f;
    [FoldoutGroup("Param")] public float flyingSpeed = 10f;
    [FoldoutGroup("Param")] public float flying_rotateSpeed = 10f;

    [FoldoutGroup("References")] public Transform elenaTransform;
    [FoldoutGroup("References")] public GameObject flyingModeEffect;
    [FoldoutGroup("References")] public GameObject sawModeEffect;
    [FoldoutGroup("References")] public MechizPatternRobot patternRobot;
    [FoldoutGroup("References")] public Animator animator_TurretLeft;
    [FoldoutGroup("References")] public Animator animator_TurretRight;
    [FoldoutGroup("References")] public Transform pivot_Body;
    [FoldoutGroup("References")] public Transform pivot_Head;
    [FoldoutGroup("References")] public Transform outWeaponTurretLeft;
    [FoldoutGroup("References")] public Transform outWeaponTurretRight;

    private Rigidbody rb;
    private Vector3 prevPos;
    private float velocity;

    [FoldoutGroup("Debug")]
    [Button("Fire missile")]
    public void FireMissile()
    {
        audio_RocketLaunch.Play();
        GameObject prefabMissile = Instantiate(missilePrefab.gameObject, outWeaponTurretLeft.position, Quaternion.identity);
        GameObject prefabMissile1 = Instantiate(missilePrefab.gameObject, outWeaponTurretRight.position, Quaternion.identity);

        {
            GameObject explosive1 = Instantiate(nonDamageExplosion.gameObject, outWeaponTurretLeft.position, Quaternion.identity);
            GameObject explosive2 = Instantiate(nonDamageExplosion.gameObject, outWeaponTurretRight.position, Quaternion.identity);
            explosive1.gameObject.SetActive(true);
            explosive2.gameObject.SetActive(true);
        }

        prefabMissile.gameObject.SetActive(true);
        prefabMissile1.gameObject.SetActive(true);
    }

    public override void Attacked(float damage, float repulsionForce = 1)
    {
        hitpoint -= damage;
        DamageOutputterUI.instance.DisplayText(damage);

        base.Attacked(damage, repulsionForce);
    }

    private void Update()
    {
        UpdateAIState();
        UpdateAudio();
    }


    private void UpdateAudio()
    {
        velocity = ((transform.position - prevPos).magnitude) / Time.deltaTime;
        prevPos = transform.position;

        audio_Sawmill.pitch = Mathf.Clamp(velocity * 0.1f, 0.9f, 1.7f);


    }

    public Transform target;
    private Vector3 targetMovingPos = new Vector3();

    #region AI States
    private void UpdateAIState()
    {
        AIDecision();

        if (attackPattern == AttackPattern.GroundSaw)
        {
            SawMode();
            flyingModeEffect.gameObject.SetActive(false);
            sawModeEffect.gameObject.SetActive(true);
        }
        else if (attackPattern == AttackPattern.Flying)
        {
            FlyingMode();
            flyingModeEffect.gameObject.SetActive(true);
            sawModeEffect.gameObject.SetActive(false);
        }
    }

    private float timer_AIDecisionMaking = 2f;

    private void AIDecision()
    {

        timer_AIDecisionMaking -= Time.deltaTime;

        if (timer_AIDecisionMaking < 0)
        {
            AttackPattern prevAttackPattern = attackPattern;

            float chanceAI = Random.Range(0f, 1f);

            if (chanceAI > 0.4f)
            {
                attackPattern = AttackPattern.Flying;
            }
            else //if (chanceAI > 0.2f)
            {
                attackPattern = AttackPattern.GroundSaw;
            }

            if (prevAttackPattern != attackPattern)
            {
                OnChangedAttackPattern();
            }

            timer_AIDecisionMaking = 2f;
        }
    }

    private void OnChangedAttackPattern()
    {
        timer_MissileFire = 3f;
        audio_ChangeMode.Play();
        animator_TurretLeft.ResetTrigger("Up");
        animator_TurretRight.ResetTrigger("Normal");
        animator_TurretLeft.ResetTrigger("Up");
        animator_TurretRight.ResetTrigger("Normal");

        if (attackPattern == AttackPattern.Flying)
        {
            animator_TurretLeft.SetTrigger("Up");
            animator_TurretRight.SetTrigger("Up");
        }
        else if (attackPattern == AttackPattern.GroundSaw)
        {
            animator_TurretLeft.SetTrigger("Normal");
            animator_TurretRight.SetTrigger("Normal");
        }
    }

    private float timer_ChangeGroundSawPos = 2f;

    private void SawMode()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        Vector3 targetPos = target.position;
        float speed = 1 * saw_movingSpeed;

        targetPos.y -= Mathf.Clamp(distance * 0.5f, 0, 3f);

        if (target == elenaTransform)
        {
            targetPos.y = elenaTransform.position.y + 6;
            speed *= 0.35f;

            if (target.transform.position.y < 2.3f)
            {
                targetPos.y = 9f;
            }

            if (distance < 5f)
            {
                targetPos.x = elenaTransform.position.x + 3;
                targetPos.y = elenaTransform.position.y + 6;
            }
        }


        var q = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, saw_rotateSpeed * Time.deltaTime);

        Vector3 relativePos = transform.InverseTransformPoint(targetPos);


        //is ahead
        if (relativePos.z > 1)
        {

        }
        else
        {
            speed *= 0.1f;
        }

        float step = speed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        {
            //Change saw pos
            timer_ChangeGroundSawPos -= Time.deltaTime;

            if (timer_ChangeGroundSawPos < 0)
            {
                float chance1 = Random.Range(0f, 1f);

                if (distance < 2f) chance1 -= 0.5f;

                if (chance1 < 0.4f)
                {
                    RandomSpotSawMode();
                }

                timer_ChangeGroundSawPos = 2f;
            }
        }
    }

    private float timer_ChangeGroundFlyingPos = 2f;
    private float timer_MissileFire = 3.5f;


    private void FlyingMode()
    {
        float percentageHP = hitpoint / maxHitpoint;
        float distance = Vector3.Distance(transform.position, target.position);
        Vector3 targetPos = target.position;
        float speed = 1 * flyingSpeed;

        var q = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, flying_rotateSpeed * Time.deltaTime);


        float step = speed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        {
            //Change saw pos
            timer_ChangeGroundFlyingPos -= Time.deltaTime;
            timer_MissileFire -= Time.deltaTime;

            if (timer_ChangeGroundFlyingPos < 0)
            {
                float chance1 = Random.Range(0f, 1f);

                if (distance < 2f) chance1 -= 0.5f;

                if (chance1 < 0.4f)
                {
                    RandomizeSpot(AttackPattern.Flying);
                }

                timer_ChangeGroundFlyingPos = 2f;
            }

            if (timer_MissileFire < 0)
            {
                float chance1 = Random.Range(0f, 1f);

                if (chance1 < 0.4f && percentageHP < 0.75f)
                {
                    StartCoroutine(LaunchMissiles());
                }

                timer_MissileFire = 3.5f;
            }
        }
    }


    #endregion

    #region Attack Moves

    private IEnumerator LaunchMissiles()
    {
        FireMissile();
        yield return new WaitForSeconds(1f);
        FireMissile();
        yield return new WaitForSeconds(1f);
        FireMissile();
    }

    #endregion

    [FoldoutGroup("Debug")]
    [Button("Random Spot")]
    private void RandomSpotSawMode()
    {
        RandomizeSpot(AttackPattern.GroundSaw);
    }

    private void RandomizeSpot(AttackPattern attackPattern)
    {
        if (attackPattern == AttackPattern.GroundSaw)
        {
            target = patternRobot.GetRandomSawPosition();
        }
        else if (attackPattern == AttackPattern.Flying)
        {
            target = patternRobot.GetRandomFlyingPos();
        }
    }
}
