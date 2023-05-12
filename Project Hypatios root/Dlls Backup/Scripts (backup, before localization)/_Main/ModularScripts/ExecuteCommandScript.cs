using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ExecuteCommandScript : MonoBehaviour
{

    public string args = "god";

    [FoldoutGroup("Debug")]
    [Button("Execute")]
    public void CommandExecute()
    {
        ConsoleCommand.Instance.CommandInput(args);
    }

}
