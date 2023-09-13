using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class LevelSelect_Progression : MonoBehaviour
{

    [System.Serializable]
    public class LevelPosition
    {
        public Transform spawn;
        public ChamberLevel level;
    }

    public Transform defaultSpawn;
    public Transform legend;
    public UnityEvent PlayerIsOnEclipseblazerLevel;
    public UnityEvent PlayerIsOnWIREDLevel;
    public List<LevelPosition> allLevelPositions = new List<LevelPosition>();
    public TextMesh label_progress;
    public TextMesh label_HPMax;
    public TextMesh label_HPRegen;
    public TextMesh label_Alcohol;
    public TextMesh label_TotalPlaytime;
    public TextMesh label_CurrentTime;
    public BaseStatValue Stat_TotalTime;
    public WeaponModelDisplay weapon_Slot2;
    public WeaponModelDisplay weapon_Slot3;
    public WeaponModelDisplay weapon_Slot4;

    private HypatiosSave save;

    private void Start()
    {
        save = MainMenuTitleScript.GetHypatiosSave();
        if (save != null)
        {
            InitializeProgress();
        }
        else
        {
            label_progress.text = $"No save file detected.";
            label_HPMax.gameObject.SetActive(false);
            label_HPRegen.gameObject.SetActive(false);
            label_Alcohol.gameObject.SetActive(false);

            weapon_Slot2.gameObject.SetActive(false);
            weapon_Slot3.gameObject.SetActive(false);
            weapon_Slot4.gameObject.SetActive(false);
        }
    }

    public void InitializeProgress()
    {
        int index = save.Game_LastLevelPlayed;
        LevelPosition levelPos = null;

        foreach (var lvPos in allLevelPositions)
        {
            int indexLv = lvPos.level.scene.Index;
            if (indexLv == index)
            {
                levelPos = lvPos;
                break;
            }
        }

        if (levelPos == null)
        {
            LevelWithNoPosition();
        }
        else LevelWithPosition(levelPos);


        weapon_Slot2.gameObject.SetActive(false);
        weapon_Slot3.gameObject.SetActive(false);
        weapon_Slot4.gameObject.SetActive(false);

        {
            var dateTime_runSession = ClockTimerDisplay.UnixTimeStampToDateTime(save.Player_RunSessionUnixTime + Hypatios.UnixTimeStart);
            var dateTime_totalPlaytime = ClockTimerDisplay.UnixTimeStampToDateTime(save.Game_UnixTime, true);
            string display_totalPlayTime = "";
            string display_runTime = "";

            display_totalPlayTime = $"{ClockTimerDisplay.TotalHoursPlayed(save.Game_UnixTime).ToString("00")}:{dateTime_totalPlaytime.Minute.ToString("00")}:{dateTime_totalPlaytime.Second.ToString("00")}s";
            display_runTime =  $"{dateTime_runSession.Hour}:{dateTime_runSession.Minute.ToString("00")}:{dateTime_runSession.Second.ToString("00")}";

            float maxHP = 100;
            maxHP += PlayerPerk.GetValue_MaxHPUpgrade(save.AllPerkDatas.Perk_LV_MaxHitpointUpgrade);
            label_HPMax.text = $"{Mathf.FloorToInt(save.Player_CurrentHP)}/{maxHP}";
            label_HPRegen.text = $"+{PlayerPerk.GetValue_RegenHPUpgrade(save.AllPerkDatas.Perk_LV_RegenHitpointUpgrade)}HP/s";
            label_Alcohol.text = $"{Mathf.FloorToInt(save.Player_AlchoholMeter)}/100%";
            label_TotalPlaytime.text = $"Total playtime: {display_totalPlayTime}";
            label_CurrentTime.text = $"[{display_runTime}]";

            if (save.Game_WeaponStats.Count >= 2)
            {
                for (int x = 0; x < save.Game_WeaponStats.Count; x++)
                {
                    if (x == 0) continue;
                    var weaponDat = save.Game_WeaponStats[x];
                    WeaponModelDisplay curWeapon = weapon_Slot2;
                    if (x == 2)
                        curWeapon = weapon_Slot3;
                    if (x == 3)
                        curWeapon = weapon_Slot4;

                    curWeapon.gameObject.SetActive(true);
                    curWeapon.currentWeaponDisplay = Hypatios.Assets.GetWeapon(weaponDat.weaponID);
                    curWeapon.ActivateWeapon();
                }
            }
        }
    }

    public void LevelWithNoPosition()
    {
        label_progress.text = $"{save.Game_TotalRuns} deaths";

    }

    public void LevelWithPosition(LevelPosition levelPos)
    {
        if (levelPos.spawn != null)
            legend.transform.position = levelPos.spawn.position;
        label_progress.text = $"{levelPos.level.levelName} | {save.Game_TotalRuns} deaths";

    }

    public void CheckPlayerIsOnEclipseblazer()
    {
        if (save.Game_LastLevelPlayed == Hypatios.Game.eclipseBlazerScene.Index)
        {
            PlayerIsOnEclipseblazerLevel?.Invoke();
        }
    }

    public void CheckPlayerIsOnWIRED()
    {
        var chamber = Hypatios.Assets.GetLevel(save.Game_LastLevelPlayed);

        Debug.Log(chamber.levelName);

        if (save.Game_LastLevelPlayed != Hypatios.Game.eclipseBlazerScene.Index &&
            chamber.isWIRED)
        {
            PlayerIsOnWIREDLevel?.Invoke();
        }
    }

    public void StealPlayerPerk()
    {
        var hypatiosSave = MainMenuTitleScript.GetHypatiosSave();

        if (hypatiosSave.AllPerkDatas.Perk_LV_MaxHitpointUpgrade > 30)
            hypatiosSave.AllPerkDatas.Perk_LV_MaxHitpointUpgrade -= 3;

        if (hypatiosSave.AllPerkDatas.Perk_LV_RegenHitpointUpgrade > 15)
            hypatiosSave.AllPerkDatas.Perk_LV_RegenHitpointUpgrade -= 1;

        MainMenuTitleScript.WriteSaveFile(hypatiosSave);
        save = MainMenuTitleScript.GetHypatiosSave();
        InitializeProgress();
    }
}
