using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class SentryGunUI : MonoBehaviour
{
    [FoldoutGroup("Sentry")] public Slider UI_slider_HP;
    [FoldoutGroup("Sentry")] public Slider UI_slider_Ammo;


    public static SentryGunUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Fortification_GhostSentry.Instance == null)
        {
            DisableSentryUI();
            return;
        }

        if (Time.timeScale <= 0)
            return;

        UpdateUI();

    }

    private void UpdateUI()
    {
        UI_slider_HP.value = Fortification_GhostSentry.Instance.Stats.CurrentHitpoint;
        UI_slider_HP.maxValue = Fortification_GhostSentry.Instance.Stats.MaxHitpoint.Value;
        UI_slider_Ammo.value = Fortification_GhostSentry.Instance.sentryAmmo;
        UI_slider_Ammo.maxValue = Fortification_GhostSentry.Instance.maxSentryAmmo;
    }


    private void DisableSentryUI()
    {
        gameObject.SetActive(false);
    }

    public void ActivateUI()
    {
        gameObject.SetActive(true);
    }
}
