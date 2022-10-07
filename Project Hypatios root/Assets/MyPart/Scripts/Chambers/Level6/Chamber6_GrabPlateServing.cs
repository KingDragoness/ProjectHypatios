using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Chamber6_GrabPlateServing : InteractableObject
{

    public Chamber_Level6 chamberScript;

    public override string GetDescription()
    {
        return "Take Plate";
    }

    public override void Interact()
    {
        chamberScript.AmbilPiring();
    }


}
