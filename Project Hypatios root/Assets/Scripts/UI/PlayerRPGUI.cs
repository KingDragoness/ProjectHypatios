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
    [FoldoutGroup("Inventory")] public Text itemTooltip_LeftHandedLabel;
    [FoldoutGroup("Inventory")] public Text itemTooltip_RightHandedLabel;
    [FoldoutGroup("Inventory")] public ToolTip itemTooltipScript;


    public RPG_CharPerkButton PerkButton;
    public InventoryItemButton InventoryItemButton;
    public RPG_CharPerkButton currentPerkButton;
    public Transform parentPerks;
    public Text hp_Label;
    public Slider hp_Slider;

    private List<RPG_CharPerkButton> _allCharPerkButtons = new List<RPG_CharPerkButton>();
    private List<InventoryItemButton> _allInventoryButtons = new List<InventoryItemButton>();
    private Canvas pauseCanvas;

    private void OnEnable()
    {
        RefreshUI();
        pauseCanvas = GetComponentInParent<Canvas>();
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

        //Refresh inventories
        {
            var FilteredItem = new List<HypatiosSave.ItemDataSave>();
            foreach (var item in Hypatios.Player.Inventory.allItemDatas) FilteredItem.Add(item);

            int index1 = 0;
            foreach (var item in FilteredItem)
            {
                var newButton = Instantiate(InventoryItemButton, parentInventory);
                newButton.gameObject.SetActive(true);
                newButton.index = index1;
                newButton.Refresh();
                index1++;
                _allInventoryButtons.Add(newButton);
            }

        }
    }

    private void Update()
    {


    }

    #region Highlight/dehighlight
    public void DeequipWeapon(WeaponSlotButton button)
    {
        if (button.equipIndex == 0) return; //don't ever deequip the first weapon!


    }

    public void HighlightItem(InventoryItemButton button)
    {
        var itemDat = Hypatios.Player.Inventory.allItemDatas[button.index];
        var itemClass = Hypatios.Assets.GetItem(itemDat.ID);
        string sLeft = "";
        string sRight = "";

        if (itemClass.category == ItemInventory.Category.Weapon)
        {
            var weaponClass = itemClass.attachedWeapon;
            var weaponSave = itemDat.weaponData;

            sLeft += $"Damage\n";
            sLeft += $"Fire rate\n";
            sLeft += $"Mag size\n";
            sLeft += $"Ammo Left\n";
            sLeft += $"\n<1/2/3/4 to equip slot>\n";

            sRight += $"{weaponClass.levels_Damage[weaponSave.level_Damage]}\n";
            sRight += $"{weaponClass.levels_Cooldown[weaponSave.level_Cooldown]} per sec\n";
            sRight += $"{weaponClass.levels_MagazineSize[weaponSave.level_MagazineSize]}\n";
            sRight += $"{weaponSave.totalAmmo}\n";

        }
        else
        {
            sLeft += $"{itemClass.GetDisplayText()}\n";
            sLeft += $"\n{itemClass.Description}\n";
            sRight += $"({itemDat.count})\n";
        }

        itemTooltip_LeftHandedLabel.text = sLeft;
        itemTooltip_RightHandedLabel.text = sRight;

    }

    public void DehighlightItem()
    {

    }

    public void HighlightWeaponSlot(WeaponSlotButton slotButton)
    {

        if (Hypatios.Player.Weapon.CurrentlyHeldWeapons.Count <= slotButton.equipIndex)
        {
            return;
        }

        var Weapon = Hypatios.Player.Weapon.CurrentlyHeldWeapons[slotButton.equipIndex];
        var WeaponClass = Hypatios.Assets.GetWeapon(Weapon.weaponName);

        perkTooltip_LeftHandedLabel.text = $"Magazine/Total Ammo";
        perkTooltip_RightHandedLabel.text = $"{Weapon.curAmmo}/{Weapon.totalAmmo}";

    }

    public void DehighlightWeaponSlot()
    {

    }

    public void HighlightStat(CharStatButton charStatButton)
    {
        var value1 = Hypatios.Player.GetCharFinalValue(charStatButton.category1);
        var baseStat = Hypatios.Assets.GetStatusEffect(charStatButton.category1);

        if (charStatButton.category1 == StatusEffectCategory.BonusDamageMelee)
        {
            value1 -= 1;
        }

        perkTooltip_LeftHandedLabel.text = $"{baseStat.TitlePerk}";
        perkTooltip_RightHandedLabel.text = RPG_CharPerkButton.GetDescription(charStatButton.category1, value1);

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
    }

    public void DehighlightPerk()
    {
        currentPerkButton = null;
    }

    #endregion
}
