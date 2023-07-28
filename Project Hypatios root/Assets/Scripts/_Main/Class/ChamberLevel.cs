using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "Chamber_1", menuName = "Hypatios/Chamber", order = 1)]
public class ChamberLevel : ScriptableObject
{

    public string levelName = "Chamber 1";
    public bool isCausingFatigue = true;
    public bool isWIRED = false;
    public bool showTitleCard = false;
    [FoldoutGroup("Title Card")] public string TitleCard_Title = "Math Lab";
    [FoldoutGroup("Title Card")] public string TitleCard_Subtitle = "Chamber 5";

    public string GetID()
    {
        return name;
    }

}
