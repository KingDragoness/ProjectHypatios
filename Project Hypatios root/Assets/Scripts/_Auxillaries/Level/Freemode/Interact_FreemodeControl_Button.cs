using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_FreemodeControl_Button : MonoBehaviour
{

    public Interact_FreemodeControlRoom parentScript;
    public int index = 0;
    public TextMesh label_Name;
    public Interact_Touchable touchable;

    public void Execute()
    {
        parentScript.ExecuteCommand(this);
    }

}
