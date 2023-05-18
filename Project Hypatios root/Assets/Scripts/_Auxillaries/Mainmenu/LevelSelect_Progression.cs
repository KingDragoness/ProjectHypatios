using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DevLocker.Utils;
using UnityEngine.SceneManagement;

public class LevelSelect_Progression : MonoBehaviour
{

    [System.Serializable]
    public class LevelPosition
    {
        public Transform spawn;
        public SceneReference scene;
        public ChamberLevel level;
    }

    public Transform defaultSpawn;
    public Transform legend;
    public List<LevelPosition> allLevelPositions = new List<LevelPosition>();
    public TextMesh label_progress;
    public TextMesh label_HPMax;
    public TextMesh label_HPRegen;
    public TextMesh label_Alcohol;
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

        foreach (var level in allLevelPositions)
        {
            int indexLv = level.scene.Index;
            if (indexLv == index)
            {
                levelPos = level;
                break;
            }
        }

        if (levelPos == null) 
            return;

        if (levelPos.spawn != null) legend.transform.position = levelPos.spawn.position;

        weapon_Slot2.gameObject.SetActive(false);
        weapon_Slot3.gameObject.SetActive(false);
        weapon_Slot4.gameObject.SetActive(false);

        {
            float maxHP = 100;
            maxHP += PlayerPerk.GetValue_MaxHPUpgrade(save.AllPerkDatas.Perk_LV_MaxHitpointUpgrade);
            label_progress.text = $"{levelPos.level.levelName} | {save.Game_TotalRuns} deaths";
            label_HPMax.text = $"{Mathf.FloorToInt(save.Player_CurrentHP)}/{maxHP}";
            label_HPRegen.text = $"+{PlayerPerk.GetValue_RegenHPUpgrade(save.AllPerkDatas.Perk_LV_RegenHitpointUpgrade)}HP/s";
            label_Alcohol.text = $"{Mathf.FloorToInt(save.Player_AlchoholMeter)}/100%";

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

}
