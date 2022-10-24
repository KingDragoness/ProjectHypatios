using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Text lastPlayedTime_Text;
    public Text pressAny_Text;
    public Animator anim_Transition;
    [Space]
    public CurrentStage currentStage;
    public int UNIX_Remaining = 360; //1 = 1 second
    public int test_UNIX_Start = 1640087660;
    public int levelTarget = 2;
    [Space]

    [Header("Cutscene Special")]
    public int DEBUG_CutsceneTest = 0;
    public GameObject defaultUI;
    public GameObject defaultSetting;
    public GameObject runCutscene_1;
    public GameObject run1_Setting;
    public GameObject runCutscene_3;
    public GameObject run3_Setting;

    private bool triggered = false;
    private int fakeMilisecond = 999;

    private void Start()
    {
        if (FPSMainScript.Player_RunSessionUnixTime != 0)
        {
            UNIX_Remaining = FPSMainScript.Player_RunSessionUnixTime;
        }

        UNIX_Remaining += test_UNIX_Start;
        Time.timeScale = 1;

        if (FPSMainScript.savedata != null)
        { SaveDataCheck(); }
        else
        {
            CutsceneRun();
        }
    }

    private void SaveDataCheck()
    {
        if (FPSMainScript.savedata.Game_TotalRuns == 1)
        {
            currentStage = CurrentStage.CutsceneRun1;
        }
        else if (FPSMainScript.savedata.Game_TotalRuns == 3)
        {
            currentStage = CurrentStage.CutsceneRun3;
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
        if (currentStage == CurrentStage.TimeRemainingShow)
        {
            if (Input.anyKeyDown)
            {
                currentStage = CurrentStage.TransitionLevel;
            }
        }
    }

    //Introduction Cutscene1


    private void FixedUpdate()
    {
        if (currentStage == CurrentStage.TimeRemainingShow)
        {
            if (UNIX_Remaining > test_UNIX_Start)
            {
                float speed = (UNIX_Remaining - test_UNIX_Start)/360;
                speed = Mathf.Clamp(speed, 1, 6);

                UNIX_Remaining -= Mathf.RoundToInt(speed) * 2;

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
        yield return new WaitForSeconds(2f);
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
