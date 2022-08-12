using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Chamber3_FernHerb : InteractableObject
{

    public UnityEvent OnCollected;
    public bool hasCollected = false;

    public override string GetDescription()
    {
        return "";
    }

    public override void Interact()
    {
        OnCollected?.Invoke();
        soundManagerScript.instance.Play("reward");
        gameObject.SetActive(false);

        hasCollected = true;
    }
}
