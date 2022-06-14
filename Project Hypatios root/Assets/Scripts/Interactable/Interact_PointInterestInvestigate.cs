using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interact_PointInterestInvestigate : InteractableObject
{

    public string chamberName = "Chamber1";
    public string eventKeyName = "PaintingEnter";
    public UnityEvent OnInteractEvent;

    private void Start()
    {
        string keyName1 = $"{chamberName}.{eventKeyName}";
        bool keyExist = FPSMainScript.instance.Check_EverUsed(keyName1);

        if (keyExist)
        {
            gameObject.SetActive(false);
        }
    }

    public override void Interact()
    {
        string keyName1 = $"{chamberName}.{eventKeyName}";
        bool keyExist = FPSMainScript.instance.Check_EverUsed(keyName1);

        if (!keyExist)
        {
            FPSMainScript.instance.TryAdd_ParadoxEvent(keyName1);
            gameObject.SetActive(false);
            OnInteractEvent?.Invoke();
        }
    }

    public override string GetDescription()
    {
        return "";
    }
}
