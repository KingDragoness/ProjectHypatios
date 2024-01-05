using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MobiusApp_IconDashboard : MonoBehaviour
{

    public MobiusNetUI mobiusUI;
    public Image appIcon;
    public Image i_Minimized;
    public MobiusApp openedMobiusApp; //attached
    
    public void Refresh()
    {
        appIcon.sprite = openedMobiusApp.connectedApp.appLogo;
    }

    public void OpenWindow()
    {
        openedMobiusApp.OpenWindow();
    }
}
