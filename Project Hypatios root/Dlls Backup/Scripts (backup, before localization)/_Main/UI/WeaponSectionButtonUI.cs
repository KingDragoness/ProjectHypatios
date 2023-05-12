using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//only buy ammos

public class WeaponSectionButtonUI : MonoBehaviour
{

    public enum ButtonType
    {
        Ammo,
        Weapon
    }

    public Text weaponName_Label;
    public Text ammoLeft_Text;
    public Image weaponIcon;
    public Image ammoIcon;
    public Sprite soulSprite;
    public Button mainButton;
    public ButtonType buttonType = ButtonType.Ammo;
    public int index = 0;

    private ChargeStationUI ChargeStationUI;

    private void Awake()
    {
        ChargeStationUI = MainGameHUDScript.Instance.chargeStationUI;
    }

    public void Highlight()
    {
        if (buttonType == ButtonType.Ammo)
        {
            if (index >= Hypatios.Player.Weapon.CurrentlyHeldWeapons.Count)
            { return; }

            var weapon = Hypatios.Player.Weapon.CurrentlyHeldWeapons[index];
            var weaponClass = Hypatios.Assets.GetWeapon(weapon.weaponName);
            var itemClass = Hypatios.Assets.GetItemByWeapon(weapon.weaponName);

            ChargeStationUI.ShowTooltip($"Buy x{weaponClass.buy_AmmoAmount} ammo for {weaponClass.buy_AmmoSoulPrice} souls.");
        }
        else
        {
            if (index >= ChargeStationUI.CurrentShopScript.storage.allItemDatas.Count)
            { return; }

            var itemData = ChargeStationUI.CurrentShopScript.storage.allItemDatas[index];
            var itemClass = Hypatios.Assets.GetItem(itemData.ID);
            var weaponClass = itemClass.attachedWeapon;

            ChargeStationUI.ShowTooltip($"Buy {itemClass.GetDisplayText()} for {weaponClass.buy_SoulPrice} souls.");
            ChargeStationUI.CurrentShopScript.currentHighlight = index;
        }
    }

    public void Dehighlight()
    {
        ChargeStationUI.CurrentShopScript.currentHighlight = -1;

    }

    public void AttemptBuyAmmo()
    {
        ChargeStationUI.BuyThisAmmo(this);
    }

    public void AttemptBuyWeapon()
    {
        ChargeStationUI.BuyThis(this);
    }

    public void Refresh()
    {
        if (buttonType == ButtonType.Ammo)
        {
            if (index >= Hypatios.Player.Weapon.CurrentlyHeldWeapons.Count)
            {
                //set null
                weaponIcon.sprite = null;
                mainButton.interactable = false;
                ammoIcon.sprite = null;
                weaponName_Label.text = "";
                ammoLeft_Text.text = $"~";
            }
            else
            {
                var weapon = Hypatios.Player.Weapon.CurrentlyHeldWeapons[index];
                var weaponClass = Hypatios.Assets.GetWeapon(weapon.weaponName);
                var itemClass = Hypatios.Assets.GetItemByWeapon(weapon.weaponName);

                weaponIcon.sprite = weaponClass.weaponIcon;
                mainButton.interactable = true;
                ammoIcon.sprite = weaponClass.ammoSpriteIcon;
                weaponName_Label.text = $"{itemClass.GetDisplayText().ToUpper()}";
                ammoLeft_Text.text = $"x{weapon.totalAmmo}";
            }
        }
        else
        {
            if (ChargeStationUI == null)
                ChargeStationUI = MainGameHUDScript.Instance.chargeStationUI;

            if (index >= ChargeStationUI.CurrentShopScript.storage.allItemDatas.Count)
            {
                //set null
                weaponIcon.sprite = null;
                mainButton.interactable = false;
                ammoIcon.sprite = null;
                weaponName_Label.text = "";
                ammoLeft_Text.text = $"~";
            }
            else
            {
                var itemData = ChargeStationUI.CurrentShopScript.storage.allItemDatas[index];
                var itemClass = Hypatios.Assets.GetItem(itemData.ID);
                var weaponClass = itemClass.attachedWeapon;

                weaponIcon.sprite = weaponClass.weaponIcon;
                mainButton.interactable = true;
                ammoIcon.sprite = soulSprite;
                weaponName_Label.text = $"{itemClass.GetDisplayText().ToUpper()}";
                ammoLeft_Text.text = $"x{weaponClass.buy_SoulPrice}";
            }
        }
    }


}
