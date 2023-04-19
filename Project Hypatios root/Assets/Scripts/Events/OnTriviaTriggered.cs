using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriviaTriggered : MonoBehaviour
{

    public UnityEvent OnTriviaTrigger;
    public UnityEvent OnTriviaNotActive;
    public Trivia trivia;
    public Trivia mutualExclusiveTrivia;

    public static System.Action<Trivia> OnActionTriviaTrigger;

    private void Start()
    {
        OnActionTriviaTrigger += OnTriggerTrivia;
        bool allow = CheckTriviaValid();

        if (allow)
        {
            OnTriviaTrigger?.Invoke();
        }
        else
            OnTriviaNotActive?.Invoke();
    }

    public void EnforceTrigger()
    {
        bool allow = CheckTriviaValid();

        if (allow)
        {
            OnTriviaTrigger?.Invoke();
        }
        else
            OnTriviaNotActive?.Invoke();
    }

    private void OnDestroy()
    {
        OnActionTriviaTrigger -= OnTriggerTrivia;
    }

    private void OnTriggerTrivia(Trivia _trivia)
    {
        if (_trivia == trivia)
            OnTriviaTrigger?.Invoke();

    }

    public bool CheckTriviaValid()
    {
        bool allowTrigger = false;

        if (mutualExclusiveTrivia != null)
        {
            if (Hypatios.Game.Check_TriviaCompleted(trivia) && Hypatios.Game.Check_TriviaCompleted(mutualExclusiveTrivia) == false)
                allowTrigger = true;
        }
        else
        {
            if (Hypatios.Game.Check_TriviaCompleted(trivia))
            {
                allowTrigger = true;
            }
            else
                allowTrigger = false;
        }

        return allowTrigger;
    }
}
