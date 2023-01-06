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

