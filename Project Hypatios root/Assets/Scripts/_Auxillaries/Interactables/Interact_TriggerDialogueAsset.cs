using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_TriggerDialogueAsset : MonoBehaviour
{

    public SpeechDialogueAsset dialogueAsset;
    public float TimeDisplayDialogueAgain = 15;

    private float _timeDialogue = 2f;


    public void TriggerMessage()
    {
        if (_timeDialogue > 0.1f) return;

        dialogueAsset.TriggerMessage();
        _timeDialogue = TimeDisplayDialogueAgain;
    }

    private void Update()
    {
        if (_timeDialogue > 0f)
            _timeDialogue -= Time.deltaTime;

        
    }

}
