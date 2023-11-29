using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Achievement_EntryButton : MonoBehaviour
{

    public Image achievementIcon;
    public AchievementSO achievementSO;
    public AchievementsUI achievementsUI;


    public void Highlight()
    {
        achievementsUI.HighlightButton(this);
    }

    public void Dehighlight()
    {
        achievementsUI.DehighlightButton();
    }

}
