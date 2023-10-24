﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class VaultPasscode : MonoBehaviour
{

    public Interact_TriggerDialogueAsset dialogue_Success;
    public Interact_TriggerDialogueAsset dialogue_Failed;
    public AnimatorSetBool doorScript;
    public AudioSource audio_Door;
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
            audio_Door.Play();
            success = true;
        }
        else
        {
            dialogue_Failed.TriggerMessage();
        }
    }

}