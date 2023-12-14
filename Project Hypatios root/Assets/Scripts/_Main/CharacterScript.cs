using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
using Sirenix.OdinInspector;
using Kryz.CharacterStats;


public class CharacterScript : Entity
{


    [FoldoutGroup("Stats")] public CharacterStat BonusDamageMelee; //percentage only
    [FoldoutGroup("Stats")] public CharacterStat BonusDamageGun; //percentage only
    [FoldoutGroup("Stats")] public BaseStatValue stat_jumps;
    [FoldoutGroup("Stats")] public BaseStatValue stat_dash;
    [FoldoutGroup("Stats")] public BaseStatusEffectObject ailment_BrokenLeg;

    private dashParticleManager dashManager;
    public Rigidbody rb;
    private float playerHeight = 2f;
    private wallRun _wallRun;

    [Header("Special FPS Mode")]
    [FoldoutGroup("Modes")] public bool isLimitedIntroMode = false;
    [FoldoutGroup("Modes")] public bool normalMode = true;
    [FoldoutGroup("Modes")] public bool disableInput = false;
    [FoldoutGroup("Modes")] public bool tutorialMode = false;
    [FoldoutGroup("Modes")] public bool isNoGravity = false;


    [Space]
    [Header("Movement States")]
    public float maintainMomentumStrength = 5f;
    public float crouchSpeed = 3f;
    public float collider_heightDefault = 0.9f;
    public float collider_heightCrouching = 0.3f;
    public float camera_heightDefault = 0.9f;
    public float camera_heightCrouching = 0.3f;
    [FoldoutGroup("Modes")] [ShowIf("isNoGravity", optionalValue: true)] public float noGrav_descendSpeed = 6f;
    [FoldoutGroup("Modes")] [ShowIf("isNoGravity", optionalValue: true)] public float noGrav_ascendSpeed = 3f;
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
    [FoldoutGroup("Physics")] public float maximumSlopeLimit = 60f;
    [FoldoutGroup("Physics")] public float softSlopeLimit = 40f; //angle which player greatly struggles to cliimb
    [FoldoutGroup("Physics")] public Transform groundCheck;
    [FoldoutGroup("Physics")] public float groundDistance = .4f;
    [FoldoutGroup("Physics")] public float groundRadiusCheck = .3f;
    [FoldoutGroup("Physics")] public float stepHeight = .3f;
    [FoldoutGroup("Physics")] public float stepSmooth = .2f;
    [FoldoutGroup("Physics")] public float stepDistCheck = 1f;
    [FoldoutGroup("Physics")] public Transform stepCheckHigh;
    [FoldoutGroup("Physics")] public Transform stepCheckLow;
    [FoldoutGroup("Physics")] public LayerMask stepCheckLayerMask;
    [FoldoutGroup("Physics")] public LayerMask player;
    [FoldoutGroup("Physics")] public float jumpHeight;
    [FoldoutGroup("Physics")] public float jumpDrag = 1f;
    [FoldoutGroup("Physics")] public float jumpSpeedMultiplier = 0.4f;
    [FoldoutGroup("Physics")] public float fallSpeed = 2f;
    [FoldoutGroup("Physics")] bool inAir = false;
    [FoldoutGroup("Physics")] public bool isGrounded;
    [FoldoutGroup("Physics")] public bool isCrouching = false;
    [FoldoutGroup("Physics")] public float thresholdFallHeight = 20f;
    [FoldoutGroup("Physics")] public float maxFallHeight = 50f;
    [FoldoutGroup("Physics")] public float thresholdFallVelocity = -3f;
    [FoldoutGroup("Physics")] public float minChanceBrokenLeg = 0.3f;

    //Slope & Stair Detection
    [FoldoutGroup("Physics")] private RaycastHit slopeHit;
    [FoldoutGroup("Physics")] private Vector3 slopeDirection;
    [FoldoutGroup("Physics")] public Vector3 recoil_Fall = new Vector3(33f, 10f, 6f);
    [FoldoutGroup("Physics")] public Vector3 recoil_Jump = new Vector3(60f, 25f, 15f);

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
    public HypatiosSave.PerkDataSave PerkData;
    public Inventory Inventory = new Inventory();

    [HideInInspector] public Animator Anim { get => anim; }
    public wallRun WallRun
    {
        get
        {
            if (_wallRun != null) return _wallRun;
            else return FindObjectOfType<wallRun>();
        }
    }
    private float airTime = 0;

