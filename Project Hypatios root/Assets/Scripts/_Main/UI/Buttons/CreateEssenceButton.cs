using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Sirenix.OdinInspector;

public class CreateEssenceButton : MonoBehaviour
{
    public enum Type
    {
        Inventory,
        Extractor,
        Result
    }


    public kThanidLabUI kThanidUI;
    public Type buttonType;
    public Text Name_label;
    public Text Count_label;
    public Image Subicon;

    [ShowIf("buttonType", Type.Result)] public ModifierEffectCategory ESSENCE_CATEGORY = ModifierEffectCategory.Nothing;
    [ShowIf("buttonType", Type.Result)] public string ESSENCE_STATUSEFFECT_GROUP = "";
    public int index = 0;

    public void Refresh()
    {
        if (buttonType == Type.Inventory)
        {
            var itemDat = Hypatios.Player.Inventory.allItemDatas[index];
            var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

            Name_label.text = Hypatios.RPG.GetItemName(itemClass, itemDat);
            Count_label.text = itemDat.count.ToString();
        }
        else if (buttonType == Type.Extractor)
        {
            var itemDat = kThanidUI.ExtractorInventory.allItemDatas[index];
            var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

            Name_label.text = Hypatios.RPG.GetItemName(itemClass, itemDat);
            Count_label.text = itemDat.count.ToString();
        }
        else if (buttonType == Type.Result)
        {
            if (ESSENCE_CATEGORY != ModifierEffectCategory.Nothing)
            {
                Name_label.text = Hypatios.RPG.GetEssenceName(ESSENCE_CATEGORY);
            }
            else
            {
                Name_label.text = Hypatios.RPG.GetEssenceName(ESSENCE_STATUSEFFECT_GROUP);
            }
        }
    }

    public bool IsCraftRequirementMet()
    {
        if (ESSENCE_CATEGORY != ModifierEffectCategory.Nothing)
        {
            var modifier = Hypatios.Assets.GetStatusEffect(ESSENCE_CATEGORY);

            foreach (var recipe in modifier.requirementCrafting)
            {
                if (kThanidUI.ExtractorInventory.Count(recipe.inventory.GetID()) < recipe.count)
                {
                    return false;
                }
            }
        }
        else
        {
            var ailment = Hypatios.Assets.GetStatusEffect(ESSENCE_STATUSEFFECT_GROUP);

            foreach (var recipe in ailment.requirementCrafting)
            {
                if (kThanidUI.ExtractorInventory.Count(recipe.inventory.GetID()) < recipe.count)
                {
                    return false;
                }
            }
        }

  

        return true;
    }


    public void ClickButton()
    {
        if (buttonType == Type.Inventory)
        {
            kThanidUI.TransferToExtractor(this);
        }
        else if (buttonType == Type.Extractor)
        {
            kThanidUI.TransferToMyInventory(this);
        }
        else if (buttonType == Type.Result)
        {
            kThanidUI.CraftEssence(this);
        }
    }
}
