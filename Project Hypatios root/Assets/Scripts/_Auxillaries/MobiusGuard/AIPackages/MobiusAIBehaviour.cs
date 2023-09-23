using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public interface IMAIBGoal
{
    bool IsConditionFulfilled();
    int CalculatePriority();
    void Execute();
    void OnBehaviourActive();
    void OnBehaviourDisable();
}

public abstract class MobiusAIBehaviour : MonoBehaviour, IMAIBGoal
{

    public MobiusGuardEnemy mobiusGuardScript;
    public bool isExclusive = false;
    public bool cannotBeSelectedByDecision = false;
    public bool isHostileBehaviour = true;
    public UnityEvent OnEnableBehaviour;
    public UnityEvent OnDisableBehaviour;
    [ShowIf("isExclusive")] public List<MobiusAIBehaviour> allowPreviousBehaviours = new List<MobiusAIBehaviour>();
    [HideIf("isExclusive")] public List<MobiusAIBehaviour> unallowPreviousBehaviours = new List<MobiusAIBehaviour>();
    public int basePriority = 0;
    [ReadOnly] public int currentPriorityLevel = 0;

    public virtual void Awake()
    {
        if (mobiusGuardScript == null)
            mobiusGuardScript = GetComponentInParent<MobiusGuardEnemy>();
    }

    //For calculating priority level
    private void Update()
    {
        if (Time.timeScale == 0) return;
        currentPriorityLevel = CalculatePriority();
    }

    public virtual bool IsConditionFulfilled()
    {
        return IsExclusivityChecked();
    }

    protected bool IsExclusivityChecked()
    {
        bool result = false;
        if (isExclusive)
        {
            result = false;
        }
        else
        {
            result = true;
        }

        return result;
    }

    public virtual int CalculatePriority()
    {
        return -1;
    }
    public virtual void Execute()
    {

    }
    public virtual void OnBehaviourActive()
    {

    }
    public virtual void OnBehaviourDisable()
    {

    }


}
