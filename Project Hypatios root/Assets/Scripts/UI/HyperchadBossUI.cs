using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HyperchadBossUI : MonoBehaviour
{

    public Slider hitpointSlider;

    private HyperchadEnemy hyperchadEnemy;

    private void Awake()
    {
        hyperchadEnemy = FindObjectOfType<HyperchadEnemy>();
    }

    private void Update()
    {
        hitpointSlider.value = hyperchadEnemy.Stats.CurrentHitpoint;
        hitpointSlider.maxValue = hyperchadEnemy.Stats.MaxHitpoint.Value;
    }

}
