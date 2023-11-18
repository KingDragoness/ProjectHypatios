using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

//PlayerPerks needs to be removed from the game
public enum PlayerPerks
{
    HealthMax,
    HealthRegen,
    LuckGod
}


[System.Serializable]
public class PerkCustomEffect
{
    public ModifierEffectCategory statusCategoryType = ModifierEffectCategory.Nothing;
    public string origin = "DeathPerk";
    public float Value = 0.1f;

    //Only for serum mode
    public bool isPermanent = true;
    public bool isAntiPotion = false;
    [HideIf("isPermanent")] public float timer = 9999f;

    public void Generate(string _origin)
    {
        origin = _origin;
        Value = BasePerk.GenerateValueForCustomPerk(statusCategoryType);
    }
}

[System.Serializable]
public class StatusEffectData
{
    public string ID = "Bleeding";
    public float Time = 10f;
}

public class PlayerPerk
{



    public static BasePerk GetBasePerk(ModifierEffectCategory type)
    {
        return Hypatios.Assets.AllBasePerks.Find(x => x.category == type);
    }

    public static BasePerk RandomPickBasePerk()
    {
        var ListPerk = new List<BasePerk>();
        foreach (var entry in Hypatios.Assets.AllBasePerks) ListPerk.Add(entry);
        ListPerk.RemoveAll(fx => fx.CheckLevelMaxed());
        { ListPerk.RemoveAll(e => e.BannedInGauntlet == true && Hypatios.Game.currentGamemode.isGauntlet == true); }

        int[] allProbability = new int [ListPerk.Count];

        int x = 0;
        foreach(var entry in ListPerk)
        {
            allProbability[x] = Mathf.RoundToInt(entry.Commonness);
            x++;
        }

        int pickedIndex = IsopatiosUtility.Choose(allProbability);
        return ListPerk[pickedIndex];

    }

    public static BasePerk RandomPickBaseTempPerk()
    {
        var ListPerk = new List<BasePerk>();
        foreach (var entry in Hypatios.Assets.AllBasePerks) ListPerk.Add(entry);
        ListPerk.RemoveAll(c => c.CheckLevelMaxed() && c.TemporaryPerkOverLimit == false);
        { ListPerk.RemoveAll(d => d.NoTemporaryPerk == true); }

        int[] allProbability = new int[ListPerk.Count];

        int x = 0;
        foreach (var entry in ListPerk)
        {
            allProbability[x] = Mathf.RoundToInt(entry.Commonness);
            x++;
        }

        int pickedIndex = IsopatiosUtility.Choose(allProbability);
        return ListPerk[pickedIndex];

    }


    public static float GetValue_MaxHPUpgrade(int level)
    {
        if (level == 0) return 0;

        float bonusHP = level * 8;
        return bonusHP;
    }

    public static float GetValue_RegenHPUpgrade(int level)
    {
        if (level == 0) return 0;

        float bonusRegen = level * 0.1f;
        return bonusRegen;
    }

    public static float GetValue_BonusMeleeDamage(int level)
    {
        if (level == 0) return 0;

        float bonusMelee = level * 0.12f;
        return bonusMelee;
    }

    public static float GetValue_BonusGunDamage(int level)
    {
        if (level == 0) return 0;

        float bonusGun = level * 0.05f;
        return bonusGun;
    }


    public static float GetValue_KnockbackResistUpgrade(int level)
    {
        if (level == 0) return 0;

        float bonusResistKnock = (level * 0.15f);
        return bonusResistKnock;
    }


    public static float GetValue_RecoilUpgrade(int level)
    {
        if (level == 0) return 0;

        float bonusResistRecoil = -(level * 0.06f);
        return bonusResistRecoil;
    }

    public static float GetValue_Dashcooldown(int level)
    {
        if (level == 0) return 4;

        float cooldown = 4 - (level * 0.25f);
        return cooldown;
    }


    public static int GetBonusSouls()
    {
        int soulAmount = 0;

        float chance = Random.Range(0f, 1f);
        var soulLevel = Hypatios.Player.GetNetSoulBonusPerk();

        if (soulLevel == 1)
        {
            if (chance < 0.3f)
            {
                soulAmount += 1;
            }
        }
        else if (soulLevel == 2)
        {
            if (chance < 0.6f)
            {
                soulAmount += 1;
            }
            if (chance < 0.1f)
            {
                soulAmount += 1;
            }
        }
        else if (soulLevel == 3)
        {
            soulAmount += 1;

            if (chance < 0.3f)
            {
                soulAmount += 1;
            }
        
        }
        else if (soulLevel == 4)
        {
            soulAmount += 2;

            if (chance < 0.3f)
            {
                soulAmount += 1;
            }
       
        }
        else if (soulLevel == 5)
        {
            soulAmount += 3;

            if (chance < 0.3f)
            {
                soulAmount += 1;
            }
   
        }

        return soulAmount;
    }



    #region Legacy
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
    #endregion


    public static float GetBonusShortcutDiscount(int level)
    {
        float discount = 0f;

        var shortcutLevel = level;

        if (shortcutLevel == 0)
            discount -= 0.06f;
        else if (shortcutLevel == 1)
            discount -= 0.12f;
        else if (shortcutLevel == 2)
            discount -= 0.19f;
        else if (shortcutLevel == 3)
            discount -= 0.26f;
        else if (shortcutLevel == 4)
            discount -= 0.35f;
        else if (shortcutLevel == 5)
            discount -= 0.49f;

        return discount;
    }

    public static string GetDescription_Shortcut(int level)
    {
        string s = "";

        float discount = GetBonusShortcutDiscount(level);

        s = $"{Mathf.RoundToInt(discount*100f)}% discount for shortcuts in the sewer's subway train.";

        return s;
    }

    public static string GetDescription_LuckOfGod(int level)
    {
        string s = "";

        if (level == 0)
        {
            s = "30% chance for +1 soul.";
        }
        else if (level == 1)
        {
            s = "60% chance for +1 soul, 10% chance for +2 souls.";
        }
        else if (level == 2)
        {
            s = "+1 souls by default. 30% chance for +2 souls.";
        }
        else if (level == 3)
        {
            s = "+2 souls by default. 30% chance for +3 souls.";
        }
        else if (level == 4)
        {
            s = "+3 souls by default. 30% chance for +4 souls.";
        }

        return s;
    }
}
