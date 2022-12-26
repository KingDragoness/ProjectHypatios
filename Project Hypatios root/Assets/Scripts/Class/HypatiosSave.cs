﻿using System.Collections;
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
    public List<WeaponDataSave> Game_WeaponStats = new List<WeaponDataSave>();
    public InventoryData Player_Inventory;


    [System.Serializable]
    public class ItemDataSave
    {
        public string ID = "";
        public int count = 0;
        public ItemInventory.Category category;
        public WeaponDataSave weaponData; //only for weapon

        internal void GenerateWeaponData()
        {
            var itemClass = Hypatios.Assets.GetItem(ID);
            var weaponClass = itemClass.attachedWeapon;

            weaponData = new HypatiosSave.WeaponDataSave();
            weaponData.weaponID = weaponClass.nameWeapon;

            float randomTime = UnityEngine.Random.Range(0f, 1f);
            int ammoAmount = Mathf.RoundToInt(weaponClass.rewardRate.Evaluate(randomTime)) + 5;
            weaponData.totalAmmo = ammoAmount;
        }
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
        public List<PerkCustomEffect> Temp_CustomPerk = new List<PerkCustomEffect>();

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
   

        [Button("Add Attachment")]
        public void AddAttachment(string attachment)
        {

        }
    }
    #endregion
}
