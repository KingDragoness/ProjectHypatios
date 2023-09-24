using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using ItemDataSave = HypatiosSave.ItemDataSave;
using Newtonsoft.Json;
using System;

[System.Serializable]
public class Inventory
{
    public List<ItemDataSave> allItemDatas = new List<ItemDataSave>();
    public gEvent_OnItemAdd OnItemAdded;

    //search
    public ItemDataSave SearchByID(string ID)
    {
        return allItemDatas.Find(x => x.ID == ID);
    }

    [Button("Add Item")] //Create new item from scratch
    public ItemDataSave AddItem(ItemInventory itemInventory, int count = 1)
    {
        ItemDataSave itemDataSave = null;


        if (SearchByID(itemInventory.GetID()) != null && itemInventory.category != ItemInventory.Category.Weapon
            && itemInventory.isGenericItem == false)
        {
            itemDataSave = SearchByID(itemInventory.GetID());
            itemDataSave.count += count;

        }
        else
        {
            itemDataSave = new ItemDataSave();
            itemDataSave.ID = itemInventory.GetID();
            itemDataSave.category = itemInventory.category;
            itemDataSave.count = count;
            if (itemDataSave.category == ItemInventory.Category.Weapon) itemDataSave.GenerateWeaponData();
            allItemDatas.Add(itemDataSave);
        }

        OnItemAdded?.Raise(itemInventory);
        return itemDataSave;
    }

    public ItemDataSave AddItemGenericSafe(ItemInventory itemInventory, ItemDataSave _itemDat, int count = 1)
    {
        ItemDataSave itemDataSave = null;


        if (SearchByID(itemInventory.GetID()) != null 
            && itemInventory.category != ItemInventory.Category.Weapon
            && _itemDat.isGenericItem == false)
        {
            itemDataSave = SearchByID(itemInventory.GetID());
            itemDataSave.count += count;

        }
        else
        {
            if (_itemDat.isGenericItem == false)
            {
                itemDataSave = new ItemDataSave();
                itemDataSave.ID = itemInventory.GetID();
                itemDataSave.category = itemInventory.category;
                itemDataSave.count = count;
                if (itemDataSave.category == ItemInventory.Category.Weapon) itemDataSave.GenerateWeaponData();
                allItemDatas.Add(itemDataSave);
            }
            else
            {
                allItemDatas.Add(_itemDat.Clone());
            }
        }

        OnItemAdded?.Raise(itemInventory);
        return itemDataSave;
    }

    public void RemoveItem(ItemDataSave targetData, int count = 1)
    {
        targetData.count -= count;

        if (targetData.count <= 0)
        {
            allItemDatas.Remove(targetData);
        }

    }

    public void RemoveAllItemByType(ItemInventory.Category category)
    {
        var allItemCopyList = new List<ItemDataSave>();
        allItemCopyList.AddRange(allItemDatas);

        foreach(var item in allItemCopyList)
        {
            var itemClass = Hypatios.Assets.GetItem(item.ID);

            if (itemClass.category == category)
            {
                allItemDatas.Remove(item);
            }
        }
    }

    public void RemoveItem(string ID, int count = 1)
    {
        ItemDataSave targetData = SearchByID(ID);

        if (targetData != null)
            RemoveItem(targetData, count);
        else
        {
            Debug.LogError($"Item doesn't exist: {ID}");
        }

    }

    public int Count(ItemInventory itemInventory)
    {
        int count = 0;

        count = Count(itemInventory.GetID());


        return count;
    }

    public int Count(string ID)
    {
        int count = 0;

        var itemData = SearchByID(ID);

        if (itemData != null)
        {
            count += itemData.count;
        }

        return count;
    }


    /// <summary>
    /// Transfer item from this inventory to targeted inventory.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="currentIndex">Selected index of current inventory.</param>
    public ItemDataSave TransferTo(Inventory target, int currentIndex)
    {
        var itemDat = allItemDatas[currentIndex];
        var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

        if (itemClass.category == ItemInventory.Category.Weapon)
        {
            target.allItemDatas.Add(itemDat);
        }
        else
        {
            target.AddItem(itemClass, itemDat.count);
        }
        allItemDatas.Remove(itemDat);
        OnItemAdded?.Raise(itemClass);
        return itemDat;
    }

}


