using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FW_Player : MonoBehaviour
{
    private Chamber_Level7 _chamberScript;

    public FW_Targetable myUnit;

    public virtual void Awake()
    {
        _chamberScript = Chamber_Level7.instance;
    }


    private void Start()
    {
        _chamberScript.RegisterUnit(myUnit);
    }

}
