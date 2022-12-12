using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAddItemScript : MonoBehaviour
{

    public ItemInventory item;
    public int count = 1;

    public void ExecuteFunction()
    {
        Hypatios.Player.Inventory.AddItem(item, count);
    }

}
