using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Enemy Killed", menuName = "Hypatios/BaseStatValue", order = 1)]
public class BaseStatValue : ScriptableObject
{

    public string ID = "total_Enemy_Killed";
    public string displayText = "Total Kills";
    public bool overallOnly = false; //For total deaths and total Theratios killed

    public string GetID()
    {
        return ID;
    }


}