[System.Serializable]
public class HypatiosSave
{


    #region Persistent
    public string Game_Version = "1.0.0.0";
    public bool Game_DemoMode = false;
    public int Game_LastLevelPlayed = 0;
    public int Game_TotalRuns = 1;
    public int Game_TotalSouls = 0;
    public int Game_UnixTime = 0;
    public PlayerStatSave persistent_PlayerStat;
    public List<ShareCompanySave> PortfolioShares = new List<ShareCompanySave>();
    [Space]

    [Header("Perks")]
    public PerkDataSave AllPerkDatas = new PerkDataSave();


    [Header("Flags/Paradoxes")]
    public bool everUsed_Paradox = false;
    public bool everUsed_WeaponShop = false;
    public List<string> otherEverUsed = new List<string>();
    public List<string> favoritedItems = new List<string>();
    public List<ParadoxEntity> Game_ParadoxEntities = new List<ParadoxEntity>();
    public List<ChamberDataSave> Game_ChamberSaves = new List<ChamberDataSave>();
    public List<TriviaSave> Game_Trivias = new List<TriviaSave>();

    [Space]
    #endregion

    #region Single run only
    public float Player_CurrentHP = 100;
    public float Player_AlchoholMeter = 0f;
    public int Player_RunSessionUnixTime = 0;
    public EntryCache sceneEntryCache;
    public PlayerStatSave run_PlayerStat;
    public List<GlobalFlagSave> Game_GlobalFlags = new List<GlobalFlagSave>();
    public List<WeaponDataSave> Game_WeaponStats = new List<WeaponDataSave>();
    public List<ItemDataSave> Player_Inventory = new List<ItemDataSave>();

    //If player died, subtract run by 1. If reached zero, delete the flag.
    [System.Serializable]
    public class GlobalFlagSave
    {
        public string ID = "";
        public int runRemaining = 1;

        public GlobalFlagSave(string iD, int runRemaining)
        {
            ID = iD;
            this.runRemaining = runRemaining;
        }
    }

    //Share company
    [System.Serializable]
    public class ShareCompanySave
    {
        public string ID = "WING";
        public List<ChunkShare> chunkShares = new List<ChunkShare>();

        [System.Serializable]
        public class ChunkShare
        {
            public int sharePrice = 101; //soul
            public int amount = 2;
            //total investment: 202
        }

        public int GetTotalShares()
        {
            int total = 0;
            foreach(var chunk in chunkShares)
            {
                total += chunk.amount;
            }

            return total;
        }


        public int GetTotalInvestment()
        {
            int total = 0;
            foreach (var chunk in chunkShares)
            {
                total += chunk.amount * chunk.sharePrice;
            }

            return total;
        }

        public void BuyShare(int sharePrice, int amount)
        {
            ChunkShare newChunk = new ChunkShare();
            newChunk.sharePrice = sharePrice;
            newChunk.amount = amount;
            chunkShares.Add(newChunk);
        }

        public void RemoveShare(int amount = 1)
        {
            ChunkShare currentChunk = chunkShares[0];

            for (int x = 0; x < amount; x++)
            {
                currentChunk.amount--;

                if (currentChunk.amount <= 0)
                {
                    chunkShares.Remove(currentChunk);
                }

                if (chunkShares.Count > 0)
                {
                    currentChunk = chunkShares[0];
                }
                else
                {
                    return;
                }
            }
        }
    }

    public enum EssenceType
    {
        Modifier,
        Ailment
    }

    [System.Serializable]
    public class ItemDataSave
    {


        public string ID = "";
        public int count = 0;
        public ItemInventory.Category category;
        public WeaponDataSave weaponData; //only for weapon

        //Generic items
        public bool isGenericItem = false;
        [ShowIf("GENERIC_KTHANID_SERUM", true)] public string SERUM_CUSTOM_NAME = "Some Serum";
        [ShowIf("GENERIC_KTHANID_SERUM", true)] public float SERUM_TIME = 4f;
        [ShowIf("GENERIC_KTHANID_SERUM", true)] public float SERUM_ALCOHOL = 0f;
        [ShowIf("GENERIC_KTHANID_SERUM", true)] public List<PerkCustomEffect> SERUM_CUSTOM_EFFECTS = new List<PerkCustomEffect>();
        [ShowIf("GENERIC_KTHANID_SERUM", true)] public List<string> SERUM_AILMENTS = new List<string>();

