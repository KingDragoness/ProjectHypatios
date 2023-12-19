using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Sirenix.OdinInspector;

public class PlayerRPGUI : MonoBehaviour
{

    

    [FoldoutGroup("Perk Tooltip")] public RectTransform perktooltipUI;
    [FoldoutGroup("Perk Tooltip")] public Vector3 offsetTooltip;
    [FoldoutGroup("Perk Tooltip")] public ToolTip perkTooltipScript;

    [FoldoutGroup("Inventory")] public RectTransform parentInventory;
    [FoldoutGroup("Inventory")] public Text mainTitleLabel;
    [FoldoutGroup("Inventory")] public InputField input_SearchField;
    [FoldoutGroup("Inventory")] public ToolTip itemTooltipScript;
    [FoldoutGroup("Inventory")] public Slider healthRestoreBar;
    [FoldoutGroup("Inventory")] public Slider healthRestore_BorderBar;
    [FoldoutGroup("Inventory")] public ItemInventory.Category filterCategoryType = ItemInventory.Category.None;
    [FoldoutGroup("Inventory")] public bool isFavoriteActive = false;
    [FoldoutGroup("Inventory")] public Color spriteColor;
    [FoldoutGroup("Weapon Preview")] public Previewer3DWeaponUI previewerWeapon;
    [FoldoutGroup("Weapon Preview")] public RectTransform parentAttachmentButtons;
    [FoldoutGroup("Weapon Preview")] public GameObject weaponPreviewerUI;
    [FoldoutGroup("Weapon Preview")] public Text weaponPreview_Label;
    [FoldoutGroup("Weapon Preview")] public Text weaponPreview_AmmoCount;
    [FoldoutGroup("Weapon Preview")] public Text weaponPreview_WeaponStats;
    [FoldoutGroup("Weapon Preview")] public Image weaponPreview_AmmoIcon;

    public float TimeToConsumePress = 2f;
    public float TimeToDiscardPress = 1.5f;
    public RPG_CharPerkButton PerkButton;
    public RPG_CharPerkButton StatusMonoButton;
    public AttachmentWeaponButton WeaponModButton;
    public InventoryItemButton InventoryItemButton;
    public RPG_CharPerkButton currentPerkButton;
    public Transform parentPerks;
    public Text hp_Label;
    public Slider hp_Slider;
    public Text time_label;

    private List<RPG_CharPerkButton> _allCharPerkButtons = new List<RPG_CharPerkButton>();
    private List<RPG_CharPerkButton> _allStatusMonoButtons = new List<RPG_CharPerkButton>();
    private List<InventoryItemButton> _allInventoryButtons = new List<InventoryItemButton>();
    private List<AttachmentWeaponButton> _allWeaponModButtons = new List<AttachmentWeaponButton>();
    private Canvas pauseCanvas;
    private HypatiosSave.WeaponDataSave _currentHighlightedWeaponData;

    private InventoryItemButton currentItemButton;
    private float _timeSlider = 0f;
    private ItemInventory prevItemClass;

    private float _timePressConsume = 0f;
    private float _timePressDiscard = 0f;

    private void OnEnable()
    {
        RefreshUI();
        pauseCanvas = GetComponentInParent<Canvas>();
        input_SearchField.text = "";
        HideHealthRestore();
    }

    private void Start()
    {
        DepreviewWeapon();
    }

    private void OnDisable()
    {
        DepreviewWeapon();
        _timeSlider = 0f;
    }

