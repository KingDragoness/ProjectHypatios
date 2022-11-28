using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UpgradeButtonUI : MonoBehaviour
{

    public Text levelText;
    public Button buyButtonUpgrade;
    public WeaponSectionButtonUI parentUI;
    public UpgradeWeaponType upgradeType;

    public void TooltipShowUpgrade()
    {
        string weaponTypeString = upgradeType.ToString();

        if (upgradeType == UpgradeWeaponType.MagazineSize)
        {
            weaponTypeString = "Magazine Size";
        }

        int currentLevel = 0;
        string limitLevel = "5";
        var statThisWeapon = Hypatios.Game.GetWeaponSave(parentUI.weaponID);
        var weaponItem = WeaponManager.Instance.GetWeaponItemData(parentUI.weaponID);

        if (statThisWeapon == null)
        {

        }
        else
        {
            if (upgradeType == UpgradeWeaponType.Damage)
            {
                currentLevel = statThisWeapon.level_Damage;
                var theList = weaponItem.levels_Damage;
                limitLevel = theList.Count.ToString();

            }
            else if (upgradeType == UpgradeWeaponType.Cooldown)
            {
                currentLevel = statThisWeapon.level_Cooldown;
                var theList = weaponItem.levels_Cooldown;
                limitLevel = theList.Count.ToString();

            }
            else if (upgradeType == UpgradeWeaponType.MagazineSize)
            {
                currentLevel = statThisWeapon.level_MagazineSize;
                var theList = weaponItem.levels_MagazineSize;
                limitLevel = theList.Count.ToString();

            }
        }

        MainGameHUDScript.Instance.chargeStationUI.ShowTooltip($"Upgrade component: {parentUI.weaponID} [{weaponTypeString}] [{currentLevel + 1}/{limitLevel}]" +
            $" [Soul: {parentUI.GetCostPrice(currentLevel, parentUI.weaponID, upgradeType)}]");
    }


    public void AttemptUpgrade()
    {
        parentUI.AttemptUpgrade(this);
    }
}
