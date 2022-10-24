using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Debug_ChamberLocalCommands : MonoBehaviour
{

    [System.Serializable]
    public class LevelCommand
    {
        [FoldoutGroup("Show more")] public UnityEvent OnCommandExecute;
        [FoldoutGroup("Show more")] public string commandName = "test";
        [FoldoutGroup("Show more")] public string commandHelp = "'test' this is only for test";
    }

    public List<LevelCommand> AllLocalCommands = new List<LevelCommand>();
    public string LevelName = "Beginnings";

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

        foreach(var command in AllLocalCommands)
        {
            if (command.commandName == commandName)
            {
                command.OnCommandExecute?.Invoke();
                success = true;
                break;
            }
        }


        return success;
    }

    private void Help()
    {
        List<string> helps = new List<string>();
        helps.Add($" =============== {LevelName.ToUpper()} COMMANDS =============== ");

        foreach(var localCommand in AllLocalCommands)
        {
            helps.Add(localCommand.commandHelp);
        }

        foreach (var helpString in helps)
        {
            ConsoleCommand.Instance.SendConsoleMessage(helpString);
        }

        helps.Add("");
    }
}