    public static void RetardedWayRefreshingRpgUI()
    {
        PlayerRPGUI rpgUI = FindObjectOfType<PlayerRPGUI>();
        if (rpgUI == null) return;
        rpgUI.RefreshUI();

        {
            var weaponSlots = rpgUI.GetComponentsInChildren<WeaponSlotButton>();
            foreach (var weaponSlot in weaponSlots)
            {
                weaponSlot.RefreshUI();
            }
        }
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

    public void RefreshUI()
    {
        foreach (var perkButton in _allCharPerkButtons)
        {
            Destroy(perkButton.gameObject);
        }
        foreach (var statButton in _allStatusMonoButtons)
        {
            Destroy(statButton.gameObject);
        }
        foreach (var button in _allInventoryButtons)
        {
            Destroy(button.gameObject);
        }
        _allCharPerkButtons.Clear();
        _allInventoryButtons.Clear();
        _allStatusMonoButtons.Clear();

        string HP = $"{Mathf.RoundToInt(Hypatios.Player.Health.curHealth)}/{Mathf.RoundToInt(Hypatios.Player.Health.maxHealth.Value)}";
        hp_Label.text = HP;
        hp_Slider.value = Hypatios.Player.Health.curHealth;
        hp_Slider.maxValue = Hypatios.Player.Health.maxHealth.Value;

        System.DateTime dateTime = ClockTimerDisplay.UnixTimeStampToDateTime(Hypatios.Game.UNIX_Timespan + Hypatios.UnixTimeStart);

        if (ChamberLevelController.Instance.chamberObject.isWIRED)
        {
            dateTime = ClockTimerDisplay.UnixTimeStampToDateTime(Hypatios.Game.GetUnixTime_WIRED() + Hypatios.UnixTimeStart);

        }

        time_label.text = $"{dateTime.Hour}:{dateTime.Minute.ToString("00")}:{dateTime.Second.ToString("00")}";

        //Refresh perks
        {
            var allCurrentPerk = Hypatios.Player.AllStatusInEffect.FindAll(x => x.SourceID != "PermanentPerk"); //"TempPerk"
            allCurrentPerk.RemoveAll(x => x.IsTiedToStatusMono == true);

            foreach (var perk in allCurrentPerk)
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
        {
            var AllStatusMono = Hypatios.Player.AllStatusMonos;

            foreach (var baseStatusEffect in AllStatusMono)
            {
                var statusEffect1 = Hypatios.Assets.GetStatusEffect(baseStatusEffect.statusEffect.GetID());
                if (statusEffect1 == null) continue;
                var newButton = Instantiate(StatusMonoButton, parentPerks);
                newButton.gameObject.SetActive(true);
                newButton.baseStatusEffectGroup = statusEffect1;
                newButton.attachedStatusEffectGO = baseStatusEffect;
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
                var itemClass = Hypatios.Assets.GetItem(itemData.ID);
                bool matchingSearch = false;
                bool valid = false;

                if (filterCategoryType == ItemInventory.Category.None && isFavoriteActive == false)
                {
                    valid = true;
                }
                else if (filterCategoryType == ItemInventory.Category.Materials | filterCategoryType == ItemInventory.Category.Readables)
                {
                    bool correctFilter = false;

                    if (filterCategoryType == ItemInventory.Category.Readables && itemClass.IsReadableText == true)
                    {
                        correctFilter = true;
                    }
                    else if (filterCategoryType == ItemInventory.Category.Materials && itemClass.IsMaterials == true)
                    {
                        correctFilter = true;
                    }

                    if (correctFilter && isFavoriteActive && itemData.IsFavorite)
                    {
                        valid = true;
                    }
                    else if (correctFilter && !isFavoriteActive)
                    {
                        valid = true;
                    }
                }
                else
                {
                    if (filterCategoryType == ItemInventory.Category.None && isFavoriteActive && itemData.IsFavorite)
                    {
                        valid = true;
                    }
                    else if (itemData.category == filterCategoryType && isFavoriteActive && itemData.IsFavorite)
                    {
                        valid = true;
                    }
                    else if (itemData.category == filterCategoryType && !isFavoriteActive)
                    {
                        valid = true;
                    }
                }

                if (string.IsNullOrEmpty(input_SearchField.text)) matchingSearch = true;
                else
                {
                    if (itemClass.GetDisplayText().ToLower().Contains(input_SearchField.text.ToLower()))
                        matchingSearch = true;

                    if (itemClass.subCategory.ToString().ToLower().Contains(input_SearchField.text.ToLower()))
                        matchingSearch = true;

                    if (itemClass.CheckMatchingTags(input_SearchField.text.ToLower()))
                        matchingSearch = true;
                }

                if (matchingSearch == false) valid = false;

                if (valid)
                {
                    indexes.Add(x);
                }
            }

            foreach (var index in indexes)
            {
                var itemDat = Hypatios.Player.Inventory.allItemDatas[index];
                var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

                AssetStorageDatabase.SubiconIdentifier subicon = Hypatios.Assets.GetSubcategoryItemIcon(itemClass.subCategory);
                var newButton = Instantiate(InventoryItemButton, parentInventory);
                newButton.gameObject.SetActive(true);
                newButton.index = index;
                newButton.Refresh();
                newButton.Subicon.sprite = subicon.sprite;
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
        //HandleDiscardItem();
        //HandleFavoriteItem();
        //HandleConsumeItem();
        HandlePreview();
    }


    private void HandlePreview()
    {
        if (currentItemButton == null) return;
        if (healthRestoreBar.gameObject.activeInHierarchy == false) return;
        var itemClass = currentItemButton.GetItemInventory();

        if (itemClass == null) return;
        if (itemClass.category != ItemInventory.Category.Consumables) return;
        if (prevItemClass != itemClass) _timeSlider = 0f;

        float healSpeed = itemClass.consume_HealAmount / itemClass.consume_HealTime;
        float targetHeal = Hypatios.Player.Health.targetHealth + _timeSlider;

        _timeSlider += Time.unscaledDeltaTime * healSpeed * (Hypatios.Player.Health.digestion.Value);

        if (_timeSlider > itemClass.consume_HealAmount)
        {
            _timeSlider = 0;
        }

        if (Hypatios.Player.Health.targetHealth + _timeSlider > Hypatios.Player.Health.maxHealth.Value)
        {
            _timeSlider = 0f;
        }

        healthRestoreBar.maxValue = Hypatios.Player.Health.maxHealth.Value;
        healthRestoreBar.value = targetHeal;
        healthRestore_BorderBar.maxValue = Hypatios.Player.Health.maxHealth.Value;
        healthRestore_BorderBar.value = Hypatios.Player.Health.targetHealth + itemClass.consume_HealAmount;

        prevItemClass = itemClass;

    }

    private void HandleFavoriteItem()
    {

        #region Obsolete
        if (currentItemButton == null)
            return;

        if (Input.GetKeyUp(KeyCode.F) == false)
            return;

        var button = currentItemButton;
        var itemData = button.GetItemData();

        itemData.IsFavorite = !itemData.IsFavorite;

        RefreshUI();

        perkTooltipScript.gameObject.SetActive(false);
        itemTooltipScript.gameObject.SetActive(false);
        #endregion


    }

    private void HandleDiscardItem()
    {
        #region Obsolete

        {
            bool isFailed = false;

            if (currentItemButton == null)
            {
                _timePressDiscard = 0f;
                return;
            }

            if (Input.GetKey(KeyCode.X) == false) isFailed = true;


            if (isFailed)
            {
                _timePressDiscard = 0f;
                currentItemButton.discardProgress_slider.value = 0f;
                return;
            }
        }

        _timePressDiscard += Time.unscaledDeltaTime;
        currentItemButton.discardProgress_slider.value = (_timePressDiscard / TimeToDiscardPress);


        if (_timePressDiscard < TimeToDiscardPress)
        {
            return;
        }

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
        _timePressDiscard = 0f;
        #endregion
    }


    private void HandleConsumeItem()
    {

        #region Obsolete
        //{
        //    bool isFailed = false;

        //    if (currentItemButton == null)
        //    {
        //        _timePressConsume = 0f;
        //        return;
        //    }

        //    if (Hypatios.Input.Fire1.IsPressed() == false) isFailed = true;

        //    var itemDat = currentItemButton.GetItemData();

        //    if (itemDat.category != ItemInventory.Category.Consumables)
        //    {
        //        isFailed = true;
        //    }

        //    if (isFailed)
        //    {
        //        _timePressConsume = 0f;
        //        return;
        //    }
        //}

        //_timePressConsume += Time.unscaledDeltaTime;
        //currentItemButton.consumeProgress_slider.value = (_timePressConsume / TimeToConsumePress);

        //if (_timePressConsume >= TimeToConsumePress)
        //{
        //    UseItem(currentItemButton);
        //    _timePressConsume = 0;
        //}
        #endregion
    }

    public void ContextMenuItem()
    {

    }

    public void UseItem(InventoryItemButton button)
    {
        var itemCLass = button.GetItemInventory();
        var itemData = button.GetItemData();


        //Hypatios.RPG.UseItem(itemData);
        Debug.Log(Hypatios.UI.GetMousePositionScaled());
        Hypatios.UI.ShowContextMenu(Hypatios.UI.GetMousePositionScaled(), Hypatios.RPG.GetItemCommands(itemData));

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

    public void HighlightAttachment(AttachmentWeaponButton button)
    {
        var weaponItem = Hypatios.Assets.GetWeapon(button.weaponName);
        if (weaponItem == null)
        { return; }
        var weaponMod = weaponItem.GetAttachmentWeaponMod(button.attachmentID);
        string sLeft = "";
        string sRight = "";

        sLeft = $"<size=14>{weaponMod.Description}</size>";

        Hypatios.UI.ShowTooltipBig(button.GetComponent<RectTransform>(), sLeft, sRight);
        Hypatios.UI.RefreshInventoryIcon(null);

    }

    public void HighlightItem(InventoryItemButton button)
    {
        var itemDat = Hypatios.Player.Inventory.allItemDatas[button.index];
        var itemClass = Hypatios.Assets.GetItem(itemDat.ID);
        string sLeft = Hypatios.RPG.GetPreviewItemLeftSide(itemClass, itemDat);
        string sRight = Hypatios.RPG.GetPreviewItemRightSide(itemClass, itemDat);

        if (itemClass.category == ItemInventory.Category.Weapon)
        {
            var weaponClass = itemClass.attachedWeapon;
            var weaponSave = itemDat.weaponData;
            _currentHighlightedWeaponData = weaponSave;     
            PreviewWeapon(weaponClass, weaponSave);
        }
        else
        {
            if (itemClass.category != ItemInventory.Category.Consumables)
            {
            }
            else
            {
                ShowPreviewHealthRestore();
            }     
        }

        currentItemButton = button;

        if (itemDat.GENERIC_ESSENCE_POTION == true)
            Hypatios.UI.RefreshInventoryIcon(Hypatios.RPG.GetSprite_StatusEffect(itemDat), spriteColor);
        else
        {
            Hypatios.UI.RefreshInventoryIcon(Hypatios.RPG.GetSprite_StatusEffect(itemDat));
        }

        Hypatios.UI.ShowTooltipBig(button.GetComponent<RectTransform>(), sLeft, sRight);

    }

    public void DehighlightItem()
    {
        currentItemButton = null;
        HideHealthRestore();
        Hypatios.UI.RefreshInventoryIcon(null);
    }

    public void ShowPreviewHealthRestore()
    {
        healthRestoreBar.gameObject.SetActive(true);
        healthRestore_BorderBar.gameObject.SetActive(true);

    }

    public void HideHealthRestore()
    {
        healthRestoreBar.gameObject.SetActive(false);
        healthRestore_BorderBar.gameObject.SetActive(false);

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
        string s_interaction = "<'LMB' to unequip>";


        foreach (var attachment in weaponSave.allAttachments)
        {
            s_allAttachments += $"{WeaponClass.GetAttachmentName(attachment)}, ";
        }

        if (slotButton.equipIndex != 0)
        { sLeft = $"<size=14>{s_interaction}</size>"; }
        else if (slotButton.equipIndex == 0) 
        { sLeft = $"<size=14>Cannot unequip Pistol.</size>"; }

        PreviewWeapon(WeaponClass, weaponSave, isWeaponSlot: true);
        Hypatios.UI.ShowTooltipSmall(slotButton.GetComponent<RectTransform>(), sLeft, sRight);
    }

    public void DehighlightWeaponSlot()
    {

    }

    private void PreviewWeapon(WeaponItem weaponItem, HypatiosSave.WeaponDataSave weaponData, bool isWeaponSlot = false)
    {
        foreach (var button in _allWeaponModButtons)
        {
            Destroy(button.gameObject);
        }
        BaseWeaponScript weaponScript = null;
        if (isWeaponSlot)
        {
            weaponScript = Hypatios.Player.Weapon.GetWeaponScript(weaponItem.nameWeapon);
        }

        var itemclass = Hypatios.Assets.GetItemByWeapon(weaponItem.nameWeapon);

        _allWeaponModButtons.Clear();

        previewerWeapon.gameObject.SetActive(true);
        weaponPreviewerUI.gameObject.SetActive(true);
        previewerWeapon.DisplayWeapon(weaponItem);

        foreach (var attachment in weaponData.allAttachments)
        {
            var weaponMod = weaponItem.GetAttachmentWeaponMod(attachment);
            if (weaponMod == null) continue;

            var newButton = Instantiate(WeaponModButton, parentAttachmentButtons);
            newButton.gameObject.SetActive(true);
            newButton.weaponName = weaponItem.nameWeapon;
            newButton.attachmentID = attachment;
            newButton.Refresh();
            _allWeaponModButtons.Add(newButton);
        }

        if (weaponItem.attachments.Count == 0)
        {
            var newButton = Instantiate(WeaponModButton, parentAttachmentButtons);
            newButton.gameObject.SetActive(true);
            newButton.label.text = "No possible attachment";
            newButton.tooltipTrigger.enabled = false;
            _allWeaponModButtons.Add(newButton);
        }
        else if (weaponData.allAttachments.Count == 0)
        {
            var newButton = Instantiate(WeaponModButton, parentAttachmentButtons);
            newButton.gameObject.SetActive(true);
            newButton.label.text = "No attachment";
            newButton.tooltipTrigger.enabled = false;
            _allWeaponModButtons.Add(newButton);
        }
       
        weaponPreview_Label.text = itemclass.GetDisplayText();
        if (isWeaponSlot == false)
        {
            if (weaponItem.isMachineOfMadness)
            {
                weaponPreview_AmmoCount.text = $"∞/∞";
            }
            else
            {
                weaponPreview_AmmoCount.text = $"{weaponData.currentAmmo} / {weaponData.totalAmmo}";
            }
        }
        else
        {
            weaponPreview_AmmoCount.text = $"{weaponScript.curAmmo} / {weaponScript.totalAmmo}";
        }

        {
            var weaponStat = weaponItem.GetFinalStat(weaponData.allAttachments);
            string s_weaponStat = "<size=16>";
            s_weaponStat += $"Damage: {weaponStat.damage}\n";
            s_weaponStat += $"Fire rate: {weaponStat.cooldown}\n";
            s_weaponStat += $"DPS: {Mathf.Round(weaponStat.damage * weaponStat.cooldown * 10)/10}\n";
            s_weaponStat += $"Mag size: {weaponStat.magazineSize}";
            s_weaponStat += "</size>\n\n";
            s_weaponStat += $"{itemclass.Description}";
            weaponPreview_WeaponStats.text = s_weaponStat;
        }

        weaponPreview_AmmoIcon.sprite = weaponItem.ammoSpriteIcon;
    }

    private void DepreviewWeapon()
    {
        previewerWeapon.gameObject.SetActive(false);
        weaponPreviewerUI.gameObject.SetActive(false);
    }

    public void HighlightStat(CharStatButton charStatButton)
    {
        var value1 = Hypatios.Player.GetCharFinalValue(charStatButton.category1);
        var baseStat = Hypatios.Assets.GetStatusEffect(charStatButton.category1);

        if (charStatButton.category1 == ModifierEffectCategory.BonusDamageMelee
            | charStatButton.category1 == ModifierEffectCategory.BonusDamageGun)
        {
            value1 -= 1;
        }

        string sLeft = $"{baseStat.GetTitlePerk()}";
        string sRight = Hypatios.RPG.GetDescription(charStatButton.category1, value1);


        Hypatios.UI.ShowTooltipSmall(charStatButton.GetComponent<RectTransform>(), sLeft, sRight);
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

        if (_currentPerk.type == RPG_CharPerkButton.Type.TemporaryModifier)
        {
            var value = currentPerkButton.attachedStatusEffectGO.Value;
            string sLeft = "";
            string sRight = "";

            sLeft = $"{currentPerkButton.statusEffect.GetTitlePerk()} {timerString}";

            if (value == 0 | value == -1)
            {
                sRight = "";
            }
            else
            {
                sRight = Hypatios.RPG.GetDescription(_currentPerk.statusEffect.category, value);
            }

            Hypatios.UI.ShowTooltipSmall(_currentPerk.GetComponent<RectTransform>(), sLeft, sRight);
        }
        else
        {
            var statEffectGroup = _currentPerk.baseStatusEffectGroup;
            string str1 = $"{statEffectGroup.GetDisplayText()} <size=13>{timerString}</size>\n<size=13>{statEffectGroup.Description}</size>";
            string str2 = "";
            string sLeft = "";
            string sRight = "";

            foreach (var modifier in statEffectGroup.allStatusEffects)
            {
                var baseModifier = Hypatios.Assets.GetStatusEffect(modifier.statusCategoryType);
                str2 += $"[{baseModifier.GetTitlePerk()}] [{Hypatios.RPG.GetDescription(modifier.statusCategoryType, modifier.Value)}]\n";
            }
            str2 = ""; //scrapped modifier text
            sLeft = $"{str1}\n<size=13>{str2}</size>";
            sRight = "";
            Hypatios.UI.ShowTooltipBig(_currentPerk.GetComponent<RectTransform>(), sLeft, sRight);
            Hypatios.UI.RefreshInventoryIcon(null);


        }
    }

    public void DehighlightPerk()
    {
        currentPerkButton = null;
    }

    #endregion
}
