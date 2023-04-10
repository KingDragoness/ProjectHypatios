using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class AssetStorageDatabase : MonoBehaviour
{

    public List<Trivia> AllTrivias = new List<Trivia>();
    public List<BaseStatusEffectObject> AllStatusEffects;
    public List<WeaponItem> Weapons = new List<WeaponItem>();
    public List<BasePerk> AllBasePerks;
    public List<ItemInventory> AllItems;
    public List<BaseStatValue> AllStatEntries;


    private void Awake()
    {
        RefreshDatabase();
    }

    [Button("Refresh Database")]
    public void RefreshDatabase()
    {
        var _trivias = Resources.LoadAll("", typeof(Trivia)).Cast<Trivia>().ToList();
        var _statusEffects = Resources.LoadAll("", typeof(BaseStatusEffectObject)).Cast<BaseStatusEffectObject>().ToList(); 
        var _weapons = Resources.LoadAll("", typeof(WeaponItem)).Cast<WeaponItem>().ToList(); 
        var _basePerks = Resources.LoadAll("", typeof(BasePerk)).Cast<BasePerk>().ToList(); 
        var _itemInventory = Resources.LoadAll("", typeof(ItemInventory)).Cast<ItemInventory>().ToList();
        var _statistics = Resources.LoadAll("", typeof(BaseStatValue)).Cast<BaseStatValue>().ToList();

        _trivias.RemoveAll(x => x.disableTrivia == true);
        AllTrivias = _trivias;
        AllStatusEffects = _statusEffects;
        Weapons = _weapons;
        AllBasePerks = _basePerks;
        AllItems = _itemInventory;
        AllStatEntries = _statistics;

    }

    public BaseStatusEffectObject GetStatusEffect(StatusEffectCategory category)
    {
        return AllStatusEffects.Find(x => x.category == category);
    }

    public WeaponItem GetWeapon(string ID)
    {
        return Weapons.Find(x => x.nameWeapon == ID);
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
