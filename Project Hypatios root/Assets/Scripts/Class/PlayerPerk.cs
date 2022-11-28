using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerPerks
{
    HealthMax,
    HealthRegen,
    LuckGod
}

public class PlayerPerk
{
    
    public static int GetBonusSouls()
    {
        int soulAmount = 0;

        float chance = Random.Range(0f, 1f);

        if (Hypatios.Game.LuckOfGod_Level == 1)
        {
            if (chance < 0.4f)
            {
                soulAmount += 1;
            }
        }
        else if (Hypatios.Game.LuckOfGod_Level == 2)
        {
            if (chance < 0.6f)
            {
                soulAmount += 1;
            }
            if (chance < 0.2f)
            {
                soulAmount += 1;
            }
        }
        else if (Hypatios.Game.LuckOfGod_Level == 3)
        {
            soulAmount += 1;

            if (chance < 0.4f)
            {
                soulAmount += 1;
            }
            if (chance < 0.1f)
            {
                soulAmount += 1;
            }
        }
        else if (Hypatios.Game.LuckOfGod_Level == 4)
        {
            soulAmount += 2;

            if (chance < 0.4f)
            {
                soulAmount += 1;
            }
            if (chance < 0.1f)
            {
                soulAmount += 1;
            }
        }
        else if (Hypatios.Game.LuckOfGod_Level == 5)
        {
            soulAmount += 3;

            if (chance < 0.5f)
            {
                soulAmount += 1;
            }
            if (chance < 0.1f)
            {
                soulAmount += 1;
            }
        }

        return soulAmount;
    }

    public static int GetPrice_MaxHP(float currentHP, float targetHP)
    {
        int priceSoul = 0;

        float a = (targetHP - currentHP)/2f;
        a += currentHP / 16;
        priceSoul = Mathf.RoundToInt(a);

        return priceSoul;
    }

    public static int GetPrice_RegenHP(float currenRegen, float targetRegen)
    {
        int priceSoul = 0;

        float a = Mathf.Pow(1 + (targetRegen - currenRegen), 2f) * 10;
        a += currenRegen * 100;
        priceSoul = Mathf.RoundToInt(a);

        return priceSoul;
    }

    public static int GetPrice_LuckOfGod(int levelTarget)
    {
        int priceSoul = 0;

        if (levelTarget == 0)
        {
            priceSoul = 8;
        }
        else if (levelTarget == 1)
        {
            priceSoul = 14;
        }
        else if (levelTarget == 2)
        {
            priceSoul = 23;
        }
        else if (levelTarget == 3)
        {
            priceSoul = 30;
        }
        else if (levelTarget == 4)
        {
            priceSoul = 45;
        }

        return priceSoul;
    }

    public static string GetDescription_LuckOfGod(int level)
    {
        string s = "";

        if (level == 0)
        {
            s = "[RECOMMENDED] 40% chance for +1 soul.";
        }
        else if (level == 1)
        {
            s = "[RECOMMENDED] 40% chance for +1 soul, 20% chance for +2 souls.";
        }
        else if (level == 2)
        {
            s = "[RECOMMENDED] +1 souls by default. 30% chance for +2 souls, 10% chance for +3 souls.";
        }
        else if (level == 3)
        {
            s = "[RECOMMENDED] +2 souls by default. 30% chance for +3 souls, 10% chance for +4 souls.";
        }
        else if (level == 4)
        {
            s = "[RECOMMENDED] +3 souls by default. 40% chance for +4 souls, 10% chance for +5 souls.";
        }

        return s;
    }
}
