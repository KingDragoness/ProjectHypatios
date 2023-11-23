using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class ChargeStationUI : MonoBehaviour
{


    [TextArea(3, 5)]
    public string helperString;

    public Text soulPoint_Text;
    public Text tooltip_Text;
    public List<WeaponSectionButtonUI> allAmmoSections = new List<WeaponSectionButtonUI>();
    public List<WeaponSectionButtonUI> allWeaponSections = new List<WeaponSectionButtonUI>();

    private WeaponManager weaponManager;
    private ShopScript currentShopScript;
    private bool StartExecuted = false;

    public ShopScript CurrentShopScript { get => currentShopScript; set => currentShopScript = value; }

    private void Awake()
    {
        weaponManager = Hypatios.Player.Weapon;
    }

    private void Start()
    {
        StartExecuted = true;
    }

    private void OnEnable()
    {
        if (Hypatios.Game.everUsed_WeaponShop == false)
        {
            Hypatios.Game.RuntimeTutorialHelp(Hypatios.CodexList.ChargeStation);
            //MainGameHUDScript.Instance.ShowPromptUI("CHARGE STATION", helperString, false) ;
            Hypatios.Game.everUsed_WeaponShop = true;
        }

        if (currentShopScript == null)
        {
            currentShopScript = FindObjectOfType<ShopScript>();
            currentShopScript.OpenShop();
            currentShopScript.RefreshWeaponModels();
        }

        ShowTooltip(" ");
        RefreshUI();
    }

    [FoldoutGroup("Debug")] [Button("Buy slot 1")]
    public void BuyWeapon1()
    {
        BuyThis(allWeaponSections[0]);
    }

    [FoldoutGroup("Debug")]
    [Button("Buy slot 2")]
    public void BuyWeapon2()
    {
        BuyThis(allWeaponSections[1]);
    }


    [FoldoutGroup("Debug")]
    [Button("Buy slot 3")]
    public void BuyWeapon3()
    {
        BuyThis(allWeaponSections[2]);
    }


    public void SetShopScript(ShopScript _shop)
    {
        currentShopScript = _shop;
    }

    public void ShowTooltip(string s)
    {
        tooltip_Text.text = s;
    }

    public void RefreshUI()
    {
        soulPoint_Text.text = Hypatios.Game.SoulPoint.ToString();

        int index = 0;
        foreach (var WeaponSection in allAmmoSections)
        {

            WeaponSection.Refresh();

            index++;

        }

        foreach (var WeaponSection in allWeaponSections)
        {

            WeaponSection.Refresh();

            index++;

        }

        currentShopScript.RefreshWeaponModels();
    }

    public void BuyThis(WeaponSectionButtonUI weaponSection)
    {
        var itemData = currentShopScript.storage.allItemDatas[weaponSection.index];
        var itemClass = Hypatios.Assets.GetItem(currentShopScript.storage.allItemDatas[weaponSection.index].ID);
        var weaponClass = itemClass.attachedWeapon;
        var gunScript = Hypatios.Player.Weapon.GetGunScript(weaponClass.nameWeapon);

        if (gunScript != null)
        {
            ShowTooltip("Weapon already exists! Unequip the weapon first to be able to buy!");
            MainGameHUDScript.Instance.audio_Error.Play();
            return;
        }

        if (Hypatios.Player.Weapon.CurrentlyHeldWeapons.Count > 3)
        {
            ShowTooltip("Too many weapons in the hotbar! Unequip the weapon first to be able to buy!");
            MainGameHUDScript.Instance.audio_Error.Play();
            return;
        }


        if (Hypatios.Game.SoulPoint < weaponClass.buy_SoulPrice)
        {
            ShowTooltip("Not enough souls!");
            Debug.Log("Insufficient souls!");
            MainGameHUDScript.Instance.audio_Error.Play();
            return;
        }

        var gunScript1 = Hypatios.Player.Weapon.AddWeapon(weaponClass.nameWeapon);
        if (gunScript1 == null)
        {
            Debug.LogError("Something bad happened!");
            ConsoleCommand.Instance.SendConsoleMessage("Something bad happened!");
            return;
        }

        //gunScript1.totalAmmo = 0;


        Hypatios.Game.SoulPoint -= weaponClass.buy_SoulPrice;
        CurrentShopScript.storage.allItemDatas.Remove(itemData);
        MainGameHUDScript.Instance.audio_PurchaseReward.Play();
        RefreshUI();
    }

    public void BuyThisAmmo(WeaponSectionButtonUI weaponSection)
    {
        var weapon = Hypatios.Player.Weapon.CurrentlyHeldWeapons[weaponSection.index];
        var weaponClass = Hypatios.Assets.GetWeapon(weapon.weaponName);

        if (Hypatios.Game.SoulPoint < weaponClass.buy_AmmoSoulPrice)
        {
            ShowTooltip("Not enough souls!");
            Debug.Log("Insufficient souls!");
            MainGameHUDScript.Instance.audio_Error.Play();
            return;

        }

        weapon.totalAmmo += weaponClass.buy_AmmoAmount;
        Hypatios.Game.SoulPoint -= weaponClass.buy_AmmoSoulPrice;
        MainGameHUDScript.Instance.audio_PurchaseReward.Play();
        RefreshUI();
        ShowTooltip($"Bought x{weaponClass.buy_AmmoAmount} ammo for {weaponClass.buy_AmmoSoulPrice} souls! Thank you for purchasing!");

    }
}
