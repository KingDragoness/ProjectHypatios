using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class AchievementTrigger : MonoBehaviour
{

    public AchievementSO achievementSO;
    public bool useConditioner = false;
    [ShowIf("useConditioner", true)] public Conditioner conditioner;
    [ShowIf("useConditioner", true)] public float cooldownCheck = 0.5f;

    private float timerCheck = 0.2f;
    private bool hasSuccessful = false;


    [FoldoutGroup("DEBUG")] [Button("Trigger")]
    public void TriggerAchievement()
    {
        Hypatios.Achievement.TriggerAchievement(achievementSO);
    }


    private void Update()
    {
        if (useConditioner && hasSuccessful == false)
        {
            timerCheck -= Time.deltaTime;

            if (timerCheck <= 0f)
            {
                CheckAchievements();
                timerCheck = cooldownCheck;
            }
            else
            {
            }
        }
    }

    public void CheckAchievements()
    {
        if (conditioner.GetEvaluateResult() == true)
        {
            TriggerAchievement();
            hasSuccessful = true;
        }
    }


}
