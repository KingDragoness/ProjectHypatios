using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_CasinoRoulette_WagerToken : MonoBehaviour
{

    public int ID = 0;
    public Interact_Casino_Roulette rouletteScript;

    public void TakeChip()
    {
        rouletteScript.RemoveChip(this);

    }

}
