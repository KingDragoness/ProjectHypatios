using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Animations;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Fortification_GhostSentry : EnemyScript
{

    [FoldoutGroup("References")] public ModularTurretGun turretGun;
    [FoldoutGroup("References")] public ModularTurretGun turretGun1;
    [FoldoutGroup("References")] public Transform v_target_Turret;
    [FoldoutGroup("References")] public Animator animTurret;
    [FoldoutGroup("References")] public Transform v_targetDebug;

    [FoldoutGroup("Sentry")] public GameObject Smoke50HP;
    [FoldoutGroup("Sentry")] public GameObject Smoke25HP;
    [FoldoutGroup("Sentry")] public GameObject botCorpse;    
    [FoldoutGroup("Sentry")] public DynamicObjectPivot pivotObject;
    [FoldoutGroup("Sentry")] public GameObject turretObject;
    [FoldoutGroup("Sentry")] public Text UI_label_HP;
    [FoldoutGroup("Sentry")] public Text UI_label_Ammo;
    [FoldoutGroup("Sentry")] public Slider UI_slider_HP;
    [FoldoutGroup("Sentry")] public Slider UI_slider_Ammo;
    [FoldoutGroup("Sentry")] public SentryGunUI UI_windowSentry;


    public float TimeToInitialize = 2f;
    public int sentryAmmo = 200;
    public int maxSentryAmmo = 200;
    public float maxDistance = 50f;
    public float rotationSpeed = 10f;
    public bool manualControl = false;

    public static Fortification_GhostSentry Instance;
    private float _timerInitialize = 2f;
    private bool _hasInitialized = false;
    private GameObject _windowSentryUI;

    private void Start()
    {
        _timerInitialize = TimeToInitialize;
        currentTarget = Hypatios.Enemy.FindEnemyEntity(Stats.MainAlliance);
        Instance = this;

        if (SentryGunUI.Instance == null)
        {
            _windowSentryUI = MainGameHUDScript.Instance.AttachModularUI(UI_windowSentry.gameObject);
        }
        else
        {
            _windowSentryUI = SentryGunUI.Instance.gameObject;
        }
    }


    public override void OnDestroy()
    {
        Instance = null;
        if (_windowSentryUI != null)
        {
            Destroy(_windowSentryUI.gameObject);
            _windowSentryUI = null;
        }

        base.OnDestroy();
    }

    #region Update

    public void ConsumeAmmo()
    {
        sentryAmmo -= 1;
        if (sentryAmmo <= 0)
            sentryAmmo = 0;
    }

    private void Update()
    {
        if (_hasInitialized == false)
        {
            _timerInitialize -= Time.deltaTime;

            if (_timerInitialize <= 0)
            {
                _hasInitialized = true;
                animTurret.enabled = false;
            }
        }

        if (Stats.CurrentHitpoint < 0)
        {
            if (Stats.IsDead == false) Die();
            Stats.IsDead = true;
            Stats.CurrentHitpoint = 0;
        }

        if (Mathf.RoundToInt(Time.time * 10) % 5 == 0)
            ScanForEnemies(maxDistance: maxDistance);

        if (Time.timeScale == 0) return;
        if (_hasInitialized == false) return;
        if (isAIEnabled == false) return;


        CheckEnableTurret();
        RunAI();
        UpdateUI();
        turretObject.transform.rotation = v_target_Turret.transform.rotation;

    }

    private void UpdateUI()
    {
        UI_label_HP.text = $"{Mathf.RoundToInt(Stats.CurrentHitpoint)}/{Stats.MaxHitpoint.Value}";
        UI_label_Ammo.text = $"{sentryAmmo}/{maxSentryAmmo}";
        UI_slider_HP.value = Stats.CurrentHitpoint;
        UI_slider_HP.maxValue = Stats.MaxHitpoint.Value;
        UI_slider_Ammo.value = sentryAmmo;
        UI_slider_Ammo.maxValue = maxSentryAmmo;
    }

    private void CheckEnableTurret()
    {
        bool enableTurret = false;
        if (sentryAmmo > 0)
            enableTurret = true;
        if (currentTarget == null)
            enableTurret = false;
        if (manualControl == true)
            enableTurret = true;

        if (!enableTurret)
        {
            turretGun.gameObject.SetActive(false);
            turretGun1.gameObject.SetActive(false);
        }
        else
        {
            turretGun.gameObject.SetActive(true);
            turretGun1.gameObject.SetActive(true);
        }

        if (manualControl)
        {
            turretGun.enabled = false;
            turretGun1.enabled = false;
        }
        else
        {
            turretGun.enabled = true;
            turretGun1.enabled = true;
        }

    }

    private void RunAI()
    {
        RotateToTarget();
    }

    float _timerBodyPartTargeting = 1f;
    Vector3 _targetedBodyPartPos;

    private void RotateToTarget()
    {
        if (currentTarget == null)
            return;

        if (manualControl == true) return;

        //changes and search body parts
        _timerBodyPartTargeting -= Time.deltaTime;

        if (_timerBodyPartTargeting <= 0f)
        {
            var availableTargets = currentTarget.GetComponentsInChildren<damageReceiver>();

            if (availableTargets.Length != 0)
            {
                Vector3 offset = new Vector3();
                offset.x = Random.Range(-0.35f, 0.35f);
                offset.y = Random.Range(-0.7f, 0.7f);
                offset.z = Random.Range(-0.35f, 0.35f);

                damageReceiver randomPart = availableTargets[Random.Range(0, availableTargets.Length - 1)];
                _targetedBodyPartPos = randomPart.transform.position + offset;
            }
            else
            {
                _targetedBodyPartPos = currentTarget.OffsetedBoundWorldPosition;
            }

            _timerBodyPartTargeting = 1f;
        }

        Vector3 posTarget = _targetedBodyPartPos;
        v_targetDebug.transform.position = posTarget;
        Vector3 relativePos = posTarget - v_target_Turret.transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        v_target_Turret.transform.rotation = Quaternion.Lerp(v_target_Turret.transform.rotation, rotation, Time.deltaTime * rotationSpeed);

        {
            turretGun.gameObject.transform.LookAt(posTarget);
            turretGun1.gameObject.transform.LookAt(posTarget);
        }

        float chance = Random.Range(0f, 10f);

        if ((turretGun.IsHittingTarget == true | turretGun1.IsHittingTarget == true) && chance > 0.01f && _timerBodyPartTargeting < 0.2f)
        {
            _timerBodyPartTargeting = 0.4f;
        }

        if (turretGun.IsHittingTarget == false && turretGun1.IsHittingTarget == false && _timerBodyPartTargeting > 0.2f)
        {
            _timerBodyPartTargeting = 0.2f;
        }

    }

    public void OverrideTarget(Vector3 target)
    {
        Vector3 relativePos = target - v_target_Turret.transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        v_target_Turret.transform.rotation = Quaternion.Lerp(v_target_Turret.transform.rotation, rotation, Time.deltaTime * rotationSpeed);

        {
            turretGun.gameObject.transform.LookAt(target);
            turretGun1.gameObject.transform.LookAt(target);
        }
    }

    public void FireSentry()
    {
        if (sentryAmmo <= 0) return;
        turretGun.ForceFire();
        turretGun1.ForceFire();
    }

    #endregion

    public override void Die()
    {
        if (Stats.IsDead) return;

        if (botCorpse)
        {
            var corpse1 = Instantiate(botCorpse, transform.position, transform.rotation);
            corpse1.gameObject.SetActive(true);
        }


        Destroy(gameObject);
        OnDied?.Invoke();
        Stats.IsDead = true;
    }

    private bool _isTriggered50HP = false;
    private bool _isTriggered25HP = false;

    public override void Attacked(DamageToken token)
    {
        if (token.origin == DamageToken.DamageOrigin.Player | token.origin == DamageToken.DamageOrigin.Ally)
            return;
        
        Stats.CurrentHitpoint -= token.damage;
        _lastDamageToken = token;
        float percentage = Stats.CurrentHitpoint / Stats.MaxHitpoint.Value;

        float chance = Random.Range(0f, 1f);

        if (chance > 0.5f)
            soundManagerScript.instance.Play3D("hitmetal.0", transform.position);
        else
            soundManagerScript.instance.Play3D("hitmetal.1", transform.position);

        if (percentage < 0.5f)
        {
            Smoke50HP.gameObject.SetActive(true);
            _isTriggered50HP = true;
        }
        else Smoke50HP.gameObject.SetActive(false);

        if (percentage < 0.25f)
        {
            Smoke25HP.gameObject.SetActive(true);
            _isTriggered25HP = true;
        }
        else Smoke25HP.gameObject.SetActive(false);


        if (Stats.CurrentHitpoint < 0)
        {
            Die();

        }
    }
}


