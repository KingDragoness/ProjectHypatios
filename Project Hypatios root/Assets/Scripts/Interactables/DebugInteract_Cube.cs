using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInteract_Cube : InteractableObject
{



    public override string GetDescription()
    {
        return "Test";
    }

    public override void Interact()
    {

        Debug.Log("Something happened");
        if (ConsoleCommand.Instance != null) ConsoleCommand.Instance.SendConsoleMessage("Something happened");

    }
}
