using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using ItemDataSave = HypatiosSave.ItemDataSave;
using System;

[System.Serializable]
public class InventoryData
{
    public List<ItemDataSave> allItemDatas = new List<ItemDataSave>();

    //search
    public ItemDataSave SearchByID(string ID)
    {
        return allItemDatas.Find(x => x.ID == ID);
    }

    [Button("Add Item")] //Create new item from scratch
    public ItemDataSave AddItem(ItemInventory itemInventory, int count = 1)
    {
        ItemDataSave itemDataSave = null;


        if (SearchByID(itemInventory.GetID()) != null && itemInventory.category != ItemInventory.Category.Weapon)
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

    public void RemoveItem(string ID, int count = 1)
    {
        ItemDataSave targetData = SearchByID(ID);
        RemoveItem(targetData, count);
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
    public void TransferTo(InventoryData target, int currentIndex)
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
    }
}


[System.Serializable]
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
    public List<TriviaSave> Game_Trivias = new List<TriviaSave>();

    [Space]
    #endregion

    #region Single run only
    public float Player_CurrentHP = 100;
    public float Player_AlchoholMeter = 0f;
    public int Player_RunSessionUnixTime = 0;
    public EntryCache sceneEntryCache;
    public List<TimelineEventSave> Game_TimelineEvents = new List<TimelineEventSave>(); //Do not clear until wake up/level 1 script
    public List<WeaponDataSave> Game_WeaponStats = new List<WeaponDataSave>();
    public InventoryData Player_Inventory;

    //Player died
    [System.Serializable]
    public class TimelineEventSave
    {
        public string ID = "";
    }
  
    [System.Serializable]
    public class ItemDataSave
    {
        public string ID = "";
        public int count = 0;
        public ItemInventory.Category category;
        public WeaponDataSave weaponData; //only for weapon

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
        public int Perk_LV_DashCooldown = 0;
        public int Perk_LV_IncreaseMeleeDamage = 0;
        public int Perk_LV_IncreaseGunDamage = 0;
        public List<PerkCustomEffect> Temp_CustomPerk = new List<PerkCustomEffect>();

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

        public void AddPerkLevel(StatusEffectCategory category)
        {
            if (category == StatusEffectCategory.MaxHitpointBonus)
                Perk_LV_MaxHitpointUpgrade++;

            if (category == StatusEffectCategory.RegenHPBonus)
                Perk_LV_RegenHitpointUpgrade++;

            if (category == StatusEffectCategory.SoulBonus)
                Perk_LV_Soulbonus++;

            if (category == StatusEffectCategory.ShortcutDiscount)
                Perk_LV_ShortcutDiscount++;

            if (category == StatusEffectCategory.KnockbackResistance)
                Perk_LV_KnockbackRecoil++;

            if (category == StatusEffectCategory.DashCooldown)
                Perk_LV_DashCooldown++;

            if (category == StatusEffectCategory.BonusDamageMelee)
                Perk_LV_IncreaseMeleeDamage++;

            if (category == StatusEffectCategory.BonusDamageGun)
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
    #endregion
}
