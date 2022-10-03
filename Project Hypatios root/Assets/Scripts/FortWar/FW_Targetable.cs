using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FW_Alliance
{
    INVADER,
    DEFENDER
}


public class FW_Targetable : MonoBehaviour
{

    public enum Type
    {
        Bot,
        Sentry,
        Player
    }

    [SerializeField]
    private FW_Alliance alliance;

    [SerializeReference]
    private Type _type;

    public FW_Alliance Alliance { get => alliance;}
    public Type UnitType { get => _type; }

    public FW_Alliance AllianceEnemy()
    {
        if (alliance == FW_Alliance.DEFENDER)
            return FW_Alliance.INVADER;
        else
            return FW_Alliance.DEFENDER;
    }

}
