using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Sirenix.OdinInspector;

public class PlayerRPGUI : MonoBehaviour
{

    [FoldoutGroup("Perk Tooltip")] public RectTransform perktooltipUI;
    [FoldoutGroup("Perk Tooltip")] public Text perkTooltip_LeftHandedLabel;
    [FoldoutGroup("Perk Tooltip")] public Text perkTooltip_RightHandedLabel;
    [FoldoutGroup("Perk Tooltip")] public Vector3 offsetTooltip;
    [FoldoutGroup("Perk Tooltip")] public ToolTip perkTooltipScript;

    [FoldoutGroup("Inventory")] public RectTransform parentInventory;
    [FoldoutGroup("Inventory")] public Text mainTitleLabel;
    [FoldoutGroup("Inventory")] public Text itemTooltip_LeftHandedLabel;
    [FoldoutGroup("Inventory")] public Text itemTooltip_RightHandedLabel;
    [FoldoutGroup("Inventory")] public ToolTip itemTooltipScript;
    [FoldoutGroup("Inventory")] public ItemInventory.Category filterCategoryType = ItemInventory.Category.None;
    [FoldoutGroup("Inventory")] public bool isFavoriteActive = false;


    public RPG_CharPerkButton PerkButton;
    public InventoryItemButton InventoryItemButton;
    public RPG_CharPerkButton currentPerkButton;
    public Transform parentPerks;
    public Text hp_Label;
    public Slider hp_Slider;
    public Text time_label;

    private List<RPG_CharPerkButton> _allCharPerkButtons = new List<RPG_CharPerkButton>();
    private List<InventoryItemButton> _allInventoryButtons = new List<InventoryItemButton>();
    private Canvas pauseCanvas;

    private InventoryItemButton currentItemButton;

    private void OnEnable()
    {
        RefreshUI();
        pauseCanvas = GetComponentInParent<Canvas>();
    }

    public void FavoriteMode()
    {
        isFavoriteActive = !isFavoriteActive;
        if (isFavoriteActive)
            filterCategoryType = ItemInventory.Category.None;

        RefreshUI();
    }

