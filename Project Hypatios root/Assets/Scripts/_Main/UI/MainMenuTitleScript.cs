using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DevLocker.Utils;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

public class MainMenuTitleScript : MonoBehaviour
{
    public enum Mode
    {
        Menu,
        Settings,
        PlaySelect,
        TransitLevel,
        Credits
    }

    public GameObject resumeButton;
    [FoldoutGroup("Prompt")] public GameObject fileExistPrompt;
    [FoldoutGroup("Prompt")] public GameObject differentFileVersionPrompt;
    public SceneReference introScene;
    public AudioSource music;
    public Mode currentMode;
    public float transitionTime = 3f;
    public float resumeTimeDelay = 2f;
    public UnityEvent OnTransitEvent;
    public UnityEvent OnPlayModeActivated;
    public UnityEvent OnResetProgression;
    [FoldoutGroup("Mainmenu")] public GameObject backlayerCam;
    [FoldoutGroup("Mainmenu")] public GameObject bg_UI;
    [FoldoutGroup("Mainmenu")] public GameObject settingsUI;
    [FoldoutGroup("Mainmenu")] public GameObject creditsUI;
    [FoldoutGroup("Mainmenu")] public GameObject Camera_SettingsMode;
    [FoldoutGroup("Mainmenu")] public GameObject Camera_PlayMode;
    [FoldoutGroup("Mainmenu")] public GameObject mainMenu_UI;
    [FoldoutGroup("Mainmenu")] public GameObject playMode_UI;
    [FoldoutGroup("Mainmenu")] public GameObject scene_Mainmenu;
    [FoldoutGroup("Mainmenu")] public GameObject scene_Playmode;
    [FoldoutGroup("Mainmenu")] public GameObject transitToPlay_UI;
    [FoldoutGroup("Mainmenu")] public GameObject transitResume_UI;
    [FoldoutGroup("Mainmenu")] public CooldownTimeTriggerEvent timer_StartTransition;
    [FoldoutGroup("Mainmenu")] public CooldownTimeTriggerEvent timer_Resume;
    [FoldoutGroup("Mainmenu")] public CanvasGroup canvasGroup_Transit;
    [FoldoutGroup("UIs")] public Text label_MainMenuCredit;
    [FoldoutGroup("UIs")] public Text label_Version;

    public bool Debug_EditorPlayIntro = false;

    private bool savefileExist = false;
    private bool isTriggeringResume = false;
    public static bool AlreadyPlayedCutscene = false;
    public HypatiosSave cachedSaveFile;

