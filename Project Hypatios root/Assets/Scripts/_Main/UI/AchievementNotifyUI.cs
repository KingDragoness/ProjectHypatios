using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class AchievementNotifyUI : MonoBehaviour
{

    public Image image_achievement;
    public Text label_Title;
    public Text label_Description;

    public void TriggerNotification(AchievementSO achievement)
    {
        gameObject.SetActive(true);
        image_achievement.sprite = achievement.unlockedSprite;
        label_Title.text = achievement.Title;
        label_Description.text = achievement.Description;
        MainGameHUDScript.Instance.audio_achievement.Play();
    }

}
