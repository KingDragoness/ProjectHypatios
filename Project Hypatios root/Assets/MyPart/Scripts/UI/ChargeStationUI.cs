using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeStationUI : MonoBehaviour
{

    [TextArea(3, 5)]
    public string helperString;

    public Text soulPoint_Text;
    public Text tooltip_Text;
    public List<WeaponSectionButtonUI> allWeaponSections = new List<WeaponSectionButtonUI>();

    private weaponManager weaponManager;
    private bool StartExecuted = false;

    private void Awake()
    {
        weaponManager = FindObjectOfType<weaponManager>();
    }

    private void Start()
    {
        StartExecuted = true;
    }

    private void OnEnable()
    {
        if (FPSMainScript.instance.everUsed_WeaponShop == false)
        {
            MainGameHUDScript.Instance.ShowPromptUI("CHARGE STATION", helperString, false) ;
            FPSMainScript.instance.everUsed_WeaponShop = true;
        }

        RefreshUI();
    }

    public void ShowTooltip(string s)
    {
        tooltip_Text.text = s;
    }

    public void RefreshUI()
    {
        ShowTooltip(" ");

        soulPoint_Text.text = FPSMainScript.instance.SoulPoint.ToString();

        foreach (var WeaponSection in allWeaponSections)
        {
            WeaponSection.ammoLeft_Text.text = $"-";
            WeaponSection.buyAmmo_Button.interactable = false;
            WeaponSection.buyWeapon_Button.interactable = true;

            var statThisWeapon = FPSMainScript.instance.GetWeaponSave(WeaponSection.weaponID);

            if (statThisWeapon == null)
            {
                continue;
            }

            foreach (var test1 in WeaponSection.allUpgradeButtons)
            {
                if (test1.upgradeType == UpgradeWeaponType.Damage)
                {
                    test1.levelText.text = (statThisWeapon.level_Damage + 1).ToString();
                }
                else if (test1.upgradeType == UpgradeWeaponType.MagazineSize)
                {
                    test1.levelText.text = (statThisWeapon.level_MagazineSize + 1).ToString();
                }
                else if (test1.upgradeType == UpgradeWeaponType.Cooldown)
                {
                    test1.levelText.text = (statThisWeapon.level_Cooldown + 1).ToString();
                }
            }
        }

        foreach (var weapon in weaponManager.Instance.CurrentlyHeldGuns)
        {
            print($"weapon {weapon.weaponName}");

            if (weapon == null)
            {
                continue;
            }

            var WeaponSection = allWeaponSections.Find(x => x.weaponID == weapon.weaponName);

            if (WeaponSection == null)
            {
                print("section1");
                continue;
            }

            var statThisWeapon = FPSMainScript.instance.GetWeaponSave(weapon.weaponName);


            WeaponSection.buyWeapon_Button.interactable = false;
            WeaponSection.buyAmmo_Button.interactable = true;
            WeaponSection.ammoLeft_Text.text = $"x{weapon.totalAmmo}";

            #region Upgrades
            //foreach(var test1 in WeaponSection.allUpgradeButtons)
            //{
            //    if (test1.upgradeType == UpgradeWeaponType.Damage)
            //    {
            //        test1.levelText.text = (statThisWeapon.level_Damage + 1).ToString();
            //    }
            //    else if (test1.upgradeType == UpgradeWeaponType.MagazineSize)
            //    {
            //        test1.levelText.text = (statThisWeapon.level_MagazineSize + 1).ToString();
            //    }
            //    else if (test1.upgradeType == UpgradeWeaponType.Cooldown)
            //    {
            //        test1.levelText.text = (statThisWeapon.level_Cooldown + 1).ToString();
            //    }
            //}
            #endregion

            weaponManager.SetWeaponSettings(weaponManager.gun);
        }
    }

    public void BuyThis(WeaponSectionButtonUI weaponSection)
    {
        var gunScript = weaponManager.Instance.GetGunScript(weaponSection.weaponID);

        if (gunScript != null)
        {
            ShowTooltip("Weapon already exists!");
            Debug.Log("Weapon cannot be bought! Already exist!");
            return;
        }

        if (FPSMainScript.instance.SoulPoint < weaponSection.BuyPrice)
        {
            ShowTooltip("Not enough souls!");
            Debug.Log("Insufficient souls!");
            MainGameHUDScript.Instance.audio_Error.Play();
            return;
        }

        FPSMainScript.instance.SoulPoint -= weaponSection.BuyPrice;
        var gunScript1 = weaponManager.Instance.AddWeapon(weaponSection.weaponID);
        gunScript1.totalAmmo = 0;
        MainGameHUDScript.Instance.audio_PurchaseReward.Play();
        RefreshUI();
    }

    public void BuyThisAmmo(WeaponSectionButtonUI weaponSection)
    {
        if (FPSMainScript.instance.SoulPoint < weaponSection.PurchaseAmmoPrice)
        {
            ShowTooltip("Not enough souls!");
            Debug.Log("Insufficient souls!");
            MainGameHUDScript.Instance.audio_Error.Play();
            return;

        }

        var weaponTarget = weaponManager.GetGunScript(weaponSection.weaponID);
        weaponTarget.totalAmmo += weaponSection.PurchaseAmmoAmount;
        FPSMainScript.instance.SoulPoint -= weaponSection.PurchaseAmmoPrice;
        MainGameHUDScript.Instance.audio_PurchaseReward.Play();
        RefreshUI();
 
    }
}
