using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;
using Sirenix.OdinInspector;


public enum Alliance
{
    Null = -1,
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
    [BoxGroup("Stats")] public CharacterStat VariableDamage;
    [BoxGroup("Stats")] [Tooltip("Recommended values: 0 - 150 (IQ)")] public CharacterStat Intelligence;
    [BoxGroup("Stats")] public CharacterStat Luck;
    [BoxGroup("Stats")] public CharacterStat MaxHitpoint;
    [BoxGroup("Stats")] public CharacterStat MovementBonus;
    public Alliance MainAlliance = Alliance.Mobius;
    public UnitType UnitType;
    public bool IsDeadObject = false; //For fortification!
    public bool IsDamagableBySameType = false;
    [Tooltip("This is for hackable pseudo enemies. This is only for hackable gates enemy types.")] public bool IsHackableGate = false;
    [Tooltip("Allowed by default. When false, it prevent enemies from attacking the same enemy type, even if the alliance is differ.")] public bool AllowTargetSameType = true;

    [Header("Runtime Only")]
    [ReadOnly] public float CurrentHitpoint;
    [ReadOnly] public bool IsDead = false;


    public void Initialize()
    {
        CurrentHitpoint = MaxHitpoint.BaseValue;
    }

}
