using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Interact_VendingPayAccess : MonoBehaviour
{

    public UnityEvent OnSuccessPayment;
    public int soulCost = 50;
    public float time_interactPrompt = 4f;
    public string failedText = "Failed: Not enough souls.";
    public string successText = "Success: Unlocked workbench.";
    public bool allowRepeat = false;

    private bool hasTriggered = false;

    public void ResetTrigger()
    {
        hasTriggered = false;
    }

    public void Pay()
    {
        if (soulCost > Hypatios.Game.SoulPoint)
        {
            DeadDialogue.PromptNotifyMessage_Mod(failedText, time_interactPrompt);
            return;
        }
        if (allowRepeat == false && hasTriggered == true)
            return;

        hasTriggered = true;
        Hypatios.Game.SoulPoint -= soulCost;
        MainGameHUDScript.Instance.audio_PurchaseReward.Play();
        DeadDialogue.PromptNotifyMessage_Mod(successText, time_interactPrompt);
        OnSuccessPayment?.Invoke();
    }

}
