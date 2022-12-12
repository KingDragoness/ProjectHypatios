﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Apple", menuName = "Hypatios/Item", order = 1)]

public class ItemInventory : ScriptableObject
{

    public enum Category
    {
        Normal,
        Consumables,
        Quest,
        Key,
        Weapon
    }

    [InfoBox("If DisplayName is empty, it will use ID's")]
    [SerializeField] private string _displayName = "";
    [TextArea(3,5)]
    [SerializeField] private string _description = "";
    public Category category;
    [ShowIf("category", Category.Weapon)]
    public WeaponItem attachedWeapon;

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