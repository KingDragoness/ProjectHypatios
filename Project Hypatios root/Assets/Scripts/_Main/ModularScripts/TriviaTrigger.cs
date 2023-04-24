using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriviaTrigger : MonoBehaviour
{

    public Trivia trivia;

    [ContextMenu("Trigger Trivia")]
    public void TriggerTrivia()
    {
        Hypatios.Game.TriviaComplete(trivia);
    }

}
