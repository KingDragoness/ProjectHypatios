using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Kryz.CharacterStats;




public class CharacterScript : Entity
{


    [FoldoutGroup("Stats")] public CharacterStat BonusDamageMelee; //percentage only


    private dashParticleManager dashManager;
    public Rigidbody rb;
    private float playerHeight = 2f;
    private wallRun WallRun;

    [Header("Special FPS Mode")]
    [FoldoutGroup("Modes")] public bool isLimitedIntroMode = false;
    [FoldoutGroup("Modes")] public bool normalMode = true;
    [FoldoutGroup("Modes")] public bool disableInput = false;
    [FoldoutGroup("Modes")] public bool tutorialMode = false;
    [FoldoutGroup("Modes")] public bool isNoGravity = false;

    [Space]
    [Header("Movement States")]
    public float crouchSpeed = 3f;
    public float collider_heightDefault = 0.9f;
    public float collider_heightCrouching = 0.3f;
    public float camera_heightDefault = 0.9f;
    public float camera_heightCrouching = 0.3f;
    public GameObject cameraHolder;

    [Space]
    //Moving
    public CharacterStat speedMultiplier = new CharacterStat(8);
    public float runSpeed = 12f;
    public float moveSpeed;
    [FoldoutGroup("Physics")] public Vector3 dir;
    [FoldoutGroup("Physics")] float xMovement, yMovement;
    [FoldoutGroup("Physics")] public float groundDrag = 6f;

    //Jumping
    [FoldoutGroup("Physics")] public float gravity = -18.3f;
    [FoldoutGroup("Physics")] public Transform groundCheck;
    [FoldoutGroup("Physics")] public float groundDistance = .4f;
    [FoldoutGroup("Physics")] public LayerMask player;
    [FoldoutGroup("Physics")] public float jumpHeight;
    [FoldoutGroup("Physics")] public float jumpDrag = 1f;
    [FoldoutGroup("Physics")] public float jumpSpeedMultiplier = 0.4f;
    [FoldoutGroup("Physics")] public float fallSpeed = 2f;
    [FoldoutGroup("Physics")] bool inAir = false;
    [FoldoutGroup("Physics")] public bool isGrounded;
    [FoldoutGroup("Physics")] public bool isCrouching = false;

    //Slope & Stair Detection
    [FoldoutGroup("Physics")] RaycastHit slopeHit;
    [FoldoutGroup("Physics")] Vector3 slopeDirection;

    //Dashing
    [FoldoutGroup("Dashing")] Vector3 dashDirection;
    [FoldoutGroup("Dashing")] public float dashForce = 5f;
    [FoldoutGroup("Dashing")] public float dashDuration = .5f;
    [FoldoutGroup("Dashing")] public float timeSinceLastDash;
    [FoldoutGroup("Dashing")] public CharacterStat dashCooldown = new CharacterStat(3);

    //Audio
    soundManagerScript soundManager;
    bool runningAudioPlaying = false;

    //Pick Up Items
    [FoldoutGroup("Interactions")] public GameObject[] itemOnField;
    [FoldoutGroup("Interactions")] public float distanceToPickUp = 2f;
    [FoldoutGroup("Interactions")] public GameObject closestItem;
    [FoldoutGroup("Interactions")] public WeaponManager Weapon;

    //Animation
    [ShowInInspector] [ReadOnly] private Animator anim;
    [ShowInInspector] [ReadOnly] private BaseWeaponScript weaponScript;
    public HypatiosSave.PerkDataSave PerkData = new HypatiosSave.PerkDataSave();

    public Animator Anim { get => anim; }
    private float airTime = 0;

    //Scope
    float scopingSpeed = 6f;

    //Health
    public PlayerHealth Health;
    private CapsuleCollider cc;
    

    public bool isCheatMode;

    public void Initialize()
    {
        ReloadStatEffects();

        //load and create perks here
    }

    #region Perks
    [FoldoutGroup("Debug")]
    [Button("Reload All Stat Effects")]
    public void ReloadStatEffects()
    {
        RemoveAllEffectsBySource("PermanentPerk");
        CustomPerkLoad();
        PerkInitialize(StatusEffectCategory.MaxHitpointBonus);
        PerkInitialize(StatusEffectCategory.RegenHPBonus);
        PerkInitialize(StatusEffectCategory.KnockbackResistance);
        PerkInitialize(StatusEffectCategory.BonusDamageMelee);
        dashCooldown.BaseValue = PlayerPerk.GetValue_Dashcooldown(PerkData.Perk_LV_DashCooldown); //dash cooldown is fixed due to changing too much will easily break the level design
    }

