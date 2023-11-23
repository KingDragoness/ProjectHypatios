using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Interact_TutorialPromptHelp : MonoBehaviour
{
    [FoldoutGroup("Obselete")] public string about = "Death";

    [FoldoutGroup("Obselete")]
    [TextArea(3, 5)]
    public string content;

    [FoldoutGroup("Obselete")] public string key = "LEVEL1_FirstDeath";

    public CodexHintTipsSO codex;

    public void TriggerHelp()
    {
        Hypatios.Game.RuntimeTutorialHelp(codex);
    }

}
