using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HypatiosSave
{
    #region Persistent
    public int Game_LastLevelPlayed = 0;
    public int Game_TotalRuns = 1;
    public int Game_TotalSouls = 0;
    [Space]

    [Header("Perks")]
    public PerkDataSave AllPerkDatas = new PerkDataSave();


    [Header("Flags/Paradoxes")]
    public bool everUsed_Paradox = false;
    public bool everUsed_WeaponShop = false;
    public List<string> otherEverUsed = new List<string>();
    public List<ParadoxEntity> Game_ParadoxEntities = new List<ParadoxEntity>();

    [Space]
    #endregion

    #region Single run only
    public float Player_CurrentHP = 100;
    public int Player_RunSessionUnixTime = 0;
    public List<WeaponDataSave> Game_WeaponStats = new List<WeaponDataSave>();



    [System.Serializable]
    public class PerkDataSave
    {
        public int Perk_LV_MaxHitpointUpgrade = 0;
        public int Perk_LV_RegenHitpointUpgrade = 0;
        public int Perk_LV_Soulbonus = 0;
        public int Perk_LV_ShortcutDiscount = 0;
        public int Perk_LV_KnockbackRecoil = 0;
        public int Perk_LV_DashCooldown = 0;
        public int Perk_LV_IncreaseMeleeDamage = 0;
        public List<PerkCustomEffect> Temp_CustomPerk = new List<PerkCustomEffect>();

    }

    [System.Serializable]
    public class WeaponDataSave
    {
        public string weaponID;
        public int totalAmmo;
        public int currentAmmo;
        public int level_Damage = 0;
        public int level_MagazineSize = 0;
        public int level_Cooldown = 0;
        public bool removed = false;
    }
    #endregion
}
