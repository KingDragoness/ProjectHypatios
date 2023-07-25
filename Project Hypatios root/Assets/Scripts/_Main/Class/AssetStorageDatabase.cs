using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class AssetStorageDatabase : MonoBehaviour
{
    [System.Serializable]
    public class SubiconIdentifier
    {
        public ItemInventory.SubiconCategory categoryIcon;
        public Sprite sprite;
    }

    public List<Trivia> AllTrivias = new List<Trivia>();
    public List<BaseModifierEffectObject> AllModifierEffects;
    public List<BaseStatusEffectObject> AllStatusEffects;
    public List<WeaponItem> Weapons = new List<WeaponItem>();
    public List<BasePerk> AllBasePerks;
    public List<ItemInventory> AllItems;
    public List<BaseStatValue> AllStatEntries;
    public List<ChamberLevel> AllChamberLevels;
    public List<SubiconIdentifier> AllSubIcons = new List<SubiconIdentifier>();

    [FoldoutGroup("Auxillary")] public List<StockExchange_ProfileObject> AllStockCompanies = new List<StockExchange_ProfileObject>();

    private void Awake()
    {
        RefreshDatabase();
    }

    [Button("Refresh Database")]
    public void RefreshDatabase()
    {
        var _trivias = Resources.LoadAll("", typeof(Trivia)).Cast<Trivia>().ToList();
        var _modifiers = Resources.LoadAll("", typeof(BaseModifierEffectObject)).Cast<BaseModifierEffectObject>().ToList();
        var _statusEffects = Resources.LoadAll("", typeof(BaseStatusEffectObject)).Cast<BaseStatusEffectObject>().ToList();
        var _weapons = Resources.LoadAll("", typeof(WeaponItem)).Cast<WeaponItem>().ToList(); 
        var _basePerks = Resources.LoadAll("", typeof(BasePerk)).Cast<BasePerk>().ToList(); 
        var _itemInventory = Resources.LoadAll("", typeof(ItemInventory)).Cast<ItemInventory>().ToList();
        var _statistics = Resources.LoadAll("", typeof(BaseStatValue)).Cast<BaseStatValue>().ToList();
        var _levels = Resources.LoadAll("", typeof(ChamberLevel)).Cast<ChamberLevel>().ToList();
        var _stockProfiles = Resources.LoadAll("", typeof(StockExchange_ProfileObject)).Cast<StockExchange_ProfileObject>().ToList();

        _trivias.RemoveAll(x => x.disableTrivia == true);
        AllTrivias = _trivias;
        AllModifierEffects = _modifiers;
        AllStatusEffects = _statusEffects;
        Weapons = _weapons;
        AllBasePerks = _basePerks;
        AllItems = _itemInventory;
        AllStatEntries = _statistics;
        AllChamberLevels = _levels;
        AllStockCompanies = _stockProfiles;
    }


    public StockExchange_ProfileObject GetCompanyProfile(string ID)
    {
        return AllStockCompanies.Find(x => x.indexID == ID);
    }
    public BaseModifierEffectObject GetStatusEffect(ModifierEffectCategory category)
    {
        return AllModifierEffects.Find(x => x.category == category);
    }
    public BaseStatusEffectObject GetStatusEffect(string ID)
    {
        return AllStatusEffects.Find(x => x.GetID() == ID);
    }
    public SubiconIdentifier GetSubcategoryItemIcon(ItemInventory.SubiconCategory category1)
    {
        return AllSubIcons.Find(x => x.categoryIcon == category1);
    }

    public WeaponItem GetWeapon(string ID)
    {
        return Weapons.Find(x => x.nameWeapon == ID);
    }

    public Trivia GetTrivia(string ID)
    {
        return AllTrivias.Find(x => x.ID == ID);
    }

    public ItemInventory GetItem(string ID)
    {
        return AllItems.Find(x => x.GetID() == ID);
    }

    public List<ItemInventory> GetItemsByCategory(ItemInventory.Category _category)
    {
        return AllItems.FindAll(x => x.category == _category);
    }

    public BaseStatValue GetStatEntry(string ID)
    {
        return AllStatEntries.Find(x => x.GetID() == ID);
    }

    public ChamberLevel GetLevel(string ID)
    {
        return AllChamberLevels.Find(x => x.GetID() == ID);
    }

    public ItemInventory GetItemByWeapon(string ID)
    {
        foreach(var item in AllItems)
        {
            if (item.attachedWeapon == null) continue;
            if (item.category != ItemInventory.Category.Weapon) continue;
            if (item.attachedWeapon.nameWeapon == ID) return item;
        }

        return null;
    }
}
