using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using RootMotion.FinalIK;
using Animancer;

public class MechHeavenblazerEnemy : EnemyScript
{

    public enum Stage
    {
        Stage0_Intro,
        Stage1_Fight,
        Stage2_Dust,
        Stage3_Ascend,
        Stage4_LastMessage
    }

    [System.Serializable]
    public class StageAvailableAbilities
    {
        public Stage stage;
        public List<Stance> decisions = new List<Stance>();

        [System.Serializable]
        public class Stance
        {
            public HB_AIPackage package;
            public int baseWeight = 10;
            public float additionalCooldown = 0f;

            public int GetNetWeight(MechHeavenblazerEnemy _mech)
            {
                int value = baseWeight;
                value += package.GetWeightDecision(_mech);
                return Mathf.Clamp(value, 0, 1000000);
            }
        }
    }

    public Animator mainAnimator;
    [SerializeField] private HB_AIPackage _currentAI;

    [FoldoutGroup("Heavenblazer")] public Stage currentStage = Stage.Stage0_Intro;
    [FoldoutGroup("Heavenblazer")] public List<StageAvailableAbilities> allStageSystems = new List<StageAvailableAbilities>();
    [FoldoutGroup("Heavenblazer")] public bool DEBUG_EnableDecisionMaking = true;

    [FoldoutGroup("References")] public AnimancerPlayer AnimatorPlayer;
    [FoldoutGroup("References")] public RandomSpawnArea PatrolRegion;
    [FoldoutGroup("References")] public LimbIK LeftArmIK;
    [FoldoutGroup("References")] public LimbIK RightArmIK;
    [FoldoutGroup("References")] public GameObject modularTurretGun;
    [FoldoutGroup("References")] public GameObject modularTurretGun1;
    [FoldoutGroup("References")] public ModularMissileTurretLauncher missileLaunch_Mortar;
    [FoldoutGroup("References")] public ModularMissileTurretLauncher missileLaunch_Antiback;
    [FoldoutGroup("References")] public LineRenderer laser_LineRendr;
    [FoldoutGroup("References")] public Transform laser_Sparks;
    [FoldoutGroup("References")] public Transform laser_Origin;

    [FoldoutGroup("Module Variables")] public Vector3 patrolground_WalkTarget = Vector3.zero;
    [FoldoutGroup("Module Variables")] public bool ik_target_player = false;
    [FoldoutGroup("Module Variables")] public float relativeDistZ_FireBackMissile = 3f;


    private float _refreshChangeStageTime = 0f;

    public HB_AIPackage CurrentAI { get => _currentAI; set => _currentAI = value; }
    public float RefreshChangeStageTime { get => _refreshChangeStageTime; }


    private void Start()
    {
        
    }

    private void Update()
    {
        if (currentTarget == null)
        {
            currentTarget = Hypatios.Player;
        }

        if (isAIEnabled)
        {
            CurrentAI.Run(this);
            if (DEBUG_EnableDecisionMaking) DecisionMaking();
            if (CurrentAI.category == HB_AIPackage.Category.PatrolIdle) FiringRockets();
            if (CurrentAI is HB_Stance_Laser) FireLaser();
            ControlLimbs();
        }
    }

    private void ControlLimbs()
    {
        if (ik_target_player == false)
        {
            if (LeftArmIK.solver.IKPositionWeight > 0)
                LeftArmIK.solver.IKPositionWeight -= Time.deltaTime;
            if (LeftArmIK.solver.IKRotationWeight > 0)
                LeftArmIK.solver.IKRotationWeight -= Time.deltaTime;

            if (RightArmIK.solver.IKPositionWeight > 0)
                RightArmIK.solver.IKPositionWeight -= Time.deltaTime;
            if (RightArmIK.solver.IKRotationWeight > 0)
                RightArmIK.solver.IKRotationWeight -= Time.deltaTime;
        }
        else
        {
            if (LeftArmIK.solver.IKPositionWeight < 1)
                LeftArmIK.solver.IKPositionWeight += Time.deltaTime;
            if (LeftArmIK.solver.IKRotationWeight < 1)
                LeftArmIK.solver.IKRotationWeight += Time.deltaTime;

            if (RightArmIK.solver.IKPositionWeight < 1)
                RightArmIK.solver.IKPositionWeight += Time.deltaTime;
            if (RightArmIK.solver.IKRotationWeight < 1)
                RightArmIK.solver.IKRotationWeight += Time.deltaTime;

        }
    }


    #region Make Decisions

