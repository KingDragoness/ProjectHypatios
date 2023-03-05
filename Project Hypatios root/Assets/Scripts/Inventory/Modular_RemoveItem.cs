using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modular_RemoveItem : MonoBehaviour
{

    public ItemInventory item;
    [Range(1, 99)] public int amount = 1;

    public void RemoveItem()
    {
        Hypatios.Player.Inventory.RemoveItem(item.GetID(), amount);
    }

    public void AddItem()
    {
        Hypatios.Player.Inventory.AddItem(item, amount);
    }

}
