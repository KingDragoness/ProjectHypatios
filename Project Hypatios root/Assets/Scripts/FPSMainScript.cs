﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.ImageEffects;
using DevLocker.Utils;

public class FPSMainScript : MonoBehaviour
{

    public enum CurrentGamemode
    {
        Aldrich,
        Elena,
        TutorialMode
    }

    public CurrentGamemode currentGamemode = CurrentGamemode.Aldrich;
    [FoldoutGroup("Story Selection")] public SceneReference elenaScene;
    [FoldoutGroup("Story Selection")] public SceneReference aldrichScene;
    [FoldoutGroup("References")] public PostProcessVolume postProcessVolume; //DOF, Motion blur, AO, Vignette
    [FoldoutGroup("References")] public PostProcessVolume postProcessVolume_2; //Color grading, bloom
    [FoldoutGroup("References")] public GameObject Prefab_SpawnAmmo;
    [FoldoutGroup("References")] public GameObject Prefab_SpawnSoul;
    [FoldoutGroup("References")] public UnityStandardAssets.ImageEffects.MotionBlur minorMotionBlur;
    [FoldoutGroup("References")] public List<BasePerk> AllBasePerks;

    [Header("Saves")]
    public int SoulPoint = 0;
    public int TotalRuns = 0;
    public float UNIX_Timespan = 0;
    public bool everUsed_Paradox = false;
    public bool everUsed_WeaponShop = false;

    public List<HypatiosSave.WeaponDataSave> currentWeaponStat;
    public List<ParadoxEntity> paradoxEntities = new List<ParadoxEntity>();
    public List<string> otherEverUsed = new List<string>();

    [Space]
    public bool DEBUG_ShowTutorial = false;

    public static bool LoadFromSaveFile = false;
    public static int Player_RunSessionUnixTime;

    public static readonly string GameSavePath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/Hypatios/Saves";


    public static HypatiosSave savedata; //cached


    private void Awake()
    {

        if (LoadFromSaveFile)
            LoadFromSaveBuffer();

        BuildSaveFolder();
    }

    private void Start()
    {
        var colorGrading_ = Hypatios.Game.postProcessVolume.profile.GetSetting<ColorGrading>();
        var bloom_ = Hypatios.Game.postProcessVolume.profile.GetSetting<UnityEngine.Rendering.PostProcessing.Bloom>();

        colorGrading_.active = false;
        bloom_.active = false;

        MainUI.Instance.settingsUI.inputfield_Name.SetTextWithoutNotify(Hypatios.Settings.MY_NAME);
    }

    private void BuildSaveFolder()
    {
        System.IO.Directory.CreateDirectory(GameSavePath);
    }

    #region Weapon Stats
    public HypatiosSave.WeaponDataSave GetWeaponSave(string ID)
    {
        return currentWeaponStat.Find(x => x.weaponID == ID);
    }

    public void NewWeaponStat(GunScript gunScript)
    {
        HypatiosSave.WeaponDataSave weaponData = currentWeaponStat.Find(x => x.weaponID == gunScript.weaponName);

        if (weaponData == null)
        {
            weaponData = new HypatiosSave.WeaponDataSave();
        }
        else
        {
            return;
        }

        weaponData.weaponID = gunScript.weaponName;

        currentWeaponStat.Add(weaponData);
    }

    #endregion



    public void CommandCheat(string cheatName)
    {
        if (cheatName == "ignoregravity")
        {
            
        }
    }

    public void Update()
    {
        UNIX_Timespan += Time.deltaTime;
    }

    #region Paradox and tips

    public void RuntimeTutorialHelp(string about, string description, string key)
    {
        if (Application.isEditor && DEBUG_ShowTutorial == false)
        {
            return;
        }

        if (Hypatios.Game.Check_EverUsed(key) == false)
        {
            MainGameHUDScript.Instance.ShowPromptUI(about, description, false);
            MainUI.Instance.SetTempoPause(true);
            Hypatios.Game.TryAdd_EverUsed(key);
        }
    }


