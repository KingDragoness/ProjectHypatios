using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

//You can modify this
public class CharStatButton : MonoBehaviour
{

    public PlayerRPGUI playerRPGUI;
    public ModifierEffectCategory category1;
    public Text statLabel;


    public void HighlightPerk()
    {
        playerRPGUI.HighlightStat(this);
    }

    public void DehighlightPerk()
    {
        playerRPGUI.DehighlightStat();
        Hypatios.UI.CloseAllTooltip();
    }


    private void OnEnable()
    {
        PerkInitialize(category1);
    }

    public void ForceRefresh()
    {
        PerkInitialize(category1);

    }

    private void PerkInitialize(ModifierEffectCategory category)
    {
        string s = Hypatios.RPG.GetModiferValueString_PermaPerk(category);

        statLabel.text = s;
    }

}
