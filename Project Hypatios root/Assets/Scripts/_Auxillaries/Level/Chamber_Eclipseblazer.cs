using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using DevLocker.Utils;

public class Chamber_Eclipseblazer : MonoBehaviour
{

    public EclipseblazerEnemy eclipseblazerScript;
    public SceneReference collateralDreamScene;
    public SceneReference stadiumScene;
    public AudioSource music;
    public float SurviveTime = 360;
    public UnityEvent OnRoundTimeUp;
    [FoldoutGroup("UI")] public Text label_Countdown;
    [FoldoutGroup("UI")] public GameObject missionObjectiveUI;
    [FoldoutGroup("UI")] public Slider slider_TimeCount;

    private float _survivetime = 360f;
    private bool _hasStarted = false;
    private bool _hasCompleted = false;

    private void Start()
    {
        _survivetime = SurviveTime;
    }

    private void Update()
    {
        if (Time.timeScale <= 0f) return;

        if (_hasStarted == true)
        {
            _survivetime -= Time.deltaTime;

            UpdateTimeUI();
            
            if (_survivetime <= 0f && _hasCompleted == false)
            {
                PlayerSurvived();
            }
        }
    }

    [FoldoutGroup("DEBUG")] [Button("Modify remaining time")]
    public void ModifyRoundTime(float time)
    {
        _survivetime = time;
    }

    private void PlayerSurvived()
    {
        _hasCompleted = true;
        OnRoundTimeUp?.Invoke();
    }

    private void UpdateTimeUI()
    {
        var rt = _survivetime;
        if (rt <= 0) rt = 0;
        var dateTime = ClockTimerDisplay.UnixTimeStampToDateTime(rt, true);
        label_Countdown.text = $"{ dateTime.Minute.ToString("00")}:{ dateTime.Second.ToString("00")}";
        slider_TimeCount.value = _survivetime;
        slider_TimeCount.maxValue = SurviveTime;
    }

    public void StartChamber()
    {
        _hasStarted = true;
        music.enabled = true;
        missionObjectiveUI.gameObject.SetActive(true);
        eclipseblazerScript.TriggerBossFight();
    }

    public void EndingScene()
    {
        Hypatios.Game.PlayerDie();
        var saveFile = MainMenuTitleScript.GetHypatiosSave();
        saveFile.Game_LastLevelPlayed = stadiumScene.Index;
        MainMenuTitleScript.WriteSaveFile(saveFile);
        Application.LoadLevel(collateralDreamScene.Index);
    }

}
