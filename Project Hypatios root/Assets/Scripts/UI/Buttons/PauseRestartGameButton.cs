using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Sirenix.OdinInspector;
using UnityEngine.UI.Extensions;

public class PauseRestartGameButton : MonoBehaviour
{
    [FoldoutGroup("References")] public Button restartButton;
    [FoldoutGroup("References")] public TooltipTrigger tooltipRestart;

    private void OnEnable()
    {
        if (Hypatios.Player.isGrounded)
        {
            restartButton.interactable = true;
            tooltipRestart.enabled = false;
        }
        else
        {
            restartButton.interactable = false;
            tooltipRestart.enabled = true;
        }
    }

    public void HighlightButton()
    {
        Hypatios.UI.ShowTooltipSmall(this.GetComponent<RectTransform>(), tooltipRestart.text, "");
    }
}
