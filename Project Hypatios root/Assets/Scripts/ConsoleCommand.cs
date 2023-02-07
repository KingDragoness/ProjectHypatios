﻿using System.Collections;
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

    private static ConsoleCommand _instance;

    public static ConsoleCommand Instance
    {
        get
        {
            if (_instance == null)
                _instance = Hypatios.ConsoleCommand;

            return _instance;
        }

        set
        {
            _instance = value;
        }
    }

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

            case "enemy":
                EnemyCommand(args);
                break;

            case "alcohol":
                Alcohol(args);
                break;

            case "cc":
                CommandCheat(args);
                break;

            case "additem":
                AddItem(args);
                break;

            case "mats":
                FreeMaterials(args);
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

            case "weapon":
                Weapon(args);
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

    private void Alcohol(string[] args)
    {
        try
        {
            float alcohol = 0;
            float.TryParse(args[0], out alcohol);

            Hypatios.Player.Health.alcoholMeter += alcohol;
            SendConsoleMessage($"Alcohol: {Hypatios.Player.Health.alcoholMeter}%");

        }
        catch (System.Exception e)
        {
            SendConsoleMessage("Invalid argument! alcohol [<color=#00cc99dd>float</color> levelIndex]");
            Debug.LogError(e.Message);
        }
    }

    private void AddItem(string[] args)
    {
        try
        {
            int count = 0;

            var item = Hypatios.Assets.GetItem(args[0]);

            if (args.Length > 1)
            {
                if (int.TryParse(args[1], out count))
                {
                    Hypatios.Player.Inventory.AddItem(item, count);
                }
                else
                {

                }
            }
            else
            {
                Hypatios.Player.Inventory.AddItem(item);
            }
        }
        catch
        {
            SendConsoleMessage("Invalid argument! additem [string itemID] [<color=#00cc99dd>int</color> count]");
        }
    }


    private void FreeMaterials(string[] args)
    {
        try
        {
            List<string> materials = new List<string>();
            materials.Add("Material_CommonMetal");
            materials.Add("Material_ExoticMetal");
            materials.Add("Material_Microchips");
            materials.Add("Material_NuclearMaterial");
            materials.Add("Material_RareMetal");


            foreach (var mat in materials)
            {
                string[] args1 = new string[2];
                args1[0] = $"{mat}";
                args1[1] = $"100";
                AddItem(args1);
            }
        }
        catch (System.Exception e)
        {
            SendConsoleMessage("Invalid argument! mats");
            Debug.LogError(e.Message);
        }
    }



    #region Commands
    protected void CommandCheat(string[] args)
    {

        try
        {
            Hypatios.Game.CommandCheat(args[0]);
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
            Hypatios.Game.SaveGame();
        }
        catch (Exception e)
        {
            SendConsoleMessage("Invalid argument! savefile");
            Debug.LogError(e.Message);
            Debug.LogError(e.StackTrace);
        }
    }
    protected void LoadFile(string[] args)
    {

        try
        {
            Hypatios.Game.LoadGame();
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

            Hypatios.Game.SaveGame(targetLevel: levelName);
            Hypatios.Game.BufferSaveData();

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

            Hypatios.Game.SaveGame(targetLevel: target);
            Hypatios.Game.BufferSaveData();

            Application.LoadLevel(target);

        }
        catch (Exception e)
        {
            SendConsoleMessage("Invalid argument! nextlevel");
            Debug.LogError(e.Message);
        }

    }


    private void Weapon(string[] args)
    {
        try
        {
            bool validArgument = true;
            if (args.Length != 0)
            {
                validArgument = false;

                var currentGun = Hypatios.Player.Weapon.currentGunHeld;
                var weaponClass = Hypatios.Assets.GetWeapon(currentGun.weaponName);
                var weaponData = Hypatios.Game.GetWeaponSave(currentGun.weaponName);
                var weaponStat = weaponClass.GetFinalStat(weaponData.allAttachments);

                if (args[0] == "attach")
                {
                    if (weaponClass.GetAttachmentWeaponMod(args[1]) == null)
                    {
                        throw new Exception($"Null attachment! {args[1]}");
                    }
                    weaponData.allAttachments.Add(args[1]);
                    SendConsoleMessage($"Added {args[1]} to {weaponClass.nameWeapon}");
                    SendConsoleMessage($"Reequip weapon to take effect.");

                }
                else if (args[0] == "rmvall")
                {
                    weaponData.allAttachments.Clear();
                    SendConsoleMessage($"Reequip weapon to take effect.");

                }
                else if (args[0] == "report")
                {


                    string stat = $"Damage: {weaponStat.damage}, Firerate: {weaponStat.cooldown} per second, Movespeed mult: {weaponStat.movespeedMultiplier}, Accuracy: {weaponStat.accuracy}";
                    string stat1 = $"Recoil mult: {weaponStat.recoilMultiplier}, Magazine: {weaponStat.magazineSize}";
                    string stat2 = "Current Attachments = ";
                    string s1 = "Available Attachments = ";

                    foreach(var attach in weaponData.allAttachments)
                    {
                        stat2 += $"{attach}, ";
                    }

                    foreach (var attach in weaponClass.attachments)
                    {
                        s1 += $"{attach.ID}, ";
                    }

                    SendConsoleMessage($"{stat}");
                    SendConsoleMessage($"{stat1}");
                    SendConsoleMessage($"{stat2}");
                    SendConsoleMessage("");
                    SendConsoleMessage($"{s1}");


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
                SendConsoleMessage("No argument! Use 'help weapon' to see more weapon commands!");

            }
        }
        catch (Exception e)
        {
            SendConsoleMessage("Invalid argument! Use 'help weapon' to see more weapon commands!");
            Debug.LogError(e.Message);
            Debug.LogError(e.StackTrace);
        }
    }



    private void EnemyCommand(string[] args)
    {
        try
        {
            bool validArgument = true;
            if (args.Length != 0)
            {
                validArgument = false;

                if (args[0] == "hack")
                {
                    Hypatios.DebugObjectStat.Enemy_ChangeAllianceToPlayer();
                }
                else if (args[0] == "frenzy")
                {
                    Hypatios.DebugObjectStat.Enemy_Frenzy();
                }
                else if (args[0] == "reset")
                {
                    Hypatios.DebugObjectStat.Enemy_ResetStat();
                }
                else if (args[0] == "warp")
                {
                    Hypatios.DebugObjectStat.Enemy_WarpToGizmoCrosshair();
                }
                else if (args[0] == "burn")
                {
                    Hypatios.DebugObjectStat.Enemy_Burn();
                }
                else if (args[0] == "heal")
                {
                    Hypatios.DebugObjectStat.Enemy_InstantHeal();
                }
                else if (args[0] == "paralyze")
                {
                    Hypatios.DebugObjectStat.Enemy_ParalyzeToggle();
                }
                else if (args[0] == "poison")
                {
                    Hypatios.DebugObjectStat.Enemy_Poison();
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
                SendConsoleMessage("No argument! Use 'help enemy' to see more enemy commands!");

            }
        }
        catch (Exception e)
        {
            SendConsoleMessage("Invalid argument! Use 'help enemy' to see more enemy commands!");
            Debug.LogError(e.Message);
            Debug.LogError(e.StackTrace);
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


    private void Debug_ObjectStat(string[] args)
    {
        try
        {
            bool validArgument = true;
            if (args.Length != 0)
            {
                validArgument = false;

                if (args[0] == "lock")
                {
                    Hypatios.DebugObjectStat.LockEnemy = !Hypatios.DebugObjectStat.LockEnemy;
                    if (Hypatios.DebugObjectStat.CheckThereIsEnemyOnCrosshair()) SendConsoleMessage($" {Hypatios.DebugObjectStat.LockEnemy}");
                    else SendConsoleMessage($"No enemy detected.");
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

            Hypatios.Game.SoulPoint += soul;
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
            characterScript.Health.targetHealth = characterScript.Health.maxHealth.Value;
            characterScript.Health.curHealth = characterScript.Health.maxHealth.Value;
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
            SendConsoleMessage("'givemeallweapons' is no longer supported. Don't use this command if you don't want your game become bugged.");
            SendConsoleMessage("Use 'ui 1' and 'soul 999' to access weapon easily.");
            WeaponManager weaponManager = Hypatios.Player.Weapon;

            int i = 0;

            foreach (var weapon in Hypatios.Assets.Weapons)
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
        helps.Add(" ");
        int entryPerPage = 10;
        List<string> helpCommands = new List<string>();
        {
            helpCommands.Add("'alcohol' to add alcohol meter");
            helpCommands.Add("'additem' to add items");
            helpCommands.Add("'cc' to use extra commands");
            helpCommands.Add("'giveammos' to give ammos");
            helpCommands.Add("'givemeallweapons' to unlock all weapons");
            helpCommands.Add("'god' to toggle god mode");
            helpCommands.Add("'help level' to see level commands if exist");
            helpCommands.Add("'help status' to look every status effect in the game");
            helpCommands.Add("'killall' to all enemies");
            helpCommands.Add("'killme' to commit suicide");
            helpCommands.Add("'levelnames' gets every level exists in the current build.");
            helpCommands.Add("'loadfile' to load save file");
            helpCommands.Add("'loadlevel' to load level. Resets progress!");
            helpCommands.Add("'mats' to give free materials");
            helpCommands.Add("'nextlevel' to go next level while retaining items");
            helpCommands.Add("'nospeed' to set freecam speed. 'ui 4' to use noclip.");
            helpCommands.Add("'res' to restore health & dash");
            helpCommands.Add("'savefile' to save file");
            helpCommands.Add("'screensize' to set screen size");
            helpCommands.Add("'setfps' to set game's FPS");
            helpCommands.Add("'soul' to get soul");
            helpCommands.Add("'ui' to change UI mode 0/1/2");
            helpCommands.Add("'wstat' to stat world objects. 'help wstat' to show more wstat commands");
        }

        int currentEntry = 0;
        float count1 = helpCommands.Count;
        float entry1 = entryPerPage;
        int totalEntry = Mathf.CeilToInt(count1 / entry1);

        if (args.Length == 0)
        {
            helps.Add($" =============== HELP [{currentEntry}/{totalEntry-1}] =============== ");
            helps.Add("Press ENTER to execute command");
            helps.Add("Press ~ key to toggle console");

            for (int s = currentEntry * entryPerPage; s < (currentEntry + 1) * entryPerPage; s++)
            {
                if (s >= helpCommands.Count) break;

                helps.Add($"{helpCommands[s]}");

            }
        }
        else if (int.TryParse(args[0], out currentEntry))
        {
            helps.Add($" =============== HELP [{currentEntry}/{totalEntry-1}] =============== ");
            helps.Add("Press ENTER to execute command");
            helps.Add("Press ~ key to toggle console");

            for(int s = currentEntry * entryPerPage; s < (currentEntry+1) * entryPerPage; s++)
            {
                if (s >= helpCommands.Count) break;

                helps.Add($"{helpCommands[s]}");

            }
        }
        else
        if (args.Length != 0)
        {
            if (args[0] == "level")
            {
                return;
            }
            else if (args[0] == "wstat")
            {
                helps.Add(" =============== HELP [WORLD STAT commnads] =============== ");
                helps.Add("Press ENTER to execute command");
                helps.Add("Press ~ key to toggle console");
                helps.Add("'wstat lock' to lock on targeted enemy.");
                helps.Add(" ");
            }
            else if (args[0] == "enemy")
            {
                helps.Add(" =============== HELP [ENEMIES commnads] =============== ");
                helps.Add("Press ENTER to execute command");
                helps.Add("Press ~ key to toggle console");
                helps.Add("'enemy hack' to gain control of the enemy.");
                helps.Add("'enemy frenzy' to frenzied an enemy.");
                helps.Add("'enemy burn' to burn the enemy.");
                helps.Add("'enemy poison' to poison the enemy.");
                helps.Add("'enemy heal' to restore enemy's health.");
                helps.Add("'enemy paralyze' to toggle enemy paralyze/deparalyze.");
                helps.Add("'enemy reset' to reset enemy's status.");
                helps.Add("'enemy warp' to warp enemy to gizmo position.");
                helps.Add(" ");
            }
            else if (args[0] == "weapon")
            {
                helps.Add(" =============== HELP [WEAPONS commnads] =============== ");
                helps.Add("Press ENTER to execute command");
                helps.Add("Press ~ key to toggle console");
                helps.Add("'weapon attach (string attachmentID)' to add attachments.");
                helps.Add("'weapon rmvall' to remove all attachments.");
                helps.Add("'weapon report' to get weapon's stat.");
            
                helps.Add(" ");
            }
            else if (args[0] == "status")
            {
                helps.Add(" =============== HELP [EVERY STATUS EFFECTS] =============== ");
                foreach (string name in Enum.GetNames(typeof(StatusEffectCategory)))
                {
                    helps.Add(name);
                }

                helps.Add(" ");
            }

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