        [ShowIf("GENERIC_ESSENCE_POTION", true)] public ModifierEffectCategory ESSENCE_CATEGORY = ModifierEffectCategory.Nothing;
        [ShowIf("GENERIC_ESSENCE_POTION", true)] public string ESSENCE_STATUSEFFECT_GROUP = ""; //Depression, Bleeding
        [ShowIf("GENERIC_ESSENCE_POTION", true)] public int ESSENCE_MULTIPLIER = 1;
        [ShowIf("GENERIC_ESSENCE_POTION", true)] public EssenceType ESSENCE_TYPE;
        [ShowIf("isGenericItem", true)] public bool GENERIC_KTHANID_SERUM = false;
        [ShowIf("isGenericItem", true)] public bool GENERIC_ESSENCE_POTION = false;


        public bool IsFavorite
        {
            get
            {
                if (category == ItemInventory.Category.Weapon)
                {
                    return isFavorited;
                }
                else if (isGenericItem)
                {
                    return isFavorited;
                }
                else
                {
                    return Hypatios.Game.IsItemFavorited(ID);
                }
            }

            set
            {
                if (category == ItemInventory.Category.Weapon)
                {
                    isFavorited = value;
                }
                else if (isGenericItem)
                {
                    isFavorited = value;
                }
                else
                {
                    if (value == true)
                        Hypatios.Game.AddFavorite(ID);
                    else
                        Hypatios.Game.RemoveFavorite(ID);
                }
            }
        }

        [JsonProperty] private bool isFavorited = false;

        public string Essence_PlusPlusMultiplierString()
        {
            string plus = "";

            for (int z = 0; z < ESSENCE_MULTIPLIER; z++)
            {
                if (z == 0) continue;
                plus += "+";
            }

            return plus;
        }

        public bool IsItemFavorited()
        { 
            if (category == ItemInventory.Category.Weapon)
            {
                return isFavorited;
            }
            else if (isGenericItem)
            {
                return isFavorited;
            }
            else
            {
                return Hypatios.Game.IsItemFavorited(ID);
            }

            return false;
        }

        internal void GenerateWeaponData()
        {
            AssetStorageDatabase assets = null;

            if (Hypatios.Instance == null)
                assets = UnityEngine.GameObject.FindObjectOfType<AssetStorageDatabase>();
            else
                assets = Hypatios.Assets;

            var itemClass = assets.GetItem(ID);
            var weaponClass = itemClass.attachedWeapon;

            weaponData = new HypatiosSave.WeaponDataSave();
            weaponData.weaponID = weaponClass.nameWeapon;

            float randomTime = UnityEngine.Random.Range(0f, 1f);
            int ammoAmount = Mathf.RoundToInt(weaponClass.rewardRate.Evaluate(randomTime)) + 5;
            weaponData.totalAmmo = ammoAmount;
        }

        public void RewardAmmo(int count)
        {
            AssetStorageDatabase assets = null;

            if (Hypatios.Instance == null)
                assets = UnityEngine.GameObject.FindObjectOfType<AssetStorageDatabase>();
            else
                assets = Hypatios.Assets;

            var itemClass = assets.GetItem(ID);
            var weaponClass = itemClass.attachedWeapon;

            for (int x = 0; x < count; x++)
            {
                float randomTime = UnityEngine.Random.Range(0f, 1f);
                int ammoAmount = Mathf.RoundToInt(weaponClass.rewardRate.Evaluate(randomTime)) + 5;
                weaponData.totalAmmo += ammoAmount;
            }
        }
    }

    [System.Serializable]
    public class EntryCache
    {
        public Vector3 cachedPlayerPos;
        public int entryIndex = 0; 
        //0 does nothing and just use scene's player pos
        //-1 override entry to cachedPlayerPos
    }

    [System.Serializable]
    public class PerkDataSave
    {
        public int Perk_LV_MaxHitpointUpgrade = 0;
        public int Perk_LV_RegenHitpointUpgrade = 0;
        public int Perk_LV_Soulbonus = 0;
        public int Perk_LV_ShortcutDiscount = 0;
        public int Perk_LV_KnockbackRecoil = 0;
        public int Perk_LV_WeaponRecoil = 0;
        public int Perk_LV_DashCooldown = 0;
        public int Perk_LV_IncreaseMeleeDamage = 0;
        public int Perk_LV_IncreaseGunDamage = 0;
        public List<PerkCustomEffect> Temp_CustomPerk = new List<PerkCustomEffect>();
        public List<StatusEffectData> Temp_StatusEffect = new List<StatusEffectData>(); //Status Effect will never be saved.

