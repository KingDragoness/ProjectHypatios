using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Sirenix.OdinInspector;
using UnityEngine.UI.Extensions;

public class PauseRestartGameButton : MonoBehaviour
{
    public enum PauseMenuType
    {
        None,
        Trivias,
        Inventory,
        Status,
        Statistics,
        Settings
    }

    [FoldoutGroup("References")] public Button restartButton;
    [FoldoutGroup("References")] public TooltipTrigger tooltipRestart;
    [FoldoutGroup("References")] public GameObject menu_Inventory;
    [FoldoutGroup("References")] public GameObject menu_Status;
    [FoldoutGroup("References")] public GameObject menu_Statistics;
    [FoldoutGroup("References")] public GameObject menu_Settings;

    public PauseMenuType currentPauseMenu;

    private static PauseMenuType _cachedPauseMenu; //for persistence

    private void OnEnable()
    {
        currentPauseMenu = PauseMenuType.None; //no longer cached

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

        RefreshMenus();
    }

    public void SetMenuType(int category)
    {
        currentPauseMenu = (PauseMenuType)category;
        _cachedPauseMenu = currentPauseMenu;
        RefreshMenus();
    }

    private void RefreshMenus()
    {
        if (currentPauseMenu != PauseMenuType.Inventory)
        {
            menu_Inventory.gameObject.SetActive(false);
        }
        else
        {
            menu_Inventory.gameObject.SetActive(true);
        }

        if (currentPauseMenu != PauseMenuType.Status)
        {
            menu_Status.gameObject.SetActive(false);
        }
        else
        {
            menu_Status.gameObject.SetActive(true);
        }

        if (currentPauseMenu != PauseMenuType.Settings)
        {
            menu_Settings.gameObject.SetActive(false);
        }
        else
        {
            menu_Settings.gameObject.SetActive(true);
        }

        if (currentPauseMenu != PauseMenuType.Statistics)
        {
            menu_Statistics.gameObject.SetActive(false);
        }
        else
        {
            menu_Statistics.gameObject.SetActive(true);
        }
    }

    public void HighlightButton()
    {
        Hypatios.UI.ShowTooltipSmall(this.GetComponent<RectTransform>(), tooltipRestart.text, "");
    }
}