    private float netFallVelocity = 0f;
    private float netVelocityMagnitude;
    private Vector3 netVelocityDirection;

    private List<float> fallVelocityList_Y = new List<float>();
    private int fallVelocityList_Count = 50;

    //for maintaining velocity
    private List<Vector3> VelocityDirectionList = new List<Vector3>();
    private int velocityDirectionList_Count = 10;

    private List<float> VelocityMagnitudeList = new List<float>();
    private List<Vector3> prevPositions_ForVelocity = new List<Vector3>();
    private int velocityMagnitudeList_Count = 20;

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
        PerkInitialize(ModifierEffectCategory.MaxHitpointBonus);
        PerkInitialize(ModifierEffectCategory.RegenHPBonus);
        PerkInitialize(ModifierEffectCategory.KnockbackResistance);
        PerkInitialize(ModifierEffectCategory.Recoil);
        PerkInitialize(ModifierEffectCategory.BonusDamageMelee);
        PerkInitialize(ModifierEffectCategory.BonusDamageGun);
        PerkInitialize(ModifierEffectCategory.Alcoholism);
        PerkInitialize(ModifierEffectCategory.Digestion);
        dashCooldown.BaseValue = PlayerPerk.GetValue_Dashcooldown(PerkData.Perk_LV_DashCooldown); //dash cooldown is fixed due to changing too much will easily break the level design

