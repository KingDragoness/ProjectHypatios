using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_TutorialPromptHelp : MonoBehaviour
{
    public string about = "Death";

    [TextArea(3, 5)]
    public string content;

    public string key = "LEVEL1_FirstDeath";

    public void TriggerHelp()
    {
        FPSMainScript.instance.RuntimeTutorialHelp(about, content, key);
    }

}
