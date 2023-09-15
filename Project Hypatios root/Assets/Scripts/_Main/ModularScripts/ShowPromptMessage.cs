using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPromptMessage : MonoBehaviour
{

    public string PromptMessage = "";
    public float Time = 5f;

    public void TriggerMessage()
    {
        DeadDialogue.PromptNotifyMessage(PromptMessage, Time);
    }

}
