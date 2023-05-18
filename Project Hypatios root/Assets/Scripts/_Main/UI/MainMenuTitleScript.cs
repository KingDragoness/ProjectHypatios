using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
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
        TransitLevel
    }

    public GameObject resumeButton;
    public GameObject fileExistPrompt;
    public SceneReference introScene;
    public AudioSource music;
    public Mode currentMode;
    public float transitionTime = 3f;
    public float resumeTimeDelay = 2f;
    public UnityEvent OnTransitEvent;
    [FoldoutGroup("Mainmenu")] public GameObject backlayerCam;
    [FoldoutGroup("Mainmenu")] public GameObject bg_UI;
    [FoldoutGroup("Mainmenu")] public GameObject settingsUI;
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
        string pathLoad = "";
        Cursor.lockState = CursorLockMode.None;
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
        else if (currentMode == Mode.PlaySelect)
        {
            Mode_PlaySelect();
        }
        else if (currentMode == Mode.Settings)
        {
            Mode_Settings();
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



    public void ChangeMode(int mode)
    {
        currentMode = (Mode)mode;
        if (currentMode == Mode.PlaySelect)
        {
            if (music.isPlaying == false)
                music.Play();
        }
    }

  
    private void Mode_Menu()
    {
        settingsUI.EnableGameobject(false);
        bg_UI.EnableGameobject(true);
        Camera_SettingsMode.EnableGameobject(false);
        Camera_PlayMode.EnableGameobject(false);
        playMode_UI.EnableGameobject(false);
        scene_Mainmenu.EnableGameobject(true);
        scene_Playmode.EnableGameobject(false);

    }

    private void Mode_PlaySelect()
    {
        settingsUI.EnableGameobject(false);
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
        scene_Mainmenu.EnableGameobject(true);
        scene_Playmode.EnableGameobject(false);


    }

    #endregion

}
