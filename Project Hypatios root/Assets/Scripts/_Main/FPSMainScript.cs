﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.ImageEffects;
using DevLocker.Utils;
using static HypatiosSave;

public class FPSMainScript : MonoBehaviour
{


    public Hypatios_Gamemode currentGamemode;
    public UnityEvent OnLoadFromMachineMadness;
    [FoldoutGroup("Story Selection")] public SceneReference elenaScene;
    [FoldoutGroup("Story Selection")] public SceneReference aldrichScene;
    [FoldoutGroup("Story Selection")] public SceneReference level1_Scene;
    [FoldoutGroup("Story Selection")] public SceneReference deadScene;
    [FoldoutGroup("Story Selection")] public SceneReference eclipseBlazerScene;
    [FoldoutGroup("Story Selection")] public SceneReference gameSelectionScene;
    [FoldoutGroup("References")] public PostProcessVolume postProcessVolume; //DOF, Motion blur, AO, Vignette
    [FoldoutGroup("References")] public PostProcessVolume postProcessVolume_2; //Color grading, bloom
    [FoldoutGroup("References")] public PostProcessVolume postProcessVolumeUI; //UI TV effects, lens distortion
    [FoldoutGroup("References")] public PostProcessLayer postProcessLayer_Player;
    [FoldoutGroup("References")] public PostProcessLayer postProcessLayer_UI;
    [FoldoutGroup("References")] public RefillAmmoPlayer Prefab_SpawnAmmo;
    [FoldoutGroup("References")] public SoulCapsulePlayer Prefab_SpawnSoul;
    [FoldoutGroup("References")] public HealPlayer Prefab_SpawnHeal;
    [FoldoutGroup("References")] public UnityStandardAssets.ImageEffects.MotionBlur minorMotionBlur;
    [FoldoutGroup("Cheat")] public GameObject cheatContainer;
    [Header("Saves")]
    public int SoulPoint = 0;
    public int TotalRuns = 0;
    public int TotalLevelPassed = 0; //reset per run
    public float UNIX_Timespan = 0;
    public float Total_UNIX_Timespan = 0;
    public bool everUsed_Paradox = false;
    public bool everUsed_WeaponShop = false;
    public PlayerStatSave persistent_PlayerStat;
    public PlayerStatSave run_PlayerStat;

    public List<WeaponDataSave> currentWeaponStat;
    public List<ParadoxEntity> paradoxEntities = new List<ParadoxEntity>();
    public List<TriviaSave> Game_Trivias = new List<HypatiosSave.TriviaSave>();
    public List<GlobalFlagSave> Game_GlobalFlags = new List<GlobalFlagSave>();
    public List<ChamberDataSave> Game_ChamberSaves = new List<ChamberDataSave>();
    public List<ShareCompanySave> PortfolioShares = new List<HypatiosSave.ShareCompanySave>();
    public List<string> otherEverUsed = new List<string>();
    public List<string> favoritedItems = new List<string>();

    [Space]
    public bool DEBUG_ShowTutorial = false;
    public bool DEBUG_ShowAllParadox = false;
    public bool DEBUG_ShowAllSaves = false;
    public bool DEBUG_UnlockAllParadox = false;

    public static bool LoadFromSaveFile = false;
    public static bool LoadFromMachineMadness = false;
    public static int Player_RunSessionUnixTime;

    public static readonly string GameSavePath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/Hypatios/Saves";