    public int GetNetSoulBonusPerk()
    {
        var basePerkClass = PlayerPerk.GetBasePerk(StatusEffectCategory.SoulBonus);
        int netPerk = PerkData.Perk_LV_Soulbonus;

        if (netPerk > basePerkClass.MAX_LEVEL)
            netPerk = basePerkClass.MAX_LEVEL;

        return netPerk;
    }

    private void CustomPerkLoad()
    {
        foreach (var customPerk in PerkData.Temp_CustomPerk)
        {
            var category = customPerk.statusCategoryType;
            var effectObject = GetGenericEffect(category, "TempPerk");

            if (effectObject == null)
                CreatePersistentStatusEffect(category, customPerk.Value, "TempPerk");
            else
            {
                effectObject.Value = customPerk.Value;
                effectObject.ApplyEffect();
            }
        }
    }

    private void PerkInitialize(StatusEffectCategory category)
    {
        float value = 0;

        if (category == StatusEffectCategory.MaxHitpointBonus)
        {
            value = PlayerPerk.GetValue_MaxHPUpgrade(PerkData.Perk_LV_MaxHitpointUpgrade);
        }
        else if (category == StatusEffectCategory.RegenHPBonus)
        {
            value = PlayerPerk.GetValue_RegenHPUpgrade(PerkData.Perk_LV_RegenHitpointUpgrade);
        }
        else if (category == StatusEffectCategory.KnockbackResistance)
        {
            value = PlayerPerk.GetValue_KnockbackResistUpgrade(PerkData.Perk_LV_KnockbackRecoil);
        }
        else if (category == StatusEffectCategory.BonusDamageMelee)
        {
            value = PlayerPerk.GetValue_BonusMeleeDamage(PerkData.Perk_LV_IncreaseMeleeDamage);
        }

        //value += Hypatios.Game.CustomTemporaryPerk (for single run bonus perks)

        var effectObject = GetGenericEffect(category, "PermanentPerk");

        if (effectObject == null)
            CreatePersistentStatusEffect(category, value, "PermanentPerk");
        else
        {
            effectObject.Value = value;
            effectObject.ApplyEffect();
        }
    }
    #endregion

    void Start()
    {   
        moveSpeed = runSpeed;
        WallRun = GetComponent<wallRun>();
        rb = GetComponent<Rigidbody>();
        dashManager = GetComponent<dashParticleManager>();
        rb.freezeRotation = true;
        timeSinceLastDash = dashCooldown.Value;
        soundManager = FindObjectOfType<soundManagerScript>();
        cc = GetComponent<CapsuleCollider>();

        var currentWeapon = Weapon.currentWeaponHeld;

        if (currentWeapon != null)
            anim = currentWeapon.anim;
    }

    private void Update()
    {

        if (Time.timeScale <= 0)
        {
            return;
        }

        if (!Health.isDead)
        {
            if (isNoGravity == true) { rb.useGravity = false; }

            if (Weapon != null)
            {
                anim = Weapon.anim;

                var gun = Weapon.currentGunHeld;

                if (gun != null)
                {
                    if (gun.isScoping && !gun.isReloading && gun.canScope)
                        moveSpeed = scopingSpeed;
                    else
                        moveSpeed = runSpeed;
                }
            }

            Friction();
            HandleCrouchingState();

            if (isLimitedIntroMode == false && !disableInput)
                Jumping();

            timeSinceLastDash += Time.deltaTime;
            float timeAfterDash = Mathf.Clamp(timeSinceLastDash / dashCooldown.Value, 0f, dashCooldown.Value);
            MainGameHUDScript.Instance.dashSlider.value = timeAfterDash;

            slopeDirection = Vector3.ProjectOnPlane(dir, slopeHit.normal);


            if (Anim != null)
            {
                if (!isGrounded && !WallRun.isWallRunning)
                {
                    inAir = true;
                    airTime += Time.deltaTime;
                    Anim.SetBool("inAir", true);
                }
                else if (WallRun.isWallRunning)
                {
                    inAir = false;
                    Anim.SetBool("inAir", false);

                }
                if (inAir && isGrounded)
                {
                    inAir = false;
                    if (airTime > 0.3f) soundManager.Play("falling");
                    Anim.SetBool("inAir", false);

                    {
                        float multiplierAir = Mathf.Clamp(airTime * 0.6f, 0.2f, 3);
                        Weapon.Recoil.CustomRecoil(new Vector3(5, -5f, 5f), multiplierAir);
                    }
                    airTime = 0;

                }
            }
        }
        else
        {
            if (soundManager != null) soundManager.Pause("falling");
            if (soundManager != null) soundManager.Pause("running");
            moveSpeed = 0f;
        }
    }

    bool testDashReady = false;


