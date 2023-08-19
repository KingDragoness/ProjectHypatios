using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_CasinoRoulette_Touchable : MonoBehaviour
{

    public int ID = 0;
    public Interact_Touchable touchable;
    public Interact_Casino_Roulette rouletteScript;

    public void AddChip()
    {
        rouletteScript.AddChip(this);
    }

}
