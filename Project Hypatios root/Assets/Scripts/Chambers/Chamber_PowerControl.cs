using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chamber_PowerControl : MonoBehaviour
{

    private void Start()
    {
        CheckCompletedRun();
    }

    private void CheckCompletedRun()
    {
        if (PlayerPrefs.HasKey("HIGHSCORE.CHECK")) return;

        var name = PlayerPrefs.GetString("SETTINGS.MY_NAME");

        HighScores.UploadScore(name, -Mathf.RoundToInt(Hypatios.Game.UNIX_Timespan));
        PlayerPrefs.SetString("HIGHSCORE.CHECK", "meh");
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        PlayerPrefs.DeleteKey("HIGHSCORE.CHECK");
    }    

}
