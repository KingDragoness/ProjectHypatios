using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using Animancer;

// Terminology:
// SE = Survive-Engage: two goals on the end spectrum
/// <summary>
/// 
/// </summary>
public class MobiusGuardEnemy : EnemyScript
{

    private class BoneTransform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

    }

    public bool IsAiming = false;
    public bool IsBrainEnabled = false;
    public bool DEBUG_ShowAIBehaviour = false;

    [FoldoutGroup("References")] public Animator animator;
    [FoldoutGroup("References")] public BipedIK bipedIk;
    [FoldoutGroup("References")] public NavMeshAgent agent;
    [FoldoutGroup("References")] public Rigidbody mainRagdollRigidbody;
    [FoldoutGroup("References")] public Rigidbody[] _ragdollRigidbodies;
    [FoldoutGroup("References")] public GameObject weaponModel;
    [FoldoutGroup("References")] public AnimancerPlayer AnimatorPlayer;
    [FoldoutGroup("References")] public Transform baitTarget; //because offset entity bounding box problem

    [FoldoutGroup("Decision Makings")] [ChildGameObjectsOnly(IncludeSelf = false)] public MobiusAIBehaviour startingBehaviour;

    [FoldoutGroup("Decision Makings")] 
    [ReadOnly] [Tooltip("When in the negative spectrum, prioritize survival (escape, flee, take cover). When in positive, prioritize killing the player.")] 
    [Range(-100,100)] public float survivalEngageLevel = 0;

    [FoldoutGroup("Decision Makings")]
    [ReadOnly]
    [Tooltip("Randomly generated. Per guard has different level of confidence/braveness.")]
    [Range(-100,100)]
    public int confidenceLevel = 0;

    [FoldoutGroup("Decision Makings")]
    [Tooltip("Timer to evaluate every decision. 0.6 means it will make decision every 0.6 seconds.")]
    public float EvaluateTimer = 0.6f;

    [FoldoutGroup("Decision Makings")] [ReadOnly] public MobiusAIBehaviour currentBehaviour;
    [FoldoutGroup("Decision Makings")] [ReadOnly] public List<MobiusAIBehaviour> currentAvailableNextBehaviour = new List<MobiusAIBehaviour>(); //If ragdoll mode, then the only next behaviour is wakeup.
    [FoldoutGroup("Decision Makings")] [ReadOnly] public Queue<MobiusAIBehaviour> currentPlan;
    [FoldoutGroup("Decision Makings")] public List<MobiusAIBehaviour> allBehaviours = new List<MobiusAIBehaviour>();

    [FoldoutGroup("Audios")] public AudioSource audio_Dead;

    private static List<MobiusGuardEnemy> _allActiveGuards = new List<MobiusGuardEnemy>();
    private MobiusAIBehaviour previousBehaviour;

    #region Parameter - Ragdolls

    [FoldoutGroup("Ragdoll")] public string _standupClipName = "";
    [FoldoutGroup("Ragdoll")] public string _standupFaceDownClipName = "";
    [FoldoutGroup("Ragdoll")] public int layerAimingIndex = 1;
    [FoldoutGroup("Ragdoll")] public float TimeToResetBones = 1f;
    [FoldoutGroup("Ragdoll")] public Transform Debug_RagdollForceorigin;

    private Transform _hipsBone;
    private BoneTransform[] _faceUpStandupBoneTransforms;
    private BoneTransform[] _faceDownStandupBoneTransforms;
    private BoneTransform[] _ragdollBoneTransforms;
    private Transform[] _bones;

    private bool isRagdoll = false;
    private bool isResettingBones = false;
    private bool _isFacingUp = false;
    private float _elapsedTimeBoneReset = 0f;
    private float _currentAimingValue = 1f;
    #endregion

    public bool IsMoving => agent.velocity.magnitude > 0.1f;
    public static List<MobiusGuardEnemy> AllActiveGuards { get => _allActiveGuards; }


    public override void Awake()
    {
        base.Awake();
        #region Ragdolls - Awake
        {
            _hipsBone = animator.GetBoneTransform(HumanBodyBones.Hips);
            _bones = _hipsBone.GetComponentsInChildren<Transform>();
            _faceUpStandupBoneTransforms = new BoneTransform[_bones.Length];
            _faceDownStandupBoneTransforms = new BoneTransform[_bones.Length];
            _ragdollBoneTransforms = new BoneTransform[_bones.Length];

            for (int i = 0; i < _bones.Length; i++)
            {
                _faceUpStandupBoneTransforms[i] = new BoneTransform();
                _faceDownStandupBoneTransforms[i] = new BoneTransform();
                _ragdollBoneTransforms[i] = new BoneTransform();
            }

            PopulateBoneTransformCopyFromClip(_standupClipName, _faceUpStandupBoneTransforms);
            PopulateBoneTransformCopyFromClip(_standupFaceDownClipName, _faceDownStandupBoneTransforms);

            DisableRagdoll();
        }
        #endregion


    }

    private void OnEnable()
    {
        if (Stats.IsDead)
            return;

        _allActiveGuards.RemoveAll(x => x == null);
        _allActiveGuards.Add(this);
    }

    private void OnDisable()
    {
        _allActiveGuards.Remove(this);
    }

    private void Start()
    {
        confidenceLevel = Random.Range(-100, 100);
        ScanForEnemies();
        ChangeAIBehaviour(startingBehaviour);
    }

    public override string Debug_AdditionalString()
    {
        string s1 = "\n";

        s1 += $"Current behaviour: {currentBehaviour.GetType()} ({survivalEngageLevel})\n";

        foreach (var mobiusAI in allBehaviours)
        {
            s1 += $"{mobiusAI.GetType()} [{mobiusAI.currentPriorityLevel}]\n";
        }

        s1 += "\n";
        s1 += $"[TOTAL: {_allActiveGuards.Count}]\n";

        return s1;
    }


    #region Setup

    [HorizontalGroup("xDebug", Order = 0)]
    [Button("Setup AIs", ButtonHeight = 30)]
    public void SetupAIBehavioursDetails()
    {
        var AIbehaviours = GetComponentsInChildren<MobiusAIBehaviour>();
        allBehaviours = AIbehaviours.ToList();
        foreach (var behaviour in allBehaviours)
        {
            behaviour.gameObject.name = behaviour.GetType().Name;
            behaviour.mobiusGuardScript = this;
        }
    }

    public MobiusAIBehaviour GetAIBehaviour<T>()
    {
        foreach(var behaviour in allBehaviours)
        {
            if (behaviour.GetType() == typeof(T))
            {
                return behaviour;
            }
        }
        return null;
    }

    #endregion

    #region Ragdolls

    private void PopulateBoneTransform(BoneTransform[] boneTransforms)
    {
        for (int i = 0; i < _bones.Length; i++)
        {
            boneTransforms[i].Position = _bones[i].localPosition;
            boneTransforms[i].Rotation = _bones[i].localRotation;
        }
    }


    private void PopulateBoneTransformCopyFromClip(string clipName, BoneTransform[] boneTransforms)
    {
        Vector3 posBeforeSample = transform.position;
        Quaternion rotBeforeSample = transform.rotation;

        foreach(var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                clip.SampleAnimation(gameObject, 0);
                PopulateBoneTransform(boneTransforms);
                break;
            }
        }

        transform.position = posBeforeSample;
        transform.rotation = rotBeforeSample;
    }


    [FoldoutGroup("Debug")]
    [HorizontalGroup("Debug/V1")]
    [Button("Collect rigidbodies")]
    public void CollectRigidbodies()
    {
        _ragdollRigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();
    }


    [FoldoutGroup("Debug")]
    [Button("Trigger Ragdoll")]

    public void TriggerRagdoll(Vector3 force)
    {
        EnableRagdoll();
        IsAiming = false;
        Vector3 hitpoint = Debug_RagdollForceorigin.position;
        Rigidbody hitRigidbody = FindRigidbodyNearest(hitpoint);

        hitRigidbody.AddForceAtPosition(force, hitpoint, ForceMode.Impulse);

    }

    private Rigidbody FindRigidbodyNearest(Vector3 hitPoint)
    {
        Rigidbody closestRigidbody = null;
        float closestDistance = 0;

        foreach(var rb in _ragdollRigidbodies)
        {
            float dist = Vector3.Distance(rb.position, hitPoint);

            if (closestRigidbody == null || dist < closestDistance)
            {
                closestDistance = dist;
                closestRigidbody = rb;
            }
        }

        return closestRigidbody;
    }

    [HorizontalGroup("Debug/V1")]
    [Button("Enable ragdoll")]
    public void EnableRagdoll()
    {
        Set_DisableAiming();
        ChangeAIBehaviour_Type<MAIB_Ragdoll>();
        agent.enabled = false;
        animator.enabled = false;

        foreach (var rb in _ragdollRigidbodies)
        {
            rb.isKinematic = false;
        }

        isRagdoll = true;
    }

    public void DisableRagdoll()
    {
        agent.enabled = true;
        animator.enabled = true;

        foreach (var rb in _ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }

        isRagdoll = false;
    }

    private void AlignPositionToHips()
    {
        Vector3 originalHipPosition = _hipsBone.position;
        transform.position = _hipsBone.position;

        Vector3 positionOffset = GetStandUpBoneTransforms()[0].Position;
        positionOffset.y = 0;
        //positionOffset = transform.rotation * positionOffset;
        //transform.position = positionOffset;

        //temporary
        if (Physics.Raycast(transform.position,Vector3.down, out RaycastHit hit))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
        _hipsBone.position = originalHipPosition;
    }

    private void AlignRotationToHips()
    {
        Vector3 originalHipPosition = _hipsBone.position;
        Quaternion originalHipRotation = _hipsBone.rotation;

        Vector3 desiredDirection = _hipsBone.transform.up;

        if (_isFacingUp)
        {
            desiredDirection *= -1;
        }

        desiredDirection.y = 0f;
        desiredDirection.Normalize();

        Quaternion fromToRotation = Quaternion.FromToRotation(transform.forward, desiredDirection);
        transform.rotation *= fromToRotation;

        _hipsBone.position = originalHipPosition;
        _hipsBone.rotation = originalHipRotation;
    }

    private string GetStandUpStateName()
    {
        return _isFacingUp ? _standupClipName : _standupFaceDownClipName;
    }



    private BoneTransform[] GetStandUpBoneTransforms()
    {
        return _isFacingUp ? _faceUpStandupBoneTransforms : _faceDownStandupBoneTransforms;

    }

    [HorizontalGroup("Debug/V1")]
    [Button("Wakeup")]
    public void Wakeup()
    {
        _isFacingUp = _hipsBone.transform.forward.y > 0;
        ChangeAIBehaviour_Type<MAIB_Wakeup>();

        AlignRotationToHips();
        AlignPositionToHips();
        PopulateBoneTransform(_ragdollBoneTransforms);

        foreach (var rb in _ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }
        isResettingBones = true;
        _elapsedTimeBoneReset = 0f;
    }

    private void Mode_ResettingBonesWakeUp()
    {
        _elapsedTimeBoneReset += Time.deltaTime;
        float elapsedPercentage = _elapsedTimeBoneReset / TimeToResetBones;

        BoneTransform[] standupBoneTransform = GetStandUpBoneTransforms();
        for(int x = 0; x < _bones.Length; x++)
        {
            _bones[x].localPosition = Vector3.Lerp(
                _ragdollBoneTransforms[x].Position,
                standupBoneTransform[x].Position,
                elapsedPercentage);

            _bones[x].localRotation = Quaternion.Lerp(
               _ragdollBoneTransforms[x].Rotation,
               standupBoneTransform[x].Rotation,
               elapsedPercentage);
        }

        if(elapsedPercentage >= 1)
        {
            DisableRagdoll();
            animator.Play(GetStandUpStateName(), 0, 0);
            isResettingBones = false;
        }

    }

    #endregion

    #region Behaviour Sets

    public void Set_EnableAiming()
    {

        if (IsAiming == false) IsAiming = true;

        bipedIk.enabled = true;
        bipedIk.solvers.aim.IKPositionWeight = 1f;
        baitTarget.transform.position = currentTarget.OffsetedBoundWorldPosition;
        bipedIk.solvers.aim.target = baitTarget.transform;
    }

    public void ShowRifleModel()
    {
        weaponModel.gameObject.SetActive(true);
    }

    public void HideRifleModel()
    {
        weaponModel.gameObject.SetActive(false);
    }

    public void OverrideAimingTarget()
    {
        baitTarget.transform.position = currentTarget.OffsetedBoundWorldPosition;
        bipedIk.solvers.aim.target = baitTarget.transform;

    }

    public void Set_DisableAiming()
    {

        if (IsAiming == true) IsAiming = false;

        bipedIk.enabled = false;
        bipedIk.solvers.aim.IKPositionWeight = 0f;
    }

    public void Set_StopMoving()
    {

        if (IsMoving)
        {
            agent.Stop();
        }

        animator.SetFloat("Speed", 0f);
    }

    public void Set_StartMoving(float moveSpeed, float animSpeed)
    {
        if (agent.isStopped) agent.Resume();
        if (IsMoving)
        {
            animator.SetFloat("Speed", animSpeed);
        }
        else animator.SetFloat("Speed", Mathf.MoveTowards(animator.GetFloat("Speed"), 0f, Time.deltaTime));

        agent.speed = moveSpeed;

    }

    #endregion

    #region Updates
    private float _timerBehaviour = 1f;

    private bool lastAIState = false;

    private void Update()
    {
        if (Stats.IsDead)
        {
            return;
        }
        UpdateRagdolls();

        if (isAIEnabled)
        {
            if (Hypatios.TimeTick % 10 == 0)
                ScanForEnemies();

            if (currentTarget != null)
            {
                UpdateAIState();

                if (IsBrainEnabled) RunBrainAI();
            }
        }

        if (lastAIState != isAIEnabled)
        {
            if (isAIEnabled == false)
            {
                currentBehaviour.OnBehaviourDisable();
            }
            else
            {
                currentBehaviour.OnBehaviourActive();
            }
        }

        lastAIState = isAIEnabled;
    }

    private void FixedUpdate()
    {
        if (isRagdoll && isResettingBones == false)
        {
            var originalPos = _hipsBone.transform.position;
            transform.position = _hipsBone.transform.position;
            _hipsBone.transform.position = originalPos;
        }
    }

    private void LateUpdate()
    {
        if (isRagdoll)
        {

            //transform.position = _hipsBone.position;
        }
    }


    [FoldoutGroup("Decision Makings")][Button("Change Behaviour")]
    public void ChangeAIBehaviour(MobiusAIBehaviour _behaviour)
    {
        if (currentBehaviour != null) currentBehaviour.OnBehaviourDisable();
        previousBehaviour = currentBehaviour;
        currentBehaviour = _behaviour;
        currentBehaviour.OnBehaviourActive();
    }

    public void ChangeAIBehaviour_Type<T>()
    {
        if (currentBehaviour != null) currentBehaviour.OnBehaviourDisable();
        previousBehaviour = currentBehaviour;
        currentBehaviour = allBehaviours.Find(x => x.GetType() == typeof(T));
        currentBehaviour.OnBehaviourActive();
    }


    private void UpdateAIState()
    {
        if (currentBehaviour != null)
        {
            currentBehaviour.Execute();
        }

        EvaluateAvailableBehaviours();

    }

    //running brain AI to make decisions
    private void RunBrainAI()
    {

        AI_Detection();

        _timerBehaviour -= Time.deltaTime;

        if (_timerBehaviour < 0f)
        {
            _timerBehaviour = EvaluateTimer;
        }
        else return;

        //evaluating decisions
        int ix = 0;
        int highestValue = int.MinValue;

        for(int x = 0; x < currentAvailableNextBehaviour.Count; x++)
        {
            var AI_package = currentAvailableNextBehaviour[x];
            
            if (highestValue < AI_package.CalculatePriority())
            {
                ix = x;
                highestValue = AI_package.CalculatePriority();
            }
        }

        if (currentAvailableNextBehaviour.Count > ix)
        {
            var behaviour = currentAvailableNextBehaviour[ix];
            if (behaviour != currentBehaviour)
            {
                ChangeAIBehaviour(behaviour);
            }
        }
    }

    private void EvaluateAvailableBehaviours()
    {
        currentAvailableNextBehaviour.Clear();

        foreach(var behaviour in allBehaviours)
        {

            if (currentBehaviour.cannotBeSelectedByDecision && behaviour.isExclusive == false)
                continue;

            if (behaviour.isExclusive)
            {
                var b1 = behaviour.allowPreviousBehaviours.Find(x => x == currentBehaviour);

                if (b1 != null)
                {
                    currentAvailableNextBehaviour.Add(b1);
                }
            }
            else
            {
                currentAvailableNextBehaviour.Add(behaviour);
            }

        }
    }

    private void UpdateRagdolls()
    {
        if (isResettingBones)
        {
            Mode_ResettingBonesWakeUp();
        }


        if (IsAiming)
        {
            _currentAimingValue = Mathf.MoveTowards(_currentAimingValue, 1f, Time.deltaTime);
            animator.SetLayerWeight(layerAimingIndex, _currentAimingValue);
        }
        else
        {
            _currentAimingValue = Mathf.MoveTowards(_currentAimingValue, 0f, Time.deltaTime);
            animator.SetLayerWeight(layerAimingIndex, _currentAimingValue);
        }
    }


    #endregion

    #region Enemy base class

    public override void Burn()
    {
        if (Stats.IsDead) return;

        base.Burn();
    }

    public override void Poison()
    {
        if (Stats.IsDead) return;

        base.Poison();
    }

    public override void Paralysis()
    {
        if (Stats.IsDead) return;

        base.Paralysis();
    }

    private float _lastTimeGotExplosion = 0f;

    public override void Attacked(DamageToken token)
    {


        if (_lastDamageToken != null)
        {
            //if (time1 <= _lastDamageToken.timeAttack) return;
        }


        if (token.damageType == DamageToken.DamageType.Explosion)
        {

            //mobius guard received explosion damage within 0.5 second, ignore damage
            if (Time.time - _lastTimeGotExplosion < 0.5f)
            {
                return;
            }

            _lastTimeGotExplosion = Time.time;

            if (token.damage > 30f)
            {
                float repulsionPower = Mathf.Clamp(token.damage, 30f, 1000f) * 0.1f;
                TriggerRagdoll(-transform.forward * 30f * repulsionPower);
            }
        }



        _lastDamageToken = token;
        Stats.CurrentHitpoint -= token.damage;
        if (!Stats.IsDead && token.origin == DamageToken.DamageOrigin.Player)
            DamageOutputterUI.instance.DisplayText(token.damage);

        if (Stats.CurrentHitpoint <= 0f)
        {
            Die();
        }

        base.Attacked(token);
    }

    public override void Die()
    {
        if (Stats.IsDead) return;
        audio_Dead?.Play();
        _allActiveGuards.Remove(this);
        TriggerRagdoll(-transform.forward * 160f);
        Stats.IsDead = true;
        OnDied?.Invoke();

    }
    #endregion
}
