using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBaseUI : MonoBehaviour
{
    
    public void Sound_Hover()
    {
        soundManagerScript.instance.Play("ui.hover");
    }

    public void Sound_Click()
    {
        soundManagerScript.instance.Play("ui.click");
    }

}
