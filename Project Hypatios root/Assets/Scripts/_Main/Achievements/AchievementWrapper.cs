using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AchievementWrapper : MonoBehaviour
{

    [Header("Achievements")]
    public AchievementSO FirstDeath;
    public AchievementSO FirstTrivia;
    public AchievementSO FirstChamber;
    public AchievementSO TheratiosElimination;
    public AchievementSO WIRED;
    public AchievementSO Eclipseblazer;
    public AchievementSO ToTheAbyss; //manual trigger
    public AchievementSO EternalLoop;
    [Space]
    public AchievementSO Drunk;
    public AchievementSO Blackout; //manual trigger
    public AchievementSO HP_lv1;
    public AchievementSO HP_lv2;
    public AchievementSO Movement_lv1;
    public AchievementSO Gun_lv1;
    public AchievementSO Soul_lv1;
    public AchievementSO Soul_lv2;

    [FoldoutGroup("References")] public Trivia trivia_theBeginnings;
    [FoldoutGroup("References")] public Trivia trivia_Eclipseblazer;
    [FoldoutGroup("References")] public Trivia trivia_Theratios;


    [Space]
    public bool DEBUG_Printconsole = false;
    public float cooldownCheck = 0.5f;
    private float timerCheck = 0.2f;

    private void Update()
    {
        //If it is not default level, return
        if (Hypatios.Chamber == null) return;

        timerCheck -= Time.deltaTime;

        if (timerCheck <= 0f)
        {
            timerCheck = cooldownCheck;
        }
        else
        {
            return;
        }

        //check achievements
        CheckAchievements();
    }

    public void CheckAchievements()
    {
        //'Mandatory' progressions
        {
            if (Hypatios.Game.TotalRuns >= 1)
            {
                TriggerAchievement(FirstDeath);
            }

            if (Hypatios.Game.Check_TriviaCompleted(trivia_theBeginnings))
            {
                TriggerAchievement(FirstChamber);
            }

            if (Hypatios.Game.Game_Trivias.Count >= 1)
            {
                TriggerAchievement(FirstTrivia);
            }

            if (Hypatios.Game.Check_TriviaCompleted(trivia_Theratios))
            {
                TriggerAchievement(TheratiosElimination);
            }

            if (Hypatios.Game.Check_TriviaCompleted(trivia_Eclipseblazer))
            {
                TriggerAchievement(Eclipseblazer);
            }

            if (Conditioner.IsWIREDChamber())
            {
                TriggerAchievement(WIRED);
            }

            if (Hypatios.Game.TotalRuns >= 20)
            {
                TriggerAchievement(EternalLoop);
            }

        }

        //perk progressions
        {
            if (Hypatios.Player.Health.alcoholMeter >= 25f)
            {
                TriggerAchievement(Drunk);
            }

            if (Hypatios.Player.Health.maxHealth.Value >= 400f)
            {
                TriggerAchievement(HP_lv1);
            }

            if (Hypatios.Player.Health.maxHealth.Value >= 1000f)
            {
                TriggerAchievement(HP_lv2);
            }

            if (Hypatios.Player.speedMultiplier.Value >= 40f)
            {
                TriggerAchievement(Movement_lv1);
            }

            if (Hypatios.Player.BonusDamageGun.Value >= 6f)
            {
                TriggerAchievement(Gun_lv1);
            }

            if (Hypatios.Game.SoulPoint >= 5000)
            {
                TriggerAchievement(Soul_lv1);
            }

            if (Hypatios.Game.SoulPoint >= 10000)
            {
                TriggerAchievement(Soul_lv2);
            }
        }
    }

    public void TriggerAchievement(AchievementSO achievementSO)
    {
        if (HasAchievementTriggered(achievementSO))
        {
            if (DEBUG_Printconsole) Debug.LogError($"{achievementSO.Title} has already been triggered!");
            return;
        }

        Hypatios.Game.TryAdd_EverUsed(achievementSO.GetID());
        //show achievement UI
        Hypatios.UI.AchievementNotify_UI.TriggerNotification(achievementSO);
    }

    public bool HasAchievementTriggered(AchievementSO achievementSO)
    {
        if (Hypatios.Game.Check_EverUsed(achievementSO.GetID()) == true)
        {
            return true;
        }

        return false;
    }
}
