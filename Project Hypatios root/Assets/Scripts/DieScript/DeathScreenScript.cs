using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
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
        CutsceneRun3
    }

    public enum ReaperStage
    {
        Main,
        PerkSelect,
        Done
    }


    [FoldoutGroup("References")] public Text lastPlayedTime_Text;
    [FoldoutGroup("References")] public Text pressAny_Text;
    [FoldoutGroup("References")] public Animator anim_Transition;
    [FoldoutGroup("References")] public DeadDialogue deadDialogue;
    [FoldoutGroup("References")] public PerkSelectionUI perkSelection;
    [Space]
    public CurrentStage currentStage;
    public ReaperStage currentReaperStage = ReaperStage.Main;
    public int UNIX_Remaining = 360; //1 = 1 second
    public int test_UNIX_Start = 1640087660;
    public int levelTarget = 2;
    [Space]

    [Header("Cutscene Special")]
    public int DEBUG_CutsceneTest = 0;
    public float speedMultiplierTime = 1;
    public bool DEBUG_NoKey = false;
    [FoldoutGroup("References")] public GameObject VC_MainMode;
    [FoldoutGroup("References")] public GameObject VC_SelectPerks;
    [FoldoutGroup("References")] public GameObject MainUI_MainMode;
    [FoldoutGroup("References")] public GameObject MainUI_SelectPerks;
    [FoldoutGroup("References")] public GameObject defaultUI;
    [FoldoutGroup("References")] public GameObject defaultSetting;
    [FoldoutGroup("References")] public GameObject runCutscene_1;
    [FoldoutGroup("References")] public GameObject run1_Setting;
    [FoldoutGroup("References")] public GameObject runCutscene_3;
    [FoldoutGroup("References")] public GameObject run3_Setting;

    private bool triggered = false;
    private int fakeMilisecond = 999;

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

 

    private void CutsceneRun()
    {
        run1_Setting.gameObject.SetActive(false);
        run3_Setting.gameObject.SetActive(false);
        defaultSetting.gameObject.SetActive(false);

        runCutscene_1.gameObject.SetActive(false);
        runCutscene_3.gameObject.SetActive(false);
        defaultUI.gameObject.SetActive(false);


        if (currentStage == CurrentStage.CutsceneRun1)
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

    private void Update()
    {
        if (currentStage == CurrentStage.CutsceneRun1 | currentStage == CurrentStage.CutsceneRun3)
        {
            MainUI_SelectPerks.SetActive(false);
            MainUI_MainMode.SetActive(false);
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

        if (FPSMainScript.savedata.Game_TotalRuns == 10)
            s = $"{s} \"10th death. I've counted it.\"";
        else if (FPSMainScript.savedata.Game_TotalRuns == 1)
            s = $"{s} \"Welcome to the afterlife.\"";
        else
            s = $"{s} \"Welcome back.\"";

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
        string pathSave = "";
        pathSave = FPSMainScript.GameSavePath + "/defaultSave.save";
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        //overload savefile
        HypatiosSave hypatiosSave = JsonConvert.DeserializeObject<HypatiosSave>(File.ReadAllText(pathSave), settings);

        bool isTemp = false;

        if (perkSelection.selectedPerkButton.customEffect.statusCategoryType != StatusEffectCategory.Nothing)
            isTemp = true;

        if (isTemp)
        {
            hypatiosSave.AllPerkDatas.Temp_CustomPerk.Add(perkSelection.selectedPerkButton.customEffect);
        }
        else
        { 
            if (perkSelection.selectedPerkButton.status == StatusEffectCategory.MaxHitpointBonus)
                hypatiosSave.AllPerkDatas.Perk_LV_MaxHitpointUpgrade++;

            if (perkSelection.selectedPerkButton.status == StatusEffectCategory.RegenHPBonus)
                hypatiosSave.AllPerkDatas.Perk_LV_RegenHitpointUpgrade++;

            if (perkSelection.selectedPerkButton.status == StatusEffectCategory.SoulBonus)
                hypatiosSave.AllPerkDatas.Perk_LV_Soulbonus++;

            if (perkSelection.selectedPerkButton.status == StatusEffectCategory.ShortcutDiscount)
                hypatiosSave.AllPerkDatas.Perk_LV_ShortcutDiscount++;

            if (perkSelection.selectedPerkButton.status == StatusEffectCategory.KnockbackResistance)
                hypatiosSave.AllPerkDatas.Perk_LV_KnockbackRecoil++;

            if (perkSelection.selectedPerkButton.status == StatusEffectCategory.DashCooldown)
                hypatiosSave.AllPerkDatas.Perk_LV_DashCooldown++;

            if (perkSelection.selectedPerkButton.status == StatusEffectCategory.BonusDamageMelee)
                hypatiosSave.AllPerkDatas.Perk_LV_IncreaseMeleeDamage++;

            //hypatiosSave.Perk_LV_RegenHitpointUpgrade = Perk_LV_RegenHitpointUpgrade;
            //hypatiosSave.Perk_LV_Soulbonus = Perk_LV_Soulbonus;
            //hypatiosSave.Perk_LV_ShortcutDiscount = Perk_LV_ShortcutDiscount;
            //hypatiosSave.Perk_LV_KnockbackRecoil = Perk_LV_KnockbackRecoil;
            //hypatiosSave.Perk_LV_DashCooldown = Perk_LV_DashCooldown;
            //hypatiosSave.Perk_LV_IncreaseMeleeDamage = Perk_LV_IncreaseMeleeDamage;
        }

        string jsonTypeNameAll = JsonConvert.SerializeObject(hypatiosSave, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        });


        File.WriteAllText(pathSave, jsonTypeNameAll);
        if (ConsoleCommand.Instance != null) ConsoleCommand.Instance.SendConsoleMessage($"File has been saved to {pathSave}");
        FPSMainScript.savedata = hypatiosSave;
        FPSMainScript.LoadFromSaveFile = true;
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

                if (fakeMilisecond <= 0)
                {
                    fakeMilisecond = 999;
                }
                else
                {
                    fakeMilisecond -= 99 + UnityEngine.Random.Range(1, 7);
                }
            }
            else
            {
                Debug.Log("s1");
                currentStage = CurrentStage.TransitionLevel;
            }


            var dateTime = UnixTimeStampToDateTime(UNIX_Remaining);

            lastPlayedTime_Text.text = $"{dateTime.Hour}:{dateTime.Minute.ToString("00")}:{dateTime.Second.ToString("00")}.{fakeMilisecond.ToString("000")}";
        }
        else if (currentStage == CurrentStage.TransitionLevel)
        {
            if (!triggered)
            {
                pressAny_Text.gameObject.SetActive(false);
                var dateTime = UnixTimeStampToDateTime(test_UNIX_Start);

                lastPlayedTime_Text.text = $"{dateTime.Hour}:{dateTime.Minute.ToString("00")}:{dateTime.Second.ToString("00")}.{fakeMilisecond.ToString("000")}";
                StartCoroutine(LoadDelay());
            }
            triggered = true;
        }
    }

    IEnumerator LoadDelay()
    {
        Debug.Log("Zoom1 test");
        anim_Transition.SetTrigger("Zoom");
        yield return new WaitForSeconds(5f);
        TriggerLoad();
    }

    public void TriggerLoad()
    {
        Application.LoadLevel(levelTarget);
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
