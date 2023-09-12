using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainMenu_CheckTrivia : MonoBehaviour
{

    public Trivia trivia;
    public UnityEvent OnTriviaCompleted;
    public bool _checkTriviaImmediately = true;

    private void Start()
    {
        if (_checkTriviaImmediately == true) TriviaEventCheck();
    }

    public void TriviaEventCheck()
    {
        if (IsTriviaTriggered())
        {
            OnTriviaCompleted?.Invoke();
        }
        else
        {

        }
    }

    private bool IsTriviaTriggered()
    {
        var triviaEntry = MainMenuTitleScript.GetHypatiosSave().Game_Trivias.Find(x => x.ID == trivia.ID);

        if (triviaEntry != null)
        {
            if (triviaEntry.isCompleted)
                return true;

            return false;
        }
        else
        {
            return false;
        }
    }

}
