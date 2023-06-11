using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;

public class HES_Soldier_Office : EnemyScript
{

    public enum State
    {
        Lift = 0,
        MoveToPosition = 10,
        IdlePost = 20
    }

    public State currentState;
    public bool IsAiming = false;
    public HESS_PostIdle currentPost;
    public float rotation_Speed = 15f;
    public float animFloatParam_Speed = 0.4f;
    public float move_Speed = 4f;
    [FoldoutGroup("References")] public NavMeshAgent agent;
    [FoldoutGroup("References")] public BipedIK bipedIk;
    [FoldoutGroup("References")] public Animator animator;
    [FoldoutGroup("References")] public GameObject modularWeapon;
    [FoldoutGroup("References")] public AnimancerPlayer AnimatorPlayer;
    [FoldoutGroup("References")] public Transform baitTarget; //because offset entity bounding box problem
    [FoldoutGroup("Ragdoll")] public int layerAimingIndex = 1;

    public bool IsMoving => agent.velocity.magnitude > 0.1f;
    private float _currentAimingValue = 1f;

    private void Start()
    {
        ScanForEnemies();
        agent.enabled = false;
    }

    //cannot be attacked
    public override void Attacked(DamageToken token)
    {
        base.Attacked(token);
    }

    //cannot die
    public override void Die()
    {

    }

    #region Enemy behaviours

    public void Set_EnableAiming()
    {

        if (IsAiming == false) IsAiming = true;

        bipedIk.enabled = true;
        bipedIk.solvers.aim.IKPositionWeight = 1f;
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

    #region Update


    private void Update()
    {
        if (Stats.IsDead)
        {
            return;
        }
        UpdateRagdolls();

        if (isAIEnabled)
        {
            if (Hypatios.TimeTick % 10 == 0) ScanForEnemies();
        }
        else
        {
            return;
        }

        if (currentTarget == null) return;
        AI_Detection();

        UpdateAIState();
    }

    private void UpdateRagdolls()
    {

        if (IsAiming)
        {
            _currentAimingValue = Mathf.MoveTowards(_currentAimingValue, 1f, Time.deltaTime);
            animator.SetLayerWeight(layerAimingIndex, _currentAimingValue);

            if (currentTarget != null)
            {
                baitTarget.transform.position = currentTarget.OffsetedBoundWorldPosition;
            }

            bipedIk.solvers.aim.target = baitTarget.transform;

        }
        else
        {
            _currentAimingValue = Mathf.MoveTowards(_currentAimingValue, 0f, Time.deltaTime);
            animator.SetLayerWeight(layerAimingIndex, _currentAimingValue);
        }
    }

    private float dist;

    private void UpdateAIState()
    {
        if (currentState == State.Lift)
        {
            Set_StopMoving();
            Set_EnableAiming();
            if (modularWeapon.gameObject.activeSelf == false) modularWeapon.SetActive(true);
            if (currentTarget != null)
            {
                State_LiftMode();
            }
        }
        else if (currentState == State.MoveToPosition)
        {
            if (agent.enabled == false) agent.enabled = true;
            Set_StartMoving(move_Speed, animFloatParam_Speed);
            State_MoveToPosition();
            if (modularWeapon.gameObject.activeSelf == true) modularWeapon.SetActive(false);
        }
        else if (currentState == State.IdlePost)
        {
            State_IdlePost();
            if (modularWeapon.gameObject.activeSelf == true) modularWeapon.SetActive(false);
        }
    }

    private void State_LiftMode()
    {

        Vector3 posTarget = currentTarget.OffsetedBoundWorldPosition;

        Vector3 dir = posTarget - transform.position;
        dir.y = 0;
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        Quaternion q1 = Quaternion.RotateTowards(transform.rotation, rotation, rotation_Speed * Time.deltaTime);
        transform.rotation = q1;

    }

    private void State_MoveToPosition()
    {
        dist = Vector3.Distance(OffsetedBoundWorldPosition, currentPost.transform.position);

        if (dist < currentPost.dist_StartAnimation)
        {
            AnimatorPlayer.PlayAnimation(currentPost.idleAnimation, 1f);
            currentState = State.IdlePost;
        }
        else
        {
            Set_EnableAiming();
        }

        agent.updateRotation = true;
        Set_StartMoving(move_Speed, animFloatParam_Speed);
        agent.SetDestination(currentPost.transform.position);
    }

    private void State_IdlePost()
    {
        dist = Vector3.Distance(OffsetedBoundWorldPosition, currentPost.transform.position);
        if (dist < currentPost.dist_StartAnimation)
        {
            agent.updateRotation = false;
            transform.rotation = Quaternion.Lerp(transform.rotation, currentPost.transform.rotation, Time.deltaTime * rotation_Speed);
            Set_DisableAiming();
        }
        else
        {
            currentState = State.MoveToPosition;
            AnimatorPlayer.DisableAnimancer();

        }

    }

    #endregion
}
