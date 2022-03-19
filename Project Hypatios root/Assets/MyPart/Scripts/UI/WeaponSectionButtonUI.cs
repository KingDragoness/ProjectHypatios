using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSectionButtonUI : MonoBehaviour
{



    public Text ammoLeft_Text;
    public Button buyWeapon_Button;
    public Button buyAmmo_Button;
    public string weaponID = "Shotgun";
    public int PurchaseAmmoAmount = 6;
    public int PurchaseAmmoPrice = 2;
    public int BuyPrice = 5;
    public List<UpgradeButtonUI> allUpgradeButtons = new List<UpgradeButtonUI>();

    private ChargeStationUI ChargeStationUI;

    private void Awake()
    {
        ChargeStationUI = MainGameHUDScript.Instance.chargeStationUI;
    }

    public void AttemptBuy()
    {
        MainGameHUDScript.Instance.chargeStationUI.BuyThis(this);
    }

    public void AttemptBuyAmmo()
    {
        MainGameHUDScript.Instance.chargeStationUI.BuyThisAmmo(this);
    }

    public int GetCostPrice(int level, string weaponName = "", UpgradeWeaponType upgradeType = UpgradeWeaponType.Damage)
    {
        int x = 0;

        if (weaponName == "Pistol")
        {
            x = ((level+ 1) * 2) + 6;
        }
        else
        {
            x = ((level + 1) * 6) + 5;
        }

        if (upgradeType == UpgradeWeaponType.Damage)
        {
            x = Mathf.RoundToInt(x * 1.1f);
        }
        else if (upgradeType == UpgradeWeaponType.Cooldown)
        {
            x = Mathf.RoundToInt(x * 0.6f);
        }
        else if (upgradeType == UpgradeWeaponType.MagazineSize)
        {
            x = Mathf.RoundToInt(x * 0.4f);
        }

        return x;
    }

    //Upgrade soul amount (level x 15 + 5) =
    //lv 1 (1 x 15 + 5) = 20

    public void AttemptUpgrade(UpgradeButtonUI upgradeButtonUI)
    {
         UpgradeWeaponType upgradeType = upgradeButtonUI.upgradeType;
        int currentLevel = 0;
        string limitLevel = "5";
        var statThisWeapon = FPSMainScript.instance.GetWeaponSave(weaponID);
        var weaponItem = weaponManager.Instance.GetWeaponItemData(weaponID);

        if (statThisWeapon == null)
        {
            ChargeStationUI.ShowTooltip("Cannot upgrade weapon you have never used!");
            MainGameHUDScript.Instance.audio_Error.Play();
            return;
        }
        else
        {
            if (upgradeType == UpgradeWeaponType.Damage)
            {
                currentLevel = statThisWeapon.level_Damage;
            }
            else if (upgradeType == UpgradeWeaponType.Cooldown)
            {
                currentLevel = statThisWeapon.level_Cooldown;

            }
            else if (upgradeType == UpgradeWeaponType.MagazineSize)
            {
                currentLevel = statThisWeapon.level_MagazineSize;

            }
        }

        if (currentLevel >= 4)
        {
            ChargeStationUI.ShowTooltip("Cannot upgrade anymore, maxed out.");
            MainGameHUDScript.Instance.audio_Error.Play();
            return;
        }

        int soulCost = GetCostPrice(currentLevel, weaponID, upgradeType);

        if (FPSMainScript.instance.SoulPoint < soulCost)
        {
            ChargeStationUI.ShowTooltip("Not enough souls!");
            Debug.Log("Insufficient souls!");
            MainGameHUDScript.Instance.audio_Error.Play();
            return;
        }


        if (upgradeType == UpgradeWeaponType.Damage)
        {
            statThisWeapon.level_Damage++;
            currentLevel = statThisWeapon.level_Damage;
        }
        else if (upgradeType == UpgradeWeaponType.Cooldown)
        {
            statThisWeapon.level_Cooldown++;
            currentLevel = statThisWeapon.level_Cooldown;

        }
        else if (upgradeType == UpgradeWeaponType.MagazineSize)
        {
            statThisWeapon.level_MagazineSize++;
            currentLevel = statThisWeapon.level_MagazineSize;

        }

        FPSMainScript.instance.SoulPoint -= soulCost;
        ChargeStationUI.ShowTooltip($"Upgrade successful: {upgradeType} : [{currentLevel}/5]");
        MainGameHUDScript.Instance.audio_PurchaseReward.Play();
        ChargeStationUI.RefreshUI();
    }
}
