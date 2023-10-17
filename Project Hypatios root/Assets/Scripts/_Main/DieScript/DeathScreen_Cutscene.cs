using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Sirenix.OdinInspector;

public class DeathScreen_Cutscene : MonoBehaviour
{
    public enum CutsceneType
    {
        Video,
        RealtimeCutscene
    }

    public enum Conditioner
    {
        Run,
        Trivia
    }

    [System.Serializable]
    public class DeathScreen_Conditioner
    {
        public DeathScreen_Cutscene.Conditioner conditionType;
        [ShowIf("conditionType", Conditioner.Run)] public int run = 5;
        [ShowIf("conditionType", Conditioner.Trivia)] public Trivia trivia;

    }

    public int priorityOrder = 10;
    public string paradoxID = "FirstTimer_TimekeeperTestament";

    [ShowIf("CutsceneType", CutsceneType.Video)] public VideoPlayer videoPlayer;
    [ShowIf("CutsceneType", CutsceneType.RealtimeCutscene)] public GameObject realtimeScene;
    public CutsceneType type;



}
