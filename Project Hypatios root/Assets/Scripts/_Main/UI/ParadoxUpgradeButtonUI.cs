using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParadoxUpgradeButtonUI : MonoBehaviour
{
    public Text levelText;
    public Button buyButtonUpgrade;
    public Image icon;
    public ParadoxShopUI parentUI;
    public PlayerPerks perkType;

    public void UpgradeThisPerk()
    {
        parentUI.UpgradePerk(this);
    }

    public void HoverTooltip()
    {
        parentUI.TooltipHover_Perk(this);
    }
}
