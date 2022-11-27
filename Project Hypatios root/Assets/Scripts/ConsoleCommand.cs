using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class ConsoleCommand : MonoBehaviour
{

    public InputField inputField;
    public Text consoleText;

    public delegate bool OnExecuteCommand(string commandName, string[] args); //Return success
    public static event OnExecuteCommand onExecuteCommand;

    private List<string> historyCommands = new List<string>()
    {
        "",
    };

    public static ConsoleCommand Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            CommandInput(inputField.text);
        }
    }

    public void CommandInput(string command)
    {
        string[] inputSplit = command.Split(' ');

        string commandInput = inputSplit[0];
        string[] args = inputSplit.Skip(1).ToArray();

        SendConsoleMessage("<color=#ffffffcc>" + command + "</color>");

        ProcessCommand(commandInput, args);

        historyCommands.Insert(0, command);
    }

    public void SendConsoleMessage(string msg)
    {
        if (consoleText != null) consoleText.text += "> " +  msg + "\n";
    }

    public void ProcessCommand()
    {

    }

    void ProcessCommand(string commandInput, string[] args)
    {

        bool success = false;

        if (onExecuteCommand != null) { success = onExecuteCommand.Invoke(commandInput, args); }

        switch (commandInput)
        {

            #region Commands
            case "wstat":
                Debug_ObjectStat(args);
                break;

            case "cc":
                CommandCheat(args);
                break;

            case "giveammos":
                GiveAmmos(args);
                break;

            case "levelnames":
                LevelNames(args);
                break;

            case "goto":
                GoTo(args);
                break;

            case "god":
                GodMode(args);
                break;

            case "givemeallweapons":
                GiveMeAllWeapons(args);
                break;

            case "killme":
                KillMe(args);
                break;

            case "killall":
                KillAll(args);
                break;

            case "loadlevel":
                LoadLevel(args);
                break;

            case "loadfile":
                LoadFile(args);
                break;

            case "screensize":
                ScreenSize(args);
                break;

            case "nospeed":
                NoClipSpeed(args);
                break;

            case "nextlevel":
                NextLevel(args);
                break;

            case "help":
                Help(args);
                break;

            case "res":
                Restore(args);
                break;

            case "savefile":
                SaveFile(args);
                break;

            case "setfps":
                SetFPS(args);
                break;

            case "soul":
                Soul(args);
                break;

            case "ui":
                ChangeUIMode(args);
                break;

            default:
                if (!success)
                {
                    SendConsoleMessage("Command is invalid unless maybe a level command is executed.");
                }
                break;

            #endregion
        }


    }

    private void LevelNames(string[] args)
    {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        string[] scenes = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++)
        {
            scenes[i] = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
        }

        int index1 = 0;
        foreach (var string1 in scenes)
        {
            SendConsoleMessage($"{index1} | {string1}");
            index1++;
        }
    }



    #region Commands
    protected void CommandCheat(string[] args)
    {

        try
        {
            FPSMainScript.instance.CommandCheat(args[0]);
        }
        catch
        {
            //SendConsoleMessage("Invalid argument! ui [<color=#00cc99dd>int</color> current_UI]");
        }
    }

    protected void ChangeUIMode(string[] args)
    {

        try
        {
            int enum1 = 0;
            int.TryParse(args[0], out enum1);

            var MainUI1 = MainUI.Instance;

            MainUI1.current_UI = (MainUI.UIMode)enum1;

        }
        catch
        {
            SendConsoleMessage("Invalid argument! ui [<color=#00cc99dd>int</color> current_UI]");
        }
    }


    protected void KillMe(string[] args)
    {

        try
        {
            var character = Hypatios.Player;
            character.Health.curHealth = -1;
            character.Health.targetHealth = -1;
        }
        catch
        {
            SendConsoleMessage("Invalid argument! killme");
        }
    }

    protected void NoClipSpeed(string[] args)
    {

        try
        {
            int speed = 0;
            int.TryParse(args[0], out speed);
            MainUI.Instance.Camera_Noclip.flySpeed = speed;
        }
        catch
        {
            SendConsoleMessage("Invalid argument! nospeed [<color=#00cc99dd>int</color> speed]");
        }
    }


    protected void SaveFile(string[] args)
    {

        try
        {
            FPSMainScript.instance.SaveGame();
        }
        catch
        {
            SendConsoleMessage("Invalid argument! savefile");
        }
    }
    protected void LoadFile(string[] args)
    {

        try
        {
            FPSMainScript.instance.LoadGame();
        }
        catch
        {
            SendConsoleMessage("Invalid argument! loadfile");
        }
    }


    protected void GoTo(string[] args)
    {

        try
        {
            string databaseID = args[0];

        }
        catch
        {
            SendConsoleMessage("Invalid argument! goto [<color=#00cc99dd>int</color> indexSpot]");

        }
    }

    protected void LoadLevel(string[] args)
    {

        try
        {
            int levelName = 0;
            int.TryParse(args[0], out levelName);

            FPSMainScript.instance.SaveGame(targetLevel: levelName);
            FPSMainScript.instance.BufferSaveData();

            Application.LoadLevel(levelName);
            Time.timeScale = 1;

        }
        catch (System.Exception e)
        {
            SendConsoleMessage("Invalid argument! loadlevel [<color=#00cc99dd>int</color> levelIndex]");
            Debug.LogError(e.Message);
        }
    }


    protected void NextLevel(string[] args)
    {
        try
        {
            int target = 0;
            target = Application.loadedLevel + 1;

            FPSMainScript.instance.SaveGame(targetLevel: target);
            FPSMainScript.instance.BufferSaveData();

            Application.LoadLevel(target);

        }
        catch (Exception e)
        {
            SendConsoleMessage("Invalid argument! nextlevel");
            Debug.LogError(e.Message);
        }

    }

    private void Debug_ObjectStat(string[] args)
    {
        try
        {
            bool validArgument = true;
            if (args.Length != 0)
            {
                validArgument = false;

                if (args[0] == "lockenemy")
                {
                    Hypatios.DebugObjectStat.LockEnemy = !Hypatios.DebugObjectStat.LockEnemy;
                    SendConsoleMessage($" {Hypatios.DebugObjectStat.LockEnemy}");
                }
                else
                {
                    throw new System.Exception("");
                }
            }
            else
            {
                validArgument = true;
            }

            if (validArgument == true)
            {
                bool b = Hypatios.DebugObjectStat.gameObject.activeSelf;
                Hypatios.DebugObjectStat.gameObject.SetActive(!b);
            }
        }
        catch (Exception e)
        {
            SendConsoleMessage("Invalid argument! Use 'help wstat' to see more world stat commands!");
            Debug.LogError(e.Message);
        }
    }


    protected void Soul(string[] args)
    {
        try
        {
            int soul = 0;
            int.TryParse(args[0], out soul);

            FPSMainScript.instance.SoulPoint += soul;
        }
        catch
        {
            SendConsoleMessage("Invalid argument! soul [<color=#00cc99dd>int</color> soulAmount]");

        }
    }


    protected void ScreenSize(string[] args)
    {
        try
        {
            int width = 0;
            int height = 0;
            int.TryParse(args[0], out width);
            int.TryParse(args[1], out height);

            Screen.SetResolution(width, height, Screen.fullScreen);
        }
        catch
        {
            SendConsoleMessage("Invalid argument! screensize [<color=#00cc99dd>int</color> width] [<color=#00cc99dd>int</color> height]");

        }
    }


    protected void GodMode(string[] args)
    {

        try
        {
            CharacterScript characterScript = Hypatios.Player;
            characterScript.isCheatMode = !characterScript.isCheatMode;
            SendConsoleMessage($"Cheat Mode: {characterScript.isCheatMode}");


        }
        catch
        {
            SendConsoleMessage("Invalid argument! loadlevel [<color=#00cc99dd>int</color> levelIndex]");

        }
    }

    protected void Restore(string[] args)
    {

        try
        {
            CharacterScript characterScript = Hypatios.Player;
            characterScript.Health.targetHealth = characterScript.Health.maxHealth;
            characterScript.Health.curHealth = characterScript.Health.maxHealth;
            characterScript.timeSinceLastDash = 10f;

        }
        catch
        {
            SendConsoleMessage("Invalid argument! loadlevel [<color=#00cc99dd>int</color> levelIndex]");

        }
    }

    protected void GiveAmmos(string[] args)
    {

        try
        {
            WeaponManager weaponManager = Hypatios.Player.Weapon;

            foreach (var weapon in weaponManager.CurrentlyHeldWeapons)
            {
                weapon.curAmmo = 9999;
                weapon.totalAmmo = 9999;
            }

        }
        catch
        {
            SendConsoleMessage("Invalid argument! giveammos");

        }
    }

    protected void GiveMeAllWeapons(string[] args)
    {

        try
        {
            WeaponManager weaponManager = Hypatios.Player.Weapon;

            int i = 0;

            foreach (var weapon in weaponManager.weapons)
            {
                if (i != 0)
                {
                    weaponManager.AddWeapon(weapon.nameWeapon);
                }

                i++;
            }
            
        }
        catch
        {
            SendConsoleMessage("Invalid argument! givemeallweapons");

        }
    }

    protected void KillAll(string[] args)
    {
        try
        {
            EnemyScript[] enemies = FindObjectsOfType<EnemyScript>();
            DamageToken token = new DamageToken();
            token.damage = 99999;
            token.origin = DamageToken.DamageOrigin.Environment;
            token.repulsionForce = 1f;

            foreach(var enemy in enemies)
            {
                enemy.Attacked(token);
            }

        }
        catch (Exception e)
        {
            SendConsoleMessage("Invalid argument! killall");
            Debug.LogError(e.Message);
        }

    }

    protected void Help(string[] args)
    {
        List<string> helps = new List<string>();
        bool showFirstPageHelp = false;

        if (args.Length != 0)
        {
            if (args[0] == "level")
            {
                return;
            }
            else if (args[0] == "1")
            {
                helps.Add(" =============== HELP [1/1] =============== ");
                helps.Add("Press ENTER to execute command");
                helps.Add("Press ~ key to toggle console");
                helps.Add("'cc' to use extra commands");
                helps.Add("'wstat' to stat world objects. 'help wstat' to show more wstat commands");
                helps.Add("'nospeed' to set freecam speed. 'ui 4' to use noclip.");
                helps.Add("'nextlevel' to go next level while retaining items");
                helps.Add("'levelnames' gets every level exists in the current build.");
                helps.Add("'god' to toggle god mode");
                helps.Add("'res' to restore health & dash");
                helps.Add("'soul' to get soul");
                helps.Add("'ui' to change UI mode 0/1/2");
                helps.Add("'giveammos' to give ammos");
                helps.Add("'setfps' to set game's FPS");
                helps.Add(" ");
            }
            else if (args[0] == "wstat")
            {
                helps.Add(" =============== HELP [WORLD STAT commnads] =============== ");
                helps.Add("Press ENTER to execute command");
                helps.Add("Press ~ key to toggle console");
                helps.Add("'wstat lockenemy' to lock on targeted enemy.");
                helps.Add(" ");
            }
            else
            {
                showFirstPageHelp = true;
            }
        }
        else
        {
            showFirstPageHelp = true;

        }

        if (showFirstPageHelp)
        {
            helps.Add(" =============== HELP [0/1] =============== ");
            helps.Add("Press ENTER to execute command");
            helps.Add("Press ~ key to toggle console");
            helps.Add("'loadlevel' to load level. Resets progress!");
            helps.Add("'savefile' to save file");
            helps.Add("'loadfile' to load save file");
            helps.Add("'givemeallweapons' to unlock all weapons");
            helps.Add("'help level' to see level commands if exist");
            helps.Add("'killme' to commit suicide");
            helps.Add("'killall' to all enemies");
            helps.Add("'screensize' to set screen size");
            helps.Add(" ");
        }

        foreach (var helpString in helps)
        {
            SendConsoleMessage(helpString);
        }
    }

    protected void SetFPS(string[] args)
    {

        try
        {
            int fps = 0;
            int.TryParse(args[0], out fps);

            if (fps <= 10)
            {
                SendConsoleMessage($"Target fps: {fps} is too low. Reseting to 10 fps");
                fps = 10;
            }

            Application.targetFrameRate = fps;

        }
        catch
        {
            SendConsoleMessage("Invalid argument! setfps [<color=#00cc99dd>int</color> fps]");

        }
    }

    #endregion

}
