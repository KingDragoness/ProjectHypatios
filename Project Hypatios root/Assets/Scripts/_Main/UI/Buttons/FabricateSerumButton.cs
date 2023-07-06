using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Sirenix.OdinInspector;

public class FabricateSerumButton : MonoBehaviour
{
    public enum Type
    {
        Inventory,
        Fabricator
    }

    public kThanidLabUI kThanidUI;
    public Type buttonType;
    public Text Name_label;
    public Text Count_label;
    public Image Subicon;
    public GameObject antiPotionButton;
    public GameObject antiPotionIcon;
    public int index = 0;
    public bool isAntiPotion = false;

    public void Refresh()
    {
        if (buttonType == Type.Inventory)
        {
            var itemDat = Hypatios.Player.Inventory.allItemDatas[index];
            var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

            Name_label.text = Hypatios.RPG.GetItemName(itemClass, itemDat);
            Count_label.text = itemDat.count.ToString();
        }
        else if (buttonType == Type.Fabricator)
        {
            var itemDat = kThanidUI.FabricatorInventory.allItemDatas[index];
            var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

            if (IsValidItemForAntiPotion())
            {
                antiPotionButton.gameObject.SetActive(true);
            }
            else
            {
                isAntiPotion = false;
                antiPotionButton.gameObject.SetActive(false);
            }

            if (isAntiPotion)
            {
                antiPotionIcon.gameObject.SetActive(true);
            }
            else
            {
                antiPotionIcon.gameObject.SetActive(false);
            }

            Name_label.text = Hypatios.RPG.GetItemName(itemClass, itemDat);
            Count_label.text = itemDat.count.ToString();
        }
      
    }

    public bool IsValidItemForAntiPotion()
    {
        var itemDat = kThanidUI.FabricatorInventory.allItemDatas[index];
        var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

        if (itemClass.isGenericItem && itemClass.GENERIC_ESSENCE_POTION && itemDat.ESSENCE_TYPE == HypatiosSave.EssenceType.Modifier)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void ClickButton()
    {
        if (buttonType == Type.Inventory)
        {
            kThanidUI.TransferToFabricator(this);
        }
        else if (buttonType == Type.Fabricator)
        {
            kThanidUI.TransferToMyInventory(this);
        }
    }

    public void AntiPotionToggle()
    {
        kThanidUI.ToggleAntiPotion(this);
    }
}
