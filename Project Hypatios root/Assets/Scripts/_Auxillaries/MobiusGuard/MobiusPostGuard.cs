using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Animancer;


public class MobiusPostGuard : MonoBehaviour
{

    public enum Type
    {
        Turret,
        AreaPatrol
    }

    [FoldoutGroup("Idle")] public ClipTransition idleAnimation;
    [FoldoutGroup("Events")] public UnityEvent OnGuarding; //update run
    [FoldoutGroup("Events")] public UnityEvent OnEnterPost;
    [FoldoutGroup("Events")] public UnityEvent OnLeavePost; //disable turret machine gun
    public MobiusGuardEnemy currentAssignedGuard;
    [FoldoutGroup("Params")] public bool stayPostAtAllCost = false;
    [FoldoutGroup("Params")] public int priorityPost_Normal = 10;
    [FoldoutGroup("Params")] public int priorityPost_atAllCost = 100;
    [FoldoutGroup("Params")] public float dist_StartAnimation = 0.7f;
    [FoldoutGroup("Abandon Post")] public float dist_PlayerToAbandon = 8f;
    [FoldoutGroup("Abandon Post")] public bool isTurretPost = false;
    [FoldoutGroup("Abandon Post")] public int post_threatenMultiplier = -100;
    [FoldoutGroup("Abandon Post")] [SerializeField] private RandomSpawnArea turretArea;
    public Type postGuardType;

    private bool isGuarded = false;

    public bool IsGuarded { get => isGuarded;}

    private void Start()
    {
        var patrolAI = currentAssignedGuard.GetAIBehaviour<MAIB_Patrol>() as MAIB_Patrol;
        patrolAI.currentPost = this;
        patrolAI.stayPostAtAllCost = stayPostAtAllCost;
        patrolAI.priorityPost_Normal = priorityPost_Normal;
        patrolAI.priorityPost_atAllCost = priorityPost_atAllCost;
    }

    public bool IsPlayerInTurretArea(Vector3 pos)
    {
        if (turretArea == null)
            return false;

        return turretArea.IsInsideOcclusionBox(pos);
    }

    public void OnGuard()
    {
        if (isGuarded == false)
        {
            OnEnterPost?.Invoke();
        }

        OnGuarding?.Invoke();
        isGuarded = true;
    }

    public void OnUnguarded()
    {
        if (isGuarded == true)
        {
            OnLeavePost?.Invoke();
        }

        isGuarded = false;
    }

}