    public bool Check_EverUsed(string key)
    {
        if (otherEverUsed.Contains(key))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void TryAdd_EverUsed(string key)
    {
        if (Check_EverUsed(key))
        {

        }
        else
        {
            otherEverUsed.Add(key);
        }
    }

    public bool Check_ParadoxEvent(string key)
    {
        string keyName = "PARADOX.EVENT." + key;

        if (otherEverUsed.Contains(keyName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void TryAdd_ParadoxEvent(string key)
    {
        string keyName = "PARADOX.EVENT." + key;
        if (Check_EverUsed(keyName))
        {

        }
        else
        {
            otherEverUsed.Add(keyName);
        }
    }


    public void Clear_ParadoxEvent(string key)
    {
        string keyName = "PARADOX.EVENT." + key;
        if (Check_EverUsed(keyName))
        {
            otherEverUsed.Remove(keyName);
        }
        else
        {
        }
    }

    #endregion

    #region Save System
    private void LoadFromSaveBuffer()
    {

        var Player = Hypatios.Player;
        var Weapons = FindObjectOfType<WeaponManager>();

        //perks
        {
            Player.PerkData = savedata.AllPerkDatas;

        }

        TotalRuns = savedata.Game_TotalRuns;
        SoulPoint = savedata.Game_TotalSouls;
        UNIX_Timespan = savedata.Player_RunSessionUnixTime;
        currentWeaponStat = savedata.Game_WeaponStats;
        Player.Health.targetHealth = savedata.Player_CurrentHP;
        Player.Health.curHealth = savedata.Player_CurrentHP;
        Player.PerkData.Temp_CustomPerk = savedata.AllPerkDatas.Temp_CustomPerk;
        everUsed_Paradox = savedata.everUsed_Paradox;
        everUsed_WeaponShop = savedata.everUsed_WeaponShop;
        otherEverUsed = savedata.otherEverUsed;
        paradoxEntities = savedata.Game_ParadoxEntities;
        Hypatios.Player.Initialize();
        Weapons.LoadGame_InitializeGameSetup();

        LoadFromSaveFile = false;
    }

    public void LoadGameFromKilled()
    {
        var Player = FindObjectOfType<CharacterScript>();

        //perks
        Player.PerkData = savedata.AllPerkDatas;

        TotalRuns = savedata.Game_TotalRuns;
        SoulPoint = savedata.Game_TotalSouls;
        UNIX_Timespan = savedata.Player_RunSessionUnixTime;
        currentWeaponStat = savedata.Game_WeaponStats;
        Player.Health.targetHealth = Player.Health.maxHealth.Value;
        Player.PerkData.Temp_CustomPerk = savedata.AllPerkDatas.Temp_CustomPerk;
        everUsed_Paradox = savedata.everUsed_Paradox;
        everUsed_WeaponShop = savedata.everUsed_WeaponShop;
        otherEverUsed = savedata.otherEverUsed;
        paradoxEntities = savedata.Game_ParadoxEntities;

    }

    private HypatiosSave PackSaveData(int targetLevel = -1)
    {
        HypatiosSave hypatiosSave = new HypatiosSave();
        var Player = FindObjectOfType<CharacterScript>();
        var weaponManager = FindObjectOfType<WeaponManager>();

        if (targetLevel == -1)
        {
            hypatiosSave.Game_LastLevelPlayed = Application.loadedLevel;
        }
        else
        {
            hypatiosSave.Game_LastLevelPlayed = targetLevel;
        }

        hypatiosSave.Game_TotalRuns = TotalRuns;
        hypatiosSave.Game_TotalSouls = SoulPoint;
        hypatiosSave.Player_RunSessionUnixTime = Mathf.RoundToInt(UNIX_Timespan);
        hypatiosSave.Player_CurrentHP = Player.Health.curHealth;
        hypatiosSave.Game_WeaponStats = currentWeaponStat;
        hypatiosSave.Game_ParadoxEntities = paradoxEntities;
        hypatiosSave.AllPerkDatas.Temp_CustomPerk = Player.PerkData.Temp_CustomPerk;
        hypatiosSave.everUsed_Paradox = everUsed_Paradox;
        hypatiosSave.everUsed_WeaponShop = everUsed_WeaponShop;
        hypatiosSave.otherEverUsed = otherEverUsed;
        hypatiosSave.AllPerkDatas = Player.PerkData;


        foreach (var weaponDataResource in weaponManager.weapons)
        {
            HypatiosSave.WeaponDataSave weaponDataSave = GetWeaponSave(weaponDataResource.nameWeapon);

            if (weaponDataSave == null)
            {
                continue;
            }

            var gunScript = weaponManager.GetGunScript(weaponDataResource.nameWeapon);

            if (gunScript == null)
            {
                continue;
            }

            weaponDataSave.currentAmmo = gunScript.curAmmo;
            weaponDataSave.totalAmmo = gunScript.totalAmmo;
            weaponDataSave.weaponID = gunScript.weaponName;

        }

        return hypatiosSave;
    }

    public void Death_NoSaturation()
    {
        var colorGrading_ = Hypatios.Game.postProcessVolume_2.profile.GetSetting<ColorGrading>();
        var saturation = colorGrading_.saturation;

        saturation.value = -90;
    }

    public void PlayerDie()
    {
        //If elena mode do nothing
        if (currentGamemode == CurrentGamemode.Elena)
        {
            //instance.BufferSaveData();
            return;
        }

        //On Aldrich mode, the player start from beginning

        TotalRuns++;

        PlayerPrefs.DeleteKey("HIGHSCORE.CHECK");

        HypatiosSave hypatiosSave = PackSaveData();
        hypatiosSave.Player_CurrentHP = 100;
        hypatiosSave.Player_RunSessionUnixTime = 0;
        hypatiosSave.Game_LastLevelPlayed = 4;
        hypatiosSave.AllPerkDatas.Temp_CustomPerk.Clear();

         Player_RunSessionUnixTime = Mathf.RoundToInt(UNIX_Timespan);

        foreach (var weaponStat in hypatiosSave.Game_WeaponStats)
        {
            weaponStat.currentAmmo = 0;
            weaponStat.totalAmmo = 0;
            weaponStat.removed = true;
        }

        SaveGame(targetLevel: 4, hypatiosSave: hypatiosSave);

        string pathLoad = "";
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        pathLoad = GameSavePath + "/defaultSave.save";


        try
        {
            savedata = JsonConvert.DeserializeObject<HypatiosSave>(File.ReadAllText(pathLoad), settings);
        }
        catch
        {
            Debug.LogError("Failed load!");
            ConsoleCommand.Instance.SendConsoleMessage("Save file cannot be loaded!");
        }
    }

    public void SaveGame(string path = "", int targetLevel = -1, HypatiosSave hypatiosSave = null)
    {
        string pathSave = "";

        if (path == "")
        {
            pathSave = GameSavePath + "/defaultSave.save";
        }

        print(pathSave);

        var saveData = PackSaveData(targetLevel);

        if (hypatiosSave != null)
        {
            saveData = hypatiosSave;
        }

        string jsonTypeNameAll = JsonConvert.SerializeObject(saveData, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        });


        MainUI.Instance.SavingGameIcon_UI.gameObject.SetActive(true);
        File.WriteAllText(pathSave, jsonTypeNameAll);
        if (ConsoleCommand.Instance != null) ConsoleCommand.Instance.SendConsoleMessage($"File has been saved to {pathSave}");

    }


    public void BufferSaveData(string path = "")
    {
        string pathLoad = "";
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };


        if (path == "")
        {
            pathLoad = GameSavePath + "/defaultSave.save";
        }

        try
        {
            savedata = JsonConvert.DeserializeObject<HypatiosSave>(File.ReadAllText(pathLoad), settings);

            Debug.Log("buffer save test 1");
            LoadFromSaveFile = true;

        }
        catch
        {
            Debug.LogError("Failed load!");
            ConsoleCommand.Instance.SendConsoleMessage("Save file cannot be loaded!");
        }


    }

    public static bool CheckSaveFileExist()
    {
        string pathLoad = "";
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        pathLoad = FPSMainScript.GameSavePath + "/defaultSave.save";

        try
        {
            var tempSave = JsonConvert.DeserializeObject<HypatiosSave>(File.ReadAllText(pathLoad), settings);

            if (tempSave == null)
            {
                return false;

            }
            else
            {
                return true;
            }

        }
        catch
        {
            return false;

        }

    }

    //Load player cutscene char select
    public void Menu_StartPlayGame()
    {
        Application.LoadLevel(3);

    }

    public void Menu_ResumeGame()
    {
        string pathLoad = "";
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        pathLoad = GameSavePath + "/defaultSave.save";

        try
        {
            savedata = JsonConvert.DeserializeObject<HypatiosSave>(File.ReadAllText(pathLoad), settings);

            Application.LoadLevel(savedata.Game_LastLevelPlayed);
            LoadFromSaveFile = true;

        }
        catch
        {
            throw new Exception("LOAD GAME FAILED!");
        }
    }

    public void Menu_StartElenaStory()
    {
        int index = elenaScene.Index;
        Application.LoadLevel(index);

    }

    public void Menu_StartAldrich()
    {
        int index = aldrichScene.Index;
        Application.LoadLevel(index);

    }

    private void Legacy_StartPlayGame()
    {
        string pathLoad = "";
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        pathLoad = GameSavePath + "/defaultSave.save";

        try
        {
            savedata = JsonConvert.DeserializeObject<HypatiosSave>(File.ReadAllText(pathLoad), settings);

            Application.LoadLevel(savedata.Game_LastLevelPlayed);
            LoadFromSaveFile = true;

        }
        catch
        {
            Application.LoadLevel(3);
        }
    }

    public void LoadGame(string path = "")
    {
        string pathLoad = "";
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };


        if (path == "")
        {
            pathLoad = GameSavePath + "/defaultSave.save";
        }



        try
        {
            savedata = JsonConvert.DeserializeObject<HypatiosSave>(File.ReadAllText(pathLoad), settings);

            Application.LoadLevel(savedata.Game_LastLevelPlayed);
            LoadFromSaveFile = true;

        }
        catch
        {
            Debug.LogError("Failed load!");
            ConsoleCommand.Instance.SendConsoleMessage("Save file cannot be loaded!");
        }


    }

    public static void CacheLoadSave(string path = "")
    {
        string pathLoad = "";
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        if (path == "")
        {
            pathLoad = GameSavePath + "/defaultSave.save";
        }

        try
        {
            savedata = JsonConvert.DeserializeObject<HypatiosSave>(File.ReadAllText(pathLoad), settings);
        }
        catch
        {
            Debug.LogError("Failed load!");
            ConsoleCommand.Instance.SendConsoleMessage("Save file cannot be loaded!");
        }


    }

    #endregion
}
