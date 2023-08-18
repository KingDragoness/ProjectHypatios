using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Interact_Casino_WagerToken : MonoBehaviour
{

    public int ID = 0;
    public Interact_Generic_Casino casinoWagerScript;
    public Interact_Touchable touchable;

    public void OverrideWagerToken()
    {
        var stat = casinoWagerScript.GetChipStat(ID);
        touchable.interactDescription = $"{stat.soul} souls";
    }

    public void TakeChip()
    {
        casinoWagerScript.TakeChip(this);
    }

}
