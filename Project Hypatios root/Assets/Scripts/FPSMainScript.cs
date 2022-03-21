using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.ImageEffects;

public class FPSMainScript : MonoBehaviour
{

    public enum CurrentGamemode
    {
        Aldrich,
        Elena,
        TutorialMode
    }

    public CurrentGamemode currentGamemode = CurrentGamemode.Aldrich;

    [Header("Saves")]
    public int SoulPoint = 0;
    public int TotalRuns = 0;
    public int LuckOfGod_Level = 0;
    public float UNIX_Timespan = 0;
    public bool everUsed_Paradox = false;
    public bool everUsed_WeaponShop = false;
    public PostProcessVolume postProcessVolume; //DOF, Motion blur, AO, Vignette
    public PostProcessVolume postProcessVolume_2; //Color grading, bloom
    public List<HypatiosSave.WeaponDataSave> currentWeaponStat;
    public List<ParadoxEntity> paradoxEntities = new List<ParadoxEntity>();
    public List<string> otherEverUsed = new List<string>();

    [Space]
    public GameObject Prefab_SpawnAmmo;
    public GameObject Prefab_SpawnSoul;
    public UnityStandardAssets.ImageEffects.MotionBlur minorMotionBlur;

    [Space]
    public bool DEBUG_ShowTutorial = false;

    public static FPSMainScript instance;
    public static bool LoadFromSaveFile = false;
    public static int Player_RunSessionUnixTime;

    public static readonly string GameSavePath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/Hypatios/Saves";


    public static HypatiosSave savedata;


    private void Awake()
    {
        instance = this;

        if (LoadFromSaveFile)
            LoadFromSaveBuffer();

        BuildSaveFolder();
    }

    private void Start()
    {


        var colorGrading_ = FPSMainScript.instance.postProcessVolume.profile.GetSetting<ColorGrading>();
        var bloom_ = FPSMainScript.instance.postProcessVolume.profile.GetSetting<UnityEngine.Rendering.PostProcessing.Bloom>();

        colorGrading_.active = false;
        bloom_.active = false;


        {
            if (PlayerPrefs.HasKey("SETTINGS.MY_NAME"))
            {
                SettingsUI.MY_NAME = PlayerPrefs.GetString("SETTINGS.MY_NAME");
            }
            else
            {
                SettingsUI.MY_NAME = "Aldrich";
            }
        }

        MainUI.Instance.settingsUI.inputfield_Name.SetTextWithoutNotify(SettingsUI.MY_NAME);


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

    public void NewWeaponStat(gunScript gunScript)
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

    public void RuntimeTutorialHelp(string about, string description, string key)
    {
        if (Application.isEditor && DEBUG_ShowTutorial == false)
        {
            return;
        }

        if (FPSMainScript.instance.Check_EverUsed(key) == false)
        {
            MainGameHUDScript.Instance.ShowPromptUI(about, description, false);
            MainUI.Instance.SetTempoPause(true);
            FPSMainScript.instance.TryAdd_EverUsed(key);
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

    #region Save System
    private void LoadFromSaveBuffer()
    {

        var characterScript = FindObjectOfType<characterScript>();
        var weaponManager = FindObjectOfType<weaponManager>();

        TotalRuns = savedata.Game_TotalRuns;
        SoulPoint = savedata.Game_TotalSouls;
        UNIX_Timespan = savedata.Player_RunSessionUnixTime;
        currentWeaponStat = savedata.Game_WeaponStats;
        LuckOfGod_Level = savedata.Game_Upgrade_LuckOfGod;
        characterScript.heal.targetHealth = savedata.Player_CurrentHP;
        characterScript.heal.curHealth = savedata.Player_CurrentHP;
        characterScript.heal.maxHealth = savedata.Game_Upgrade_MaxHP;
        characterScript.heal.healthRegen = savedata.Game_Upgrade_RegenHP;
        everUsed_Paradox = savedata.everUsed_Paradox;
        everUsed_WeaponShop = savedata.everUsed_WeaponShop;
        otherEverUsed = savedata.otherEverUsed;
        paradoxEntities = savedata.Game_ParadoxEntities;
        weaponManager.LoadGame_InitializeGameSetup();

        LoadFromSaveFile = false;
    }

    public void LoadGameFromKilled()
    {
        var characterScript = FindObjectOfType<characterScript>();

        TotalRuns = savedata.Game_TotalRuns;
        SoulPoint = savedata.Game_TotalSouls;
        UNIX_Timespan = savedata.Player_RunSessionUnixTime;
        currentWeaponStat = savedata.Game_WeaponStats;
        LuckOfGod_Level = savedata.Game_Upgrade_LuckOfGod;
        characterScript.heal.maxHealth = savedata.Game_Upgrade_MaxHP;
        characterScript.heal.targetHealth = characterScript.heal.maxHealth;
        characterScript.heal.healthRegen = savedata.Game_Upgrade_RegenHP;
        everUsed_Paradox = savedata.everUsed_Paradox;
        everUsed_WeaponShop = savedata.everUsed_WeaponShop;
        otherEverUsed = savedata.otherEverUsed;
        paradoxEntities = savedata.Game_ParadoxEntities;

    }

    public void Death_NoSaturation()
    {
        var colorGrading_ = FPSMainScript.instance.postProcessVolume_2.profile.GetSetting<ColorGrading>();
        var saturation = colorGrading_.saturation;

        saturation.value = -90;
    }

    public void PlayerDie()
    {
        //If elena mode do nothing
        if (currentGamemode == CurrentGamemode.Elena)
        {
            instance.BufferSaveData();
            return;
        }

        //On Aldrich mode, the player start from beginning

        TotalRuns++;

        PlayerPrefs.DeleteKey("HIGHSCORE.CHECK");

        HypatiosSave hypatiosSave = PackSaveData();
        hypatiosSave.Player_CurrentHP = 100;
        hypatiosSave.Player_RunSessionUnixTime = 0;
        hypatiosSave.Game_LastLevelPlayed = 3;

        Player_RunSessionUnixTime = Mathf.RoundToInt(UNIX_Timespan);

        foreach (var weaponStat in hypatiosSave.Game_WeaponStats)
        {
            weaponStat.currentAmmo = 0;
            weaponStat.totalAmmo = 0;
            weaponStat.removed = true;
        }

        SaveGame(targetLevel: 3, hypatiosSave: hypatiosSave);

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

    private HypatiosSave PackSaveData(int targetLevel = -1)
    {
        HypatiosSave hypatiosSave = new HypatiosSave();
        var characterScript = FindObjectOfType<characterScript>();
        var weaponManager = FindObjectOfType<weaponManager>();

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
        hypatiosSave.Player_CurrentHP = characterScript.heal.curHealth;
        hypatiosSave.Game_Upgrade_MaxHP = characterScript.heal.maxHealth;
        hypatiosSave.Game_Upgrade_RegenHP = characterScript.heal.healthRegen;
        hypatiosSave.Game_Upgrade_LuckOfGod = LuckOfGod_Level;
        hypatiosSave.Game_WeaponStats = currentWeaponStat;
        hypatiosSave.Game_ParadoxEntities = paradoxEntities;
        hypatiosSave.everUsed_Paradox = everUsed_Paradox;
        hypatiosSave.everUsed_WeaponShop = everUsed_WeaponShop;
        hypatiosSave.otherEverUsed = otherEverUsed;

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

    }

    public void Menu_StartAldrich()
    {
        Application.LoadLevel(4);

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

    #endregion
}
