using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParadoxSectionButtonUI : MonoBehaviour
{
    public Text paradoxName_Text;
    public Text paradoxPrice_Text;
    public Button buttonBuy;
    public ParadoxShopUI parentUI;
    public ParadoxLevelScript attachedParadox;

    public void Hover()
    {
        parentUI.HoverThis(this);
    }

    public void HoverBuyButton()
    {
        parentUI.ShowTooltip($"Buy {attachedParadox.paradoxName} for {attachedParadox.soulPrice} souls");
    }

    public void Preview()
    {
        parentUI.TogglePreview();
    }

    public void Dehover()
    {
        parentUI.Unpreview();
    }

    public void AttemptBuy()
    {
        parentUI.AttemptBuy(this);
    }
}