    public StageAvailableAbilities.Stance GetEntry(StageAvailableAbilities decisionStance)
    {
        int output = 0;

        //Getting a random weight value
        var totalWeight = GetTotalWeight(decisionStance.decisions);

        var rndWeightValue = Random.Range(1, totalWeight + 1);

        //Checking where random weight value falls
        var processedWeight = 0;
        int index1 = 0;
        foreach (var entry in decisionStance.decisions)
        {
            processedWeight += entry.GetNetWeight(this);
            if (rndWeightValue <= processedWeight)
            {
                output = index1;
                break;
            }
            index1++;
        }


        return decisionStance.decisions[output];
    }

    public int GetTotalWeight(List<StageAvailableAbilities.Stance> decisions)
    {
        int total = 0;
        foreach (var entry1 in decisions)
        {
            total += entry1.GetNetWeight(this);
        }
        return total;
    }


    private void DecisionMaking()
    {
        StageAvailableAbilities csd = allStageSystems.Find(x => x.stage == currentStage);

        if (csd == null) return;

        if (_refreshChangeStageTime > 0)
        {
            _refreshChangeStageTime -= Time.deltaTime;
            return;
        }

        //Time to decide move
        _refreshChangeStageTime = 2f;
        var entryDecision = GetEntry(csd);
        ChangePackage(entryDecision.package);
        _refreshChangeStageTime += entryDecision.additionalCooldown;
    }

    #endregion

    #region Custom Systems

    public float RocketCooldown = 2f;
    public float LaserDamage = 5f;

    [Range(0f,1f)] public float FailChanceLimit = 0.6f;
    public const float LASER_PER_ATTACK = 0.15f;

    private float _timerFireRocketCooldown = 2f;
    private float _timerLaserPerAttack = 0.15f;
    private int _failTimes = 0;

    public void FiringRockets()
    {
        if (_timerFireRocketCooldown > 0)
        {
            _timerFireRocketCooldown -= Time.deltaTime;
            return;
        }

        _timerFireRocketCooldown = RocketCooldown;

        if (_failTimes < 5)
        {
            float chanceFire = Random.Range(_failTimes * 0.15f, 1f);

            if (chanceFire > FailChanceLimit)
            {
                _failTimes++;
                return;
            }
        }

        bool isBehind = false;      

        Vector3 turretRelative = transform.InverseTransformPoint(currentTarget.transform.position);
        if (turretRelative.z < relativeDistZ_FireBackMissile) isBehind = true;

        if (isBehind)
        {
            missileLaunch_Antiback.Debug_Fire(Random.Range(4, 8));
        }

        _failTimes = 0;
        missileLaunch_Mortar.Debug_Fire(Random.Range(3, 6));
    }



    public void FireLaser()
    {
        if (_timerLaserPerAttack > 0)
        {
            _timerLaserPerAttack -= Time.deltaTime;
            return;
        }

        _timerLaserPerAttack = LASER_PER_ATTACK;

        Vector3 origin = laser_Origin.position;
        RaycastHit hit;
        var damageToken = new DamageToken();
        damageToken.originEnemy = this;
        damageToken.origin = DamageToken.DamageOrigin.Enemy;

        if (Physics.Raycast(origin, laser_Origin.forward, out hit, 1000f, Hypatios.Enemy.baseSolidLayer, QueryTriggerInteraction.Ignore))
        {
            float multiplierDamage1 = 1f;
            float damageDist = (LaserDamage) * multiplierDamage1;
            damageDist = Mathf.Clamp(damageDist, 1, 9999);

            damageToken.damage = damageDist;
            UniversalDamage.TryDamage(damageToken, hit.collider.transform, transform);
        }

    }

    #endregion

    public override void Attacked(DamageToken token)
    {
        float damageProcessed = token.damage;


        Stats.CurrentHitpoint -= damageProcessed;

        if (currentStage == Stage.Stage0_Intro)
        {
            ForceChangeStage(Stage.Stage1_Fight);
        }

        if (Stats.CurrentHitpoint > 0f)
            DamageOutputterUI.instance.DisplayText(damageProcessed);

        base.Attacked(token);
    }

    public override void Die()
    {
        if (Stats.IsDead == false)
        {
            OnDied?.Invoke();
        }
        Stats.IsDead = true;
        Stats.CurrentHitpoint = 0;
    }

    public void ChangePackage(HB_AIPackage newPackage)
    {
        if (newPackage != CurrentAI)
            CurrentAI.NotRun(this);
        else return;

        CurrentAI = newPackage;
        CurrentAI.OnChangedToThis(this);
    }

    public void ForceChangeStage(Stage _newStage)
    {
        currentStage = _newStage;
    }
}
