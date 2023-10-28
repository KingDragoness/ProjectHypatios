using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AnalogueClockTimeHand : MonoBehaviour
{

    public Transform hand_Hour;
    public Transform hand_Minute;
    public Transform hand_Second;
    public bool useX = true;

    [Button("Test Clock Hand")]
    public void DEBUG_ClockHand()
    {
        UpdateClockHand(Hypatios.Game.UNIX_Timespan + Hypatios.UnixTimeStart);
    }


    public void UpdateClockHand(double unixTime)
    {
        var dateTime = ClockTimerDisplay.UnixTimeStampToDateTime(unixTime);
        int hour = dateTime.Hour;
        int minute = dateTime.Minute;
        int second = dateTime.Second;
        if (hour >= 12) hour -= 12;

        float rotX_hour = 360 - (hour / 12f + (minute / 24f / 60f)) * 360f;
        float rotX_minute = 360 - (minute / 60f + (second / 60f / 60f)) * 360f;
        float rotX_second = 360 - (second / 60f) * 360f;

        if (useX)
        {
            hand_Hour.transform.localEulerAngles = new Vector3(rotX_hour, 0, 0);
            hand_Minute.transform.localEulerAngles = new Vector3(rotX_minute, 0, 0);
            hand_Second.transform.localEulerAngles = new Vector3(rotX_second, 0, 0);
        }
        else
        {
            hand_Hour.transform.localEulerAngles = new Vector3(0, 0, rotX_hour);
            hand_Minute.transform.localEulerAngles = new Vector3(0, 0, rotX_minute);
            hand_Second.transform.localEulerAngles = new Vector3(0, 0, rotX_second);
        }
    }

}
