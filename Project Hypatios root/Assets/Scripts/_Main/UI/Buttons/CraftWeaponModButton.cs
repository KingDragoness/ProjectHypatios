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
    public Image attachIcon;
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
        var _spriteIcon = workstationUI.GetAttachIcon(weaponMod.slot);

        weaponMod_label.text = $"{weaponMod.Name}";
        ingredient_label.text = $"{weaponMod.GetRequirementText()}";

        if (_spriteIcon != null)
        {
            attachIcon.gameObject.SetActive(true);
            attachIcon.sprite = _spriteIcon;
        }
        else
        {
            attachIcon.gameObject.SetActive(false);
        }

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
