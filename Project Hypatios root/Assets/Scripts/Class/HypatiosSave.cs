using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HypatiosSave
{

    public int Game_LastLevelPlayed = 0;
    public int Game_TotalRuns = 0;
    public int Game_TotalSouls = 0;
    [Space]

    [Header("Perks")]
    public float Game_Upgrade_MaxHP = 100;
    public float Game_Upgrade_RegenHP = 0;
    public int Game_Upgrade_LuckOfGod = 0;
    [Space]

    public float Player_CurrentHP = 100;
    public int Player_RunSessionUnixTime = 0;
    public List<WeaponDataSave> Game_WeaponStats = new List<WeaponDataSave>();
    public List<ParadoxEntity> Game_ParadoxEntities = new List<ParadoxEntity>();
    [Space]

    [Header("First Timers")]
    public bool everUsed_Paradox = false;
    public bool everUsed_WeaponShop = false;
    public List<string> otherEverUsed = new List<string>();

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
}
