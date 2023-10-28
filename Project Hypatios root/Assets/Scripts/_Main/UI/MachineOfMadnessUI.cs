using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MachineOfMadnessUI : MonoBehaviour
{

    public GameObject vCam_Transit;
    public GameObject vCam_MainUI;
    public MachineMad_SaveFileButton saveFileButton_prefab;
    public Transform parent_SaveFiles;
    public UnityEvent OnTimeVortexEvent;
    public GameEvent g_TimeVortexEvent;

    [FoldoutGroup("UI")] public Text label_Description;
    [FoldoutGroup("References")] public AnalogueClockTimeHand clockAnalogHand_UI;
    [FoldoutGroup("References")] public AnalogueClockTimeHand clockAnalogHand;
    [FoldoutGroup("References")] public GameObject button_TimeVortex;

    private List<MachineMad_SaveFileButton> allSaveFileButtons = new List<MachineMad_SaveFileButton>();
    [ShowInInspector] [ReadOnly] private List<HypatiosSave> allSaveFiles = new List<HypatiosSave>();

    private MachineMad_SaveFileButton selectedButton;

    private void OnEnable()
    {
        UpdateUI();
        button_TimeVortex.gameObject.SetActive(false);
        selectedButton = null;
        label_Description.text = "";
    }

    public void UpdateUI()
    {
        foreach(var button in allSaveFileButtons)
        {
            if (button != null)
                Destroy(button.gameObject);
        }

        allSaveFileButtons.Clear();

        allSaveFiles = Hypatios.Game.GetAllLevelSaves();

        if (!Hypatios.Game.DEBUG_ShowAllSaves)
        {
            allSaveFiles.RemoveAll(x => x.Game_TotalRuns != Hypatios.Game.TotalRuns);
        }

        int index = 0;

        foreach(var saveFile in allSaveFiles)
        {
            var button = Instantiate(saveFileButton_prefab, parent_SaveFiles);
            button.gameObject.SetActive(true);
            button.index = index;
            button.Refresh();

            allSaveFileButtons.Add(button);
            index++;
        }

    }

    public HypatiosSave GetSave(int index)
    {
        return allSaveFiles[index];
    }

    private void Update()
    {
     
    }

    #region Buttons

    public void SelectedButton(MachineMad_SaveFileButton button)
    {
        string s = "";
        var saveFile = GetSave(button.index);
        var chamberObj = Hypatios.Assets.GetLevel(saveFile.Game_LastLevelPlayed);
        var dateTime = ClockTimerDisplay.UnixTimeStampToDateTime(saveFile.Player_RunSessionUnixTime + Hypatios.UnixTimeStart);
        float maxHP = 100;
        maxHP += PlayerPerk.GetValue_MaxHPUpgrade(saveFile.AllPerkDatas.Perk_LV_MaxHitpointUpgrade);

        clockAnalogHand.UpdateClockHand(saveFile.Player_RunSessionUnixTime + Hypatios.UnixTimeStart);
        clockAnalogHand_UI.UpdateClockHand(saveFile.Player_RunSessionUnixTime + Hypatios.UnixTimeStart);

        s += $"{dateTime.Hour}:{dateTime.Minute.ToString("00")}:{dateTime.Second.ToString("00")}\n";
        s += chamberObj.TitleCard_Title + "\n";
        s += "\n";
        s += $"{Mathf.RoundToInt(saveFile.Player_CurrentHP)}/{maxHP}";
        button_TimeVortex.gameObject.SetActive(true);

        label_Description.text = s;
        selectedButton = button;
    }

    public void ClickButton()
    {
        
    }

    public void InitiateTimeVortex()
    {
        Hypatios.UI.ChangeCurrentMode(0);
        Hypatios.UI.SetPauseState(false);
        Hypatios.UI.canvas_Main.enabled = false;
        Hypatios.UI.disableInput = true;
        Hypatios.Player.Weapon.disableInput = true;
        MachineMadnessWeapon.Instance.InitiateTimeVortex(GetSave(selectedButton.index));
        g_TimeVortexEvent?.Raise();
        OnTimeVortexEvent?.Invoke();
    }

    #endregion

}
