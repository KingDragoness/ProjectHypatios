using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System;

public class CraftingWorkstationUI : MonoBehaviour
{

    public Text stat_Description_Label;
    public Text stat_Title_Label;
    public Text button_Ammo_Label;
    public Text weaponMod_CurrentWeapon_Label;
    public Text tooltip_Text;
    public GameObject parent_WeaponMod;
    public GameObject parent_ScrollView_WeaponMod;
    public GameObject parent_Weapon;
    public GameObject parent_ScrollView_Weapon;
    public CraftWeaponModButton WeaponModButton_prefab;
    public CraftNewWeaponButton NewWeaponButton_prefab;
    public List<CraftingWeaponSelectButton> allMainButtons = new List<CraftingWeaponSelectButton>();
    [FoldoutGroup("Statistics")] public BaseStatValue stat_weaponCrafted;
    [FoldoutGroup("Statistics")] public BaseStatValue stat_weaponModCrafted;
    [FoldoutGroup("Audios")] public AudioSource audio_DrillCraft;

    private CraftingWorkstationTrigger _currentBench;
    private CraftingWeaponSelectButton _currentButton;
    private List<CraftWeaponModButton> _allWeaponModButtons = new List<CraftWeaponModButton>();
    private List<CraftNewWeaponButton> _allWeaponCraftButtons = new List<CraftNewWeaponButton>();
    private bool StartExecuted = false;

    public CraftingWorkstationTrigger CurrentWorkbench { get => _currentBench; set => _currentBench = value; }

    public void SetShopScript(CraftingWorkstationTrigger _shop)
    {
        _currentBench = _shop;
    }


    private void OnEnable()
    {
    
        if (_currentBench == null)
        {
            _currentBench = FindObjectOfType<CraftingWorkstationTrigger>();
            _currentBench.OpenShop();
        }

        WeaponModButton_prefab.gameObject.SetActive(false);
        NewWeaponButton_prefab.gameObject.SetActive(false);
        parent_Weapon.gameObject.SetActive(false);
        parent_WeaponMod.gameObject.SetActive(false);
        stat_Description_Label.text = "";
        stat_Title_Label.text = "";
        ShowTooltip(" ");
        _currentButton = null;

        RefreshUI();
    }

    public void RefreshUI()
    {

        int index = 0;
        foreach (var button in allMainButtons)
        {
            button.Refresh();

            index++;
        }
     
    }
    public void ShowTooltip(string s)
    {
        tooltip_Text.text = s;
    }

    public BaseWeaponScript GetCurrentWeaponOnTable()
    {
        if (_currentButton != null)
            return Hypatios.Player.Weapon.CurrentlyHeldWeapons[_currentButton.index];
        else return null;
    }

    #region Highlights and Actions

    public void HighlightWeaponMod(CraftWeaponModButton _button)
    {

        var weaponMod = _button.GetWeaponMod();
        stat_Title_Label.text = $"{weaponMod.Name}";
        stat_Description_Label.text = $"{weaponMod.Description}";

    }

    public void HighlightAmmo()
    {
        var weapon = Hypatios.Player.Weapon.CurrentlyHeldWeapons[_currentButton.index];
        var weaponClass = Hypatios.Assets.GetWeapon(weapon.weaponName);
        var itemClass = Hypatios.Assets.GetItemByWeapon(weapon.weaponName);

        stat_Title_Label.text = $"{itemClass.GetDisplayText()}";
        stat_Description_Label.text = $"{weaponClass.GetRequirementAmmosText()}\n[Ammo: {weapon.curAmmo}/{weapon.totalAmmo}]";
    }

    public void HighlightWeapon(CraftNewWeaponButton _button)
    {

        var weaponClass = _button.weaponItem;
        var itemClass = Hypatios.Assets.GetItemByWeapon(weaponClass.nameWeapon);
        stat_Title_Label.text = $"{itemClass.GetDisplayText()}";
        stat_Description_Label.text = $"{itemClass.Description}";

        CurrentWorkbench.displayWeapon_Weapon.currentWeaponDisplay = weaponClass;
        CurrentWorkbench.RefreshWeaponModels();
        CurrentWorkbench.currentMode = CraftingWorkstationTrigger.CameraMode.Weapon;
    }

