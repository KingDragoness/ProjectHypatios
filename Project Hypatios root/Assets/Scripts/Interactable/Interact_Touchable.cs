using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interact_Touchable : InteractableObject
{

    public UnityEvent OnInteractEvent;

    public override void Interact()
    {
        OnInteractEvent?.Invoke();
    }

    public override string GetDescription()
    {
        return "";
    }
}

