using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RogueliteRewarding : MonoBehaviour
{

    [FoldoutGroup("Stats")] public BaseStatValue stat_EnemyKills;
    [ReadOnly] public int nextLevelEnemyKills = 0;
    public int levelUp_EnemyKills = 50;

    [SerializeField] private float refreshTime = 0.1f;
    private float _timer;
    private int currentKills = 0;
    private int lastKillUpgrade = 0;
    private HypatiosSave.PlayerStatValueSave killStat;

    private void Start()
    {
        nextLevelEnemyKills = 9999;
        killStat = Hypatios.Game.Get_StatEntryData(stat_EnemyKills, true);
        if (killStat != null)
            RefreshNextLevel();
    }


    private void Update()
    {
        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
            return;
        }

        _timer = refreshTime;
        CheckEnemyKill();

    }

    private void RefreshNextLevel()
    {
        currentKills = killStat.value_int;

        float f = (currentKills / (float)levelUp_EnemyKills); //222 / 50 = 4.44 [0 / 50 = 0]
        nextLevelEnemyKills = Mathf.CeilToInt(f) * levelUp_EnemyKills; //4.44 -> 5
        if (nextLevelEnemyKills < 1)
        {
            nextLevelEnemyKills = levelUp_EnemyKills;
        }
    }

    private void CheckEnemyKill()
    {
        if (killStat == null)
        {
            killStat = Hypatios.Game.Get_StatEntryData(stat_EnemyKills, true);
            return;
        }
        currentKills = killStat.value_int;

        if (currentKills >= nextLevelEnemyKills && lastKillUpgrade != currentKills)
        {
            RewardPerkHP();
        }
        RefreshNextLevel();
   


   
    }

    [FoldoutGroup("Debug")]
    [Button("Reward Perk")]
    public void RewardPerkHP()
    {
        var perkData = Hypatios.Player.PerkData;
        float chance = Random.Range(0f, 1f);

        if (chance < 0.75f)
        {
            //Max HP
            perkData.AddPerkLevel(ModifierEffectCategory.MaxHitpointBonus);
        }
        else
        {
            //Regen HP
            perkData.AddPerkLevel(ModifierEffectCategory.RegenHPBonus);
        }

        lastKillUpgrade = currentKills;
        Hypatios.Player.ReloadStatEffects();

    }
}
