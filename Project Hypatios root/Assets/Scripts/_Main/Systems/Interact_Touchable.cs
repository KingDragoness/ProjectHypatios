using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Interact_Touchable : InteractableObject
{

    public UnityEvent OnInteractEvent;
    [ShowIf("useHoverEvent")] public UnityEvent OnHoveringEvent;
    [ShowIf("useHoverEvent")] public UnityEvent OnNotHoverEvent;
    public AudioSource interactSound;
    public bool useHoverEvent = false;
    public string interactDescription = "Interact";
    public string text_interactPrompt = "A door has been unlocked.";
    public float time_interactPrompt = 4f;

    [Button("Interact")]
    public override void Interact()
    {
        OnInteractEvent?.Invoke();
        if (interactSound != null) interactSound.Play();
    }

    public override string GetDescription()
    {
        return interactDescription;
    }

    private bool safetyBoolCheck = false;

    public void Prompt()
    {
        DeadDialogue.PromptNotifyMessage_Mod(text_interactPrompt, time_interactPrompt);
    }

    private void Update()
    {
        if (useHoverEvent == false) return;
        if (Time.timeScale == 0) return;

        if (InteractableCamera.instance.currentInteractable == this)
        {
            if (safetyBoolCheck == false) OnHoveringEvent?.Invoke();
            safetyBoolCheck = true;
        }
        else
        {
            if (safetyBoolCheck == true) OnNotHoverEvent?.Invoke();
            safetyBoolCheck = false;
        }
    }
}

