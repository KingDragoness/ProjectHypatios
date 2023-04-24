using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FW_UI_DebugCP : MonoBehaviour
{

    public FW_ControlPoint controlPoint;
    public Image invaderBar;
    public Image lockImage;
    public Text capturerText;

    private void Update()
    {
        lockImage.gameObject.SetActive(controlPoint.isCaptured);
        invaderBar.fillAmount = controlPoint.captureProgress;

        if (controlPoint.CurrentInvadersInArea > 0 && !controlPoint.isCaptured)
        {
            capturerText.gameObject.SetActive(true);
            capturerText.text = $"{controlPoint.CurrentInvadersInArea}";
        }
        else
        {
            capturerText.gameObject.SetActive(false);
        }
    }
}
