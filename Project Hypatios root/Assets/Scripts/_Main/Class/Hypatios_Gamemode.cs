using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Aldrich", menuName = "Hypatios/Gamemode", order = 1)]
public class Hypatios_Gamemode : ScriptableObject
{
  
    public enum MainCharacter
    {
        Blank,
        Aldrich,
        Elena,
        Customizeable
    }

    public MainCharacter character;
    public bool isGauntlet = false;
    public bool randomizedSeed = false;
    public bool allowTipsAndHints = true;
    public bool canSaveGame = false; //for tutorial and gauntlet

}
