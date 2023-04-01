using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBaseUI : MonoBehaviour
{

    private Animator _button;

    public void Sound_Hover()
    {
        soundManagerScript.instance.Play("ui.hover");
    }

    public void Sound_Click()
    {
        soundManagerScript.instance.Play("ui.click");
    }

    public void Button_Anim(string triggerName)
    {
        if (_button == null) _button = GetComponent<Animator>();
        _button.SetTrigger(triggerName);
    }


}
