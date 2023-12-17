using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class WIRED_FactoryLine : MonoBehaviour
{

    public ItemInventory Item_EssenceBottle;
    public string text_noBottle;

    public void PutBottle()
    {
        HypatiosSave.ItemDataSave itemDat = Hypatios.Player.Inventory.SearchByID(Item_EssenceBottle.GetID());
        bool success = false;

        if (itemDat != null)
        {
            if (itemDat.isGenericItem && itemDat.ESSENCE_CATEGORY == ModifierEffectCategory.MaxHitpointBonus)
            {
                success = true;
            }
        }

        if (success == false)
        {
            Hypatios.UI.PromptNotifyMessage("You need [MAX HP] essence bottle.", 5f);
            return;
        }
    }

}
