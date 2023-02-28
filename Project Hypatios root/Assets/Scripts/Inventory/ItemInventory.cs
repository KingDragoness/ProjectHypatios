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
        None = 999 //For inventory filter
    }

    [InfoBox("If DisplayName is empty, it will use ID's")] [SerializeField] private string _displayName = "";
    [TextArea(3,5)] [SerializeField] private string _description = "";
    public Category category;
    [ShowIf("category", Category.Weapon)] public WeaponItem attachedWeapon;
    [ShowIf("category", Category.Consumables)] public bool isKillerPill = false;
    [ShowIf("category", Category.Consumables)] public bool isInstantDashRefill = false;
    [ShowIf("category", Category.Consumables)] public float consume_HealAmount = 10;
    [ShowIf("category", Category.Consumables)] public float consume_AlcoholAmount = 0;
    [ShowIf("category", Category.Consumables)] [Range(0.1f,30f)] public float consume_HealTime = 5; //this is different from speed (higher heal time means longer heal time)

    public string Description { get => _description;  }

    public string GetID()
    {
        return name;
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
}
