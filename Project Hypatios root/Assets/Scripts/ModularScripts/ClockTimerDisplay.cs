using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockTimerDisplay : MonoBehaviour
{

    public TextMesh clockTimeText;
    public int unixTimeStart = 1640087660;

    private void Update()
    {
        var dateTime = UnixTimeStampToDateTime(Hypatios.Game.UNIX_Timespan + unixTimeStart);

        clockTimeText.text = $"{dateTime.Hour}:{dateTime.Minute.ToString("00")}:{dateTime.Second.ToString("00")}";
    }

    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }

}
