using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class CraftNewWeaponButton : MonoBehaviour
{
    public Text weaponName_label;
    public Text ingredient_label;
    public Image weaponIcon;
    public Button button;
    public WeaponItem weaponItem;
    private CraftingWorkstationUI workstationUI;
    private void OnEnable()
    {
        workstationUI = MainGameHUDScript.Instance.craftingUI;
    }

    public void Refresh()
    {
        var weaponClass = weaponItem;
        var itemClass = Hypatios.Assets.GetItemByWeapon(weaponClass.nameWeapon);

        weaponName_label.text = $"{itemClass.GetDisplayText()}";
        weaponIcon.sprite = weaponClass.weaponIcon;
        ingredient_label.text = $"{weaponClass.GetRequirementText()}";

        if (Hypatios.Player.Weapon.CurrentlyHeldWeapons.Count >= 4 | 
            Hypatios.Player.Weapon.GetWeaponScript(weaponClass.nameWeapon) != null)
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
        workstationUI.CraftNewWeapon(this);
    }

    public void Highlight()
    {
        workstationUI.HighlightWeapon(this);

    }
}