        public static PerkDataSave GetPerkDataSave()
        {
            if (Hypatios.Player == null)
            {
                return FPSMainScript.savedata.AllPerkDatas;
            }
            else
            {
                return Hypatios.Player.PerkData;
            }
        }

        public void CheatTempPerk(ModifierEffectCategory category, float value)
        {
            PerkCustomEffect perkOfType = Temp_CustomPerk.Find(x => x.origin == "CheatPerk" && x.statusCategoryType == category);

            if (perkOfType == null)
            {
                perkOfType = new PerkCustomEffect();
                perkOfType.statusCategoryType = category;
                perkOfType.origin = "CheatPerk";
                perkOfType.Value = value;
                Temp_CustomPerk.Add(perkOfType);
            }
            else
            {
                perkOfType.Value = value;
            }
        }

        public void AddPerkLevel(ModifierEffectCategory category)
        {
            if (category == ModifierEffectCategory.MaxHitpointBonus)
                Perk_LV_MaxHitpointUpgrade++;

            if (category == ModifierEffectCategory.RegenHPBonus)
                Perk_LV_RegenHitpointUpgrade++;

            if (category == ModifierEffectCategory.SoulBonus)
                Perk_LV_Soulbonus++;

            if (category == ModifierEffectCategory.ShortcutDiscount)
                Perk_LV_ShortcutDiscount++;

            if (category == ModifierEffectCategory.KnockbackResistance)
                Perk_LV_KnockbackRecoil++;

            if (category == ModifierEffectCategory.Recoil)
                Perk_LV_WeaponRecoil++;

            if (category == ModifierEffectCategory.DashCooldown)
                Perk_LV_DashCooldown++;

            if (category == ModifierEffectCategory.BonusDamageMelee)
                Perk_LV_IncreaseMeleeDamage++;

            if (category == ModifierEffectCategory.BonusDamageGun)
                Perk_LV_IncreaseGunDamage++;
        }
    }

    [System.Serializable]
    public class TriviaSave
    {
        public string ID = "Chamber1.Completed";
        public bool isCompleted = false;

    }

    [System.Serializable]
    public class WeaponDataSave
    {
        public string weaponID;
        [LabelText("Ammo")] [HideLabel()] [HorizontalGroup("1")] public int totalAmmo;
        [HideLabel()] [HorizontalGroup("1")] public int currentAmmo;
        public bool removed = false; //also for not equipping
        public List<string> allAttachments = new List<string>();

        [ReadOnly] [ShowInInspector] [HideInEditorMode] private WeaponItem attachedWeapon;
   

        public void AddAttachment(string attachment)
        {
            allAttachments.Add(attachment);
        }

        public bool AttachmentExists(string attachment)
        {
            return allAttachments.Find(x => x == attachment) != null ? true : false;
        }
    }
    
    [System.Serializable]
    public class PlayerStatSave
    {
        public List<PlayerStatValueSave> allPlayerStats = new List<PlayerStatValueSave>();

        public PlayerStatValueSave GetValueStat(BaseStatValue baseStat)
        {
            var targetStat = allPlayerStats.Find(x => x.ID == baseStat.ID);

            if (targetStat == null)
            {
                targetStat = new PlayerStatValueSave(baseStat.ID);
                allPlayerStats.Add(targetStat);
            }

            return targetStat;
        }
    }


    //All of the chamber's save data are handled by the ChamberLevelController script
    [System.Serializable]
    public class ChamberDataSave
    {
        public string ID = "Chamber1";
        public int timesVisited = 0;
        public int timesCompleted = 0;
        public int lastRunVisit = 0;

        public ChamberDataSave(string iD)
        {
            ID = iD;
        }
    }

    [System.Serializable]
    public class PlayerStatValueSave
    {
        public string ID = "total_Enemy_Kill_Melee";
        public int value_int = 0;

        public PlayerStatValueSave(string _id)
        {
            ID = _id;
        }
    }


    #endregion
}
