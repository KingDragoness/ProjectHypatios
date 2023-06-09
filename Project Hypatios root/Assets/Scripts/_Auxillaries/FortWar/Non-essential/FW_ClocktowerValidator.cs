using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FW_ClocktowerValidator : MonoBehaviour
{

    public Transform hand_Hour;
    public Transform hand_Minute;
    public Transform hand_Second;

    private void Start()
    {
        OnPause();
    }


    public void OnPause()
    {
        var dateTime = ClockTimerDisplay.UnixTimeStampToDateTime(Hypatios.Game.UNIX_Timespan + Hypatios.UnixTimeStart);
        int hour = dateTime.Hour;
        int minute = dateTime.Minute;
        int second = dateTime.Second;
        if (hour >= 12) hour -= 12;

        float rotX_hour = 360 - (hour/12f + (minute/24f/60f)) * 360f;
        float rotX_minute = 360 - (minute / 60f + (second /60f/60f)) * 360f;
        float rotX_second = 360 - (second / 60f) * 360f;

        hand_Hour.transform.eulerAngles = new Vector3(rotX_hour, 0, 0);
        hand_Minute.transform.eulerAngles = new Vector3(rotX_minute, 0, 0);
        hand_Second.transform.eulerAngles = new Vector3(rotX_second, 0, 0);

    }

}
