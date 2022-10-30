using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Interact_Touchable : InteractableObject
{

    public UnityEvent OnInteractEvent;
    public AudioSource interactSound;
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
}

