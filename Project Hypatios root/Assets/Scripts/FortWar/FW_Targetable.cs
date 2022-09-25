using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FW_Targetable : MonoBehaviour
{
    [SerializeField]
    private FW_Alliance alliance;

    public FW_Alliance Alliance { get => alliance;}

    public FW_Alliance AllianceEnemy()
    {
        if (alliance == FW_Alliance.DEFENDER)
            return FW_Alliance.INVADER;
        else
            return FW_Alliance.DEFENDER;
    }

}
