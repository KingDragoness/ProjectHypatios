using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "WING Corps", menuName = "Hypatios/StockCompany", order = 1)]
public class StockExchange_ProfileObject : ScriptableObject
{

    [InfoBox("Index always four letters.")]
    public string indexID = "WING"; 
    public string companyDisplayName = "WING Corps";
    public Sprite companyLogo;
    [TextArea(2,3)] public string companyDescription = "Manufactures infantry weapon, heavy gun, artillery and ammunition.";
    public int seed = 128101;

    [Space]
    [InfoBox("2 souls = 1 centurion")]
    public float startingSharePrice = 93.5f;
    public float marketCap = 322.4f; //in billions, Centurion
    public bool isMobiusCorps = false; //Mobius Corps falling 1% per 5 minutes

    private static float MINOR_DEVIATION = 0.07f; //7% maximum changes, per 5 minute
    private static float MAJOR_DEVIATION = 0.15f; //multiplies 15% maximum changes, per 1 hour
    private static int DIVIDER_MINUTE = 5;

    public float[] GetCurrentSharePrice(int length)
    {
        float[] data = new float[length];

        //get current time, length * divider_minute
        var currentUnix = Hypatios.Game.UNIX_Timespan + Hypatios.UnixTimeStart;
        var currentDateTime = ClockTimerDisplay.UnixTimeStampToDateTime(currentUnix);

        //closest to 5 minute
        int closest5Unix = Mathf.FloorToInt((currentUnix) / (60 * DIVIDER_MINUTE));
        closest5Unix *= 60 * DIVIDER_MINUTE;


        for (int x = 0; x < length; x++)
        {
            int timeReverseAmount = x * DIVIDER_MINUTE * 60;
            var unixTime = closest5Unix - timeReverseAmount;
            var current_dt1 = ClockTimerDisplay.UnixTimeStampToDateTime(unixTime);

            float sharePrice = startingSharePrice;

            if (isMobiusCorps == false)
            {
                int movement_minor = 0;
                int movement_major = 0;
                var _stockSeed = seed - current_dt1.Hour - current_dt1.Minute;
                var _stockSeed_Major = seed - current_dt1.Hour;
                var RandomSys = new System.Random(_stockSeed);
                var RandomSys1 = new System.Random(_stockSeed_Major);
                movement_minor = RandomSys.Next(-200, 0);
                movement_major = RandomSys1.Next(-100, 100);
                sharePrice *= 1 + (movement_minor * 0.01f * MINOR_DEVIATION);
                sharePrice *= 1f + (movement_major * 0.01f * MAJOR_DEVIATION);
            }
            else
            {
                var _stockSeed = seed - current_dt1.Hour - current_dt1.Minute;
                float cumulative = (current_dt1.Hour * 60 + current_dt1.Minute);
                var RandomSys = new System.Random(_stockSeed);
                int movement_minor = RandomSys.Next(-10, 10);
                float pz1 = (cumulative / 1440f) * 1f;
                float l1 = 0;
                if (current_dt1.Hour >= 19)
                {
                    l1 += Mathf.Clamp(((19 - current_dt1.Hour) * 60f + current_dt1.Minute) / 400f, 0f,0.5f);
                }
                sharePrice *= 1f + (movement_minor * 0.01f * MINOR_DEVIATION);
                sharePrice *= (1.7f - pz1 - l1);
            }

            data[x] = sharePrice;
        }


        return data;
    }

    public string[] GetHorizontalLabels(int length)
    {
        string[] data = new string[length];

        //get current time, length * divider_minute
        var currentUnix = Hypatios.Game.UNIX_Timespan + Hypatios.UnixTimeStart;
        var currentDateTime = ClockTimerDisplay.UnixTimeStampToDateTime(currentUnix);

        //closest to 5 minute
        int closest5Unix = Mathf.FloorToInt((currentUnix) / (60 * DIVIDER_MINUTE));
        closest5Unix *= 60 * DIVIDER_MINUTE;


        for (int x = 0; x < length; x++)
        {
            int timeReverseAmount = x * DIVIDER_MINUTE * 60;
            var unixTime = closest5Unix - timeReverseAmount;
            var current_dt1 = ClockTimerDisplay.UnixTimeStampToDateTime(unixTime);

            if (current_dt1.Minute == 0 | current_dt1.Minute == 15 | current_dt1.Minute == 30 | current_dt1.Minute == 45) data[x] = $"{current_dt1.Hour}:{current_dt1.Minute.ToString("00")}";
            else data[x] = "";
        }


        return data;
    }

    public float GetNetChange()
    {
        var data1 = GetCurrentSharePrice(2);
        float delta = data1[0] - data1[1];
        float percent = (delta / data1[1]);
        percent = Mathf.Round(percent * 100f * 10f) / 10;

        return percent;
    }

    public string GetString_NetChange()
    {
        float percent = GetNetChange();
        bool isUp = Mathf.Sign(percent) > 0f ? true : false;

        if (isUp)
        {
            return $"▲ {Mathf.Abs(percent)}%";
        }
        else
        {
            return $"▼ {Mathf.Abs(percent)}%";
        }
    }

}
