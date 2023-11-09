using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class kThanidLabUI : MonoBehaviour
{

    public enum Mode
    {
        None,
        Essence,
        SerumFabricate
    }

    public GameObject UI_Panel_Essence;
    public GameObject UI_Panel_SerumFabricate;
    public GameObject retardedGO_CloseTooltip;
    public Mode currentMode;
    public ItemInventory essenceBottle;
    public ItemInventory exoticCore;
    [FoldoutGroup("Create Essence")] public RectTransform rt_Essence_MyInventoryParent;
    [FoldoutGroup("Create Essence")] public RectTransform rt_Essence_ExtractorParent;
    [FoldoutGroup("Create Essence")] public RectTransform rt_Essence_ResultParent;
    [FoldoutGroup("Create Essence")] public CreateEssenceButton prefab_Essence_MyItemButton;
    [FoldoutGroup("Create Essence")] public CreateEssenceButton prefab_Essence_ExtractorButton;
    [FoldoutGroup("Create Essence")] public CreateEssenceButton prefab_Essence_ResultButton;
    [FoldoutGroup("Create Essence")] public GameObject Essence_labelNoItem;
    [FoldoutGroup("Create Essence")] public Color spriteColor;
    [FoldoutGroup("Quick Button")] public Button quickButton_AddBottle;
    [FoldoutGroup("Quick Button")] public Button quickButton_AddExoticCore;
    [FoldoutGroup("Quick Button")] public Button quickButton_RandomMaterial;
    private List<CreateEssenceButton> all_Essence_MyItemButtons = new List<CreateEssenceButton>();
    private List<CreateEssenceButton> all_Essence_ExtractorButtons = new List<CreateEssenceButton>();
    private List<CreateEssenceButton> all_Essence_ResultButtons = new List<CreateEssenceButton>();


    [FoldoutGroup("Fabricator")] public kThanidUI_SerumCreator SerumCreator;
    [FoldoutGroup("Fabricator")] public RectTransform rt_Serum_MyInventoryParent;
    [FoldoutGroup("Fabricator")] public RectTransform rt_Serum_FabricatorParent;
    [FoldoutGroup("Fabricator")] public FabricateSerumButton prefab_Serum_MyItemButton;
    [FoldoutGroup("Fabricator")] public FabricateSerumButton prefab_Serum_FabricatorButton;
    [FoldoutGroup("Create Essence")] public GameObject Fabricator_labelNoItem;
    private List<FabricateSerumButton> all_Serum_MyItemButtons = new List<FabricateSerumButton>();
    private List<FabricateSerumButton> all_Serum_FabricatorButtons = new List<FabricateSerumButton>();


    [FoldoutGroup("DEBUG")] [HideInEditorMode] public string DEBUG_SERUM_CUSTOM_NAME = "Pathetic Serum";
    [FoldoutGroup("DEBUG")] [HideInEditorMode] public List<PerkCustomEffect> DEBUG_SERUM_CUSTOM_EFFECTS = new List<PerkCustomEffect>();
    [FoldoutGroup("DEBUG")] [HideInEditorMode] public List<BaseStatusEffectObject> DEBUG_SERUM_AILMENTS = new List<BaseStatusEffectObject>();
    [FoldoutGroup("DEBUG")] [HideInEditorMode] public float DEBUG_SERUM_TIME = 4f;
    [FoldoutGroup("DEBUG")] [HideInEditorMode] public float DEBUG_SERUM_ALCOHOL = 0f;
    [FoldoutGroup("DEBUG")] [HideInEditorMode] [ShowIf("ESSENCE_TYPE", HypatiosSave.EssenceType.Modifier)] public ModifierEffectCategory DEBUG_ESSENCE_CATEGORY = ModifierEffectCategory.ArmorRating;
    [FoldoutGroup("DEBUG")] [HideInEditorMode] [ShowIf("ESSENCE_TYPE", HypatiosSave.EssenceType.Ailment)] public string DEBUG_ESSENCE_STATSGROUP = "";
    [FoldoutGroup("DEBUG")] [HideInEditorMode] public HypatiosSave.EssenceType DEBUG_ESSENCE_TYPE;


    [ShowInInspector] [ReadOnly] private Inventory extractorInventory = new Inventory();
    [ShowInInspector] [ReadOnly] private Inventory fabricatorInventory = new Inventory();

    public Inventory ExtractorInventory { get => extractorInventory; }
    public Inventory FabricatorInventory { get => fabricatorInventory; }
    public List<int> Index_AntiPotions { get => index_AntiPotions; }

    [ShowInInspector] [ReadOnly] private List<int> index_AntiPotions = new List<int>();
    private List<int> index_PrevAntiPotions = new List<int>();

    private CraftingkThanidLabTrigger _currentBench;

    public CraftingkThanidLabTrigger CurrentWorkbench { get => _currentBench; set => _currentBench = value; }

    public void SetShopScript(CraftingkThanidLabTrigger _shop)
    {
        _currentBench = _shop;
    }

    public int EssenceMultiplierCraft()
    {
        return essence_multiplier;
    }

    private bool hasStarted = false;

    private void Start()
    {
        prefab_Essence_MyItemButton.gameObject.SetActive(false);
        prefab_Essence_ExtractorButton.gameObject.SetActive(false);
        prefab_Essence_ResultButton.gameObject.SetActive(false);
        prefab_Serum_MyItemButton.gameObject.SetActive(false);
        prefab_Serum_FabricatorButton.gameObject.SetActive(false);
        if (hasStarted == false) RefreshUI();
        hasStarted = true;
    }

    private void OnEnable()
    {
        if (hasStarted == false) return;
        ChangeMode(0);
        RefreshUI();
    }

    private void OnDisable()
    {
        //return all item back
        foreach (var itemDat in extractorInventory.allItemDatas)
        {
            var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

            Hypatios.Player.Inventory.AddItemGenericSafe(itemClass, itemDat, itemDat.count);
        }

        foreach (var itemDat in fabricatorInventory.allItemDatas)
        {
            var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

            Hypatios.Player.Inventory.AddItemGenericSafe(itemClass, itemDat, itemDat.count);
        }

        extractorInventory.allItemDatas.Clear();
        fabricatorInventory.allItemDatas.Clear();
        Hypatios.UI.CloseAllTooltip();
    }


    #region Create Debug

    [FoldoutGroup("DEBUG")]
    [HideInEditorMode]
    [Button("Create kThanid Item")]
    public void CustomCreate_kThanid()
    {
        HypatiosSave.ItemDataSave itemDat = new HypatiosSave.ItemDataSave();
        itemDat.ID = "Serum_kThanid";
        itemDat.count = 1;
        itemDat.category = ItemInventory.Category.Consumables;
        itemDat.isGenericItem = true;
        itemDat.GENERIC_KTHANID_SERUM = true;
        itemDat.SERUM_CUSTOM_NAME = DEBUG_SERUM_CUSTOM_NAME;
        itemDat.SERUM_CUSTOM_EFFECTS = new List<PerkCustomEffect>();
        itemDat.SERUM_ALCOHOL = DEBUG_SERUM_ALCOHOL;
        foreach (var effect in DEBUG_SERUM_CUSTOM_EFFECTS)
        {
            var dupeEffect = effect.Clone();
            dupeEffect.origin = itemDat.SERUM_CUSTOM_NAME;
            dupeEffect.isPermanent = false;
            dupeEffect.timer = DEBUG_SERUM_TIME;
            itemDat.SERUM_CUSTOM_EFFECTS.Add(dupeEffect);
        }
        foreach(var ailment in DEBUG_SERUM_AILMENTS)
        {
            itemDat.SERUM_AILMENTS.Add(ailment.GetID());
        }
        itemDat.SERUM_TIME = DEBUG_SERUM_TIME;
        Hypatios.Player.Inventory.allItemDatas.Add(itemDat);
    }

    [FoldoutGroup("DEBUG")]
    [HideInEditorMode]
    [Button("Create Essence Potion")]
    public void CustomCreate_EssencePotion()
    {
        HypatiosSave.ItemDataSave itemDat = new HypatiosSave.ItemDataSave();
        itemDat.ID = "Essence_Potion";
        itemDat.count = 1;
        itemDat.category = ItemInventory.Category.Normal;
        itemDat.isGenericItem = true;
        itemDat.GENERIC_ESSENCE_POTION = true;
        itemDat.ESSENCE_CATEGORY = DEBUG_ESSENCE_CATEGORY;
        itemDat.ESSENCE_STATUSEFFECT_GROUP = DEBUG_ESSENCE_STATSGROUP;
        itemDat.ESSENCE_TYPE = DEBUG_ESSENCE_TYPE;

        Hypatios.Player.Inventory.allItemDatas.Add(itemDat);
    }
    #endregion

    private void Update()
    {
        
    }

    #region Refresh UI

    public void RefreshUI()
    {

        RefreshButton_Inventory();
        RefreshButton_Extractor();
        RefreshButton_Fabricator();
        RefreshButton_Result();
        Reload_QuickButtons();
        ModifyAntiPotionIndex();
        SerumCreator.Refresh();
        RefreshLabel();
        retardedGO_CloseTooltip.gameObject.SetActive(true);
        //Hypatios.UI.CloseAllTooltip();

    }

    private void RefreshLabel()
    {
        if (all_Essence_MyItemButtons.Count == 0)
        {
            Essence_labelNoItem.gameObject.SetActive(true);
        }
        else
        {
            Essence_labelNoItem.gameObject.SetActive(false);
        }

        if (all_Serum_MyItemButtons.Count == 0)
        {
            Fabricator_labelNoItem.gameObject.SetActive(true);
        }
        else
        {
            Fabricator_labelNoItem.gameObject.SetActive(false);
        }
    }

    private void RefreshButton_Inventory()
    {
        //refresh all my inventory buttons
        foreach (var button in all_Essence_MyItemButtons)
        {
            Destroy(button.gameObject);
        }
        foreach (var button in all_Serum_MyItemButtons)
        {
            Destroy(button.gameObject);
        }

        all_Essence_MyItemButtons.Clear();
        all_Serum_MyItemButtons.Clear();

        //refresh inventories in Essence
        {
            List<int> indexes = new List<int>();
            var All_Items = Hypatios.Player.Inventory.allItemDatas;


            for (int x = 0; x < All_Items.Count; x++)
            {
                var itemData = All_Items[x];
                var itemClass = Hypatios.Assets.GetItem(itemData.ID);
                bool allowFilter = false;

                //if (itemData.category == ItemInventory.Category.Normal && itemClass.subCategory == ItemInventory.SubiconCategory.Material && itemData.isGenericItem == false)
                //if (itemData.category == ItemInventory.Category.Consumables && itemData.isGenericItem == false)

                if (IsValidEssenceCraftingItem(itemClass)) allowFilter = true;

                if (allowFilter)
                {
                    indexes.Add(x);
                }
            }

            foreach (var index in indexes)
            {
                var itemDat = Hypatios.Player.Inventory.allItemDatas[index];
                var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

                AssetStorageDatabase.SubiconIdentifier subicon = Hypatios.Assets.GetSubcategoryItemIcon(itemClass.subCategory);
                var newButton = Instantiate(prefab_Essence_MyItemButton, rt_Essence_MyInventoryParent);
                newButton.gameObject.SetActive(true);
                newButton.index = index;
                newButton.Refresh();
                newButton.Subicon.sprite = subicon.sprite;
                all_Essence_MyItemButtons.Add(newButton);
            }
        }

        //refresh inventories in Serum
        {
            List<int> indexes = new List<int>();
            var All_Items = Hypatios.Player.Inventory.allItemDatas;


            for (int x = 0; x < All_Items.Count; x++)
            {
                var itemData = All_Items[x];
                var itemClass = Hypatios.Assets.GetItem(itemData.ID);
                bool allowFilter = false;

                if (itemData.category == ItemInventory.Category.Normal && itemClass.subCategory == ItemInventory.SubiconCategory.Essence)
                {
                    allowFilter = true;
                }

                if (itemData.category == ItemInventory.Category.Consumables && itemClass.subCategory == ItemInventory.SubiconCategory.Alcohol && itemData.isGenericItem == false)
                {
                    allowFilter = true;
                }

                if (allowFilter)
                {
                    indexes.Add(x);
                }
            }

            foreach (var index in indexes)
            {
                var itemDat = Hypatios.Player.Inventory.allItemDatas[index];
                var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

                AssetStorageDatabase.SubiconIdentifier subicon = Hypatios.Assets.GetSubcategoryItemIcon(itemClass.subCategory);
                var newButton = Instantiate(prefab_Serum_MyItemButton, rt_Serum_MyInventoryParent);
                newButton.gameObject.SetActive(true);
                newButton.index = index;
                newButton.Refresh();
                newButton.Subicon.sprite = subicon.sprite;
                all_Serum_MyItemButtons.Add(newButton);
            }
        }
    }

    private void RefreshButton_Extractor()
    {
        //refresh all my inventory buttons
        foreach (var button in all_Essence_ExtractorButtons)
        {
            Destroy(button.gameObject);
        }

        all_Essence_ExtractorButtons.Clear();

        //refresh inventories
        {
            List<int> indexes = new List<int>();
            var All_Items = extractorInventory.allItemDatas;


            for (int x = 0; x < All_Items.Count; x++)
            {
                var itemData = All_Items[x];
                var itemClass = Hypatios.Assets.GetItem(itemData.ID);
                bool allowFilter = false;

                if (itemData.category == ItemInventory.Category.Normal && itemClass.subCategory == ItemInventory.SubiconCategory.Material)
                {
                    allowFilter = true;
                }
                if (itemData.category == ItemInventory.Category.Consumables && itemData.isGenericItem == false)
                {
                    allowFilter = true;
                }

                if (allowFilter)
                {
                    indexes.Add(x);
                }
            }

            foreach (var index in indexes)
            {
                var itemDat = extractorInventory.allItemDatas[index];
                var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

                AssetStorageDatabase.SubiconIdentifier subicon = Hypatios.Assets.GetSubcategoryItemIcon(itemClass.subCategory);
                var newButton = Instantiate(prefab_Essence_ExtractorButton, rt_Essence_ExtractorParent);
                newButton.gameObject.SetActive(true);
                newButton.index = index;
                newButton.Refresh();
                newButton.Subicon.sprite = subicon.sprite;
                all_Essence_ExtractorButtons.Add(newButton);
            }
        }
    }

    private void RefreshButton_Result()
    {
        //refresh all my inventory buttons
        foreach (var button in all_Essence_ResultButtons)
        {
            Destroy(button.gameObject);
        }

        all_Essence_ResultButtons.Clear();

        var allCraftableModifiers = GetCraftableModifiers();
        var allCraftableAilments = GetCraftableAilments();
        Refresh_EssenceMultiplier();

        foreach (var craftable in allCraftableModifiers)
        {
            var newButton = Instantiate(prefab_Essence_ResultButton, rt_Essence_ResultParent);
            newButton.gameObject.SetActive(true);
            newButton.ESSENCE_CATEGORY = craftable.category;
            newButton.Refresh();
            all_Essence_ResultButtons.Add(newButton);
        }

        foreach (var craftable in allCraftableAilments)
        {
            var newButton = Instantiate(prefab_Essence_ResultButton, rt_Essence_ResultParent);
            newButton.gameObject.SetActive(true);
            newButton.ESSENCE_STATUSEFFECT_GROUP = craftable.GetID();
            newButton.Refresh();
            all_Essence_ResultButtons.Add(newButton);
        }
    }

    private void RefreshButton_Fabricator()
    {
        //refresh all my inventory buttons
        foreach (var button in all_Serum_FabricatorButtons)
        {
            Destroy(button.gameObject);
        }

        all_Serum_FabricatorButtons.Clear();

        //refresh inventories
        {
            List<int> indexes = new List<int>();
            var All_Items = fabricatorInventory.allItemDatas;


            for (int x = 0; x < All_Items.Count; x++)
            {
                var itemData = All_Items[x];
                var itemClass = Hypatios.Assets.GetItem(itemData.ID);
                bool allowFilter = false;

                if (itemData.category == ItemInventory.Category.Normal && itemClass.subCategory == ItemInventory.SubiconCategory.Essence)
                {
                    allowFilter = true;
                }

                if (itemData.category == ItemInventory.Category.Consumables && itemClass.subCategory == ItemInventory.SubiconCategory.Alcohol && itemData.isGenericItem == false)
                {
                    allowFilter = true;
                }

                if (allowFilter)
                {
                    indexes.Add(x);
                }
            }

            foreach (var index in indexes)
            {
                var itemDat = fabricatorInventory.allItemDatas[index];
                var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

                AssetStorageDatabase.SubiconIdentifier subicon = Hypatios.Assets.GetSubcategoryItemIcon(itemClass.subCategory);
                var newButton = Instantiate(prefab_Serum_FabricatorButton, rt_Serum_FabricatorParent);
                newButton.gameObject.SetActive(true);
                newButton.index = index;
                if (Index_AntiPotions.Contains(index)) newButton.isAntiPotion = true;
                newButton.Refresh();
                newButton.Subicon.sprite = subicon.sprite;
                all_Serum_FabricatorButtons.Add(newButton);
            }
        }
    }

    //This is absolutely fucking retarded
    private void ModifyAntiPotionIndex()
    {

        foreach(var fabricatorButton in all_Serum_FabricatorButtons)
        {
            if (fabricatorButton.IsValidItemForAntiPotion() == false)
            {
                Index_AntiPotions.Remove(fabricatorButton.index);
            }
        }

        index_PrevAntiPotions = Index_AntiPotions;
    }

    public List<BaseModifierEffectObject> GetCraftableModifiers()
    {
        List<BaseModifierEffectObject> listModifiers = new List<BaseModifierEffectObject>();
        var allModifiers = Hypatios.Assets.AllModifierEffects.FindAll(x => x.craftableEssence == true);
        
        foreach(var modifier in allModifiers)
        {
            bool isFailed = false;

            foreach(var craft in modifier.requirementCrafting)
            {
                if (extractorInventory.Count(craft.inventory.GetID()) < craft.count)
                {
                    isFailed = true;
                    break;
                }
            }

            //Check if there is essence bottle
            if (extractorInventory.Count(essenceBottle.GetID()) < 1) isFailed = true;

            if (isFailed) continue;

            listModifiers.Add(modifier);
        }

        return listModifiers;
    }

    public List<BaseStatusEffectObject> GetCraftableAilments()
    {
        List<BaseStatusEffectObject> listAilments = new List<BaseStatusEffectObject>();
        var allAilments = Hypatios.Assets.AllStatusEffects.FindAll(x => x.craftableEssence == true);

        foreach (var modifier in allAilments)
        {
            bool isFailed = false;

            foreach (var craft in modifier.requirementCrafting)
            {
                if (extractorInventory.Count(craft.inventory.GetID()) < craft.count)
                {
                    isFailed = true;
                    break;
                }
            }

            //Check if there is essence bottle
            if (extractorInventory.Count(essenceBottle.GetID()) < 1) isFailed = true;

            if (isFailed) continue;

            listAilments.Add(modifier);
        }

        return listAilments;
    }

    #endregion


    #region Extractor-Essences

    public void TransferToExtractor(CreateEssenceButton button)
    {
        var itemDat = Hypatios.Player.Inventory.allItemDatas[button.index];
        var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

        //remove from my inventory
        //onDisable, return all of it to the player back from extractor
        Hypatios.Player.Inventory.RemoveItem(itemDat);
        extractorInventory.AddItemGenericSafe(itemClass, itemDat);
        RefreshUI();

    }

    public void TransferToMyInventory(CreateEssenceButton button)
    {
        var itemDat = extractorInventory.allItemDatas[button.index];
        var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

        extractorInventory.RemoveItem(itemDat);
        Hypatios.Player.Inventory.AddItemGenericSafe(itemClass, itemDat);
        RefreshUI();

    }

    private int essence_multiplier = 1;

    public void CraftEssence(CreateEssenceButton button)
    {
        if (button.IsCraftRequirementMet() == false)
        {
            DeadDialogue.PromptNotifyMessage_Mod("Insufficient materials!", 4f);
            Debug.Log("Insufficient materials!");
            MainGameHUDScript.Instance.audio_Error.Play();
            return;
        }

        if (button.ESSENCE_CATEGORY != ModifierEffectCategory.Nothing)
        {
            var modifier = Hypatios.Assets.GetStatusEffect(button.ESSENCE_CATEGORY);

            foreach (var recipe in modifier.requirementCrafting)
            {
                extractorInventory.RemoveItem(recipe.inventory.GetID(), recipe.count);
            }
        }
        else
        {
            var ailment = Hypatios.Assets.GetStatusEffect(button.ESSENCE_STATUSEFFECT_GROUP);

            foreach (var recipe in ailment.requirementCrafting)
            {
                extractorInventory.RemoveItem(recipe.inventory.GetID(), recipe.count);
            }
        }

        Refresh_EssenceMultiplier();
        extractorInventory.RemoveItem(essenceBottle.GetID(), 1);

        if (ExtractorInventory.Count(exoticCore) > 0)
        {
            int limitCore = Mathf.Clamp(extractorInventory.Count(exoticCore), 0, 3);
            extractorInventory.RemoveItem(exoticCore.GetID(), limitCore);
        }
        CreateEssencePotion(button);
        MainGameHUDScript.Instance.audio_PurchaseReward.Play();
        RefreshUI();

    }

    private void Refresh_EssenceMultiplier()
    {
        essence_multiplier = 1 + extractorInventory.Count(exoticCore);
        essence_multiplier = Mathf.Clamp(essence_multiplier, 0, 3+1);
    }

    public void CreateEssencePotion(CreateEssenceButton button)
    {
        HypatiosSave.ItemDataSave itemDat = new HypatiosSave.ItemDataSave();
        itemDat.ID = "Essence_Potion";
        itemDat.count = 1;
        itemDat.category = ItemInventory.Category.Normal;
        itemDat.isGenericItem = true;
        itemDat.GENERIC_ESSENCE_POTION = true;
        itemDat.ESSENCE_MULTIPLIER = essence_multiplier;
        itemDat.ESSENCE_CATEGORY = button.ESSENCE_CATEGORY;
        itemDat.ESSENCE_STATUSEFFECT_GROUP = button.ESSENCE_STATUSEFFECT_GROUP;
        if (itemDat.ESSENCE_CATEGORY != ModifierEffectCategory.Nothing)
        {
            itemDat.ESSENCE_TYPE = HypatiosSave.EssenceType.Modifier;
            DeadDialogue.PromptNotifyMessage_Mod($"Crafted {Hypatios.RPG.GetEssenceName(itemDat.ESSENCE_CATEGORY)}.", 4f);
        }
        else
        {
            itemDat.ESSENCE_TYPE = HypatiosSave.EssenceType.Ailment;
            DeadDialogue.PromptNotifyMessage_Mod($"Crafted {Hypatios.RPG.GetEssenceName(itemDat.ESSENCE_STATUSEFFECT_GROUP)}.", 4f);
        }

        Hypatios.Player.Inventory.allItemDatas.Add(itemDat);
    }

    #endregion

    #region Essencor Functions

    public void Reload_QuickButtons()
    {
        if (Hypatios.Player.Inventory.Count(essenceBottle) <= 0)
        {
            quickButton_AddBottle.interactable = false;
        }
        else
        {
            quickButton_AddBottle.interactable = true;
        }

        if (Hypatios.Player.Inventory.Count(exoticCore) <= 0)
        {
            quickButton_AddExoticCore.interactable = false;
        }
        else
        {
            quickButton_AddExoticCore.interactable = true;
        }

    }

    public void Function_AddBottle()
    {
        var button_EssenceBottle = GetButtonByItem(essenceBottle);

        if (button_EssenceBottle == null) return;

        TransferToExtractor(button_EssenceBottle);
    }

    public void Function_AddExoticCore()
    {
        var button_Exotic = GetButtonByItem(exoticCore);

        if (button_Exotic == null) return;

        TransferToExtractor(button_Exotic);
    }

    public void Function_Random()
    {
        //search through the perk database and find if item exists
        int tries = 0; //limit = 32
        ItemInventory itemGot = null;

        while (tries < 32 && itemGot == null)
        {
            itemGot = GetItemToCraft();
            tries++;
        }

        if (itemGot == null) return;
        var button_ItemTarget = GetButtonByItem(itemGot);
        if (button_ItemTarget == null) return;

        TransferToExtractor(button_ItemTarget);
    }

    public CreateEssenceButton GetButtonByItem(ItemInventory itemClass)
    {
        foreach(var button in all_Essence_MyItemButtons)
        {
            if (button.IsItemMatch(itemClass)) return button;
        }

        return null;
    }

    public ItemInventory GetItemToCraft()
    {
        var list_ModifierEffect = Hypatios.Assets.AllModifierEffects;
        list_ModifierEffect.Shuffle();

        foreach (var perkModifier in list_ModifierEffect)
        {
            if (perkModifier.craftableEssence == false) continue;

            foreach (var craftMat in perkModifier.requirementCrafting)
            {
                if (Hypatios.Player.Inventory.Count(craftMat.inventory) > 0)
                {
                    return craftMat.inventory;
                }
            }
        }

        var list_StatEffect = Hypatios.Assets.AllStatusEffects;
        list_StatEffect.Shuffle();

        foreach (var statusEffect in list_StatEffect)
        {
            if (statusEffect.craftableEssence == false) continue;

            foreach (var craftMat in statusEffect.requirementCrafting)
            {
                if (Hypatios.Player.Inventory.Count(craftMat.inventory) > 0)
                {
                    return craftMat.inventory;
                }
            }
        }

        return null;
    }

    public bool IsValidEssenceCraftingItem(ItemInventory itemInventory)
    {
        var list_StatEffect = Hypatios.Assets.AllStatusEffects;
        var list_ModifierEffect = Hypatios.Assets.AllModifierEffects;

        if (itemInventory == exoticCore) return true;
        if (itemInventory == essenceBottle) return true;

        foreach (var perkModifier in list_ModifierEffect)
        {
            if (perkModifier.craftableEssence == false) continue;

            foreach (var craftMat in perkModifier.requirementCrafting)
            {
                if (craftMat.inventory == itemInventory)
                    return true;
            }
        }

        foreach (var statusEffect in list_StatEffect)
        {
            if (statusEffect.craftableEssence == false) continue;

            foreach (var craftMat in statusEffect.requirementCrafting)
            {
                if (craftMat.inventory == itemInventory)
                    return true;
            }
        }



        return false;
    }

    #endregion

    #region Fabricator Serum

    public void TransferToFabricator(FabricateSerumButton button)
    {
        var itemDat = Hypatios.Player.Inventory.allItemDatas[button.index];
        var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

        //remove from my inventory
        //onDisable, return all of it to the player back from extractor
        fabricatorInventory.AddItemGenericSafe(itemClass, itemDat);
        Hypatios.Player.Inventory.RemoveItem(itemDat);
        RefreshUI();

    }

    public void TransferToMyInventory(FabricateSerumButton button)
    {
        var itemDat = fabricatorInventory.allItemDatas[button.index];
        var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

        Hypatios.Player.Inventory.AddItemGenericSafe(itemClass, itemDat);
        fabricatorInventory.RemoveItem(itemDat);
        RefreshUI();

    }

    public void ToggleAntiPotion(FabricateSerumButton button)
    {
        if (Index_AntiPotions.Contains(button.index) == false)
        {
            Index_AntiPotions.Add(button.index);
        }
        else
        {
            Index_AntiPotions.Remove(button.index);
        }
        RefreshUI();
    }

    #endregion

    #region Highlight Buttons

    public void HighlightButton(CreateEssenceButton button)
    {
        string sLeft = "";
        string sRight = "";
        string sInteraction = "\n";

        HypatiosSave.ItemDataSave itemDat = null;
        ItemInventory itemClass = null;

        if (button.buttonType == CreateEssenceButton.Type.Inventory)
        {
            itemDat = Hypatios.Player.Inventory.allItemDatas[button.index];
            itemClass = Hypatios.Assets.GetItem(itemDat.ID);
            sLeft = Hypatios.RPG.GetPreviewItemLeftSide(itemClass, itemDat, true);
            sRight = Hypatios.RPG.GetPreviewItemRightSide(itemClass, itemDat);

            sInteraction += "<'LMB' to add to Extractor>\n";
        }
        else if (button.buttonType == CreateEssenceButton.Type.Extractor)
        {
            itemDat = extractorInventory.allItemDatas[button.index];
            itemClass = Hypatios.Assets.GetItem(itemDat.ID);

            sLeft = Hypatios.RPG.GetPreviewItemLeftSide(itemClass, itemDat, true);
            sRight = Hypatios.RPG.GetPreviewItemRightSide(itemClass, itemDat);

            sInteraction += "<'LMB' to return to Inventory>\n";

        }
        else if (button.buttonType == CreateEssenceButton.Type.Result)
        {
            sLeft = button.GetString_Highlight();

        }

        Hypatios.UI.ShowTooltipBig(button.GetComponent<RectTransform>(), sLeft + sInteraction, sRight);

        if (itemClass != null)
        {
            Hypatios.UI.RefreshInventoryIcon(itemClass.GetSprite());
        }
        else
        {
            Hypatios.UI.RefreshInventoryIcon(null);

        }

        if (button.buttonType == CreateEssenceButton.Type.Result)
        {
            Hypatios.UI.RefreshInventoryIcon(button.GetSprite(), spriteColor);

        }
    }

    public void HighlightButton(FabricateSerumButton button)
    {
        string sLeft = "";
        string sRight = "";
        string sInteraction = "\n";

        HypatiosSave.ItemDataSave itemDat = null;
        ItemInventory itemClass = null;

        if (button.buttonType == FabricateSerumButton.Type.Inventory)
        {
            itemDat = Hypatios.Player.Inventory.allItemDatas[button.index];
            itemClass = Hypatios.Assets.GetItem(itemDat.ID);
            sLeft = Hypatios.RPG.GetPreviewItemLeftSide(itemClass, itemDat, true);
            sRight = Hypatios.RPG.GetPreviewItemRightSide(itemClass, itemDat);

            sInteraction += "<'LMB' to add to Fabricator>\n";

        }
        else if (button.buttonType == FabricateSerumButton.Type.Fabricator)
        {
            itemDat = fabricatorInventory.allItemDatas[button.index];
            itemClass = Hypatios.Assets.GetItem(itemDat.ID);

            sLeft = Hypatios.RPG.GetPreviewItemLeftSide(itemClass, itemDat, true);
            sRight = Hypatios.RPG.GetPreviewItemRightSide(itemClass, itemDat);

            sInteraction += "<'LMB' to return to Inventory>\n";

        }

        Hypatios.UI.ShowTooltipBig(button.GetComponent<RectTransform>(), sLeft + sInteraction, sRight);

        if (itemClass != null)
        {
            if (itemDat.GENERIC_ESSENCE_POTION == true)
                Hypatios.UI.RefreshInventoryIcon(Hypatios.RPG.GetSprite_StatusEffect(itemDat), spriteColor);
            else
            {
                Hypatios.UI.RefreshInventoryIcon(Hypatios.RPG.GetSprite_StatusEffect(itemDat));
            }
        }
        else
        {
            Hypatios.UI.RefreshInventoryIcon(null);

        }
    }

    public void DehighlightButton()
    {
        Hypatios.UI.RefreshInventoryIcon(null);
    }

    public void Retarded_CloseTooltip()
    {
        Hypatios.UI.CloseAllTooltip();
    }

    #endregion


    #region Modes

    public void ChangeMode(int _mode)
    {
        currentMode = (Mode)_mode;
        Hypatios.UI.CloseAllTooltip();
        UpdateMode();
    }

    private void UpdateMode()
    {
        if (currentMode == Mode.None)
        {
            UI_Panel_Essence.gameObject.SetActive(false);
            UI_Panel_SerumFabricate.gameObject.SetActive(false);
        }

        if (currentMode == Mode.Essence)
        {
            if (UI_Panel_Essence.activeSelf == false) UI_Panel_Essence.gameObject.SetActive(true);
        }
        else
        {
            UI_Panel_Essence.gameObject.SetActive(false);
        }

        if (currentMode == Mode.SerumFabricate)
        {
            if (UI_Panel_SerumFabricate.activeSelf == false) UI_Panel_SerumFabricate.gameObject.SetActive(true);
        }
        else
        {
            UI_Panel_SerumFabricate.gameObject.SetActive(false);
        }
    }

    #endregion

}
