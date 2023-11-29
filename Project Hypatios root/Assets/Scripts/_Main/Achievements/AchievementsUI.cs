using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsUI : MonoBehaviour
{

    public Transform parentAchievementStat;
    public Achievement_EntryButton achievementButton;
    public Text label_title;
    public Text label_Description;

    [ReadOnly] [SerializeField] private List<Achievement_EntryButton> pooledAchievementButtons = new List<Achievement_EntryButton>();
    private Achievement_EntryButton currentButton;

    private void OnEnable()
    {
        label_Description.text = "";
        label_title.text = "";
        currentButton = null;
    }

    private void Update()
    {
        if (currentButton != null)
        {
            bool showDescription = false;

            if (Hypatios.Achievement.HasAchievementTriggered(currentButton.achievementSO) == true)
            {
                showDescription = true;
            }

            if (showDescription)
            {
                label_title.text = currentButton.achievementSO.Title;
                label_Description.text = currentButton.achievementSO.Description;
            }
            else
            {
                label_title.text = RandomNumberGenerator.RandomText(3);
                label_Description.text = RandomNumberGenerator.RandomText(5) + currentButton.achievementSO.Description + RandomNumberGenerator.RandomText(5);
            }
        }
    }

    public void Refresh()
    {
        foreach(var button in pooledAchievementButtons)
        {
            if (button == null) continue;
            Destroy(button.gameObject);
        }

        pooledAchievementButtons.Clear();

        foreach(var achievement in Hypatios.Assets.AllAchievements)
        {
            if (achievement.dontShowLocked && Hypatios.Achievement.HasAchievementTriggered(achievement) == false) continue;

            var button = Instantiate(achievementButton, parentAchievementStat);
            button.gameObject.SetActive(true);
            button.achievementSO = achievement;

            if (Hypatios.Achievement.HasAchievementTriggered(achievement))
            {
                button.achievementIcon.sprite = achievement.unlockedSprite;
            }
            else
            {
                button.achievementIcon.sprite = achievement.lockedSprite;
            }

            pooledAchievementButtons.Add(button);
        }
    }

    public void HighlightButton(Achievement_EntryButton button)
    {
        currentButton = button;
        //bool blurDescription = false;

        if (Hypatios.Achievement.HasAchievementTriggered(button.achievementSO) == false && button.achievementSO.dontShowLocked)
        {
            //blurDescription = true;
        }
      

    }

    public void DehighlightButton()
    {
        label_title.text = "";
        label_Description.text = "";
    }
}
