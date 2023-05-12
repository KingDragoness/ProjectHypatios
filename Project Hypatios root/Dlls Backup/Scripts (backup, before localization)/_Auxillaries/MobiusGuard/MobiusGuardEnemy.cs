using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;

public class MobiusGuardEnemy : MonoBehaviour
{

    private class BoneTransform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

    }



    [SerializeField] private Animator animator;
    [SerializeField] private AimController aimController;
    [SerializeField] private AimIK aimIK;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Rigidbody[] _ragdollRigidbodies;

    #region Parameter - Ragdolls

    [FoldoutGroup("Ragdoll")] public string _standupClipName = "";
    [FoldoutGroup("Ragdoll")] public string _standupFaceDownClipName = "";
    [FoldoutGroup("Ragdoll")] public int layerAimingIndex = 1;
    [FoldoutGroup("Ragdoll")] public float TimeToResetBones = 1f;
    [FoldoutGroup("Ragdoll")] public bool IsAiming = false;
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
    private float _currentAimingValue = 0f;
    #endregion

    private void Awake()
    {
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

    [FoldoutGroup("Debug")] [Button("Collect rigidbodies")]
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

    [FoldoutGroup("Debug")] [Button("Enable ragdoll")]
    public void EnableRagdoll()
    {
        //aimIK.enabled = false;
        //aimController.enabled = false;
        agent.enabled = true;
        animator.enabled = false;

        foreach (var rb in _ragdollRigidbodies)
        {
            rb.isKinematic = false;
        }

        isRagdoll = true;
    }

    public void DisableRagdoll()
    {
        // aimIK.enabled = true;
        //aimController.enabled = true;
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

        Vector3 desiredDirection = _hipsBone.up;

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

    [FoldoutGroup("Debug")]
    [Button("Wakeup")]
    private void Wakeup()
    {
        _isFacingUp = _hipsBone.forward.y > 0;

        AlignRotationToHips();
        AlignPositionToHips();
        PopulateBoneTransform(_ragdollBoneTransforms);
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


    #region Updates
    private void Update()
    {
        UpdateRagdolls();
    }

    private void LateUpdate()
    {
        if (isRagdoll)
        {
            //transform.position = _hipsBone.position;
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
}
