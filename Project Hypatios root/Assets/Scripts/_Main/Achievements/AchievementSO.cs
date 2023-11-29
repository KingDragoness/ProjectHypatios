using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "FirstTrivia", menuName = "Hypatios/Achievement", order = 1)]

public class AchievementSO : ScriptableObject
{

    [SerializeField] private string ID = "FirstTrivia";
    public string Title = "Forbidden Knowledge";
    public Sprite unlockedSprite;
    public Sprite lockedSprite;
    public bool dontShowLocked = false; //dont' show for spoilers and plot-related.

    [TextArea(3, 6)] public string Description = "Gains the first trivia.";

    public string RawID { get => ID; }

    public string GetID()
    {
        return $"Achievements.{ID}";
    }

    [FoldoutGroup("DEBUG")]
    [Button("Trigger Achievement")]
    public void TriggerAchievement()
    {
        Hypatios.Achievement.TriggerAchievement(this);
    }

}
