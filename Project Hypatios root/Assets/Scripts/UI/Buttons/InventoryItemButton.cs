using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Sirenix.OdinInspector;

public class InventoryItemButton : MonoBehaviour
{

    public PlayerRPGUI rpgUI;
    public Text Name_label;
    public Text Count_label;
    public int index = 0;

    public void Refresh()
    {
        var itemDat = Hypatios.Player.Inventory.allItemDatas[index];
        var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

        Name_label.text = itemClass.GetDisplayText();
        Count_label.text = itemDat.count.ToString();
    }

    public void HighlightButton()
    {
        rpgUI.HighlightItem(this);
    }

    public void DehighlightButton()
    {
        Hypatios.UI.CloseAllTooltip();

    }

    public void ClickButton()
    {
        rpgUI.UseItem(this);
        var button = GetComponent<Button>();
        var selectable1 = button.FindSelectableOnDown();
        selectable1.Select();
    }

    public ItemInventory GetItemInventory()
    {
        var itemDat = Hypatios.Player.Inventory.allItemDatas[index];
        var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

        return itemClass;
    }

    public HypatiosSave.ItemDataSave GetItemData()
    {
        var itemDat = Hypatios.Player.Inventory.allItemDatas[index];

        return itemDat;
    }

}
