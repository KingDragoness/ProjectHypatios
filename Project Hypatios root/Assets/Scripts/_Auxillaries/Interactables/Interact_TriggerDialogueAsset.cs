using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_TriggerDialogueAsset : MonoBehaviour
{

    public SpeechDialogueAsset dialogueAsset;

    public void TriggerMessage()
    {
        dialogueAsset.TriggerMessage();

    }

}
