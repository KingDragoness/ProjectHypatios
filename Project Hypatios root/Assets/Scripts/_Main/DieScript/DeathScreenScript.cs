using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Sirenix.OdinInspector;
using Newtonsoft.Json;

public class DeathScreenScript : MonoBehaviour
{
   
    public enum CurrentStage
    {
        TimelineShow,
        TimeRemainingShow,
        TransitionLevel,
        CutsceneRun1,
        CutsceneRun3,
        CustomCutscene
    }

    public enum ReaperStage
    {
        Main,
        PerkSelect,
        Done
    }

    public List<DeathScreen_Cutscene> allCutscenes = new List<DeathScreen_Cutscene>();
    public List<ReaperDialogueEntry> allReaperDialogues = new List<ReaperDialogueEntry>();
    [FoldoutGroup("References")] public Text lastPlayedTime_Text;
    [FoldoutGroup("References")] public Text pressAny_Text;
    [FoldoutGroup("References")] public Animator anim_Transition;
    [FoldoutGroup("References")] public DeadDialogue deadDialogue;
    [FoldoutGroup("References")] public PerkSelectionUI perkSelection;
    [FoldoutGroup("References")] public VideoPlayer videoPlayer;
    [Space]
    public CurrentStage currentStage;
    public ReaperStage currentReaperStage = ReaperStage.Main;
    public int UNIX_Remaining = 360; //1 = 1 second
    public int test_UNIX_Start = 1640087660;
    public FPSMainScript fpsMainScript;
    [Space]


    [Header("Cutscene Special")]
    public int DEBUG_CutsceneTest = 0;
    public float speedMultiplierTime = 1;
    public bool DEBUG_NoKey = false;
    [FoldoutGroup("UI_References")] public GameObject VC_MainMode;
    [FoldoutGroup("UI_References")] public GameObject VC_SelectPerks;
    [FoldoutGroup("UI_References")] public GameObject MainUI_MainMode;
    [FoldoutGroup("UI_References")] public GameObject MainUI_SelectPerks;
    [FoldoutGroup("UI_References")] public GameObject defaultUI;
    [FoldoutGroup("UI_References")] public GameObject defaultSetting;
    [FoldoutGroup("UI_References")] public GameObject customSetting;
    [FoldoutGroup("Run_Specific")] public GameObject runCutscene_1;
    [FoldoutGroup("Run_Specific")] public GameObject run1_Setting;
    [FoldoutGroup("Run_Specific")] public GameObject runCutscene_3;
    [FoldoutGroup("Run_Specific")] public GameObject run3_Setting;

    public static DeathScreenScript Instance;
    private bool triggered = false;
    private bool triggered_CustomCutscene = false;
    private DeathScreen_Cutscene currentCustomCutscene;
    private int _miliSecondMockup = 999;
    private float _minimumTime_CutsceneClose = 1f;