    public void CategoryFilter(int _filterCategory)
    {
        var cat = (ItemInventory.Category)_filterCategory;

        if (filterCategoryType != cat)
            filterCategoryType = cat;
        else
            filterCategoryType = ItemInventory.Category.None;

        if (isFavoriteActive)
        {
            isFavoriteActive = false;
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        foreach (var perkButton in _allCharPerkButtons)
        {
            Destroy(perkButton.gameObject);
        }
        foreach (var button in _allInventoryButtons)
        {
            Destroy(button.gameObject);
        }
        _allCharPerkButtons.Clear();
        _allInventoryButtons.Clear();

        string HP = $"{Mathf.RoundToInt(Hypatios.Player.Health.curHealth)}/{Mathf.RoundToInt(Hypatios.Player.Health.maxHealth.Value)}";
        hp_Label.text = HP;
        hp_Slider.value = Hypatios.Player.Health.curHealth;
        hp_Slider.maxValue = Hypatios.Player.Health.maxHealth.Value;

        var dateTime = ClockTimerDisplay.UnixTimeStampToDateTime(Hypatios.Game.UNIX_Timespan + Hypatios.UnixTimeStart);
        time_label.text = $"{dateTime.Hour}:{dateTime.Minute.ToString("00")}:{dateTime.Second.ToString("00")}";

        //Refresh perks
        {
            var allCurrentPerk = Hypatios.Player.AllStatusInEffect.FindAll(x => x.SourceID != "PermanentPerk"); //"TempPerk"

            foreach(var perk in allCurrentPerk)
            {
                var statusEffect1 = Hypatios.Assets.GetStatusEffect(perk.statusCategoryType);
                if (statusEffect1 == null) continue;
                var newButton = Instantiate(PerkButton, parentPerks);
                newButton.gameObject.SetActive(true);
                newButton.statusEffect = statusEffect1;
                newButton.attachedStatusEffectGO = perk;
                newButton.Refresh();
                _allCharPerkButtons.Add(newButton);
            }
        }

        string s = "";

        if (isFavoriteActive)
            s += $"FAVORITES/";
        else
            s += $"INVENTORY/";

        if (filterCategoryType == ItemInventory.Category.None)
        {
            s += $"ALL";
        }
        else
        {
            s += $"{filterCategoryType.ToString().ToUpper()}";
        }

        mainTitleLabel.text = s;

        //Refresh inventories
        {
            List<int> indexes = new List<int>();
            var All_Items = Hypatios.Player.Inventory.allItemDatas;


            for(int x = 0; x < All_Items.Count; x++)
            {
                var itemData = All_Items[x];

                if (filterCategoryType == ItemInventory.Category.None && isFavoriteActive == false)
                {
                    indexes.Add(x);
                }
                else
                {
                    if (filterCategoryType == ItemInventory.Category.None && isFavoriteActive && itemData.isFavorited)
                    {
                        indexes.Add(x);
                    }
                    else if (itemData.category == filterCategoryType && isFavoriteActive && itemData.isFavorited)
                    {
                        indexes.Add(x);
                    }
                    else if (itemData.category == filterCategoryType && !isFavoriteActive)
                    {
                        indexes.Add(x);
                    }
                }
            }

            foreach (var index in indexes)
            {
                var newButton = Instantiate(InventoryItemButton, parentInventory);
                newButton.gameObject.SetActive(true);
                newButton.index = index;
                newButton.Refresh();
                _allInventoryButtons.Add(newButton);
            }

        }

        var charStatButtons = GetComponentsInChildren<CharStatButton>();

        foreach(var button in charStatButtons)
        {
            button.ForceRefresh();
        }
    }

    private void Update()
    {
        HandleDiscardItem();
        HandleFavoriteItem();

    }

    private void HandleFavoriteItem()
    {
        if (currentItemButton == null)
            return;

        if (Input.GetKeyUp(KeyCode.F) == false)
            return;

        var button = currentItemButton;
        var itemData = button.GetItemData();

        itemData.isFavorited = !itemData.isFavorited;

        RefreshUI();

        perkTooltipScript.gameObject.SetActive(false);
        itemTooltipScript.gameObject.SetActive(false);
    }

    private void HandleDiscardItem()
    {
        if (currentItemButton == null)
            return;

        if (Input.GetKeyUp(KeyCode.X) == false)
            return;

        var button = currentItemButton;
        var itemData = button.GetItemData();
        var itemCLass = button.GetItemInventory();

        if (itemCLass.category == ItemInventory.Category.Weapon)
        {
            Hypatios.Player.Weapon.TransferAmmo_PrepareDelete(itemData);
        }

        Hypatios.Player.Inventory.allItemDatas.Remove(itemData);

        RefreshUI();

        perkTooltipScript.gameObject.SetActive(false);
        itemTooltipScript.gameObject.SetActive(false);
        currentItemButton = null;
    }

    public void UseItem(InventoryItemButton button)
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
            soundManagerScript.instance.PlayOneShot("consume");
            Hypatios.Player.Health.Heal((int)itemCLass.consume_HealAmount);
            Hypatios.Player.Health.alcoholMeter += itemCLass.consume_AlcoholAmount;

            if (itemCLass.isInstantDashRefill)
            {
                Hypatios.Player.timeSinceLastDash = 10f;
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


        RefreshUI();


        perkTooltipScript.gameObject.SetActive(false);
        itemTooltipScript.gameObject.SetActive(false);
    }

    #region Highlight/dehighlight
    public void DeequipWeapon(WeaponSlotButton button)
    {
        if (button.equipIndex == 0) return; //don't ever deequip the first weapon!

        var gunScript = Hypatios.Player.Weapon.CurrentlyHeldWeapons[button.equipIndex];
        var saveData = Hypatios.Game.currentWeaponStat.Find(x => x.weaponID == gunScript.weaponName);
        var itemClass = Hypatios.Assets.GetItemByWeapon(saveData.weaponID);
        var itemData = Hypatios.Player.Inventory.AddItem(itemClass);

        itemData.weaponData = saveData;
        //itemData.weaponData.currentAmmo = gunScript.curAmmo;
        itemData.weaponData.totalAmmo = gunScript.totalAmmo + gunScript.curAmmo;

        Hypatios.Player.Weapon.RemoveWeapon(gunScript as GunScript); //transfer to weapon and create item from scratch

       {
            var weaponSlots = GetComponentsInChildren<WeaponSlotButton>();
            foreach(var weaponSlot in weaponSlots)
            {
                weaponSlot.RefreshUI();
            }
        }

        {
        }

        RefreshUI();
        perkTooltipScript.gameObject.SetActive(false);
        itemTooltipScript.gameObject.SetActive(false);

    }

    public void HighlightItem(InventoryItemButton button)
    {
        var itemDat = Hypatios.Player.Inventory.allItemDatas[button.index];
        var itemClass = Hypatios.Assets.GetItem(itemDat.ID);
        string sLeft = "";
        string sRight = "";
        string s_interaction = "";

        if (itemClass.category == ItemInventory.Category.Weapon)
        {
            var weaponClass = itemClass.attachedWeapon;
            var weaponSave = itemDat.weaponData;
            var weaponStat = weaponClass.GetFinalStat(weaponSave.allAttachments);
            int totalAmmoOfType = Hypatios.Player.Weapon.GetTotalAmmoOfWeapon(weaponClass.nameWeapon);
            string s_allAttachments = "";
            bool isSimilarWeaponEquipped = false;

            if (Hypatios.Player.Weapon.GetGunScript(itemClass.attachedWeapon.nameWeapon) != null) isSimilarWeaponEquipped = true;


            foreach (var attachment in weaponSave.allAttachments)
            {
                s_allAttachments += $"{weaponClass.GetAttachmentName(attachment)}, ";
            }

            if (!isSimilarWeaponEquipped)
            {
                s_interaction = "[LMB to equip weapon] [X to discard> [F to favorite]";
            }
            else
            {
                s_interaction = "[X to discard] [F to favorite]";
            }


            sLeft += $"Damage\n";
            sLeft += $"Fire rate\n";
            sLeft += $"Mag size\n";
            sLeft += $"Ammo Left\n";
            sLeft += $"\n<size=14>{s_interaction}</size>\n";
            sLeft += $"\n<size=14>{s_allAttachments}</size>";

            sRight += $"{weaponStat.damage}\n";
            sRight += $"{weaponStat.cooldown} per sec\n";
            sRight += $"{weaponStat.magazineSize}\n";
            sRight += $"{totalAmmoOfType}\n";

        }
        else
        {
            if (itemClass.category != ItemInventory.Category.Consumables)
            {
                s_interaction = "[X to discard] [F to favorite]";
            }
            else
            {
                s_interaction = "[LMB to consume] [X to discard] [F to favorite]";
            }

            sLeft += $"{itemClass.GetDisplayText()}\n";
            sLeft += $"\n{itemClass.Description}\n";
            sLeft += $"\n<size=14>{s_interaction}</size>\n";
            sRight += $"({itemDat.count})\n";
        }

        itemTooltip_LeftHandedLabel.text = sLeft;
        itemTooltip_RightHandedLabel.text = sRight;
        currentItemButton = button;

        Hypatios.UI.ShowTooltipBig(button.GetComponent<RectTransform>());

    }

    public void DehighlightItem()
    {
        currentItemButton = null;
    }

    public void HighlightWeaponSlot(WeaponSlotButton slotButton)
    {

        if (Hypatios.Player.Weapon.CurrentlyHeldWeapons.Count <= slotButton.equipIndex)
        {
            return;
        }

        var Weapon = Hypatios.Player.Weapon.CurrentlyHeldWeapons[slotButton.equipIndex];
        var weaponSave = Hypatios.Game.GetWeaponSave(Weapon.weaponName);
        var WeaponClass = Hypatios.Assets.GetWeapon(Weapon.weaponName);
        string s_allAttachments = "";
        string sLeft = "";
        string sRight = "";

        foreach (var attachment in weaponSave.allAttachments)
        {
            s_allAttachments += $"{WeaponClass.GetAttachmentName(attachment)}, ";
        }

        sLeft += $"Magazine/Total Ammo";
        if (weaponSave.allAttachments.Count != 0) sLeft += $"\n<size=14>{s_allAttachments}</size>";
        sRight += $"{Weapon.curAmmo}/{Weapon.totalAmmo}";
        if (weaponSave.allAttachments.Count != 0) sRight += "\n";

        perkTooltip_LeftHandedLabel.text = sLeft;
        perkTooltip_RightHandedLabel.text = sRight;

        Hypatios.UI.ShowTooltipSmall(slotButton.GetComponent<RectTransform>());
    }

    public void DehighlightWeaponSlot()
    {

    }

    public void HighlightStat(CharStatButton charStatButton)
    {
        var value1 = Hypatios.Player.GetCharFinalValue(charStatButton.category1);
        var baseStat = Hypatios.Assets.GetStatusEffect(charStatButton.category1);

        if (charStatButton.category1 == StatusEffectCategory.BonusDamageMelee
            | charStatButton.category1 == StatusEffectCategory.BonusDamageGun)
        {
            value1 -= 1;
        }


        perkTooltip_LeftHandedLabel.text = $"{baseStat.TitlePerk}";
        perkTooltip_RightHandedLabel.text = RPG_CharPerkButton.GetDescription(charStatButton.category1, value1);

        Hypatios.UI.ShowTooltipSmall(charStatButton.GetComponent<RectTransform>());
    }

    public void DehighlightStat()
    {

    }


    public void HighlightPerk(RPG_CharPerkButton _currentPerk)
    {
        currentPerkButton = _currentPerk;
        string timerString = $"{(Mathf.RoundToInt(currentPerkButton.attachedStatusEffectGO.EffectTimer*10f)/10f)}";
        if (currentPerkButton.attachedStatusEffectGO.EffectTimer >= 9999f) timerString = $"";
        else timerString = $"({timerString}s)";
        var value = currentPerkButton.attachedStatusEffectGO.Value;
        perkTooltip_LeftHandedLabel.text = $"{currentPerkButton.statusEffect.TitlePerk} {timerString}";

        if (value == 0  | value == -1)
        {
            perkTooltip_RightHandedLabel.text = "";
        }
        else
        {
            perkTooltip_RightHandedLabel.text = RPG_CharPerkButton.GetDescription(_currentPerk.statusEffect.category, value);
        }

        Hypatios.UI.ShowTooltipSmall(_currentPerk.GetComponent<RectTransform>());

    }

    public void DehighlightPerk()
    {
        currentPerkButton = null;
    }

    #endregion
}
