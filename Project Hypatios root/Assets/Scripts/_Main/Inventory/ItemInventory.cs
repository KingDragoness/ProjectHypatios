using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Apple", menuName = "Hypatios/Item", order = 1)]

public class ItemInventory : ScriptableObject
{

    [System.Serializable]
    public enum Category
    {
        Normal,
        Consumables,
        Quest,
        Key,
        Weapon,
        Materials = 100, //for category filter
        Readables = 101,
        None = 999 //For inventory filter
    }

    public enum SubiconCategory
    {
        Default,
        Book = 10,
        Material,
        Notes,
        FictionBook,
        Alcohol = 20,
        Serum,
        Meds,
        Foods,
        Drinks,
        Cocktail,
        Key = 40,
        Weapon = 50,
        Essence
    }

    [InfoBox("If DisplayName is empty, it will use ID's")] [SerializeField] private string _displayName = "";
    [TextArea(3,5)] [SerializeField] private string _description = "";
    [SerializeField] private Sprite itemSprite;
    public Category category;
    public SubiconCategory subCategory;
    [ShowIf("category", Category.Weapon)] public WeaponItem attachedWeapon;
    [ShowIf(nameof(IsABook))] public Item_ReadableBook readableBook;
    public bool isGenericItem = false;
    public int value = 10;
    [HideIf("isGenericItem", true)] [ShowIf("subCategory", SubiconCategory.Essence)] public bool IS_REACTANT = false;
    [ShowIf("subCategory", SubiconCategory.Essence)] [ShowIf("IS_REACTANT", true)] [Range(1f, 100)] public float Reactant_ReduceAlcohol = 30;
    [ShowIf("subCategory", SubiconCategory.Essence)] [ShowIf("IS_REACTANT", true)] [Range(1f, 1000)] public float Reactant_BonusEfficiency = 20;
    [ShowIf("isGenericItem", true)] public bool GENERIC_KTHANID_SERUM = false;
    [ShowIf("isGenericItem", true)] public bool GENERIC_ESSENCE_POTION = false;
    [HideIf("isGenericItem", true)] [ShowIf("category", Category.Consumables)] public bool isKillerPill = false;
    [HideIf("isGenericItem", true)] [ShowIf("category", Category.Consumables)] public bool isInstantDashRefill = false;
    [HideIf("isGenericItem", true)] public bool isTriggerTrivia = false;
    [HideIf("isGenericItem", true)] [ShowIf("isTriggerTrivia", true)] public Trivia trivia;
    [HideIf("isGenericItem", true)] [ShowIf("category", Category.Consumables)] public bool isTriggerFlag = false;
    [HideIf("isGenericItem", true)] [ShowIf("isTriggerFlag", true)] public GlobalFlagSO flag;
    [HideIf("isGenericItem", true)] [ShowIf("isTriggerFlag", true)] [Range(1,100)] public int runLastFlag = 1;
    [HideIf("isGenericItem", true)] [ShowIf("category", Category.Consumables)] public float consume_HealAmount = 10;
    [HideIf("isGenericItem", true)] [ShowIf("category", Category.Consumables)] public float consume_AlcoholAmount = 0;
    [HideIf("isGenericItem", true)] [ShowIf("category", Category.Consumables)] public BaseStatusEffectObject statusEffect;
    [HideIf("isGenericItem", true)] [ShowIf("category", Category.Consumables)] public List<BaseStatusEffectObject> statusEffectToRemove;
    [HideIf("isGenericItem", true)] [ShowIf("statusEffect", true)] public float statusEffectTime = 5;
    [HideIf("isGenericItem", true)] [ShowIf("category", Category.Consumables)] [Range(0.1f,30f)] public float consume_HealTime = 5; //this is different from speed (higher heal time means longer heal time)

    public string Description { get => _description;  }

    public bool IsABook => subCategory == SubiconCategory.FictionBook || subCategory == SubiconCategory.Book;
    public bool IsAlcoholic => subCategory == SubiconCategory.Alcohol || subCategory == SubiconCategory.Cocktail;
    public bool IsReadableText => subCategory == SubiconCategory.FictionBook || subCategory == SubiconCategory.Book || subCategory == SubiconCategory.Notes;
    public bool IsMaterials => subCategory == SubiconCategory.Default || subCategory == SubiconCategory.Material || subCategory == SubiconCategory.Essence;

    public bool IsReadable()
    {
        if (IsABook == false)
            return false;

        return readableBook != null;
    }

    public string GetID()
    {
        return name;
    }

    public Sprite GetSprite()
    {
        return itemSprite;
    }

    public Sprite GetSubcategorySprite()
    {
        var subcategory = Hypatios.Assets.GetSubcategoryItemIcon(subCategory);

        return subcategory.sprite;
    }


    public bool CheckMatchingTags(string input)
    {
        if (subCategory.ToString().ToLower().Contains(input))
        {
            return true;
        }

        List<string> additionalTags = new List<string>();


        if (subCategory == SubiconCategory.Meds)
        {
            additionalTags.Add("drugs");
            additionalTags.Add("medicines");
        }
        if (subCategory == SubiconCategory.Notes)
        {
            additionalTags.Add("papers");
        }

        foreach (var cTag in additionalTags)
        {
            if (cTag.ToLower().Contains(input))
                return true;
        }

        return false;
    }

    public string GetDisplayText()
    {
        if (_displayName == "")
        {
            return name;
        }
        else
        {
            return _displayName;
        }
    }

    [FoldoutGroup("Debug")]
    [Button("Add item to Player")]
    public HypatiosSave.ItemDataSave AddItem()
    {
        return Hypatios.Player.Inventory.AddItem(this);
    }
}
