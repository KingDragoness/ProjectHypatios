using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAddItemScript : MonoBehaviour
{

    public ItemInventory item;
    public int count = 1;
    public bool displayPrompt = false;

    public void ExecuteFunction()
    {
        Hypatios.Player.Inventory.AddItem(item, count);
        MainGameHUDScript.Instance.lootItemUI.NotifyItemLoot(item, count);
        if (displayPrompt) DeadDialogue.PromptNotifyMessage_Mod($"Added {item.GetDisplayText()} ({count})", 3.5f);
    }

}
