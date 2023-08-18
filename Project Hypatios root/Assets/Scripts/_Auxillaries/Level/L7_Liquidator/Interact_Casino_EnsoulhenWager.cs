using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Interact_Casino_EnsoulhenWager : Interact_Generic_Casino
{

    public int rewardSoul = 0;

    public override bool LockOnBet()
    {
        if (base.LockOnBet() == false)
        {
            return false;
        }


        rewardSoul = _totalSoul;
        Hypatios.Dialogue.QueueDialogue($"Ensoulhen Wager [{_totalSoul} souls]. Win and you'll win {_totalSoul * 2} souls. Lose, you will lose your money and die.", "SYSTEM", 5f, shouldOverride: true);
        return true;
    }

    public void TriggerReward()
    {
        Hypatios.Game.SoulPoint += rewardSoul * 2;
    }

}