    private void Start()
    {
        if (FPSMainScript.Player_RunSessionUnixTime != 0)
        {
            UNIX_Remaining = FPSMainScript.Player_RunSessionUnixTime;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UNIX_Remaining += test_UNIX_Start;
        Time.timeScale = 1;
        FPSMainScript.CacheLoadSave();
        Instance = this;

        if (FPSMainScript.savedata != null)
        { SaveDataCheck(); }
        else
        {
            CutsceneRun();
        }

        //if (!Application.isEditor) DEBUG_NoKey = false;
    }

    private void SaveDataCheck()
    {
        if (FPSMainScript.savedata.Game_TotalRuns == 1)
        {
            currentStage = CurrentStage.CutsceneRun1;
        }
        else if (FPSMainScript.savedata.Game_TotalRuns == 3)
        {
            //currentStage = CurrentStage.CutsceneRun3;
        }
        CutsceneRun();
    }

    //Set CurrentStage
    [ContextMenu("RunTest_Debug")]
    public void RunTest_Debug()
    {
        currentStage = (CurrentStage)DEBUG_CutsceneTest;
        CutsceneRun();
    }

    public void RunTest(int stage)
    {
        currentStage = (CurrentStage)stage;
        CutsceneRun();
    }

 

    //also refresh UI, cutscenes, etc
    private void CutsceneRun()
    {
        run1_Setting.gameObject.SetActive(false);
        run3_Setting.gameObject.SetActive(false);
        defaultSetting.gameObject.SetActive(false);

        runCutscene_1.gameObject.SetActive(false);
        runCutscene_3.gameObject.SetActive(false);
        defaultUI.gameObject.SetActive(false);
        customSetting.gameObject.SetActive(false);

        DeathScreen_Cutscene customCutscene = AnyCutsceneAvailableToPlay();

        if (triggered_CustomCutscene == false)
        {
            if (customCutscene != null)
            {
                currentStage = CurrentStage.CustomCutscene;
                currentCustomCutscene = customCutscene;
            }
        }
        else
        {
            currentStage = CurrentStage.TimeRemainingShow;
        }

        if (currentStage == CurrentStage.CustomCutscene)
        {
            customCutscene.LaunchScene();
            _minimumTime_CutsceneClose = 1f;
            triggered_CustomCutscene = true;
            customSetting.gameObject.SetActive(true);
        }
        else if (currentStage == CurrentStage.CutsceneRun1)
        {
            runCutscene_1.gameObject.SetActive(true);
            run1_Setting.gameObject.SetActive(true);

        }
        else if (currentStage == CurrentStage.CutsceneRun3)
        {
            runCutscene_3.gameObject.SetActive(true);
            run3_Setting.gameObject.SetActive(true);

        }
        else if (currentStage == CurrentStage.TimeRemainingShow)
        {
            defaultUI.gameObject.SetActive(true);
            defaultSetting.gameObject.SetActive(true);

        }
    }

    private DeathScreen_Cutscene AnyCutsceneAvailableToPlay()
    {
        DeathScreen_Cutscene result = null;

        foreach(var cutscene in allCutscenes)
        {
            if (cutscene.HasCutsceneAlreadyPlayed())
            {
                continue;
            }

            if (cutscene.GetEvaluateResult() == true)
            {
                result = cutscene;
                break;
            }
        }

        return result;
    }

    [FoldoutGroup("DEBUG")] [Button("Refresh all cutscene objects")]
    public void GetAllCutsceneObjects()
    {
        allCutscenes = FindObjectsOfType<DeathScreen_Cutscene>().ToList();
        allReaperDialogues = FindObjectsOfType<ReaperDialogueEntry>().ToList();
        allCutscenes = allCutscenes.OrderBy(x => x.priorityOrder).ToList();
    }

    private void Update()
    {
        if (currentStage == CurrentStage.CutsceneRun1 | currentStage == CurrentStage.CutsceneRun3)
        {
            MainUI_SelectPerks.SetActive(false);
            MainUI_MainMode.SetActive(false);
        }
        else if (currentStage == CurrentStage.CustomCutscene)
        {
            MainUI_SelectPerks.SetActive(false);
            MainUI_MainMode.SetActive(false);
            _minimumTime_CutsceneClose -= Time.deltaTime;

            if (currentCustomCutscene.cutsceneType == DeathScreen_Cutscene.CutsceneType.Video)
            {
                if (_minimumTime_CutsceneClose < 0f && videoPlayer.isPlaying == false)
                {
                    RunTest(1);
                }
            }
        }
        else 
        {
            ReaperScenes();
        }

        if (DEBUG_NoKey) return;

        if (currentStage == CurrentStage.TimeRemainingShow)
        {
            if (Input.anyKeyDown)
            {
                currentStage = CurrentStage.TransitionLevel;
            }
        }

    }

    #region Reaper Scene handle

    public void SetReaper(int stage)
    {
        currentReaperStage = (ReaperStage)stage;
    }

    public void TriggerReaperSpeech()
    {
        string s = "Reaper: ";

        if (perkSelection.CheckFlagExist(perkSelection.flag_KillerPill))
            s = $"{s} \"Killer Pill will prevent you from permanent kThanid administration.\"";
        else if (FPSMainScript.savedata.Game_TotalRuns == 10)
            s = $"{s} \"10th death. I've counted it.\"";
        else if (FPSMainScript.savedata.Game_TotalRuns == 1)
            s = $"{s} \"Welcome to the afterlife.\"";
        else
        {
            string s1 = allReaperDialogues[UnityEngine.Random.Range(0, allReaperDialogues.Count)].dialogueEntry;
            s = $"{s} \"{s1}\"";
        }

        deadDialogue.TestText(s);

    }

    private void ReaperScenes()
    {
        if (currentReaperStage == ReaperStage.Main)
        {
            MainUI_SelectPerks.SetActive(false);

            if (!MainUI_MainMode.activeSelf) MainUI_MainMode.SetActive(true);
            if (!VC_MainMode.activeSelf) VC_MainMode.SetActive(true);
            if (VC_SelectPerks.activeSelf) VC_SelectPerks.SetActive(false);
        }
        else if (currentReaperStage == ReaperStage.PerkSelect)
        {
            if (!VC_SelectPerks.activeSelf)
            {
                VC_SelectPerks.SetActive(true);
                MainUI_SelectPerks.SetActive(true);
            }
            if (VC_MainMode.activeSelf) VC_MainMode.SetActive(false);
            if (MainUI_MainMode.activeSelf) MainUI_MainMode.SetActive(false);

        }
    }

    #endregion

    public void LoadTransition()
    {
        HypatiosSave hypatiosSave = Hypatios.GetHypatiosSave();

        bool isTemp = false;

        if (perkSelection.selectedPerkButton.customEffect.statusCategoryType != ModifierEffectCategory.Nothing)
            isTemp = true;

        if (isTemp)
        {
            hypatiosSave.AllPerkDatas.Temp_CustomPerk.Add(perkSelection.selectedPerkButton.customEffect);
        }
        else
        {
            hypatiosSave.AllPerkDatas.AddPerkLevel(perkSelection.selectedPerkButton.status);
        }

        string jsonTypeNameAll = JsonConvert.SerializeObject(hypatiosSave, Formatting.Indented, FPSMainScript.JsonSettings());

        Hypatios.ManualSave(hypatiosSave, "CONFIRM", jsonTypeNameAll);
        currentStage = CurrentStage.TransitionLevel;
    }

    private void FixedUpdate()
    {
        if (currentStage == CurrentStage.TimeRemainingShow)
        {
            if (UNIX_Remaining > test_UNIX_Start)
            {
                float speed = (UNIX_Remaining - test_UNIX_Start)/360;
                speed = Mathf.Clamp(speed, 1, 6) * speedMultiplierTime;

                if (currentReaperStage == ReaperStage.Done) UNIX_Remaining -= Mathf.RoundToInt(speed) * 2;

                if (_miliSecondMockup <= 0)
                {
                    _miliSecondMockup = 999;
                }
                else
                {
                    _miliSecondMockup -= 99 + UnityEngine.Random.Range(1, 7);
                }
            }
            else
            {
                Debug.Log("s1");
                currentStage = CurrentStage.TransitionLevel;
            }


            var dateTime = UnixTimeStampToDateTime(UNIX_Remaining);

            lastPlayedTime_Text.text = $"{dateTime.Hour}:{dateTime.Minute.ToString("00")}:{dateTime.Second.ToString("00")}.{_miliSecondMockup.ToString("000")}";
        }
        else if (currentStage == CurrentStage.TransitionLevel)
        {
            if (!triggered)
            {
                pressAny_Text.gameObject.SetActive(false);
                var dateTime = UnixTimeStampToDateTime(test_UNIX_Start);

                lastPlayedTime_Text.text = $"{dateTime.Hour}:{dateTime.Minute.ToString("00")}:{dateTime.Second.ToString("00")}.{_miliSecondMockup.ToString("000")}";
                StartCoroutine(LoadDelay());
            }
            triggered = true;
        }
    }

    #region Save Game




    #endregion

    IEnumerator LoadDelay()
    {
        Debug.Log("Zoom1 test");
        anim_Transition.SetTrigger("Zoom");
        yield return new WaitForSeconds(5f);
        TriggerLoad();
    }

    public void TriggerLoad()
    {
        Application.LoadLevel(fpsMainScript.level1_Scene.Index);
        Time.timeScale = 1;
    }

    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }
}