        //bugged value
        {
            var test1 = Health.armorRating.Value;
            test1 = Health.digestion.Value;
        }
    }

    public override void RemoveStatusEffectGroup(BaseStatusEffectObject _statusEffect)
    {
        base.RemoveStatusEffectGroup(_statusEffect);
        var tempDataStats = PerkData.Temp_StatusEffect.Find(x => x.ID == _statusEffect.GetID());
        if (tempDataStats != null)
        {
            PerkData.Temp_StatusEffect.Remove(tempDataStats);
        }
    }

    public int GetNetSoulBonusPerk()
    {
        var basePerkClass = PlayerPerk.GetBasePerk(ModifierEffectCategory.SoulBonus);
        int netPerk = PerkData.Perk_LV_Soulbonus;

        if (netPerk > basePerkClass.MAX_LEVEL)
            netPerk = basePerkClass.MAX_LEVEL;

        return netPerk;
    }

    public int GetNetShortcutPerk()
    {
        var basePerkClass = PlayerPerk.GetBasePerk(ModifierEffectCategory.ShortcutDiscount);
        int netPerk = PerkData.Perk_LV_ShortcutDiscount;

        if (netPerk > basePerkClass.MAX_LEVEL)
            netPerk = basePerkClass.MAX_LEVEL;

        return netPerk;
    }

    public float GetCharBaseValue(ModifierEffectCategory category)
    {
        if (category == ModifierEffectCategory.MaxHitpointBonus)
        {
            return Health.maxHealth.BaseValue;
        }
        else if (category == ModifierEffectCategory.RegenHPBonus)
        {
            return Health.healthRegen.BaseValue;
        }
        else if (category == ModifierEffectCategory.KnockbackResistance)
        {
            return Weapon.Recoil.knockbackResistance.BaseValue;
        }
        else if (category == ModifierEffectCategory.Recoil)
        {
            return Weapon.Recoil.baseRecoil.BaseValue;
        }
        else if (category == ModifierEffectCategory.BonusDamageMelee)
        {
            return BonusDamageMelee.BaseValue;
        }
        else if (category == ModifierEffectCategory.BonusDamageGun)
        {
            return BonusDamageGun.BaseValue;
        }
        else if (category == ModifierEffectCategory.DashCooldown)
        {
            return dashCooldown.BaseValue;
        }
        else if (category == ModifierEffectCategory.Alcoholism)
        {
            return 0;
        }
        else if (category == ModifierEffectCategory.MovementBonus)
        {
            return speedMultiplier.BaseValue;
        }
        else if (category == ModifierEffectCategory.ArmorRating)
        {
            return Health.armorRating.BaseValue;
        }
        else if (category == ModifierEffectCategory.Digestion)
        {
            return Health.digestion.BaseValue;
        }

        return 0;
    }

    public float GetCharFinalValue(ModifierEffectCategory category)
    {
        if (category == ModifierEffectCategory.MaxHitpointBonus)
        {
            return Health.maxHealth.Value;
        }
        else if (category == ModifierEffectCategory.RegenHPBonus)
        {
            return Health.healthRegen.Value;
        }
        else if (category == ModifierEffectCategory.KnockbackResistance)
        {
            return Weapon.Recoil.knockbackResistance.Value;
        }
        else if (category == ModifierEffectCategory.Recoil)
        {
            return Weapon.Recoil.baseRecoil.Value;
        }
        else if (category == ModifierEffectCategory.BonusDamageMelee)
        {
            return BonusDamageMelee.Value;
        }
        else if (category == ModifierEffectCategory.BonusDamageGun)
        {
            return BonusDamageGun.Value;
        }
        else if (category == ModifierEffectCategory.DashCooldown)
        {
            return dashCooldown.Value;
        }
        else if (category == ModifierEffectCategory.Alcoholism)
        {
            return Health.alcoholMeter;
        }
        else if (category == ModifierEffectCategory.MovementBonus)
        {
            return speedMultiplier.Value;
        }
        else if (category == ModifierEffectCategory.ArmorRating)
        {
            return Health.armorRating.Value;
        }
        else if (category == ModifierEffectCategory.Digestion)
        {
            return Health.digestion.Value;
        }

        return 0;
    }

    private void CustomPerkLoad()
    {
        foreach (var customPerk in PerkData.Temp_CustomPerk)
        {
            var category = customPerk.statusCategoryType;
            GenericStatus effectObject = GetGenericEffect(category, $"{customPerk.origin}-TempPerk");

            if (customPerk.isPermanent)
            {
                
            }
            else
            {
                effectObject = GetGenericEffect(category, $"SERUM-{customPerk.origin}");
            }

            if (effectObject == null)
            {
                if (customPerk.isPermanent)
                {
                    CreatePersistentStatusEffect(category, customPerk.Value, $"{customPerk.origin}-TempPerk");
                }
                else
                {
                    CreateTimerStatusEffect(category, customPerk.Value, customPerk.timer, $"SERUM-{customPerk.origin}", allowDuplicate: true);
                }
            }
            else
            {
                effectObject.Value = customPerk.Value;
                effectObject.ApplyEffect();
            }
        }

        foreach (var statusEffect in PerkData.Temp_StatusEffect)
        {
            var baseStatusEffect = Hypatios.Assets.GetStatusEffect(statusEffect.ID);
            StatusEffectMono statusMono = null;
            if (IsStatusEffectGroup(baseStatusEffect) == false)
            {
                statusMono = CreateStatusEffectGroup(baseStatusEffect, statusEffect.Time);
            }
            else statusMono = GetStatusEffectGroup(baseStatusEffect);
            foreach (var modifier in baseStatusEffect.allStatusEffects)
            {
                var category = modifier.statusCategoryType;
                var effectObject = GetGenericEffect(category, $"playerModifier_{statusEffect.ID}");

                if (effectObject == null)
                {
                    effectObject = CreateTimerStatusEffect(category, modifier.Value, statusEffect.Time, $"playerModifier_{statusEffect.ID}", allowDuplicate: true);
                    effectObject.statusMono = statusMono;
                }
                else
                {
                    effectObject.Value = modifier.Value;
                    effectObject.ApplyEffect();
                }

            }

        }
    }

    private void PerkInitialize(ModifierEffectCategory category)
    {
        float value = 0;

        if (category == ModifierEffectCategory.MaxHitpointBonus)
        {
            value = PlayerPerk.GetValue_MaxHPUpgrade(PerkData.Perk_LV_MaxHitpointUpgrade);
        }
        else if (category == ModifierEffectCategory.RegenHPBonus)
        {
            value = PlayerPerk.GetValue_RegenHPUpgrade(PerkData.Perk_LV_RegenHitpointUpgrade);
        }
        else if (category == ModifierEffectCategory.KnockbackResistance)
        {
            value = PlayerPerk.GetValue_KnockbackResistUpgrade(PerkData.Perk_LV_KnockbackRecoil);
        }
        else if (category == ModifierEffectCategory.Recoil)
        {
            value = PlayerPerk.GetValue_RecoilUpgrade(PerkData.Perk_LV_WeaponRecoil);
        }
        else if (category == ModifierEffectCategory.BonusDamageMelee)
        {
            value = PlayerPerk.GetValue_BonusMeleeDamage(PerkData.Perk_LV_IncreaseMeleeDamage);
        }
        else if (category == ModifierEffectCategory.BonusDamageGun)
        {
            value = PlayerPerk.GetValue_BonusGunDamage(PerkData.Perk_LV_IncreaseGunDamage);
        }
        else if (category == ModifierEffectCategory.Alcoholism)
        {
            value = Health.alcoholMeter;
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

    public override void Heal(float healAmount)
    {
        Health.curHealth += healAmount;
        Health.targetHealth += healAmount;
    }
    #endregion



    void Start()
    {
        moveSpeed = runSpeed;
        _wallRun = GetComponent<wallRun>();
        rb = GetComponent<Rigidbody>();
        dashManager = GetComponent<dashParticleManager>();
        rb.freezeRotation = true;
        timeSinceLastDash = dashCooldown.Value;
        soundManager = FindObjectOfType<soundManagerScript>();
        cc = GetComponent<CapsuleCollider>();
        prevPositions_ForVelocity = new List<Vector3>(velocityMagnitudeList_Count);

        {
            Vector3 v3 = stepCheckHigh.transform.position;
            v3.y = stepCheckLow.transform.position.y + stepHeight;
            stepCheckHigh.transform.position = v3;
        }

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
            isGrounded = Physics.CheckSphere(groundCheck.position, groundRadiusCheck, player, queryTriggerInteraction: QueryTriggerInteraction.Ignore);

            if (isLimitedIntroMode == false && !disableInput && Hypatios.IsDemoFinished() == false)
                Jumping();

            timeSinceLastDash += Time.deltaTime;
            float timeAfterDash = Mathf.Clamp(timeSinceLastDash / dashCooldown.Value, 0f, dashCooldown.Value);
            MainGameHUDScript.Instance.dashSlider.value = timeAfterDash;

            slopeDirection = Vector3.ProjectOnPlane(dir, slopeHit.normal);


            if (Anim != null)
            {
                if (!isGrounded && !_wallRun.isWallRunning)
                {
                    inAir = true;
                    airTime += Time.deltaTime;
                    if (isNoGravity == false) Anim.SetBool("inAir", true);
                }
                else if (_wallRun.isWallRunning)
                {
                    inAir = false;
                    Anim.SetBool("inAir", false);

                }
                if (inAir && isGrounded)
                {
                    inAir = false;
                    if (airTime > 0.3f && isNoGravity == false) soundManager.Play("falling");
                    Anim.SetBool("inAir", false);

                    if (isNoGravity == false)
                    {
                        float multiplierAir = Mathf.Clamp(airTime * 0.6f, 0.2f, 5);
                        multiplierAir *= 2f;
                        Weapon.Recoil.CustomRecoil(recoil_Fall, multiplierAir);
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

        //dash handling
        if (isLimitedIntroMode == false)
        {
            if (isCheatMode == false)
            {
                HandleDash_Update();
            }
            else if (Hypatios.Input.Dash.triggered)
            {
                b_triggerDash = true;
            }
        }
    }

    bool testDashReady = false;

    #region Physics and Interactions

    private void HandleDash_Update()
    {
        float dashCooldownLimit = Mathf.Clamp(dashCooldown.Value, 0.2f, 99f); //Dash cooldown can never reach under 0.2 second

        if (Hypatios.Input.Dash.triggered && timeSinceLastDash > dashCooldownLimit)
        {
            b_triggerDash = true;
        }

    }

    private bool b_triggerDash = false;
    private bool b_noGrav_Descend = false;
    private bool b_noGrav_Ascend = false;

    void FixedUpdate()
    {
        Moving();
        CalculateFallVelocity();
        CalculateVelocity();
        CalculateVelocityDir();

        if (isLimitedIntroMode)
        {
            return;
        }

        if (Hypatios.IsDemoFinished())
        {
            return;
        }

        float dashCooldownLimit = Mathf.Clamp(dashCooldown.Value, 0.2f, 99f); //Dash cooldown can never reach under 0.2 second
        if (timeSinceLastDash > dashCooldownLimit)
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

        if (b_triggerDash)
        {
            Hypatios.Game.Increment_PlayerStat(stat_dash);
            StartCoroutine(Dash());
            timeSinceLastDash = 0;
            b_triggerDash = false;
        }

        FixedUp_NoGravity();

    }

    private void FixedUp_NoGravity()
    {
        if (b_noGrav_Ascend)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * noGrav_ascendSpeed * speedMultiplier.BaseValue, ForceMode.Impulse);
            b_noGrav_Ascend = false;
        }

        if (b_noGrav_Descend)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * -noGrav_descendSpeed * speedMultiplier.BaseValue, ForceMode.Impulse);
            b_noGrav_Descend = false;
        }
    }

    #region Velocity
    private void CalculateFallVelocity()
    {
        float y = transform.position.y;
        fallVelocityList_Y.Insert(0, y);

        {
            var thisIsStupid_List = new List<float>();
            thisIsStupid_List.AddRange(fallVelocityList_Y);

            int index = 0;

            foreach (var _yPos in thisIsStupid_List)
            {
                if (index >= fallVelocityList_Count)
                {
                    fallVelocityList_Y.RemoveAt(index);
                }

                index++;
            }
        }

        CalcualtedNetFallVelocity();
    }


    private void CalcualtedNetFallVelocity()
    {
        if (fallVelocityList_Y.Count <= 1)
        {
            netFallVelocity = 0f;
            return;
        }

        float sum = 0;
        float net = 0;

        int index = 0;

        foreach (var _yPos in fallVelocityList_Y)
        {
            if (index != 0) sum -= _yPos;
            index++;
        }

        net = sum / (fallVelocityList_Y.Count - 1);
        net += fallVelocityList_Y[0];
        net /= 2f;
        netFallVelocity = net;
    }


    private void CalculateVelocityDir()
    {
        Vector3 dir = prevPositions_ForVelocity[1] - transform.position;
        dir.Normalize();
        VelocityDirectionList.Insert(0, dir);

        {
            var thisIsStupid_List = new List<Vector3>();
            thisIsStupid_List.AddRange(VelocityDirectionList);

            int index = 0;

            foreach (var _velocity in thisIsStupid_List)
            {
                if (index >= velocityDirectionList_Count)
                {
                    VelocityDirectionList.RemoveAt(index);
                }

                index++;
            }
        }

        CalcualtedNetVelocityDir();
    }

    private void CalcualtedNetVelocityDir()
    {
        if (VelocityDirectionList.Count <= 1)
        {
            netVelocityDirection = transform.forward;
            return;
        }

        Vector3 sum = transform.forward;
        Vector3 net = transform.forward;

        int index = 0;

        foreach (var _velocity in VelocityDirectionList)
        {
            if (index != 0)
            {
                sum += _velocity;
            }
            index++;
        }

        net = sum / (VelocityDirectionList.Count - 1);
        //net += VelocityList[0];
        //net /= 2f;
        netVelocityDirection = net;
        netVelocityDirection.y = 0; //no vertical speed momentum

    }



    private void CalculateVelocity()
    {
        float velocity = 0f;
        if (VelocityMagnitudeList.Count >= 2)
        {
            Vector3 _velocity = prevPositions_ForVelocity[1] - transform.position;
            velocity = _velocity.magnitude / Time.deltaTime; //it checks every Time.deltaTime
        }

        VelocityMagnitudeList.Insert(0, velocity);
        prevPositions_ForVelocity.Insert(0, transform.position);

        {
            var thisIsStupid_List = new List<float>();
            thisIsStupid_List.AddRange(VelocityMagnitudeList);

            int index = 0;

            foreach (var _yPos in thisIsStupid_List)
            {
                if (index >= velocityMagnitudeList_Count)
                {
                    VelocityMagnitudeList.RemoveAt(index);
                }

                index++;
            }

            var thisIsStupid_List2 = new List<Vector3>();
            thisIsStupid_List2.AddRange(prevPositions_ForVelocity);
            int indexZ = 0;

            foreach (var _yPos in thisIsStupid_List2)
            {
                if (indexZ >= velocityMagnitudeList_Count)
                {
                    prevPositions_ForVelocity.RemoveAt(indexZ);
                }

                indexZ++;
            }
        }

        CalcualtedNetVelocity();
    }


    private void CalcualtedNetVelocity()
    {
        if (VelocityMagnitudeList.Count <= 1)
        {
            netVelocityMagnitude = 0f;
            return;
        }

        float sum = 0;
        float net = 0;

        int index = 0;

        foreach (var _yPos in VelocityMagnitudeList)
        {
            if (index != 0) sum += _yPos;
            index++;
        }

        net = sum / (VelocityMagnitudeList.Count - 1);
        net += VelocityMagnitudeList[0];
        net /= 2f;
        netVelocityMagnitude = Mathf.Abs(net);
    }
    #endregion


    /// <summary>
    /// Exclusively for maintain momentum direction
    /// </summary>
    public void MaintainMomentum()
    {
        rb.AddForce(-netVelocityDirection * netVelocityMagnitude * maintainMomentumStrength);
    }

    public void HandleCrouchingState()
    {
        if (isLimitedIntroMode | isNoGravity)
        {
            return;
        }

        if (Hypatios.Input.Crouch.triggered)
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
        if (!disableInput && !Hypatios.IsDemoFinished())
        {
            var moveVector = Hypatios.Input.Move.ReadValue<Vector2>();
            xMovement = moveVector.x;//Input.GetAxisRaw("Horizontal");
            yMovement = moveVector.y;//Input.GetAxisRaw("Vertical");
        }
        else
        {
            if (Weapon != null)
            {
                var gun = Weapon.currentGunHeld;

                if (gun != null)
                {
                    if (rb.velocity.magnitude > 0.2f)
                    {
                        if (isNoGravity == false) Anim.SetBool("isRunning", true);
                    }
                    else
                    {
                        Anim.SetBool("isRunning", false);
                    }
                }
            }

            xMovement = 0f;
            yMovement = 0f;
        }

        {
            bool isTooSlope = false;
            bool isSoftSlope = false;
            SlopeCheck();

            if (_slopeAngle > maximumSlopeLimit && _slopeHit.collider != null)
                isTooSlope = true;
            if (_slopeAngle > softSlopeLimit && _slopeHit.collider != null)
                isSoftSlope = true;

            if (isTooSlope && !_wallRun.isWallRunning)
            {
                Vector3 slopeDir = Vector3.up - _slopeHit.normal * Vector3.Dot(Vector3.up, _slopeHit.normal);
                var netDir = slopeDir * -fallSpeed;
                //netDir.y = (netDir.y - _slopeHit.point.y) * Time.deltaTime;
                dir = netDir + (transform.right * xMovement * 0.5f) + (transform.forward * yMovement * 0.5f);
            }
            else if (isSoftSlope && !_wallRun.isWallRunning)
            {
                Vector3 slopeDir = Vector3.up - _slopeHit.normal * Vector3.Dot(Vector3.up, _slopeHit.normal);
                var netDir = slopeDir * (-fallSpeed * 0.15f);
                dir = (netDir) + transform.right * xMovement + transform.forward * yMovement;
            }
            else
                dir = transform.right * xMovement + transform.forward * yMovement;
        }

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

            if (isNoGravity == false)
            {
                rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
            }
        }
        else if (isGrounded && onSlope())
        {
            rb.AddForce(dir.normalized * speed * speedMultiplier.Value, ForceMode.Acceleration);
        }

        if (isLimitedIntroMode == false)
        {
            if (isGrounded && isNoGravity == false && dir.magnitude > 0f)
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
                if (dir.magnitude > 0f || _wallRun.isWallRunning)
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

        StepClimb();
    }

    [ReadOnly] [ShowInInspector] private float _slopeAngle = 0f;
    private RaycastHit _slopeHit;

    private void SlopeCheck()
    {
        Vector3 checkPos = groundCheck.position + new Vector3(0, 0.1f, 0);
        Vector3 checkPos1 = groundCheck.position + (transform.forward*cc.radius) + new Vector3(0, 0.1f, 0);
        Vector3 checkPos2 = groundCheck.position + (-transform.forward * cc.radius) + new Vector3(0, 0.1f, 0);

        Debug.DrawRay(checkPos, Vector3.down * groundDistance * 2f, Color.red);
        Debug.DrawRay(checkPos1, Vector3.down * groundDistance * 2f, Color.red);
        Debug.DrawRay(checkPos2, Vector3.down * groundDistance * 2f, Color.red);

        if (Physics.Raycast(checkPos, Vector3.down, out _slopeHit, groundDistance * 2f, player, QueryTriggerInteraction.Ignore))
        {
            _slopeAngle = Vector3.Angle(_slopeHit.normal, Vector3.up);
        }
        else if (Physics.Raycast(checkPos1, Vector3.down, out _slopeHit, groundDistance * 2f, player, QueryTriggerInteraction.Ignore))
        {
            _slopeAngle = Vector3.Angle(_slopeHit.normal, Vector3.up);
        }
        else if (Physics.Raycast(checkPos2, Vector3.down, out _slopeHit, groundDistance * 2f, player, QueryTriggerInteraction.Ignore))
        {
            _slopeAngle = Vector3.Angle(_slopeHit.normal, Vector3.up);
        }
    }

    private void StepClimb()
    {
        float checkDistLow = cc.radius;
        float checkDistHigh = cc.radius;
        float netMovement = Mathf.Clamp(Mathf.Abs(xMovement + yMovement)/2f, 0f, 1f);
        RaycastHit hitLower;

        bool allowStepClimb = true;

        if (netMovement < 0.1f)
            allowStepClimb = false;

        bool isTooSlope = false;
        bool isSoftSlope = false;
        SlopeCheck();

        if (_slopeAngle > maximumSlopeLimit && _slopeHit.collider != null)
            isTooSlope = true;
        if (_slopeAngle > softSlopeLimit && _slopeHit.collider != null)
            isSoftSlope = true;

        if (isSoftSlope | isTooSlope)
            allowStepClimb = false;

        if (allowStepClimb)
        {
            if (Physics.Raycast(stepCheckLow.transform.position, transform.TransformDirection(0, 0, 1), out hitLower, stepDistCheck, player, QueryTriggerInteraction.Ignore))
            {
                RaycastHit hitUpper;

                if (!Physics.Raycast(stepCheckHigh.transform.position, transform.TransformDirection(0, 0, 1), out hitUpper, stepDistCheck, player, QueryTriggerInteraction.Ignore))
                {
                    rb.AddForce(new Vector3(0f, stepSmooth * netMovement, 0f));
                }

            }
        }

        Debug.DrawRay(stepCheckLow.transform.position, stepCheckLow.forward * stepDistCheck, Color.cyan);
        Debug.DrawRay(stepCheckHigh.transform.position, stepCheckHigh.forward * stepDistCheck, Color.red);

        
        Vector3[] omniDirs = new Vector3[7];
        omniDirs[0] = new Vector3(1.5f, 0, 1);
        omniDirs[1] = new Vector3(-1.5f, 0, 1);
        omniDirs[2] = new Vector3(-1, 0, 0f);
        omniDirs[3] = new Vector3(1, 0, 0f);
        omniDirs[4] = new Vector3(0f, 0, -1);
        omniDirs[5] = new Vector3(1.5f, 0, -1);
        omniDirs[6] = new Vector3(-1.5f, 0, -1);

        foreach (var dir1 in omniDirs)
        {
            RaycastHit hitLower1;

            if (allowStepClimb)
            {
                if (Physics.Raycast(stepCheckLow.transform.position, transform.TransformDirection(dir1), out hitLower1, stepDistCheck, player, QueryTriggerInteraction.Ignore))
                {
                    RaycastHit hitUpper1;

                    if (!Physics.Raycast(stepCheckHigh.transform.position, transform.TransformDirection(dir1), out hitUpper1, stepDistCheck, player, QueryTriggerInteraction.Ignore))
                    {
                        //RETARDED CODE
                        Vector3 dirFinal = dir1.normalized;
                        Vector3 netMovementDir = new Vector3(xMovement, 0, yMovement);
                        float maxDist = 0.9f;
                        bool dontTrigger = false;

                        netMovementDir -= dirFinal;
                        netMovementDir.x = Mathf.Clamp(netMovementDir.x, -1f, 1f);
                        netMovementDir.z = Mathf.Clamp(netMovementDir.z, -1f, 1f);
                        {
                            float x1 = Mathf.Abs(netMovementDir.x);
                            float y1 = Mathf.Abs(netMovementDir.z);
                            if (x1 > maxDist)
                                dontTrigger = true;
                            if (y1 > maxDist)
                                dontTrigger = true;
                        }

                        float velocityFinal = 1f;
                        if (dontTrigger)
                            velocityFinal = 0f;
                        rb.AddForce(new Vector3(0f, stepSmooth * velocityFinal, 0f));
                    }
                }
            }

            Debug.DrawRay(stepCheckLow.transform.position, transform.TransformDirection(dir1) * stepDistCheck, Color.cyan);
            Debug.DrawRay(stepCheckHigh.transform.position, transform.TransformDirection(dir1) * stepDistCheck, Color.red);
        }
    }

    private float _highestHeight = -99f;

    void Jumping()
    {

        if (!isCheatMode)
        {

            bool isTooSlope = false;
            SlopeCheck();

            if (_slopeAngle > maximumSlopeLimit && _slopeHit.collider != null)
                isTooSlope = true;

            if (isGrounded && Hypatios.Input.Jump.triggered && Gamepad.current == null)
            {
                Vector3 dirJump = transform.up * jumpHeight;

                if (isTooSlope)
                {
                    Vector3 slopeDir = _slopeHit.normal;
                    dirJump = slopeDir * jumpHeight;
                }

                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(dirJump, ForceMode.Impulse);
                Weapon.Recoil.CustomRecoil(recoil_Jump, 1f);
                soundManager.Play("jumping");
                Anim.SetTrigger("jumping");
                Hypatios.Game.Increment_PlayerStat(stat_jumps);

            }
            else if (isGrounded && Hypatios.Input.Jump.triggered && Gamepad.current != null && InteractableCamera.instance.currentInteractable == null)
            {
                Vector3 dirJump = transform.up * jumpHeight;

                if (isTooSlope)
                {
                    Vector3 slopeDir = _slopeHit.normal;
                    dirJump = slopeDir * jumpHeight;
                }

                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(dirJump, ForceMode.Impulse);
                soundManager.Play("jumping");
                Anim.SetTrigger("jumping");
                Hypatios.Game.Increment_PlayerStat(stat_jumps);

            }
            else if (!isGrounded && !_wallRun.isWallRunning) 
            {
                rb.AddForce(-transform.up * fallSpeed, ForceMode.Acceleration);
            }
            else if (isGrounded && isTooSlope && !_wallRun.isWallRunning)
            {
                Vector3 slopeDir = Vector3.up - _slopeHit.normal * Vector3.Dot(Vector3.up, _slopeHit.normal);
                var netDir = slopeDir * -fallSpeed;
                netDir.y = netDir.y - _slopeHit.point.y;
                rb.AddForce(netDir, ForceMode.Acceleration);
            }

          
        }
        else
        {
            if (Hypatios.Input.Jump.triggered)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
                soundManager.Play("jumping");
                Anim.SetTrigger("jumping");
                Hypatios.Game.Increment_PlayerStat(stat_jumps);

            }
        }

        float y = transform.position.y;

        if (!isGrounded && !_wallRun.isWallRunning)
        {
            if (_highestHeight < y)
                _highestHeight = y;
        }
        else
        {
            if (y < _highestHeight && !isNoGravity)
            {
                TryFallDamage();
            }
            if (IsStatusEffectGroup(ailment_BrokenLeg) == true) isCrouching = true;

            _highestHeight = -100f;
        }

        if (isNoGravity)
        {
            if (Hypatios.Input.Jump.IsPressed())
            {
                b_noGrav_Ascend = true;

            }
   

            if (Hypatios.Input.Crouch.IsPressed())
            {
                b_noGrav_Descend = true;
            }

            if (isCheatMode)
            {
                if (!isGrounded && !_wallRun.isWallRunning)
                {
                    rb.AddForce(-transform.up * fallSpeed, ForceMode.Acceleration);
                }
            }
        }

    }

    private void TryFallDamage()
    {
        float fallHeight = _highestHeight - transform.position.y;

        if (fallHeight < thresholdFallHeight) return;
        if (thresholdFallVelocity <= netFallVelocity) return;
        soundManager.Play("bones");

        float random = Random.Range(0f,1f);
        float f = maxFallHeight - thresholdFallHeight;
        float t = (fallHeight - thresholdFallHeight)/f;
        float chanceBrokenLeg = Mathf.Lerp(thresholdFallHeight, maxFallHeight, t);
        chanceBrokenLeg = chanceBrokenLeg / maxFallHeight;
        //Debug.Log($"Broken leg chance: {chanceBrokenLeg*100f}%");

        if (random < chanceBrokenLeg && !isCheatMode)
        {
            if (IsStatusEffectGroup(ailment_BrokenLeg) == false)
            {
                ailment_BrokenLeg.AddStatusEffectPlayer(9999f);
                DeadDialogue.PromptNotifyMessage_Mod("Aldrich broke his legs.", 4f);
                Hypatios.Game.RuntimeTutorialHelp(Hypatios.CodexList.FallDamage);
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
        Weapon.Recoil.CustomRecoil(dashDirection, 30f);
        rb.AddForce(dashDirection * dashForce * Time.deltaTime * 50f, ForceMode.Force);
        soundManager.Play("dash");
        dashManager.manageDash();

        yield return new WaitForSeconds(dashDuration);

        if (!isCheatMode)
        {
            rb.velocity = dir * moveSpeed;

        }
        else
            rb.velocity = dir * moveSpeed;

        if (isNoGravity == false) rb.useGravity = true;
    }

    #endregion
}
