using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using UnityEngine.Events;



[RequireComponent(typeof(FW_Targetable))]
public class Enemy_FW_SentryGun : EnemyScript
{
    public FW_Targetable myUnit;

    [FoldoutGroup("Base")] public UnityEvent OnBotKilled;
    [FoldoutGroup("Base")] public UnityEvent OnPlayerKill;
    [FoldoutGroup("Base")] public UnityEvent On50HP;
    [FoldoutGroup("Base")] public UnityEvent On25HP;
    [FoldoutGroup("Base")] public GameObject botCorpse;
    [FoldoutGroup("Sensors")] public FW_AI_SensorEnemy sensor;
    [ReadOnly] [FoldoutGroup("Weapon")] public Transform target;
    [FoldoutGroup("Weapon")] public Transform v_target_Turret;
    [FoldoutGroup("Weapon")] public Transform[] allTurretOrigin;
    [FoldoutGroup("Weapon")] public LayerMask weapon_WeaponLayer;
    [FoldoutGroup("Weapon")] public float weapon_Damage = 20;
    [FoldoutGroup("Weapon")] public GameObject damageSpark;
    [FoldoutGroup("Weapon")] public AudioSource audio_Fire;

    public delegate void OnAITick();
    public delegate void OnAIFixedTick();
    public event OnAITick onAITick;
    public event OnAIFixedTick onAIFixedTick;
    private float _tickTimer = 0.1f;
    private int _tick = 0;
    private const float TICK_MAX = 0.1f;

    private Chamber_Level7 _chamberScript;


    public void Start()
    {
        _chamberScript = Chamber_Level7.instance;
        _chamberScript.RegisterUnit(myUnit);
    }
    public Transform GetCurrentTarget()
    {
        return target;
    }

    private void Update()
    {
        UpdateTick();
        UpdateVisuals();
    }

    private void UpdateTick()
    {
        if (Time.realtimeSinceStartup < 2f) return;
        if (_chamberScript.currentStage != Chamber_Level7.Stage.Ongoing) return;

        {
            _tickTimer -= Time.deltaTime;

            if (_tickTimer < 0)
            {
                _tick++;
                RefreshAI();
                onAITick?.Invoke();
                _tickTimer = TICK_MAX;
            }
        }
    }

    private int _timeSinceEnemyLastSeen = 80; //1 tick = 0.1s
    private bool _isAttack = false;

    private void UpdateVisuals()
    {
        RotateToTarget();

        if (_isAttack)
        {
            if (audio_Fire.isPlaying == false)
                audio_Fire.Play();
        }
        else
        {
            if (audio_Fire.isPlaying == true)
                audio_Fire.Stop();
        }
    }

    private void RefreshAI()
    {
        var botsInSight = sensor.GetBotsInSight(myUnit.AllianceEnemy()).ToList();
        bool anyBotsInSight = false; if (botsInSight.Count > 0) anyBotsInSight = true;
        bool isTargetBlocked = false;
        bool isAttacking = false;

        if (target != null)
        {
            isTargetBlocked = sensor.IsTargetBlocked(target);
        }


        if (anyBotsInSight)
        {
            _timeSinceEnemyLastSeen = 50;
            target = botsInSight[0].transform;

            if (isTargetBlocked == false)
            {
                Attack();
                isAttacking = true;
            }

        }
        else
        {
            _timeSinceEnemyLastSeen--;
        }

        _isAttack = isAttacking;
    }

    #region Weaponary

    private void RotateToTarget()
    {
        if (target == null)
            return;

        Vector3 posTarget = GetCurrentTarget().position;
        v_target_Turret.LookAt(posTarget);
    }

    private void Attack()
    {
        if (target == null)
            return;

        int index = 0;

        foreach(var origin in allTurretOrigin)
        {
            if (origin == null) continue;
            index++;
            
            if (_tick % (index+1) == 0)
                FireGun(origin);
        }
    }

    private void FireGun(Transform origin)
    {
        origin.gameObject.SetActive(true);
        Vector3 targetPos = GetCurrentTarget().position; targetPos.y += 0.5f;
        { //inaccuracy
            targetPos.x += Random.Range(-1, 1f);
            targetPos.z += Random.Range(-1, 1f);
        }

        Vector3 dir = targetPos - origin.transform.position;


        RaycastHit hit;

        if (Physics.Raycast(origin.transform.position, dir, out hit, 100f, weapon_WeaponLayer))
        {
            var token = new DamageToken();
            token.damage = weapon_Damage;
            if (Stats.MainAlliance != Alliance.Player) token.origin = DamageToken.DamageOrigin.Enemy; else token.origin = DamageToken.DamageOrigin.Ally;
            token.originEnemy = this;
            UniversalDamage.TryDamage(token, hit.transform, transform);


            SparkFX(hit.point, hit.normal);
            Debug.DrawRay(origin.transform.position, dir * hit.distance, Color.blue);
        }
    }
    private void SparkFX(Vector3 pos, Vector3 normal)
    {
        GameObject bulletSpark_ = Hypatios.ObjectPool.SummonObject(damageSpark, 20, 30);
        if (bulletSpark_ != null)
        {
            bulletSpark_.transform.position = pos;
            bulletSpark_.transform.rotation = Quaternion.LookRotation(normal);
            bulletSpark_.DisableObjectTimer(1f);
        }
    }



    #endregion

    private bool _isTriggered50HP = false;
    private bool _isTriggered25HP = false;

    public override void Attacked(DamageToken token)
    {
        Stats.CurrentHitpoint -= token.damage;
        if (token.origin == DamageToken.DamageOrigin.Player) DamageOutputterUI.instance.DisplayText(token.damage);

        float percentage = Stats.CurrentHitpoint / Stats.MaxHitpoint.Value;

        float chance = Random.Range(0f, 1f);

        if (chance > 0.5f)
            soundManagerScript.instance.Play3D("hitmetal.0", transform.position);
        else
            soundManagerScript.instance.Play3D("hitmetal.1", transform.position);

        if (percentage < 0.5f && !_isTriggered50HP)
        {
            On50HP?.Invoke();
            _isTriggered50HP = true;
        }

        if (percentage < 0.25f && !_isTriggered25HP)
        {
            On25HP?.Invoke();
            _isTriggered25HP = true;
        }


        if (Stats.CurrentHitpoint < 0)
        {
            Die();

            if (token.origin == DamageToken.DamageOrigin.Player)
            {
                OnPlayerKill?.Invoke();
            }
        }

        base.Attacked(token);

    }

    private void Die()
    {
        _chamberScript.DeregisterUnit(myUnit);

        if (botCorpse)
        {
            var corpse1 = Instantiate(botCorpse, transform.position, transform.rotation);
            corpse1.gameObject.SetActive(true);
        }
        Destroy(gameObject);
    }

}
