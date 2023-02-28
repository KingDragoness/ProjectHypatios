using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class VaultPasscode : MonoBehaviour
{

    public SpeechDialogueAsset dialogue_Success;
    public SpeechDialogueAsset dialogue_Failed;
    public AnimatorSetBool doorScript;
    public Trivia trivia;

    private bool success = false;

    private void OnEnable()
    {
        if (success)
        {
            doorScript.SetBool(true);
        }
    }

    public void TryInteract()
    {
        if (success) return;

        if (Hypatios.Game.Check_TriviaCompleted(trivia))
        {
            dialogue_Success.TriggerMessage();
            doorScript.SetBool(true);
            success = true;
        }
        else
        {
            dialogue_Failed.TriggerMessage();
        }
    }

}
