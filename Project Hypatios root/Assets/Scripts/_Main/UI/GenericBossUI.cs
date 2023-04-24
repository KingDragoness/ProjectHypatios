using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericBossUI : MonoBehaviour
{

    public EnemyScript currentEnemy;
    public CanvasGroup canvasGroup;
    public Slider sliderHitpoint;
    public Text labelHitpoint;
    public Text labelBossName;

    private void Start()
    {
        canvasGroup.alpha = 0;
    }

    void Update()
    {
        bool shouldHide = false;
        if (currentEnemy == null) shouldHide = true;
        else if (currentEnemy.Stats == null) shouldHide = true;

        if (shouldHide)
        {
            HideUI();
            return;
        }
        else
        {
            EnableUI();
        }

        float valueHitpoint = Mathf.Floor(currentEnemy.Stats.CurrentHitpoint);
        if (valueHitpoint <= 0f) valueHitpoint = 0f;
        labelHitpoint.text = $"{valueHitpoint}";
        sliderHitpoint.value = currentEnemy.Stats.CurrentHitpoint;
        sliderHitpoint.maxValue = currentEnemy.Stats.MaxHitpoint.Value;
        labelBossName.text = $"{currentEnemy.EnemyName}";
    }

    private void HideUI()
    {
        labelHitpoint.text = "0";
        if (canvasGroup.alpha >= 0) canvasGroup.alpha -= Time.deltaTime * 1f;

    }

    private void EnableUI()
    {
        if (canvasGroup.alpha < 1) canvasGroup.alpha += Time.deltaTime *1f;
    }
}
