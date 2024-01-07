using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using ButtonCommandElement = WindowConfirmPrompt.ButtonCommandElement;


public class Modular_LaunchConfirmPrompt : MonoBehaviour
{

    public string header = "Quit Menu";
    [TextArea(3,4)] public string description = "";
    public List<ButtonCommandElement> commands = new List<ButtonCommandElement>();

    public void LaunchPrompt()
    {
        WindowConfirmPrompt.LaunchPrompt(header, description, commands);
    }

}
