using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interact_NewsTV : MonoBehaviour
{
    [System.Serializable]
    public class Newsreel
    {
        public int hour = 18;
        public int minute = 54;
        public Interact_MultiDialoguesTrigger dialoguePrefab;
    }

    public List<Newsreel> AllNewsReels = new List<Newsreel>();

    private Newsreel GetNewsreel()
    {
        Newsreel result = AllNewsReels[0];
        var dateTime = ClockTimerDisplay.UnixTimeStampToDateTime(Hypatios.Game.UNIX_Timespan + Hypatios.UnixTimeStart);
        int highestMinute_raw = 0;
        int myTime_raw = dateTime.Minute + (dateTime.Hour * 60);

        foreach (var news in AllNewsReels)
        {
            int rawMinute = news.minute + (news.hour * 60);

            if (myTime_raw < rawMinute)
                continue;

            if (rawMinute > highestMinute_raw)
            {
                //select this
                result = news;
                highestMinute_raw = rawMinute;
            }
        }

        return result;

    }

    public void Interact()
    {
        var newsreel = GetNewsreel();
        var objectPrefab1 = Instantiate(newsreel.dialoguePrefab);
        objectPrefab1.TriggerMessage();
        Destroy(objectPrefab1, 1f);
    }
}
