using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Sirenix.OdinInspector;

public class MobiusExplorerEnemy : EnemyScript
{

    [FoldoutGroup("AI")] public List<RandomSpawnArea> allRandomPatrols = new List<RandomSpawnArea>();
    [FoldoutGroup("AI")] public float CooldownUpdateAI = 0.2f;
    [FoldoutGroup("References")] private Rigidbody rb;
    [FoldoutGroup("Weapon")] [Range(0f,1f)] public float chanceFiring = 0.03f;
    [FoldoutGroup("Weapon")] public float CooldownFireWeapon = 15f;
    [FoldoutGroup("Weapon")] public GameObject missile_kThanid;
    [FoldoutGroup("Weapon")] public int PerTickFire = 10;
    [FoldoutGroup("Audios")] public AudioSource audio_Fire;
    public float distanceToTarget = 10f;
    public float distanceTooFar = 20f;
    public float flySpeed = 6f;
    public float rotateSpeed = 6f;

    private float _timerUpdate = 0.1f;
    private float _lastTimeFireWeapon = 0;
    private int _tick = 0;
    private Vector3 _currentPatrolPosition;
    private RandomSpawnArea _currentPatrol;

    void Start()
    {
        currentTarget = Hypatios.Enemy.FindEnemyEntity(Stats.MainAlliance);
    }

    public override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }

    private RandomSpawnArea GetPatrol()
    {
        float targetDist = 999999f;
        RandomSpawnArea result = null;
        foreach (var area in allRandomPatrols)
        {
            Vector3 posPlayer = Hypatios.Player.transform.position;
            posPlayer.y = area.transform.position.y;
            float dist1 = Vector3.Distance(posPlayer, area.transform.position);

            if (dist1 < targetDist)
            {
                targetDist = dist1;
                result = area;
            }
        }

        return result;
    }

    private bool IsPatrolInsideArea(Vector3 pos)
    {
        bool inside = false;
        foreach (var area in allRandomPatrols)
        {
            if (area.IsInsideOcclusionBox(pos))
                return true;
        }

        return inside;
    }

    private void Update()
    {
        if (isAIEnabled == false) return;
        if (Time.timeScale <= 0) return;

        if (Mathf.RoundToInt(Time.time) % 2 == 0)
            ScanForEnemies();


        if (currentTarget != null) ProcessAI();
        else ScanForEnemies();
    }

    private void ProcessAI()
    {
        _lastTimeFireWeapon += Time.deltaTime;

        if (_timerUpdate > 0f)
        {
            _timerUpdate -= Time.deltaTime;
            return;
        }

        _timerUpdate = CooldownUpdateAI;
        _tick++;

        AI_Detection();
        UpdateMovementTarget();

        if (hasSeenPlayer == false) return;

        UpdateAttack();
    }

    private void FixedUpdate()
    {


        float dist = Vector3.Distance(transform.position, _currentPatrolPosition);

        var step = flySpeed * Time.deltaTime;

        if (dist > 0.5f) //look quaternion is zero
            transform.position = Vector3.MoveTowards(transform.position, _currentPatrolPosition, step);

        {
            Vector3 dir = _currentPatrolPosition - transform.position;
            dir.y = 0;
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * rotateSpeed);
        }
    }

    private void UpdateMovementTarget()
    {
        if (_currentPatrol != null)
        {
            float random = Random.Range(0f, 1f);

            float dist = Vector3.Distance(transform.position, _currentPatrolPosition);

            if (IsPatrolInsideArea(_currentPatrolPosition) == false)
            {
                _currentPatrolPosition = _currentPatrol.GetAnyPositionInsideBox();
            }
            else if (dist < distanceToTarget && random < 0.1f)
            {
                _currentPatrolPosition = _currentPatrol.GetAnyPositionInsideBox();

            }
        }

        _currentPatrol = GetPatrol();
    }

    private void UpdateAttack()
    {
        if (_tick % PerTickFire != 0) return; //every 2s
        if (_lastTimeFireWeapon < CooldownFireWeapon) return;

        float random = Random.Range(0f, 1f);

        if (random < chanceFiring)
        {
            LaunchAttack();
        }
    }

    //fire launch
    [FoldoutGroup("Debug")] [Button("Fire missile")]
    public void LaunchAttack()
    {
        _lastTimeFireWeapon = 0f;
        Vector3 dir = eyeLocation.forward;
        if (currentTarget != null)
        {
            dir = currentTarget.OffsetedBoundWorldPosition - eyeLocation.position;
        }
        audio_Fire?.Play();
        GameObject prefabMissile = Instantiate(missile_kThanid.gameObject, eyeLocation.position, Quaternion.LookRotation(dir, Vector3.up));
        prefabMissile.gameObject.SetActive(true);
        var missile = prefabMissile.GetComponent<MissileChameleon>();
        missile.OverrideTarget(currentTarget, Stats.MainAlliance);
    }

    public override void Attacked(DamageToken token)
    {
        _lastDamageToken = token;

        hasSeenPlayer = true;
        Stats.CurrentHitpoint -= token.damage;


        if (Stats.IsDead == false)
            if (token.origin == DamageToken.DamageOrigin.Player | token.origin == DamageToken.DamageOrigin.Ally) DamageOutputterUI.instance.DisplayText(token.damage);

        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }
    }

    public override void Die()
    {
        Destroy(gameObject);
        OnSelfKilled?.Invoke();
    }




}
