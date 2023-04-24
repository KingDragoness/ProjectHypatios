using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriviaBallButton : MonoBehaviour
{

    public TriviaMapUI triviaScript;
    public Trivia trivia;
    public GameObject triviaCompleteIcon;
    public GameObject triviaNotComplete;

    
    public void Hover()
    {

    }

    public void RefreshTrivia()
    {
        if (Hypatios.Game.Check_TriviaCompleted(trivia))
        {
            triviaCompleteIcon.SetActive(true);
            triviaNotComplete.SetActive(false);
        }
        else
        {
            triviaCompleteIcon.SetActive(false);
            triviaNotComplete.SetActive(true);

        }
    }
}
