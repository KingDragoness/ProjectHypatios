using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DisplayHighscores : MonoBehaviour 
{

    public HighscoreRankTemplateButtonUI buttonUI;
    public StatEntryButton statButton;
    public Transform parentHighscores;
    public Transform parentPlayerStat;
    public bool isPersistent = false;
    HighScores myScores;

    [SerializeField] private List<HighscoreRankTemplateButtonUI> pooledButtons = new List<HighscoreRankTemplateButtonUI>();
    [ReadOnly] [SerializeField] private List<StatEntryButton> pooledStatButtons = new List<StatEntryButton>();

    public int amountRank = 30;

    void Start() //Fetches the Data at the beginning
    {
        for (int i = 0; i < amountRank; i++)
        {
            var prefab1 = Instantiate(buttonUI, parentHighscores);
            prefab1.gameObject.SetActive(true);
            prefab1.labelRanking.text = $"#{i+1}";
            pooledButtons.Add(prefab1);
        }


        for (int i = 0; i < pooledButtons.Count;i ++)
        {
            var prefab1 = pooledButtons[i];
            prefab1.labelRanking.text = $"#{i+1} Fetching...";
        }

        myScores = FindObjectOfType<HighScores>();
        StartCoroutine("RefreshHighscores");
    }

    public void ChangeModeStat(bool _isPersist)
    {
        isPersistent = _isPersist;
        RefreshStats();
    }

    private void OnEnable()
    {
        myScores = FindObjectOfType<HighScores>();
        RefreshStats();
        StartCoroutine("RefreshHighscores");
    }

    public void RefreshStats()
    {
        foreach (var button in pooledStatButtons)
        {
            if (button != null)
                Destroy(button.gameObject);
        }

        pooledStatButtons.Clear();

        var allStatEntries = Hypatios.Assets.AllStatEntries;

        if (isPersistent)
        {
            int bonusButton = 2;
            //add bonus buttons for multiple runs
            for (int x = 0; x < bonusButton; x++)
            {
                AddBonusButton(x);
            }
        }

        foreach (var entry in allStatEntries)
        {
            if (entry.overallOnly && !isPersistent)
                continue;

            var prefab1 = Instantiate(statButton, parentPlayerStat);
            prefab1.gameObject.SetActive(true);
            prefab1.isPersistent = isPersistent;
            prefab1.ID = entry.ID;
            prefab1.Refresh();
            pooledStatButtons.Add(prefab1);
        }

       
    }

    private void AddBonusButton(int index)
    {
        string titleName = "";
        string display = "";

        if (index == 0)
        {
            titleName = "Total Runs";
            display = $"{Hypatios.Game.TotalRuns}";
        }
        else if (index == 1)
        {
            var dateTime = ClockTimerDisplay.UnixTimeStampToDateTime(Hypatios.Game.Total_UNIX_Timespan, true);
            titleName = "Total Time Played";
            display = $"{ClockTimerDisplay.TotalHoursPlayed(Hypatios.Game.Total_UNIX_Timespan).ToString("00")}:{dateTime.Minute.ToString("00")}:{dateTime.Second.ToString("00")}s";
        }


        var prefab1 = Instantiate(statButton, parentPlayerStat);
        prefab1.gameObject.SetActive(true);
        prefab1.isPersistent = true;
        prefab1.SetLabelManual(titleName, display);
        pooledStatButtons.Add(prefab1);
    }

    public void SetScoresToMenu(PlayerScore[] highscoreList) //Assigns proper name and score for each text value
    {
        var unixTimeStart = 1640087660;

        highscoreList = highscoreList.Reverse().ToArray();

        for (int i = 0; i < pooledButtons.Count;i ++)
        {
            var prefab1 = pooledButtons[i];
            var dateTime = UnixTimeStampToDateTime(0);

            prefab1.labelRanking.text = $"#{i + 1}";

            if (highscoreList.Length > i)
            {
                dateTime = UnixTimeStampToDateTime(-highscoreList[i].score + unixTimeStart);
                //rScores[i].text = highscoreList[i].score.ToString();
                prefab1.labelName.text = highscoreList[i].username;

            }

            prefab1.labelTimeUnix.text = $"{dateTime.Hour}:{dateTime.Minute.ToString("00")}:{dateTime.Second.ToString("00")}";
        }
    }
    IEnumerator RefreshHighscores() //Refreshes the scores every 30 seconds
    {
        while(true)
        {
            myScores.DownloadScores();
            yield return new WaitForSecondsRealtime(30);
        }
    }

public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
{
    // Unix timestamp is seconds past epoch
    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
    return dateTime;
}
}
