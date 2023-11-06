using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using FulfillCondition = Conditioner.FulfillCondition;

public class DeathScreen_Cutscene : MonoBehaviour
{
    public enum CutsceneType
    {
        Video,
        RealtimeCutscene
    }

    public enum ConditionType
    {
        Run,
        Trivia
    }

    [System.Serializable]
    public class DeathScreen_Conditioner
    {
        public DeathScreen_Cutscene.ConditionType conditionType;
        [ShowIf("conditionType", ConditionType.Run)] public int runTarget = 5;
        [ShowIf("conditionType", ConditionType.Trivia)] public Trivia triviaTarget;

        public bool IsConditionChecked()
        {
            if (conditionType == ConditionType.Trivia)
            {
                var triviaEntry = FPSMainScript.savedata.Game_Trivias.Find(x => x.ID == triviaTarget.ID);

                if (triviaEntry != null)
                {
                    if (triviaEntry.isCompleted)
                        return true;

                }
            }
            else if (conditionType == ConditionType.Run)
            {
                var run = FPSMainScript.savedata.Game_TotalRuns;

                if (run >= runTarget)
                {
                    return true;
                }
            }

            return false;
        }

    }

    public int priorityOrder = 10;
    public string Title = "";
    public string paradoxID = "FirstTimer_TimekeeperTestament";
    public List<DeathScreen_Conditioner> AllConditions = new List<DeathScreen_Conditioner>();
    public CutsceneType cutsceneType;
    public FulfillCondition conditionForTrue;

    [ShowIf("cutsceneType", CutsceneType.Video)] public VideoClip videoClip;
    [ShowIf("cutsceneType", CutsceneType.RealtimeCutscene)] public GameObject realtimeScene;


    public bool HasCutsceneAlreadyPlayed()
    {
        string keyName = "DEATH.CUTSCENE." + paradoxID;

        if (FPSMainScript.savedata.otherEverUsed.Contains(keyName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetCutsceneKey(string key)
    {
        string keyName = "DEATH.CUTSCENE." + key;
        HypatiosSave hypatiosSave = Hypatios.GetHypatiosSave();


        if (FPSMainScript.savedata.otherEverUsed.Contains(keyName))
        {
            //already exist
        }
        else
        {
            hypatiosSave.otherEverUsed.Add(keyName);
        }

        string jsonTypeNameAll = JsonConvert.SerializeObject(hypatiosSave, Formatting.Indented, FPSMainScript.JsonSettings());

        Hypatios.ManualSave(hypatiosSave, "CONFIRM", jsonTypeNameAll);

    }

    public bool GetEvaluateResult()
    {
        bool result = false;
        string output_str = $"{Title} ";


        if (conditionForTrue == FulfillCondition.OR)
        {
            result = false;

            foreach (var condition in AllConditions)
            {
                output_str += $" {condition.conditionType}";
                output_str += $": {condition.IsConditionChecked()}";

                if (condition.IsConditionChecked())
                {
                    result = true;
                    break;
                }

                output_str += $" | ";
            }
        }
        else if (conditionForTrue == FulfillCondition.AND)
        {
            result = true;

            foreach (var condition in AllConditions)
            {
                output_str += $" {condition.conditionType}";
                output_str += $": {condition.IsConditionChecked()}";

                if (condition.IsConditionChecked() == false)
                {
                    result = false;
                    break;
                }

                output_str += $" | ";
            }

        }
        else if (conditionForTrue == FulfillCondition.NOT)
        {
            foreach (var condition in AllConditions)
            {
                output_str += $" {condition.conditionType}";
                output_str += $": {condition.IsConditionChecked()}";

                if (condition.IsConditionChecked() == true)
                {
                    result = false;
                    break;
                }

                output_str += $" | ";
            }

        }

        Debug.Log(output_str);
        return result;
    }

    public void LaunchScene()
    {
        //set cutscene key
        if (cutsceneType == CutsceneType.Video)
        {
            var videoPlayer = DeathScreenScript.Instance.videoPlayer;
            videoPlayer.clip = videoClip;
            videoPlayer.Play();
        }
        else
        {

        }

        SetCutsceneKey(paradoxID);
    }

}
