using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DevLocker.Utils;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

public class MainMenuTitleScript : MonoBehaviour
{

    public GameObject resumeButton;
    public GameObject fileExistPrompt;
    public SceneReference introScene;
    public bool Debug_EditorPlayIntro = false;

    private bool savefileExist = false;
    public static bool AlreadyPlayedCutscene = false;
    public HypatiosSave cachedSaveFile;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        AlreadyPlayedCutscene = false;
    }

    private void Start()
    {
        string pathLoad = "";
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
    }

}
