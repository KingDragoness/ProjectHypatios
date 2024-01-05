﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public class MobiusOS_DesktopApp : MonoBehaviour
{

    public MobiusNetUI mobiusUI;
    public MobiusApp_SO mobiusSO;

    public void OpenMobiusApp()
    {
        mobiusUI.OpenWindowApp(mobiusSO);
    }

}
