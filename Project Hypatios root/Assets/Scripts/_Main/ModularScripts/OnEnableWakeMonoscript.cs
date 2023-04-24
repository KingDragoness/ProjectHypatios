using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableWakeMonoscript : MonoBehaviour
{

    public List<MonoBehaviour> scriptsToEnable;

    private void Start()
    {
        foreach(var script in scriptsToEnable)
        {
            script.enabled = true;
        }
    }

}
