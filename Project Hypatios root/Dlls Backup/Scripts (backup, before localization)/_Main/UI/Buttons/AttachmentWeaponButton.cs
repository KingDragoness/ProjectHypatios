using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class AttachmentWeaponButton : MonoBehaviour
{

    public PlayerRPGUI rpgUI;
    public TooltipTrigger tooltipTrigger;
    public string weaponName = "Rifle";
    public string attachmentID = "IonCapacitor";
    public Text label;

    public void Refresh()
    {
        var weaponClass = Hypatios.Assets.GetWeapon(weaponName);
        var itemClass = Hypatios.Assets.GetItem(weaponName);
        var attachment = weaponClass.GetAttachmentWeaponMod(attachmentID);

        if (attachment == null)
        {
            label.text = "ATTACHMENT NOT FOUND";
            tooltipTrigger.enabled = false;
            return;
        }
        tooltipTrigger.enabled = true;
        label.text = attachment.Name;
    }

    public void TriggerTooltip()
    {
        rpgUI.HighlightAttachment(this);
    }

}
