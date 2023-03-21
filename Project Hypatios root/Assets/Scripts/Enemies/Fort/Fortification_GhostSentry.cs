using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Animations;
using Sirenix.OdinInspector;

public class Fortification_GhostSentry : EnemyScript
{

    [FoldoutGroup("References")] public ModularTurretGun turretGun;
    [FoldoutGroup("References")] public ModularTurretGun turretGun1;
    [FoldoutGroup("References")] public Transform v_target_Turret;
    [FoldoutGroup("Sentry")] public GameObject Smoke50HP;
    [FoldoutGroup("Sentry")] public GameObject Smoke25HP;
    [FoldoutGroup("Sentry")] public GameObject botCorpse;    
    [FoldoutGroup("Sentry")] public DynamicObjectPivot pivotObject;
    [FoldoutGroup("Sentry")] public AimConstraint aimConstraint;

    public float TimeToInitialize = 2f;
    public int sentryAmmo = 200;
    public int maxSentryAmmo = 200;
    public float maxDistance = 50f;
    public bool manualControl = false;

    public static Fortification_GhostSentry Instance;
    private float _timerInitialize = 2f;
    private bool _hasInitialized = false;

    private void Start()
    {
        _timerInitialize = TimeToInitialize;
        currentTarget = Hypatios.Enemy.FindEnemyEntity(Stats.MainAlliance);
        Instance = this;
    }


    public override void OnDestroy()
    {
        Instance = null;
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
                aimConstraint.enabled = true;
            }
        }

        if (Stats.CurrentHitpoint < 0)
        {
            if (Stats.IsDead == false) Die();
            Stats.IsDead = true;
            Stats.CurrentHitpoint = 0;
        }

        if (Mathf.RoundToInt(Time.time) % 5 == 0)
            ScanForEnemies(maxDistance: maxDistance);

        if (Time.timeScale == 0) return;
        if (_hasInitialized == false) return;
        if (isAIEnabled == false) return;


        CheckEnableTurret();
        RunAI();
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

    private void RotateToTarget()
    {
        if (currentTarget == null)
            return;

        if (manualControl == true) return;

        Vector3 posTarget = currentTarget.transform.position;
        v_target_Turret.LookAt(posTarget);
    }

    public void OverrideTarget(Vector3 target)
    {
        v_target_Turret.LookAt(target);
    }

    public void FireSentry()
    {
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