    public void CraftWeaponMod(CraftWeaponModButton _button)
    {
        var weapon = Hypatios.Player.Weapon.CurrentlyHeldWeapons[_currentButton.index];
        var weaponClass = Hypatios.Assets.GetWeapon(weapon.weaponName);
        var weaponData = Hypatios.Game.GetWeaponSave(weapon.weaponName);

        if (weaponClass.IsCraftAttachmentRequirementMet(_button.weaponModID) == false)
        {
            ShowTooltip("Insufficient materials!");
            Debug.Log("Insufficient materials!");
            MainGameHUDScript.Instance.audio_Error.Play();
            return;
        }

        weaponData.AddAttachment(_button.weaponModID);

        var attachment = weaponClass.GetAttachmentWeaponMod(_button.weaponModID);

        foreach (var recipe in attachment.RequirementCrafting)
        {
            Hypatios.Player.Inventory.RemoveItem(recipe.inventory.GetID(), recipe.count);

        }

        Hypatios.Game.Increment_PlayerStat(stat_weaponModCrafted);
        OpenWeaponMod(_currentButton);
        audio_DrillCraft.Play();

        var WeaponManager = Hypatios.Player.Weapon;
        WeaponManager.SetWeaponSettings(WeaponManager.currentGunHeld);
        MainGameHUDScript.Instance.audio_PurchaseReward.Play();
        Selectable newSelectable = _button.button.FindSelectableOnDown();
        newSelectable.Select();
    }

    public void CraftNewWeapon(CraftNewWeaponButton _button)
    {
        var weaponClass = _button.weaponItem;
        var gunScript = Hypatios.Player.Weapon.GetGunScript(weaponClass.nameWeapon);

        if (gunScript != null)
        {
            ShowTooltip("Weapon already exists! Unequip the weapon first to be able to buy!");
            MainGameHUDScript.Instance.audio_Error.Play();
            return;
        }

        if (Hypatios.Player.Weapon.CurrentlyHeldWeapons.Count > 3)
        {
            ShowTooltip("Too many weapons in the hotbar! Unequip the weapon first to be able to buy!");
            MainGameHUDScript.Instance.audio_Error.Play();
            return;
        }

        if (weaponClass.IsRequirementMet() == false)
        {
            ShowTooltip("Insufficient materials!");
            Debug.Log("Insufficient materials!");
            MainGameHUDScript.Instance.audio_Error.Play();
            return;
        }

        var gunScript1 = Hypatios.Player.Weapon.AddWeapon(weaponClass.nameWeapon);
        if (gunScript1 == null)
        {
            Debug.LogError("Something bad happened!");
            ConsoleCommand.Instance.SendConsoleMessage("Something bad happened!");
            return;
        }

        foreach (var recipe in weaponClass.WeaponRequirementCrafting)
        {
            Hypatios.Player.Inventory.RemoveItem(recipe.inventory.GetID(), recipe.count);
        }

        Hypatios.Game.Increment_PlayerStat(stat_weaponCrafted);
        MainGameHUDScript.Instance.audio_PurchaseReward.Play();
        audio_DrillCraft.Play();
        RefreshUI();

        OpenWeapon();
        Selectable newSelectable = _button.button.FindSelectableOnDown();
        newSelectable.Select();
    }

