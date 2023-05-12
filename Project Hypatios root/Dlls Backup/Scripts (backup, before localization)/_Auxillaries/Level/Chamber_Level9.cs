using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class Chamber_Level9 : MonoBehaviour
{

    public GameObject lava;
    private ConsoleCommand consoleCommand;

    private void Awake()
    {
        ConsoleCommand.onExecuteCommand += OnExecuteCommand;
        consoleCommand = ConsoleCommand.Instance;
    }
    private void OnDestroy()
    {
        ConsoleCommand.onExecuteCommand -= OnExecuteCommand;
    }

    private bool OnExecuteCommand(string commandName, string[] args)
    {
        bool success = false;

        if (commandName == "help" && args.Length != 0)
        {
            if (args[0] == "level")
                Help();

            success = true;
        }

        if (commandName == "setlava")
        {
            try
            {
                float height = 0;
                float.TryParse(args[0], out height);

                SetHeightLava(height);
                success = true;
            }
            catch
            {
                ConsoleCommand.Instance.SendConsoleMessage("Invalid argument! setlava [<color=#00cc99dd>float</color> heightY]");
            }
        }

        return success;

    }

    private void Help()
    {
        List<string> helps = new List<string>();
        helps.Add("'setlava' to set lava's height pos Y. [-10 to reset]");

        foreach (var helpString in helps)
        {
            ConsoleCommand.Instance.SendConsoleMessage(helpString);
        }

        helps.Add("");
    }

    [Button("Set Height")]
    public void SetHeightLava(float heightY)
    {
        var pos = lava.transform.position;
        pos.y = heightY;
        lava.transform.position = pos;
    }

}
