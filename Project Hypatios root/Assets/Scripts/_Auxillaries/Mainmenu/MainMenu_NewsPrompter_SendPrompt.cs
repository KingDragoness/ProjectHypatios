using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_NewsPrompter_SendPrompt : MonoBehaviour
{

    [TextArea(3,6)] public string text = "Test.";

    public void SendText()
    {
        MainMenu_NewsPrompter.instance.InsertText(text);
    }

}
