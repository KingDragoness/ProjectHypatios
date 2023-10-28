using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MachineMad_SaveFileButton : MonoBehaviour
{

    public MachineOfMadnessUI machineUI;
    public Text label_LevelName;
    public Text label_Time;
    public int index = 0;

    public void Refresh()
    {
        var saveFile = machineUI.GetSave(index);
        var chamberObj = Hypatios.Assets.GetLevel(saveFile.Game_LastLevelPlayed);
        var dateTime = ClockTimerDisplay.UnixTimeStampToDateTime(saveFile.Player_RunSessionUnixTime + Hypatios.UnixTimeStart);

        label_LevelName.text = chamberObj.TitleCard_Title;
        label_Time.text = $"{dateTime.Hour}:{dateTime.Minute.ToString("00")}:{dateTime.Second.ToString("00")}";

    }

    public void HighlightButton()
    {
        machineUI.SelectedButton(this);
    }

    public void DehighlightButton()
    {

    }

}