    public static HypatiosSave savedata; //cached

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        LoadFromSaveFile = false;
        Player_RunSessionUnixTime = 0;
    }

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

        //MainUI.Instance.settingsUI.inputfield_Name.SetTextWithoutNotify(Hypatios.Settings.MY_NAME);

        if (LoadFromMachineMadness)
        {
            OnLoadFromMachineMadness?.Invoke();
            LoadFromMachineMadness = false;
        }
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

    public void NewWeaponStat(BaseWeaponScript weaponScript)
    {
        HypatiosSave.WeaponDataSave weaponData = currentWeaponStat.Find(x => x.weaponID == weaponScript.weaponName);

        if (weaponData == null)
        {
            weaponData = new HypatiosSave.WeaponDataSave();
        }
        else
        {
            return;
        }

        weaponData.weaponID = weaponScript.weaponName;

        currentWeaponStat.Add(weaponData);
    }

    #endregion

    #region Player Stats

    public PlayerStatValueSave Get_StatEntryData(BaseStatValue statEntry, bool isPersistent = false)
    {
        if (statEntry == null)
        {
            Debug.LogError("Stat Entry not assigned!");
            return null;
        }
        PlayerStatValueSave data = null;

        if (isPersistent)
        {
            data = persistent_PlayerStat.GetValueStat(statEntry);
        }
        else
        {
            data = run_PlayerStat.GetValueStat(statEntry);

        }

        return data;
    }

    public void Increment_PlayerStat(string id)
    {
        var statEntry = Hypatios.Assets.AllStatEntries.Find(x => x.ID == id);

        if (statEntry == null)
        {
            Debug.LogError("No stat entry detected.");
            return;
        }

        var persistenceEntry = persistent_PlayerStat.GetValueStat(statEntry);
        var currentRunEntry = run_PlayerStat.GetValueStat(statEntry);

        persistenceEntry.value_int++;
        currentRunEntry.value_int++;
    }


    public void Increment_PlayerStat(BaseStatValue statEntry)
    {
        if (statEntry == null)
        {
            Debug.LogError("Stat Entry not assigned!");
            return;
        }

        var persistenceEntry = persistent_PlayerStat.GetValueStat(statEntry);
        var currentRunEntry = run_PlayerStat.GetValueStat(statEntry);

        persistenceEntry.value_int++;
        currentRunEntry.value_int++;
    }

    public void Add_PlayerStat(BaseStatValue statEntry, int amount = 0)
    {
        if (statEntry == null)
        {
            Debug.LogError("Stat Entry not assigned!");
            return;
        }

        var persistenceEntry = persistent_PlayerStat.GetValueStat(statEntry);
        var currentRunEntry = run_PlayerStat.GetValueStat(statEntry);

        persistenceEntry.value_int += amount;
        currentRunEntry.value_int += amount;
    }

    #endregion

    public void CommandCheat(string cheatName)
    {
        if (cheatName == "sentry")
        {
            ConsoleCommand.Instance.CommandInput("mats");
            ConsoleCommand.Instance.CommandInput("additem SentryPDA");
        }
        if (cheatName == "lv5")
        {
            ConsoleCommand.Instance.CommandInput("mats");
            ConsoleCommand.Instance.CommandInput("setperk 50 5");
            ConsoleCommand.Instance.CommandInput("setperk 6 300");
            ConsoleCommand.Instance.CommandInput("soul 999");
            ConsoleCommand.Instance.CommandInput("res");
            cheatContainer.gameObject.SetActive(true);
            cheatContainer.transform.position = Hypatios.Player.transform.position + Hypatios.Player.transform.forward;
        }
        if (cheatName == "lv10")
        {
            ConsoleCommand.Instance.CommandInput("mats");
            ConsoleCommand.Instance.CommandInput("setperk 50 10");
            ConsoleCommand.Instance.CommandInput("setperk 6 600");
            ConsoleCommand.Instance.CommandInput("soul 999");
            ConsoleCommand.Instance.CommandInput("res");
            cheatContainer.gameObject.SetActive(true);
            cheatContainer.transform.position = Hypatios.Player.transform.position + Hypatios.Player.transform.forward;

        }
        if (cheatName == "tt")
        {
            for (int x = 0; x < 30; x++)
            {
                var trivia = Hypatios.Assets.AllTrivias[x];
                if (trivia.disableTrivia) continue;
                trivia.TriggerTrivia();
            }
        }
    }

    public void Update()
    {
        UNIX_Timespan += Time.deltaTime;
        Total_UNIX_Timespan += Time.deltaTime;

        if (SoulPoint < 0) SoulPoint = 0;
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
            //MainUI.Instance.SetTempoPause(true);
            Hypatios.Game.TryAdd_EverUsed(key);
        }
    }

    public bool Check_TriviaCompleted(Trivia triviaTarget)
    {
        var triviaEntry = Game_Trivias.Find(x => x.ID == triviaTarget.ID);

        if (triviaEntry != null)
        {
            if (triviaEntry.isCompleted)
                return true;

            return false;
        }
        else
        {
            return false;
        }
    }

    public void TriviaComplete(Trivia triviaTarget)
    {
        var triviaEntry1 = Game_Trivias.Find(x => x.ID == triviaTarget.ID);
        if (triviaEntry1 != null) return;

        var triviaEntry = new HypatiosSave.TriviaSave();
        triviaEntry.ID = triviaTarget.ID;
        triviaEntry.isCompleted = true;
        MainGameHUDScript.Instance.triviaUI.gameObject.SetActive(true);
        MainGameHUDScript.Instance.typewriter_Trivia.TypeThisDialogue($">TRIVIA ADDED:\n{triviaTarget.Title} Completed.");
        OnTriviaTriggered.OnActionTriviaTrigger?.Invoke(triviaTarget);

        Game_Trivias.Add(triviaEntry);
    }

    public bool Check_FlagTriggered(string flagID)
    {
        var triviaEntry = Game_GlobalFlags.Find(x => x.ID == flagID);

        if (triviaEntry != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void TriggerFlag(string flagID, int run)
    {
        var flagEntry1 = Game_GlobalFlags.Find(x => x.ID == flagID);
        if (flagEntry1 != null) return;

        var flagSO = Hypatios.Assets.GetGlobalFlag(flagID);
        GlobalFlagSave newFlag = new GlobalFlagSave(flagID, run);
        if (flagSO.PrefabToSpawn != null) Instantiate(flagSO.PrefabToSpawn);

        Game_GlobalFlags.Add(newFlag);
    }

    public void SetParadoxEntity(string key, string value)
    {
        var entity = Hypatios.Game.paradoxEntities.Find(x => x.ID == key);

        if (entity == null)
        {
            entity = new ParadoxEntity();
            entity.ID = key;
            entity.value = value;
            paradoxEntities.Add(entity);
        }
        else
        {
            entity.value = value;
        }

        Debug.Log($"{entity.ID} has been set value: {entity.value}");
    }

    public string GetParadoxEntityValue(string key)
    {
        var entity = Hypatios.Game.paradoxEntities.Find(x => x.ID == key);

        if (entity == null)
            return "";

        return entity.value;
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

    public bool IsItemFavorited(string ID)
    {
        if (favoritedItems.Find(x => x == ID) != null)
        {
            return true;
        }
        return false;
    }    

    public void AddFavorite(string ID)
    {
        SanityCheck_WeaponFavorite();

        if (IsItemFavorited(ID))
        {
            //already favorited.
            return;
        }
        favoritedItems.Add(ID);
    }

    //sanity check to remove weapon favorites
    private void SanityCheck_WeaponFavorite()
    {
        foreach (var item in Hypatios.Assets.AllItems)
        {
            if (item.category == ItemInventory.Category.Weapon)
            {
                if (IsItemFavorited(item.GetID()))
                    favoritedItems.Remove(item.GetID());

            }
        }
    }

    public void RemoveFavorite(string ID)
    {
        SanityCheck_WeaponFavorite();

        if (IsItemFavorited(ID) == false)
        {
            //already not favorited.
            return;
        }
        favoritedItems.Remove(ID);
    }



    #region Transit Level Save System
    public void SaveGame_TransitLevel(string objectJson)
    {
        string pathSave = $"{GameSavePath}/saveLevel_{TotalLevelPassed}.save";
        File.WriteAllText(pathSave, objectJson);
        if (ConsoleCommand.Instance != null) ConsoleCommand.Instance.SendConsoleMessage($"Level transit save file #{TotalLevelPassed.ToString("D2")} has been saved to {pathSave}");
    }

    [FoldoutGroup("DEBUG")] [Button("Test Transit Level save")]
    public void DEBUG_TestTransitLvSaves()
    {
        var levelFiles = GetListLevelTransitSave();

        foreach(var lv in levelFiles)
        {
            Debug.Log(lv);
        }
    }


    public List<string> GetListLevelTransitSave()
    {
        List<string> listResult = new List<string>();
        DirectoryInfo d = new DirectoryInfo($"{GameSavePath}");
        FileInfo[] Files = d.GetFiles("*.save");

        foreach (FileInfo file in Files)
        {
            if (file.Name.StartsWith("saveLevel")) listResult.Add(file.FullName);
        }


        return listResult;
    }

    public List<HypatiosSave> GetAllLevelSaves()
    {
        List<HypatiosSave> listResult = new List<HypatiosSave>();
        var allPath = GetListLevelTransitSave();

        foreach(var path in allPath)
        {
            HypatiosSave saveFile = UnpackSaveFile(path);
            if (saveFile != null)
            {
                listResult.Add(saveFile);
            }
        }

        return listResult;
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
        TotalLevelPassed = savedata.Total_Level_Passed;
        UNIX_Timespan = savedata.Player_RunSessionUnixTime;
        Total_UNIX_Timespan = savedata.Game_UnixTime;
        currentWeaponStat = savedata.Game_WeaponStats;
        Player.Health.targetHealth = savedata.Player_CurrentHP;
        Player.Health.curHealth = savedata.Player_CurrentHP;
        Player.Health.alcoholMeter = savedata.Player_AlchoholMeter;
        Player.PerkData.Temp_CustomPerk = savedata.AllPerkDatas.Temp_CustomPerk;
        Player.PerkData.Temp_StatusEffect = savedata.AllPerkDatas.Temp_StatusEffect;
        everUsed_Paradox = savedata.everUsed_Paradox;
        everUsed_WeaponShop = savedata.everUsed_WeaponShop;
        otherEverUsed = savedata.otherEverUsed;
        favoritedItems = savedata.favoritedItems;
        paradoxEntities = savedata.Game_ParadoxEntities;
        Game_Trivias = savedata.Game_Trivias;
        Game_GlobalFlags = savedata.Game_GlobalFlags;
        Game_ChamberSaves = savedata.Game_ChamberSaves;
        PortfolioShares = savedata.PortfolioShares;
        Player.Inventory.allItemDatas = savedata.Player_Inventory;
        persistent_PlayerStat = savedata.persistent_PlayerStat;
        run_PlayerStat = savedata.run_PlayerStat;
        Hypatios.Player.Initialize();
        Weapons.LoadGame_InitializeGameSetup();

        if (savedata.sceneEntryCache == null) savedata.sceneEntryCache = new HypatiosSave.EntryCache();
        if (savedata.sceneEntryCache.entryIndex == -1)
        {
            Player.transform.position = savedata.sceneEntryCache.cachedPlayerPos;
        }

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
        Total_UNIX_Timespan = savedata.Game_UnixTime;
        currentWeaponStat = savedata.Game_WeaponStats;
        Player.Health.targetHealth = Player.Health.maxHealth.Value;
        Player.Health.HealthSpeed = 50f;
        Player.PerkData.Temp_CustomPerk = savedata.AllPerkDatas.Temp_CustomPerk;
        Player.PerkData.Temp_StatusEffect.RemoveAll(x => x.Time < 9999); // = savedata.AllPerkDatas.Temp_StatusEffect.FindAll(x => x.Time >= 9999);
        everUsed_Paradox = savedata.everUsed_Paradox;
        everUsed_WeaponShop = savedata.everUsed_WeaponShop;
        otherEverUsed = savedata.otherEverUsed;
        favoritedItems = savedata.favoritedItems;
        paradoxEntities = savedata.Game_ParadoxEntities;
        Game_GlobalFlags = savedata.Game_GlobalFlags;
        Game_Trivias = savedata.Game_Trivias;
        PortfolioShares = savedata.PortfolioShares;
        Game_ChamberSaves = savedata.Game_ChamberSaves;
        //Player.Inventory = savedata.Player_Inventory;
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

        if (hypatiosSave.sceneEntryCache == null) hypatiosSave.sceneEntryCache = new HypatiosSave.EntryCache();

        hypatiosSave.Game_Version = Application.version;
        hypatiosSave.Game_DemoMode = Hypatios.IsDemoMode;
        hypatiosSave.AllPerkDatas = Player.PerkData;
        hypatiosSave.PortfolioShares = PortfolioShares;
        hypatiosSave.Game_TotalRuns = TotalRuns;
        hypatiosSave.Game_TotalSouls = SoulPoint;
        hypatiosSave.Game_WeaponStats = currentWeaponStat;
        hypatiosSave.Game_ParadoxEntities = paradoxEntities;
        hypatiosSave.Game_Trivias = Game_Trivias;
        hypatiosSave.Game_GlobalFlags = Game_GlobalFlags;
        hypatiosSave.Game_ChamberSaves = Game_ChamberSaves;
        hypatiosSave.Game_UnixTime = Mathf.RoundToInt(Total_UNIX_Timespan);
        hypatiosSave.Player_RunSessionUnixTime = Mathf.RoundToInt(UNIX_Timespan);
        hypatiosSave.Total_Level_Passed = TotalLevelPassed;
        hypatiosSave.Player_CurrentHP = Player.Health.curHealth;
        hypatiosSave.Player_AlchoholMeter = Player.Health.alcoholMeter;
        hypatiosSave.Player_Inventory = Player.Inventory.allItemDatas;
        hypatiosSave.persistent_PlayerStat = persistent_PlayerStat;
        hypatiosSave.run_PlayerStat = run_PlayerStat;
        hypatiosSave.AllPerkDatas.Temp_CustomPerk = Player.PerkData.Temp_CustomPerk;
        hypatiosSave.AllPerkDatas.Temp_StatusEffect.RemoveAll(x => x.Time < 9999); //= Player.PerkData.Temp_StatusEffect.FindAll(x => x.Time >= 9999);
        hypatiosSave.everUsed_Paradox = everUsed_Paradox;
        hypatiosSave.everUsed_WeaponShop = everUsed_WeaponShop;
        hypatiosSave.otherEverUsed = otherEverUsed;
        hypatiosSave.favoritedItems = favoritedItems;


        foreach (var weaponDataResource in Hypatios.Assets.Weapons)
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

    public static void WipeCurrentRunProgress(HypatiosSave hypatiosSave)
    {
        hypatiosSave.Total_Level_Passed = 0;
        hypatiosSave.Player_CurrentHP = 100;
        hypatiosSave.Player_AlchoholMeter = 0f;
        hypatiosSave.Player_RunSessionUnixTime = 0;
        hypatiosSave.Game_LastLevelPlayed = Hypatios.Game.level1_Scene.Index;
        hypatiosSave.AllPerkDatas.Temp_CustomPerk.Clear();
        hypatiosSave.AllPerkDatas.Temp_StatusEffect.Clear();
        hypatiosSave.Player_Inventory = new List<ItemDataSave>();
        hypatiosSave.run_PlayerStat = new PlayerStatSave();
        hypatiosSave.Game_WeaponStats.Clear();

        foreach(var flag in hypatiosSave.Game_GlobalFlags)
        {
            flag.runRemaining--;
        }

        hypatiosSave.Game_GlobalFlags.RemoveAll(x => x.runRemaining <= 0);
    }

    public void PlayerDie()
    {
        if (currentGamemode.character != Hypatios_Gamemode.MainCharacter.Aldrich)
            return;
        if (currentGamemode.canSaveGame == false)
            return;

        //On Aldrich mode, the player start from beginning

        TotalRuns++;

        PlayerPrefs.DeleteKey("HIGHSCORE.CHECK");

        HypatiosSave hypatiosSave = PackSaveData();
        WipeCurrentRunProgress(hypatiosSave);

        Player_RunSessionUnixTime = Mathf.RoundToInt(UNIX_Timespan);

        SaveGame(targetLevel: level1_Scene.Index, hypatiosSave: hypatiosSave);

        string pathLoad = "";
        JsonSerializerSettings settings = JsonSettings();
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

    public void MachineMadnessSave(HypatiosSave targetSaveFile)
    {
        var _currentSave = PackSaveData();

        _currentSave.Total_Level_Passed = targetSaveFile.Total_Level_Passed;
        _currentSave.Player_CurrentHP = targetSaveFile.Player_CurrentHP;
        _currentSave.Player_AlchoholMeter = targetSaveFile.Player_AlchoholMeter;
        _currentSave.Player_RunSessionUnixTime = targetSaveFile.Player_RunSessionUnixTime;
        _currentSave.Game_LastLevelPlayed = targetSaveFile.Game_LastLevelPlayed;
        _currentSave.AllPerkDatas = targetSaveFile.AllPerkDatas;
        _currentSave.Player_Inventory = targetSaveFile.Player_Inventory;
        _currentSave.run_PlayerStat = targetSaveFile.run_PlayerStat;
        _currentSave.Game_WeaponStats = targetSaveFile.Game_WeaponStats;

        SaveGame(hypatiosSave: _currentSave);
        savedata = _currentSave;
        LoadFromSaveFile = true;
        LoadFromMachineMadness = true;
    }

    public void SaveGame(string path = "", int targetLevel = -1, HypatiosSave hypatiosSave = null, HypatiosSave.EntryCache EntryToken = null)
    {
        string pathSave = path;
        bool isSameLevel = true;

        if (path == "") pathSave = GameSavePath + "/defaultSave.save";

        print(pathSave);
        if (Application.loadedLevel != targetLevel)
        {
            TotalLevelPassed++;
            isSameLevel = false;
        }

        var _saveData = PackSaveData(targetLevel);

        if (hypatiosSave != null)
        {
            _saveData = hypatiosSave;
        }


        if (EntryToken != null)
        {
            _saveData.sceneEntryCache = EntryToken;
        }
        else
        {
            _saveData.sceneEntryCache.entryIndex = 0;
        }

        string jsonTypeNameAll = JsonConvert.SerializeObject(_saveData, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        });


        MainUI.Instance.SavingGameIcon_UI.gameObject.SetActive(true);
        File.WriteAllText(pathSave, jsonTypeNameAll);
        if (isSameLevel == false) SaveGame_TransitLevel(jsonTypeNameAll);
        if (ConsoleCommand.Instance != null) ConsoleCommand.Instance.SendConsoleMessage($"File has been saved to {pathSave}");

    }

    public void BufferSaveData(string path = "")
    {
        string pathLoad = "";
        JsonSerializerSettings settings = JsonSettings();


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
        JsonSerializerSettings settings = JsonSettings();

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

    /// <summary>
    /// 22-06-2023: 
    /// Elena levels removed from the game to reduce scope.
    /// Now straight to Aldrich level.
    /// </summary>
    //Load player cutscene char select
    public void Menu_StartPlayGame()
    {
        int index = aldrichScene.Index;
        Application.LoadLevel(index);
    }

    public void Menu_ResumeGame()
    {
        string pathLoad = "";
        JsonSerializerSettings settings = JsonSettings();

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
        savedata = null;
        LoadFromSaveFile = false;
        Application.LoadLevel(index);

    }

    public void Menu_StartAldrich()
    {
        int index = aldrichScene.Index;
        savedata = null;
        LoadFromSaveFile = false;
        Application.LoadLevel(index);

    }

    private void Legacy_StartPlayGame()
    {
        string pathLoad = "";
        JsonSerializerSettings settings = JsonSettings();

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
        JsonSerializerSettings settings = JsonSettings();


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

    public HypatiosSave UnpackSaveFile(string path)
    {
        HypatiosSave result = null;
        JsonSerializerSettings settings = JsonSettings();

        try
        {
            result = JsonConvert.DeserializeObject<HypatiosSave>(File.ReadAllText(path), settings);
        }
        catch
        {
            Debug.LogError("Failed load!");
            ConsoleCommand.Instance.SendConsoleMessage("Save file cannot be loaded!");
        }

        return result;
    }

    public static void CacheLoadSave(string path = "")
    {
        string pathLoad = "";
        JsonSerializerSettings settings = JsonSettings();

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

    public static JsonSerializerSettings JsonSettings()
    {
        JsonSerializerSettings settings = new JsonSerializerSettings { 
            TypeNameHandling = TypeNameHandling.All, 
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        return settings;

    }

    #endregion
}
