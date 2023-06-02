﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class FavoriteMenuUI : MonoBehaviour
{

    public enum Mode
    {
        SelectCategory,
        SelectItems
    }

    public Image cursorImage;
    public Mode currentMode;
    public List<FavoriteRadialCategoryButton> radialButtons = new List<FavoriteRadialCategoryButton>();
    public FavoriteRadialCategoryButton currentRadial;
    public GameObject favoriteListUI;
    public GameObject itemStatUI;
    public CanvasGroup cg;
    [FoldoutGroup("Item List")] public Text title;
    [FoldoutGroup("Item List")] public Transform parentItems;
    [FoldoutGroup("Item List")] public FavItemButton prefabItemButton;
    [FoldoutGroup("Item List")] public Text itemStat_Title;
    [FoldoutGroup("Item List")] public Text itemStat_Description;

    [ReadOnly] public float currentAngle = 0;

    private Vector3 mousePos;
    private Vector3 middlePos;
    private ItemInventory.SubiconCategory _currentSubCategory;
    private List<FavItemButton> allFavButtons = new List<FavItemButton>();
    private FavItemButton _currentFavItemButton;

    private void OnEnable()
    {
        ResetUI();
    }

    private void ResetUI()
    {
        ChangeMode(Mode.SelectCategory);
        favoriteListUI.gameObject.SetActive(false);
        itemStatUI.gameObject.SetActive(false);
        cursorImage.gameObject.SetActive(true);
        cg.alpha = 1f;

        itemStat_Title.text = "";
        itemStat_Description.text = "";
    }

    public void ChangeMode(Mode _mode)
    {
        if (currentMode != _mode)
        {
            if (_mode == Mode.SelectCategory)
            {
                cursorImage.gameObject.SetActive(true);
                favoriteListUI.gameObject.SetActive(false);
                cg.alpha = 1f;
            }
            else if (_mode == Mode.SelectItems)
            {
                cursorImage.gameObject.SetActive(false);
                favoriteListUI.gameObject.SetActive(true);
                cg.alpha = 0.4f;
            }
        }

        currentMode = _mode;
    }

    private void Update()
    {
        if (currentMode == Mode.SelectCategory)
        {
            mousePos = Input.mousePosition;
            middlePos = new Vector3(Screen.width / 2f, Screen.height / 2f);
            Vector3 up = Vector3.up;
            Vector3 dir = mousePos - middlePos;

            currentAngle = Vector3.SignedAngle(up, dir, Vector3.forward);

            UpdateRadialCursor();
        }
        else if (currentMode == Mode.SelectItems)
        {

        }

        if (Hypatios.Input.Fire2.triggered)
        {
            ChangeMode(Mode.SelectCategory);
        }
    }

    private void UpdateSelectItem()
    {

    }

    private void UpdateRadialCursor()
    {
        currentRadial = null;
        float pieAngle = 360f * cursorImage.fillAmount;
        float offset = pieAngle / 2f;
        Vector3 rot = new Vector3(0f, 0f, currentAngle + offset);
        cursorImage.transform.localEulerAngles = rot;

        foreach(var button in radialButtons)
        {
            button.UpdateButton(currentAngle);
            if (button.IsSelected(currentAngle) && currentRadial == null)
            {
                currentRadial = button;
            }
        }

        if (Hypatios.Input.Fire1.triggered && currentRadial != null)
        {
            OpenUIFavorite(currentRadial.subCategory);
        }
    }

    //duplicated code
    public void UseItem(FavItemButton button)
    {
        var itemCLass = button.GetItemInventory();
        var itemData = button.GetItemData();


        if (itemCLass.category == ItemInventory.Category.Weapon)
        {
            if (Hypatios.Player.Weapon.GetGunScript(itemCLass.attachedWeapon.nameWeapon) == null
                && Hypatios.Player.Weapon.CurrentlyHeldWeapons.Count <= 3)
            {
                Hypatios.Game.currentWeaponStat.Add(itemData.weaponData);
                Hypatios.Player.Weapon.TransferAllInventoryAmmoToOneItemData(ref itemData);
                Hypatios.Player.Weapon.RefreshWeaponLoadout(itemData.ID);
                Hypatios.Player.Inventory.allItemDatas.Remove(itemData);

                itemData.weaponData.currentAmmo = 0;
            }
            else
            {
                Debug.LogError("Cannot use same weapon/tto many weapons equipped");
                return;
            }
        }
        else if (itemCLass.category == ItemInventory.Category.Consumables)
        {
            float healSpeed = itemCLass.consume_HealAmount / itemCLass.consume_HealTime;

            soundManagerScript.instance.PlayOneShot("consume");
            Hypatios.Player.Health.Heal((int)itemCLass.consume_HealAmount, healSpeed);
            Hypatios.Player.Health.alcoholMeter += itemCLass.consume_AlcoholAmount;

            if (itemCLass.isInstantDashRefill)
            {
                Hypatios.Player.timeSinceLastDash = 10f;
            }
            if (itemCLass.statusEffect != null)
            {
                itemCLass.statusEffect.AddStatusEffectPlayer(itemCLass.statusEffectTime);
            }
            if (itemCLass.statusEffectToRemove.Count > 0)
            {
                foreach (var sg in itemCLass.statusEffectToRemove)
                {
                    if (sg == null) continue;
                    Hypatios.Player.RemoveStatusEffectGroup(sg);
                }
            }

            Hypatios.Player.Inventory.RemoveItem(itemData);
        }

        {
            var weaponSlots = GetComponentsInChildren<WeaponSlotButton>();
            foreach (var weaponSlot in weaponSlots)
            {
                weaponSlot.RefreshUI();
            }
        }

        RefreshUIContainer();
    }

    public void OpenUIFavorite(ItemInventory.SubiconCategory subCategory)
    {
        ChangeMode(Mode.SelectItems);
        _currentSubCategory = subCategory;

        if (subCategory != ItemInventory.SubiconCategory.Default)
        {
            title.text = subCategory.ToString().ToUpper();
        }
        else
        {
            title.text = "FAVORITED";
        }

        RefreshUIContainer();
    }

    public void RefreshUIContainer()
    {
        foreach (var button in allFavButtons)
        {
            Destroy(button.gameObject);
        }
        allFavButtons.Clear();

        //Refresh inventories
        {
            List<int> indexes = new List<int>();
            var All_Items = Hypatios.Player.Inventory.allItemDatas;


            for (int x = 0; x < All_Items.Count; x++)
            {
                var itemData = All_Items[x];
                var itemClass = Hypatios.Assets.GetItem(itemData.ID);
                if (itemClass == null) continue;

                if (itemClass.subCategory == _currentSubCategory && _currentSubCategory != ItemInventory.SubiconCategory.Default)
                {
                    indexes.Add(x);
                }
                else if (_currentSubCategory == ItemInventory.SubiconCategory.Default)
                {
                    if (itemData.IsFavorite)
                    {
                        indexes.Add(x);
                    }
                }
            }

            foreach (var index in indexes)
            {
                var itemDat = Hypatios.Player.Inventory.allItemDatas[index];
                var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

                AssetStorageDatabase.SubiconIdentifier subicon = Hypatios.Assets.GetSubcategoryItemIcon(itemClass.subCategory);
                var newButton = Instantiate(prefabItemButton, parentItems);
                newButton.gameObject.SetActive(true);
                newButton.index = index;
                newButton.Refresh();
                newButton.imageIcon.sprite = subicon.sprite;
                allFavButtons.Add(newButton);
            }

        }
    }

    public void HighlightButton(FavItemButton button)
    {
        _currentFavItemButton = button;
        var itemDat = Hypatios.Player.Inventory.allItemDatas[button.index];
        var itemClass = Hypatios.Assets.GetItem(itemDat.ID);
        if (itemClass == null) return;

        itemStatUI.gameObject.SetActive(true);
        itemStat_Title.text = itemClass.GetDisplayText();
        itemStat_Description.text = itemClass.Description;
    }

}