using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingWeaponSelectButton : MonoBehaviour
{
   
    public enum ButtonType
    {
        Weapon,
        WeaponMods
    }

    public ButtonType buttonType;
    public Text label;
    public int index = 0;

    private CraftingWorkstationUI workstationUI;

    private void OnEnable()
    {
        workstationUI = MainGameHUDScript.Instance.craftingUI;
    }

    public void Click()
    {
        if (buttonType == ButtonType.Weapon)
        {
            workstationUI.OpenWeapon();
        }
        else if (buttonType == ButtonType.WeaponMods)
        {
            workstationUI.OpenWeaponMod(this);
        }
    }

    public void Refresh()
    {
        if (index >= Hypatios.Player.Weapon.CurrentlyHeldWeapons.Count)
        {
            buttonType = ButtonType.Weapon;
        }
        else
        {
            buttonType = ButtonType.WeaponMods;

        }

        if (buttonType == ButtonType.Weapon)
        {
            label.text = "New Weapon";

        }
        else
        {
            var weapon = Hypatios.Player.Weapon.CurrentlyHeldWeapons[index];
            var weaponClass = Hypatios.Assets.GetWeapon(weapon.weaponName);
            var itemClass = Hypatios.Assets.GetItemByWeapon(weapon.weaponName);

            label.text = $"{itemClass.GetDisplayText().ToUpper()}";

        }
    }
}
