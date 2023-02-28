using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.Utility;
using Sirenix.OdinInspector;

public class BossOrphanimEnemy : EnemyScript
{

    public enum Stage
    {
        Idle,
        Battle,
        Death
    }

    [System.Serializable]
    public class Ring
    {
        public string ringName = "Ring 1";
        public List<ModularLaserWeapon> allModularLasers = new List<ModularLaserWeapon>();
        public List<LookAtPlayer> lookAtPlayers = new List<LookAtPlayer>();
        public AutoMoveAndRotate autoMoveAndRotate;
        public GameObject lockParticle;
        public bool isLockMode = false;

        public void Lock()
        {
            autoMoveAndRotate.enabled = false;
            foreach (var lookScript in lookAtPlayers)
                lookScript.enabled = true;

            lockParticle.gameObject.SetActive(true);
            foreach (var laser in allModularLasers)
            {
                laser.LaserPerAttack = 0.25f;
                laser.VariableCooldown = 0.1f;
            }

            isLockMode = true;
        }

        public void Unlock()
        {
            autoMoveAndRotate.enabled = true;
            foreach (var lookScript in lookAtPlayers)
                lookScript.enabled = false;

            foreach (var laser in allModularLasers)
            {
                laser.LaserPerAttack = 0.1f;
                laser.VariableCooldown = 0f;
            }

            isLockMode = false;
        }

        public void DisableFirst()
        {

            foreach (var laser in allModularLasers)
            {
                laser.enabled = false;
            }
        }

        public void EnableFirst()
        {

            foreach (var laser in allModularLasers)
            {
                laser.enabled = true;
            }
        }
    }


    public Chamber_Wired_Heaven chamberScript;
    public RandomSpawnArea patrolRegion;
    public GameObject corpseOphanim;
    public UnityEvent OnDieEvent;
    public List<Ring> allRings = new List<Ring>();
    public Stage currentStage = Stage.Idle;
    public float RingLockTimer = 10f;
    public float distanceToTarget = 10f;
    public float flySpeed = 6f;
    public float rotateSpeed = 6f;
    public AudioSource audio_Lock;

    private float _timerPatrol = 1f;
    private float _timerRingLocking = 8f;
    private Vector3 _currentPatrolPosition = Vector3.zero;

    private void Start()
    {
        currentTarget = Hypatios.Player;
        _currentPatrolPosition = patrolRegion.GetAnyPositionInsideBox();

        foreach(var ring in allRings)
        {
            ring.DisableFirst();
        }
    }

    private void Update()
    {
        if (currentTarget == null)
        {
            currentTarget = Hypatios.Player;
        }

        if (isAIEnabled)
        {
            if (currentStage == Stage.Battle)
            {
                HandleAI();
                HandleLocking();
            }
        }
    }

    #region AI System

    private void HandleLocking()
    {
        if (_timerRingLocking > 0)
        {
            _timerRingLocking -= Time.deltaTime;
            return;
        }
        bool shouldUnlock = false;
        Ring ringToUnlocked = null ;
        _timerRingLocking = RingLockTimer;
        foreach (var ring in allRings)
        {
            if (ring.isLockMode == true)
            {
                shouldUnlock = true;
                ringToUnlocked = ring;
            }
        }

        if (shouldUnlock)
        {
            ringToUnlocked.Unlock();
            return;
        }

        float chance = Random.Range(0f, 1f);
        if (chance > 0.5f)
            return;

        Ring ringToLock = allRings[Random.Range(0, allRings.Count)];
        ringToLock.Lock();
        _timerRingLocking -= 1.5f;
        audio_Lock.Play();

    }

    private void HandleAI()
    {
        float dist = Vector3.Distance(transform.position, _currentPatrolPosition);
        float distPlayer = Vector3.Distance(transform.position, Hypatios.Player.transform.position);

        if (dist < distanceToTarget)
        {
            _currentPatrolPosition = patrolRegion.GetAnyPositionInsideBox();
        }

        var step = flySpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _currentPatrolPosition, step);

        {
            Vector3 dir = _currentPatrolPosition - transform.position;
            dir.y = 0;
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * rotateSpeed);
        }
    }

    #endregion

    #region Choice Decisions

    public void ChangeStage(Stage _stage)
    {
        if (currentStage == Stage.Idle && _stage != Stage.Idle)
        {
            InitiateBattle();
        }

        currentStage = _stage;

    }

    private void InitiateBattle()
    {
        foreach (var ring in allRings)
            ring.EnableFirst();

        chamberScript.InitiateBattle();
    }

    #endregion

    public override void Die()
    {
        if (Stats.IsDead == false)
        {
            OnDied?.Invoke();
         
        }
        Stats.IsDead = true;
        OnDieEvent?.Invoke();
        corpseOphanim.gameObject.SetActive(true);
        corpseOphanim.transform.position = transform.position;
        corpseOphanim.transform.rotation = transform.rotation;
        Destroy(gameObject);
    }

    public override void Attacked(DamageToken token)
    {
        float damageProcessed = token.damage;

        if (currentStage == Stage.Idle)
        {
            ChangeStage(Stage.Battle);
        }

        Stats.CurrentHitpoint -= damageProcessed;
        _lastDamageToken = token;


        if (Stats.CurrentHitpoint > 0f)
            DamageOutputterUI.instance.DisplayText(damageProcessed);
        else
        {
            Die();
        }

        base.Attacked(token);
    }

}