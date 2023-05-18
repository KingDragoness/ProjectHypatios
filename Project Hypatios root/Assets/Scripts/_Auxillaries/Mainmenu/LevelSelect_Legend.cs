using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.Utility;
using Sirenix.OdinInspector;

public class LevelSelect_Legend : MonoBehaviour
{

    public AutoMoveAndRotate autoRotateScript;
    public AnimatorSetBool animatorBool;
    public UnityEvent OnButtonClicked;

    private ButtonBaseUI buttonBase;

    private void Start()
    {
        buttonBase = gameObject.AddComponent<ButtonBaseUI>();
    }

    public void OnHovering()
    {
        if (autoRotateScript.enabled == false)
        {
            autoRotateScript.enabled = true;
            buttonBase.Sound_Hover();
        }

        animatorBool.SetBool(true);
    }

    public void StopHovering()
    {
        autoRotateScript.enabled = false;
        animatorBool.SetBool(false);

    }

    public void Click()
    {
        OnButtonClicked?.Invoke();
        buttonBase.Sound_Click();
    }
}
