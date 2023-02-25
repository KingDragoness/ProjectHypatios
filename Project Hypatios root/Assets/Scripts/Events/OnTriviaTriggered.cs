using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriviaTriggered : MonoBehaviour
{

    public UnityEvent OnTriviaTrigger;
    public UnityEvent OnTriviaNotActive;
    public Trivia trivia;

    public static System.Action<Trivia> OnActionTriviaTrigger;

    private void Start()
    {
        OnActionTriviaTrigger += OnTriggerTrivia;

        if (Hypatios.Game.Check_TriviaCompleted(trivia))
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

}