    public bool IsTriggeringResume { get => isTriggeringResume;  }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        AlreadyPlayedCutscene = false;
    }


    private void Start()
    {
        transitResume_UI.gameObject.SetActive(false);
        label_Version.text = $"PROJECT;HYPATIOS <v.{Application.version}>";
        string pathLoad = "";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        JsonSerializerSettings settings = FPSMainScript.JsonSettings();

        pathLoad = FPSMainScript.GameSavePath + "/defaultSave.save";

        try
        {
            var tempSave = JsonConvert.DeserializeObject<HypatiosSave>(File.ReadAllText(pathLoad), settings);

            if (tempSave == null)
            {
                savefileExist = false;

            }
            else
            {
                savefileExist = true;
                cachedSaveFile = tempSave;
            }

        }
        catch
        {
            savefileExist = false;

        }

        if (savefileExist == false)
        {
            resumeButton.gameObject.SetActive(false);

          
        }
        else
        {
            if (IsSaveFileVersionMatched() == false && IsWipingSaveFileNeeded() == true)
            {
                differentFileVersionPrompt.gameObject.SetActive(true);
   
            }
            else
            {
                differentFileVersionPrompt.gameObject.SetActive(false);
            }
        }

        {
            bool allowPlay = false;

            if (savefileExist)
                allowPlay = true;

            if (Application.isEditor && Debug_EditorPlayIntro)
                allowPlay = true;
            if (Application.isEditor && Debug_EditorPlayIntro == false)
                allowPlay = false;
            if (Application.isEditor == false)
                allowPlay = true;
            if (cachedSaveFile.Game_TotalRuns < 9999999 | AlreadyPlayedCutscene == true)
                allowPlay = false;


            if (allowPlay)
            {
                PlayRetardCutscene();
            }
        }
    }


    #region Save File Different
    public bool IsSaveFileVersionMatched()
    {
        return (GetHypatiosSave().Game_Version == Application.version) ? true : false;
    }

    public bool IsWipingSaveFileNeeded()
    {
        int totalLevelInBuild = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;

        if (GetHypatiosSave().LastVersion_TotalLevel != totalLevelInBuild)
        {
            return true;
        }

        return false;
    }


    public void WipeCurrentRunProgress()
    {
        //rewrite file
        var saveToModify = GetHypatiosSave();
        FPSMainScript.WipeCurrentRunProgress(saveToModify);

        WriteSaveFile(saveToModify);
        cachedSaveFile = saveToModify;

        //restart level
        Application.LoadLevel(Application.loadedLevel);
    }

    #endregion

    public static HypatiosSave GetHypatiosSave()
    {
        string pathLoad = "";
        JsonSerializerSettings settings = FPSMainScript.JsonSettings();

        pathLoad = FPSMainScript.GameSavePath + "/defaultSave.save";

        try
        {
            var tempSave = JsonConvert.DeserializeObject<HypatiosSave>(File.ReadAllText(pathLoad), settings);

            if (tempSave == null)
            {
                return null;
            }
            else
            {
                return tempSave;
            }

        }
        catch
        {

        }

        return null;
    }

    public static void WriteSaveFile(HypatiosSave _newSaveData)
    {
        string pathSave = "";
        pathSave = FPSMainScript.GameSavePath + "/defaultSave.save";
        JsonSerializerSettings settings = FPSMainScript.JsonSettings();
        _newSaveData.Game_Version = Application.version;
        _newSaveData.LastVersion_TotalLevel = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        _newSaveData.Game_DemoMode = Hypatios.IsDemoMode;

        string jsonTypeNameAll = JsonConvert.SerializeObject(_newSaveData, Formatting.Indented, settings);

        File.WriteAllText(pathSave, jsonTypeNameAll);
    }

    [Button("PlayCutscene")]
    public void PlayRetardCutscene()
    {
        int index = introScene.Index;
        Application.LoadLevel(index);
        AlreadyPlayedCutscene = true;
    }

    public void CheckAndNewGame()
    {
        if (savefileExist)
        {
            fileExistPrompt.SetActive(true);

        }
        else
        {
            Hypatios.Game.Menu_StartPlayGame();
        }
    }

    private void Update()
    {
        Hypatios.UI.RefreshUI_Resolutions();

        if (currentMode == Mode.Menu)
        {
            Mode_Menu();
        }
        else
        {

        }

        if (currentMode == Mode.PlaySelect)
        {
            Mode_PlaySelect();
        }
        else
        {
            scene_Mainmenu.EnableGameobject(true);
            scene_Playmode.EnableGameobject(false);
        }

        if (currentMode == Mode.Settings)
        {
            Mode_Settings();
        }
        else
        {
            settingsUI.EnableGameobject(false);
        }

        if (currentMode == Mode.Credits)
        {
            Mode_Credits();
        }
        else
        {
            creditsUI.EnableGameobject(false);

        }
    }

    #region Play Modes
    public void InitiateTransitionPlayMode()
    {
        mainMenu_UI.gameObject.SetActive(false);
        transitToPlay_UI.gameObject.SetActive(true);
        timer_StartTransition.ExecuteTrigger(transitionTime);
        canvasGroup_Transit.alpha = 0f;
    }

    public void InitiateResume()
    {
        music.Stop();
        playMode_UI.EnableGameobject(false);
        timer_Resume.ExecuteTrigger(resumeTimeDelay);
        currentMode = Mode.TransitLevel;
        isTriggeringResume = true;
        transitResume_UI.gameObject.SetActive(true);
        OnTransitEvent?.Invoke();
    }

    private bool _hasTriggeredPlayMode = false;


    public void ChangeMode(int mode)
    {
        currentMode = (Mode)mode;
        if (currentMode == Mode.PlaySelect)
        {
            if (music.isPlaying == false)
                music.Play();


            if (_hasTriggeredPlayMode == false)
            {
                OnPlayModeActivated?.Invoke();
                _hasTriggeredPlayMode = true;
            }
        }
    }

  
    private void Mode_Menu()
    {
        bg_UI.EnableGameobject(true);
        Camera_SettingsMode.EnableGameobject(false);
        Camera_PlayMode.EnableGameobject(false);
        playMode_UI.EnableGameobject(false);


    }

    private void Mode_PlaySelect()
    {
        bg_UI.EnableGameobject(false);
        Camera_SettingsMode.EnableGameobject(false);
        Camera_PlayMode.EnableGameobject(true);
        transitToPlay_UI.EnableGameobject(false);
        playMode_UI.EnableGameobject(true);
        scene_Mainmenu.EnableGameobject(false);
        scene_Playmode.EnableGameobject(true);

    }

    private void Mode_Settings()
    {
        settingsUI.EnableGameobject(true);
        bg_UI.EnableGameobject(false);
        Camera_SettingsMode.EnableGameobject(true);
        Camera_PlayMode.EnableGameobject(false);
        playMode_UI.EnableGameobject(false);


    }

    private void Mode_Credits()
    {
        creditsUI.EnableGameobject(true);
        bg_UI.EnableGameobject(false);
        Camera_SettingsMode.EnableGameobject(true);
        Camera_PlayMode.EnableGameobject(false);
        playMode_UI.EnableGameobject(false);


    }

    #endregion

}