    public void CraftAmmos()
    {
        var weapon = Hypatios.Player.Weapon.CurrentlyHeldWeapons[_currentButton.index];
        var weaponClass = Hypatios.Assets.GetWeapon(weapon.weaponName);
        var weaponData = Hypatios.Game.GetWeaponSave(weapon.weaponName);
        var itemClass = Hypatios.Assets.GetItemByWeapon(weapon.weaponName);

        if (weaponClass.IsAmmoRequirementMet() == false)
        {
            ShowTooltip("Insufficient materials!");
            Debug.Log("Insufficient materials!");
            MainGameHUDScript.Instance.audio_Error.Play();
            return;
        }

        foreach (var recipe in weaponClass.AmmoRequirementCrafting)
        {
            Hypatios.Player.Inventory.RemoveItem(recipe.inventory.GetID(), recipe.count);
        }

        MainGameHUDScript.Instance.audio_PurchaseReward.Play();
        RefreshUI();

        weapon.totalAmmo += weaponClass.craft_AmmoAmount;
        OpenWeaponMod(_currentButton);

        stat_Title_Label.text = $"{itemClass.GetDisplayText()}";
        stat_Description_Label.text = $"{weaponClass.GetRequirementAmmosText()}\n[Ammo: {weapon.curAmmo}/{weapon.totalAmmo}]";
    }

    #endregion

    public void OpenWeaponMod(CraftingWeaponSelectButton button)
    {
        _currentButton = button;
        var weapon = Hypatios.Player.Weapon.CurrentlyHeldWeapons[_currentButton.index];
        var weaponClass = Hypatios.Assets.GetWeapon(weapon.weaponName);
        var itemClass = Hypatios.Assets.GetItemByWeapon(weapon.weaponName);

        parent_Weapon.gameObject.SetActive(false);
        parent_WeaponMod.gameObject.SetActive(true);
        weaponMod_CurrentWeapon_Label.text = itemClass.GetDisplayText().ToUpper();

        foreach (var button1 in _allWeaponModButtons)
        {
            if (button1 == null) continue;
            Destroy(button1.gameObject);
        }

        _allWeaponModButtons.Clear();

        foreach (var attachment in weaponClass.attachments)
        {
            var newButton = Instantiate(WeaponModButton_prefab, parent_ScrollView_WeaponMod.transform);
            newButton.gameObject.SetActive(true);
            newButton.weaponItemName = weaponClass.nameWeapon;
            newButton.weaponModID = attachment.ID;
            newButton.Refresh();
            _allWeaponModButtons.Add(newButton);
        }

        var weaponSave = Hypatios.Game.GetWeaponSave(weapon.weaponName);
        string s_allAttachments = "";
        int i = 0;

        foreach (var attachment in weaponSave.allAttachments)
        {
            s_allAttachments += $"{weaponClass.GetAttachmentName(attachment)}";

            if (i < weaponSave.allAttachments.Count - 1)
            {
                s_allAttachments += ", ";
            }

            i++;
        }
        if (weaponSave.allAttachments.Count == 0)
            s_allAttachments = "None";

        CurrentWorkbench.displayWeapon_WeaponMod.currentWeaponDisplay = weaponClass;
        CurrentWorkbench.RefreshWeaponModels();
        CurrentWorkbench.currentMode = CraftingWorkstationTrigger.CameraMode.WeaponMod;

        stat_Title_Label.text = $"{itemClass.GetDisplayText()}";
        stat_Description_Label.text = $"Current attachments: {s_allAttachments}";
        button_Ammo_Label.text = $"+{weaponClass.craft_AmmoAmount}";
    }


    public void OpenWeapon()
    {
        parent_Weapon.gameObject.SetActive(true);
        parent_WeaponMod.gameObject.SetActive(false);

        var weaponListCraftable = Hypatios.Assets.Weapons.FindAll(x => x.isCraftable);

        foreach (var button1 in _allWeaponCraftButtons)
        {
            if (button1 == null) continue;
            Destroy(button1.gameObject);
        }

        _allWeaponCraftButtons.Clear();

        foreach (var weapon in weaponListCraftable)
        {
            var newButton = Instantiate(NewWeaponButton_prefab, parent_ScrollView_Weapon.transform);
            newButton.gameObject.SetActive(true);
            newButton.weaponItem = weapon;
            newButton.Refresh();
            _allWeaponCraftButtons.Add(newButton);
        }

        CurrentWorkbench.currentMode = CraftingWorkstationTrigger.CameraMode.Weapon;

    }

}
