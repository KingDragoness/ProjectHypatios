using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModularUI_ClockTime : MonoBehaviour
{
    public Text time_label;


    // Update is called once per frame
    void Update()
    {
        System.DateTime dateTime = ClockTimerDisplay.UnixTimeStampToDateTime(Hypatios.Game.UNIX_Timespan + Hypatios.UnixTimeStart);

        if (ChamberLevelController.Instance.chamberObject.isWIRED)
        {
            dateTime = ClockTimerDisplay.UnixTimeStampToDateTime(Hypatios.Game.GetUnixTime_WIRED() + Hypatios.UnixTimeStart);

        }

        time_label.text = $"{dateTime.Hour}:{dateTime.Minute.ToString("00")}:{dateTime.Second.ToString("00")}";

    }
}