    void FixedUpdate()
    {
        Moving();

        if (isLimitedIntroMode)
        {
            return;
        }

        if (timeSinceLastDash > dashCooldown.Value)
        {
            MainGameHUDScript.Instance.dashOk.gameObject.SetActive(true);

            if (testDashReady == false)
            {
                MainGameHUDScript.Instance.PlayDash();
                testDashReady = true;
            }

        }
        else
        {
            MainGameHUDScript.Instance.dashOk.gameObject.SetActive(false);
            testDashReady = false;
        }

        if (isCheatMode == false)
        {
            if (Input.GetKey(KeyCode.LeftShift) && timeSinceLastDash > dashCooldown.Value)
            {

                StartCoroutine(Dash());
                timeSinceLastDash = 0;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                StartCoroutine(Dash());
                timeSinceLastDash = 0;
            }
        }
    }

    public void HandleCrouchingState()
    {
        if (isLimitedIntroMode)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
        }

        if (isCrouching)
        {
            Vector3 v3CamHolder = cameraHolder.transform.localPosition;
            v3CamHolder.y = camera_heightCrouching;
            cameraHolder.transform.localPosition = v3CamHolder;
            cc.height = collider_heightCrouching;

        }
        else
        {
            Vector3 v3CamHolder = cameraHolder.transform.localPosition;
            v3CamHolder.y = camera_heightDefault;
            cameraHolder.transform.localPosition = v3CamHolder;
            cc.height = collider_heightDefault;

        }
    }

    void Friction()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = jumpDrag;
        }
    }

    void Moving()
    {
        if (!disableInput)
        {
            xMovement = Input.GetAxisRaw("Horizontal");
            yMovement = Input.GetAxisRaw("Vertical");
        }

        dir = transform.right * xMovement + transform.forward * yMovement;

        float speed = moveSpeed;

        if (isCrouching)
        {
            speed = crouchSpeed;
        }

        if (isGrounded && !onSlope())
        {
            rb.AddForce(dir.normalized * speed * speedMultiplier.Value);
        }
        else if (!isGrounded)
        {
            rb.AddForce(dir.normalized * speed * speedMultiplier.Value * jumpSpeedMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && onSlope())
        {
            rb.AddForce(slopeDirection.normalized * speed * speedMultiplier.Value, ForceMode.Acceleration);
        }

        if (isLimitedIntroMode == false)
        {
            if (isGrounded && dir.magnitude > 0f)
            {
                if (!runningAudioPlaying)
                {
                    if (isCrouching == false) soundManager.Play("running");
                    runningAudioPlaying = true;

                }

                if (isCrouching == true) soundManager.Pause("running");
            }
            else
            {
                soundManager.Pause("running");
                runningAudioPlaying = false;
            }
        }

        if (Weapon != null)
        {
            var gun = Weapon.currentGunHeld;

            if (gun != null)
            {
                if (dir.magnitude > 0f || WallRun.isWallRunning)
                {
                    if (isNoGravity == false) Anim.SetBool("isRunning", true);

                 
                }
                else
                {
                    Anim.SetBool("isRunning", false);
                }

                if (isCrouching)
                {
                    Anim.SetBool("isRunning", false);
                }
            }
        }
    }

    void Jumping()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, player, queryTriggerInteraction: QueryTriggerInteraction.Ignore);

        if (!isCheatMode)
        {
            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
                soundManager.Play("jumping");
                Anim.SetTrigger("jumping");
            }
            else if (!isGrounded && !WallRun.isWallRunning)
            {
                rb.AddForce(-transform.up * fallSpeed, ForceMode.Acceleration);
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
                soundManager.Play("jumping");
                Anim.SetTrigger("jumping");
            }
        }

        if (isNoGravity)
        {
            if (Input.GetButton("Jump"))
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(transform.up * jumpHeight * 0.1f, ForceMode.Impulse);
            }

            if (isCheatMode)
            {
                if (!isGrounded && !WallRun.isWallRunning)
                {
                    rb.AddForce(-transform.up * fallSpeed, ForceMode.Acceleration);
                }
            }
        }

    }

    bool onSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight/2 + .5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public IEnumerator Dash()
    {
        if (xMovement == 0 && yMovement == 0)
        {
            dashDirection = transform.forward;
        }
        else if (yMovement > 0)
        {
            dashDirection = transform.forward;
        }
        else if (yMovement < 0)
        {
            dashDirection = -transform.forward;   
        }
        else if (yMovement == 0)
        {
            if (xMovement > 0)
            {
                dashDirection = transform.right;
            }
            else if (xMovement < 0)
            {
                dashDirection = -transform.right;
            }
        }

        if (isNoGravity == false) rb.useGravity = false;
        rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
        soundManager.Play("dash");
        dashManager.manageDash();
        yield return new WaitForSeconds(dashDuration);

        rb.velocity = dir * moveSpeed;
        if (isNoGravity == false) rb.useGravity = true;
    }
}
