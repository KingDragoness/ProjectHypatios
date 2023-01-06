using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Sirenix.OdinInspector;

public class WeaponSlotButton : MonoBehaviour
{
    public PlayerRPGUI rpgUI;
    public Text WeaponName_Label;
    public Image WeaponIcon;
    public int equipIndex = 0;

    private Selectable selectable;
    private TooltipTrigger tooltipTrigger;

    private void OnEnable()
    {
        selectable = GetComponent<Selectable>();
        tooltipTrigger = GetComponent<TooltipTrigger>();
        RefreshUI();  
    }


    public void HighlightButton()
    {
        rpgUI.HighlightWeaponSlot(this);
    }


    public void DehighlightButton()
    {
        rpgUI.DehighlightPerk();
        Hypatios.UI.CloseAllTooltip();

    }

    public void ClickButton()
    {
        //deequip
        rpgUI.DeequipWeapon(this);
    }

    public void RefreshUI()
    {

        for (int x = 0; x < Hypatios.Player.Weapon.CurrentlyHeldWeapons.Count; x++)
        {
            var Weapon = Hypatios.Player.Weapon.CurrentlyHeldWeapons[x];
            var WeaponClass = Hypatios.Assets.GetWeapon(Weapon.weaponName);
            var itemClass = Hypatios.Assets.GetItemByWeapon(Weapon.weaponName);

            if (x != equipIndex)
            {
                WeaponName_Label.text = "";
                WeaponIcon.gameObject.SetActive(false);
                selectable.interactable = false;
                tooltipTrigger.enabled = false;
                continue;
            }
            else
            {
                WeaponName_Label.text = $"{itemClass.GetDisplayText()}";
                WeaponIcon.sprite = WeaponClass.weaponIcon;
                WeaponIcon.gameObject.SetActive(true);
                selectable.interactable = true;
                tooltipTrigger.enabled = true;

                break;
            }
        }
    }

}
