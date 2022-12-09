using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class HyperchadEnemy : EnemyScript
{
    
    public enum MoveStances
    {
        Unactivated,
        Prepare,
        Normal,
        Attack,
        VoltaAttack,
        PrepareFinal,
        FinalForm,
        PrepareDie
    }


    //STANCES
    //Normal: Throws projectile and fly around X
    //[50% HP less] Volta Attack: Charging up for 10 seconds while looking at the player (1 sec before the attack, locks rotation) then fire the charge then switch back to normal (extra 10s cooldown decision) X
    //[50% HP less] Platform Rise: rise the platform when done (extra 15s decision)
    //[50% HP less] Summon Zombot: summons zombots, dont pick while platform rise active (extra 20s decision)
    //[30% HP less] Prepare Final: Don't do anything but wait for final form finish
    //[30% HP less] Final Form: 

    public MoveStances currentStance = MoveStances.Unactivated;
    public Transform playerTarget;
    public Animator bossAnimator;
    public GameObject molotovBomb;
    public ThrowProjectileEnemy throwProjectileEnemy;

    [Space]
    [Header("Properties")]
    public bool allowAIDecision = true;

    [Space]
    [Header("Objects")]
    public List<GameObject> allTrails = new List<GameObject>();

    [Space]
    [Header("Normal Stance")]
    [FoldoutGroup("Normal Stance")] public float normal_SpeedRotate = 10;
    [FoldoutGroup("Normal Stance")] public Vector2 normal_RandomRangePatrolPlayer = new Vector2(20, 50);
    [FoldoutGroup("Normal Stance")] public Vector2 normal_RandomOffsetHeight = new Vector2(20, 50);
    [FoldoutGroup("Normal Stance")] public float normal_FlyingForce = 10;
    [FoldoutGroup("Normal Stance")] public float normal_CooldownReloadPos = 2f;
    [FoldoutGroup("Normal Stance")] public float distanceMinimumDropBomb = 20;
    [FoldoutGroup("Normal Stance")] public float cooldownDroppingBomb = 1f;
    [FoldoutGroup("Normal Stance")] public float randomRangeDroppingBomb = 5;
    [FoldoutGroup("Normal Stance")] public float throwBombForce = 10;
    [FoldoutGroup("Normal Stance")] public float delayPerBomb = 0.2f;

    [Space]
    [Header("Attack Stance")]
    [FoldoutGroup("Attack Stance")] public float attack_SpeedRotate = 10;
    [FoldoutGroup("Attack Stance")] public float attack_YSpeedRotate = 10;
    [FoldoutGroup("Attack Stance")] public float attack_FlyingForce = 10;
    [FoldoutGroup("Attack Stance")] public Vector2 randomPlasmaCount = new Vector2(6, 10);
    [FoldoutGroup("Attack Stance")] public float ascentDescentSpeed = 10;
    [FoldoutGroup("Attack Stance")] public float cooldownPlasmaFire = 1f;
    [FoldoutGroup("Attack Stance")] public float delayPerPlasma = 0.2f;
    [FoldoutGroup("Attack Stance")] public float distanceMinimumFire = 30;
    [FoldoutGroup("Attack Stance")] public float damage;
    [FoldoutGroup("Attack Stance")] public float variableDamage = 1;
    [FoldoutGroup("Attack Stance")] public List<Transform> out_Projectile = new List<Transform>();

    [Space]
    [FoldoutGroup("Volta Stance")] public float volta_SpeedRotate = 10;
    [FoldoutGroup("Volta Stance")] public GameObject voltaChargeVFX;
    [FoldoutGroup("Volta Stance")] public GameObject voltaUnleashVFX;
    [FoldoutGroup("Volta Stance")] public Transform voltaOriginLookAt;
    [FoldoutGroup("Volta Stance")] public Vector3 voltaOffsetLookAt = new Vector3();
    [FoldoutGroup("Volta Stance")] public float cooldownVoltaAttack = 1f;
    [FoldoutGroup("Volta Stance")] public float cooldownVoltaEndAttack = 2f;
    [FoldoutGroup("Volta Stance")] public float distanceMinimumVoltaAttack = 40;

    [Space]
    [FoldoutGroup("Final Form")] public float finalFormTimer = 10;
    [FoldoutGroup("Final Form")] public Transform transform_GoToMecha;
    [FoldoutGroup("Final Form")] public Transform restingPlaceTarget;


    [Space]
    [FoldoutGroup("Audios")] public AudioSource audio_FireLaser;
    [FoldoutGroup("Audios")] public AudioSource audio_VoltaChargingUp;
    [FoldoutGroup("Audios")] public AudioSource audio_LaserVolta;
    [FoldoutGroup("Audios")] public AudioSource audio_PlanePassing;
    [FoldoutGroup("Audios")] public float thresholdVelocityPlanePass;

    
    [Space]

    private float cooldownDecision = 7f;
    private float currentCooldownDecision = 10f;
    private bool hasTriggeredFinalForm = false;
    private bool isOnFinalForm = false;

    private Rigidbody rb;

    void Start()
    {
        if (playerTarget == null)
        {
            playerTarget = FindObjectOfType<CharacterScript>().transform;
        }
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        if (Stats.CurrentHitpoint <= 0)
        {
            Die();
            return;
        }

        if (isAIEnabled == false) return;

        if (allowAIDecision && currentStance != MoveStances.Unactivated && !hasTriggeredFinalForm)
        {
            AI_Decision();
        }
        else if (allowAIDecision && currentStance != MoveStances.Unactivated && isOnFinalForm)
        {
            AI_Decision();
        }

    }

    [FoldoutGroup("Debug")]
    [Button("Enemy set mode to attack")]
    public void SetEnemyAttack()
    {
        currentStance = MoveStances.Attack;
        currentCooldownDecision = -100f;
    }

    public override void Die()
    {
        if (Stats.IsDead == false) BossDied();
        Stats.IsDead = true;
        Stats.CurrentHitpoint = 0;
        currentStance = MoveStances.PrepareDie;
    }

    private void FixedUpdate()
    {
        if (isAIEnabled == false) return;

        if (currentStance == MoveStances.Normal)
        {
            Normal();
        }
        else if (currentStance == MoveStances.Attack)
        {
            Attack();
        }
        else if (currentStance == MoveStances.VoltaAttack)
        {
            Volta();
        }
        else if (currentStance == MoveStances.PrepareFinal)
        {
            PrepareFinalForm();
        }
        else if (currentStance == MoveStances.FinalForm)
        {
            FinalForm();
        }
        else if (currentStance == MoveStances.PrepareDie)
        {
            PrepareDie();
        }
    }


    public override void Attacked(DamageToken token)
    {
        if (currentStance == MoveStances.Unactivated)
        {
            //ActivateEnemy();
        }

        if (token.originEnemy == this)
        {
            return;
        }

        float damageProcessed = token.damage;

        if (isOnFinalForm)
        {
            float dist = Vector3.Distance(playerTarget.position, this.transform.position);

            if (currentStance == MoveStances.Normal && dist < 16)
            {
                currentStance = MoveStances.Attack;
            }

            damageProcessed *= 0.6f;
        }

        Stats.CurrentHitpoint -= damageProcessed;

        if (Stats.CurrentHitpoint > 0f)
            DamageOutputterUI.instance.DisplayText(damageProcessed);

        base.Attacked(token);
    }

    private void AI_Decision()
    {
        currentCooldownDecision += Time.deltaTime;

        if (currentCooldownDecision > cooldownDecision)
        {
            int chance = Random.Range(1, 5);
            //random select
            currentCooldownDecision = 0;

            bool isHealthHalf = (Stats.CurrentHitpoint / Stats.MaxHitpoint.Value) < 0.5f ? true : false;
            bool isHealthQuarter = (Stats.CurrentHitpoint / Stats.MaxHitpoint.Value) < 0.26f ? true : false;

            if (chance == 1)
            {
                currentStance = MoveStances.Normal;
            }
            else if (chance == 2)
            {
                float chance1 = Random.Range(0f, 1f);

                if (!isHealthHalf)
                {
                    chance1 = 1;
                }

                if (chance1 > 0.5f)
                {
                    currentStance = MoveStances.Attack;
                }
                else
                {
                    currentStance = MoveStances.VoltaAttack;
                    currentCooldownDecision -= 3;
                }
            }
            else if (chance == 3 && isHealthHalf)
            {
                currentStance = MoveStances.VoltaAttack;
                currentCooldownDecision -= 3;
            }
            else if (chance == 4 && isOnFinalForm)
            {
                currentStance = MoveStances.FinalForm;
                currentCooldownDecision -= 5;
            }


        }
    }

    #region Modes

    private float f_currCooldownReloadPos = 1;
    private float f_cooldownReloadPos = 1;
    private float f_cooldownDroppingBomb = 1;
    private float f_currCooldownPlasma = 1;
    private float f_currCooldownVolta = 0;
    private Vector3 tempPositionCirclePlayer;

    public void Normal()
    {
        Visual_Flying();

        //fly around in the air
        LookAt(tempPositionCirclePlayer, normal_SpeedRotate);
        RandomPosCheck();

        float dist = Vector3.Distance(transform.position, tempPositionCirclePlayer);
        dist = Mathf.Clamp(dist / 10, 1, 10);
        rb.AddForce(transform.forward * normal_FlyingForce * dist * rb.mass * Time.deltaTime);

        {
            f_cooldownDroppingBomb -= Time.deltaTime;
            bool allowDrop = false;

            float distance_X = Mathf.Abs(transform.position.x - playerTarget.transform.position.x);
            float distance_Z = Mathf.Abs(transform.position.z - playerTarget.transform.position.z);
            float dist1 = (distance_X + distance_Z) / 2;

            if (dist1 < distanceMinimumDropBomb)
            {
                allowDrop = true;
            }

            if (f_cooldownDroppingBomb <= 0 && allowDrop)
            {
                StartCoroutine(DropBomb(delayPerBomb));
                f_cooldownDroppingBomb = cooldownDroppingBomb;
            }
        }
    }

    private IEnumerator DropBomb(float delayPerBomb = 0.1f)
    {
        int randomCount = Random.Range(1, 5);
        f_cooldownDroppingBomb += 15f;

        for (int x = 0; x < randomCount; x++)
        {
            Vector3 X_spaceTest = transform.right * Random.Range(-randomRangeDroppingBomb, randomRangeDroppingBomb);
            Vector3 Y_spaceTest = transform.up * Random.Range(0, randomRangeDroppingBomb/2.5f);

            Vector3 posSpawn = (transform.forward * Random.Range(1, 4f)) + Y_spaceTest;
            posSpawn += transform.position;

            var bomb1 = Instantiate(molotovBomb, posSpawn, Quaternion.identity);
            bomb1.gameObject.SetActive(true);
            var rb1 = bomb1.GetComponent<Rigidbody>();
            rb1.AddForce((rb.velocity / 10) * rb1.mass * throwBombForce);

            yield return new WaitForSeconds(delayPerBomb);
        }

    }

    private void Attack()
    {
        //fly around look at player
        LookAt(playerTarget.position, attack_SpeedRotate);
        RandomPosCheck(0.1f, 0.3f);

        float dist = Vector3.Distance(transform.position, playerTarget.position);
        dist = Mathf.Clamp(dist / 10, 1, 5);
        rb.AddForce(transform.forward * attack_FlyingForce * dist * rb.mass * Time.deltaTime);

        //fly down and up
        {
            float distY = (playerTarget.position.y - transform.position.y + 3);
            rb.AddForce(Vector3.up * distY * rb.mass * ascentDescentSpeed * Time.deltaTime);
        }

        if (dist < 4)
        {
            Visual_Attack();
        }
        else
        {
            Visual_Flying();
        }

        ////make stand upright
        //{
        //    Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
        //    transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * attack_YSpeedRotate);
        //}


        {
            f_currCooldownPlasma -= Time.deltaTime;
            bool allowFire = false;

            float distance_X = Mathf.Abs(transform.position.x - playerTarget.transform.position.x);
            float distance_Z = Mathf.Abs(transform.position.z - playerTarget.transform.position.z);
            float dist1 = (distance_X + distance_Z) / 2;

            if (dist1 < distanceMinimumFire)
            {
                allowFire = true;
            }

            if (f_currCooldownPlasma <= 0 && allowFire)
            {
                StartCoroutine(PulsePlasmaFire(delayPerPlasma));
                f_currCooldownPlasma = cooldownPlasmaFire;
            }
        }
    }

    private IEnumerator PulsePlasmaFire(float delayPerPlasma = 0.1f)
    {
        int randomCount = Mathf.RoundToInt(Random.Range(randomPlasmaCount.x, randomPlasmaCount.y));
        f_currCooldownPlasma += 5f;

        yield return new WaitForSeconds(.5f);


        for (int x = 0; x < randomCount; x++)
        {
            Vector3 posSpawn = Vector3.zero;

            if (x % out_Projectile.Count == 0)
            {
                posSpawn = out_Projectile[0].position;
            }
            else
            {
                posSpawn = out_Projectile[1].position;
            }

            Vector3 randomPos = posSpawn;
            randomPos = OffsetRandomizeVector3(randomPos, 1f);
            randomPos.y = transform.position.y;
            Vector3 dir = (playerTarget.position - randomPos).normalized;

            var projectile1 = throwProjectileEnemy.FireProjectile(dir);
            projectile1.Damage = damage + Random.Range(0, variableDamage);
            projectile1.transform.position = posSpawn;
            projectile1.gameObject.SetActive(true);
            Visual_Attack_Fire();
            audio_FireLaser.PlayOneShot(audio_FireLaser.clip);

            yield return new WaitForSeconds(delayPerPlasma);
        }

    }

    private bool isChargingVolta = true;

    private void Volta()
    {
        //fly around look at player
        LookAt(playerTarget.position + voltaOffsetLookAt, voltaOriginLookAt.position, volta_SpeedRotate);
        RandomPosCheck(0.1f, 0.3f);

        float dist = Vector3.Distance(transform.position, playerTarget.position);
        dist = Mathf.Clamp(dist / 10, 1, 5);
        rb.AddForce(transform.forward * attack_FlyingForce * dist * rb.mass * Time.deltaTime);

        //fly down and up
        {
            float distY = (playerTarget.position.y - transform.position.y + 3);
            rb.AddForce(Vector3.up * distY * rb.mass * ascentDescentSpeed * Time.deltaTime);
        }

        {
            f_currCooldownVolta -= Time.deltaTime;
            bool allowFire = false;

            float distance_X = Mathf.Abs(transform.position.x - playerTarget.transform.position.x);
            float distance_Z = Mathf.Abs(transform.position.z - playerTarget.transform.position.z);
            float dist1 = (distance_X + distance_Z) / 2;

            if (dist1 < distanceMinimumVoltaAttack)
            {
                allowFire = true;
            }

            //Acquire target when close
            if (allowFire && !isChargingVolta)
            {
                VoltaFire();
            }
        }

        //Build up charge
        if (isChargingVolta)
        {
            Visual_Volta();
            VoltaFire();
        }
        else
        {

            if (dist < 4)
            {
                Visual_Attack();
            }
            else
            {
                Visual_Flying();
            }


            //stand up right
            Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * attack_YSpeedRotate);
        }
    }

    public bool DEBUG_Unleash_Volta = false;
    private float f_voltaChargeTimer = 0;

    public void VoltaFire()
    {
        isChargingVolta = true;

        //charge up
        {
            f_voltaChargeTimer += Time.deltaTime;

            if (voltaChargeVFX.activeSelf == false)
            {
                voltaChargeVFX.SetActive(true);
            }


            if (audio_VoltaChargingUp.isPlaying == false)
            {
                audio_VoltaChargingUp.Play();
            }
        }

        //unleash
        if (DEBUG_Unleash_Volta | f_voltaChargeTimer >= cooldownVoltaAttack)
        {
            DEBUG_Unleash_Volta = false;
            voltaChargeVFX.SetActive(true);
            voltaUnleashVFX.SetActive(true);
            currentCooldownDecision = 0f;



            if (audio_LaserVolta.isPlaying == false)
            {
                audio_LaserVolta.Play();
            }

            if (f_voltaChargeTimer > cooldownVoltaEndAttack)
            {
                currentStance = MoveStances.Normal;
                voltaChargeVFX.SetActive(false);
                voltaUnleashVFX.SetActive(false);
                f_voltaChargeTimer = 0;
                isChargingVolta = false;
                audio_LaserVolta.Pause();
                audio_VoltaChargingUp.Pause();
                //reset
            }

            // Don't reset only after firing stop
            //f_currCooldownVolta = cooldownVoltaAttack;
            //f_voltaChargeTimer = 0;
        }
    }

    #endregion



    #region Final FOrm!

    private float f_timerPrepareFinal = 0;

    public void PrepareFinalForm()
    {
        f_timerPrepareFinal += Time.deltaTime;
        hasTriggeredFinalForm = true;

        if (f_timerPrepareFinal > finalFormTimer)
        {
            currentStance = MoveStances.FinalForm;
        }

        Visual_Flying();

        //fly around in the air
        LookAt(tempPositionCirclePlayer, normal_SpeedRotate);
        tempPositionCirclePlayer = transform_GoToMecha.position;

        float dist = Vector3.Distance(transform.position, tempPositionCirclePlayer);
        dist = Mathf.Clamp(dist / 10, 1, 10);
        rb.AddForce(transform.forward * normal_FlyingForce * dist * rb.mass * Time.deltaTime);


    }

    public void FinalForm()
    {
        if (isOnFinalForm == false) currentStance = MoveStances.Normal;
        isOnFinalForm = true;

        Visual_Flying();

        //fly around in the air
        LookAt(Vector3.zero, normal_SpeedRotate);
        RandomPosCheck();

        float dist = Vector3.Distance(transform.position, tempPositionCirclePlayer);
        dist = Mathf.Clamp(dist / 10, 1, 10);
        rb.AddForce(transform.forward * normal_FlyingForce * dist * rb.mass * Time.deltaTime);
    }

    public void BossDied()
    {

        Visual_Flying();

        //fly around in the air
        LookAt(restingPlaceTarget.transform.position, normal_SpeedRotate);
        RandomPosCheck();

        float dist = Vector3.Distance(transform.position, restingPlaceTarget.transform.position);
        dist = Mathf.Clamp(dist / 10, 1, 10);
        rb.AddForce(transform.forward * normal_FlyingForce * dist * rb.mass * Time.deltaTime);
    }

    #endregion


    private void PrepareDie()
    {

    }

    //debug
    public void Cheat_KillBoss()
    {
        Stats.CurrentHitpoint = 0f;
    }

    #region Effects and Visuals

    private void Visual_Flying()
    {
        bossAnimator.SetBool("Attacking", false);
        bossAnimator.SetBool("VoltaCharge", false);

        foreach (var trail in allTrails)
        {
            trail.gameObject.SetActive(true);
        }

        if (rb.velocity.magnitude > thresholdVelocityPlanePass)
        {
            int time = Mathf.RoundToInt(Time.time * 10);
            float chance = 0;

            if (time % 10 == 0)
            {
                chance = Random.Range(0f, 1f);
            }

            if (audio_PlanePassing.isPlaying == false && chance >= 0.9f)
            {
                audio_PlanePassing.Play();
            }
        }
    }

    private void Visual_Attack()
    {
        bossAnimator.SetBool("Attacking", true);
        bossAnimator.SetBool("VoltaCharge", false);

        foreach (var trail in allTrails)
        {
            trail.gameObject.SetActive(false);
        }
    }

    private void Visual_Volta()
    {
        bossAnimator.SetBool("Attacking", false);
        bossAnimator.SetBool("VoltaCharge", true);
    }

    private void Visual_Attack_Fire()
    {
        bossAnimator.SetTrigger("Fire");

    }

    #endregion

    #region Modular

    private Vector3 OffsetRandomizeVector3(Vector3 origin, float range)
    {
        float targetRange_X = Random.Range(-range, range);
        float targetRange_Y = Random.Range(-range, range);
        float targetRange_Z = Random.Range(-range, range);

        return new Vector3(origin.x + targetRange_X, origin.y + targetRange_Y, origin.z + targetRange_Z);
    }

    private void LookAt(Vector3 target, float speedRotation = 30)
    {
        var q = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, speedRotation * Time.deltaTime);
    }

    private void LookAt(Vector3 target, Vector3 origin, float speedRotation = 30)
    {
        var q = Quaternion.LookRotation(target - origin);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, speedRotation * Time.deltaTime);
    }


    private void RandomPosCheck(float rangeModifier = 1, float offsetModifier = 1)
    {

        f_currCooldownReloadPos -= Time.deltaTime;

        if (f_currCooldownReloadPos <= 0)
        {
            float targetRange_X = Random.Range(normal_RandomRangePatrolPlayer.x * rangeModifier, normal_RandomRangePatrolPlayer.y * rangeModifier);
            float targetRange_Z = Random.Range(normal_RandomRangePatrolPlayer.x * rangeModifier, normal_RandomRangePatrolPlayer.y * rangeModifier);

            tempPositionCirclePlayer = playerTarget.position;
            tempPositionCirclePlayer.y += 1 + Random.Range(normal_RandomOffsetHeight.x * offsetModifier, normal_RandomOffsetHeight.y * offsetModifier);
            tempPositionCirclePlayer.x += (targetRange_X % 2 == 0) ? targetRange_X : -targetRange_X;
            tempPositionCirclePlayer.z += (targetRange_Z % 2 == 0) ? targetRange_Z : -targetRange_Z;

            f_currCooldownReloadPos = f_cooldownReloadPos + Random.Range(0, normal_CooldownReloadPos);
        }
    }

    #endregion

    #region Switch Stances

    [ContextMenu("ActivateEnemy")]
    public void ActivateEnemy()
    {
        currentStance = MoveStances.Normal;
    }

    #endregion
}
