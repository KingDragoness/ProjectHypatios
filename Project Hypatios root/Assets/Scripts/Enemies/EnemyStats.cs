using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;
using Sirenix.OdinInspector;


public enum Alliance
{
    Rogue = 0,
    Mobius = 10,
    Player = 100
}

public enum UnitType
{
    Biological = 0,
    Mechanical = 1,
    Boss = 2,
    Projectile = 10
}

[System.Serializable]
public class EnemyStats
{

    [BoxGroup("Stats")] public CharacterStat BaseDamage;
    [BoxGroup("Stats")] public CharacterStat Intelligence;
    [BoxGroup("Stats")] public CharacterStat Luck;
    [BoxGroup("Stats")] public CharacterStat MaxHitpoint;
    public Alliance MainAlliance;
    public UnitType UnitType;
    public bool IsDamagableBySameType = false;

 


}
