using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class FavItemButton : MonoBehaviour
{

    public FavoriteMenuUI favUI;
    public Text Name_label;
    public Text Count_label;
    public Image imageIcon;
    public int index = 0;

    public void Refresh()
    {
        var itemDat = Hypatios.Player.Inventory.allItemDatas[index];
        var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

        Name_label.text = Hypatios.RPG.GetItemName(itemClass, itemDat);
        Count_label.text = itemDat.count.ToString();


    }

    public void HighlightButton()
    {
        favUI.HighlightButton(this);
    }

    public void DehighlightButton()
    {
        Hypatios.UI.CloseAllTooltip();

    }

    public void ClickButton()
    {
        favUI.UseItem(this);
        var button = GetComponent<Button>();
        //var selectable1 = button.FindSelectableOnDown();
        //selectable1.Select();
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
