using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public class CraftWeaponModButton : MonoBehaviour
{
    public Text weaponMod_label;
    public Text ingredient_label;
    public Button button;
    public string weaponModID = "Receiver";
    public string weaponItemName = "IonBlaster";

    private CraftingWorkstationUI workstationUI;
    private void OnEnable()
    {
        workstationUI = MainGameHUDScript.Instance.craftingUI;
    }

    public WeaponItem.Attachment GetWeaponMod()
    {
        var itemClass = Hypatios.Assets.GetItemByWeapon(weaponItemName);
        var weaponClass = itemClass.attachedWeapon;
        return weaponClass.GetAttachmentWeaponMod(weaponModID);
    }

    public void Refresh()
    {
        var itemClass = Hypatios.Assets.GetItemByWeapon(weaponItemName);
        var weaponData = Hypatios.Game.GetWeaponSave(weaponItemName);
        var weaponClass = itemClass.attachedWeapon;
        var weaponMod = weaponClass.GetAttachmentWeaponMod(weaponModID);

        weaponMod_label.text = $"{weaponMod.Name} [{weaponMod.slot.ToString()}]";
        ingredient_label.text = $"{weaponMod.GetRequirementText()}";

        if (weaponClass.IsAttachmentSlotOccupied(weaponModID, weaponData.allAttachments))
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }

    public void Click()
    {
        workstationUI.CraftWeaponMod(this);
    }

    public void Highlight()
    {
        workstationUI.HighlightWeaponMod(this);

    }
}
