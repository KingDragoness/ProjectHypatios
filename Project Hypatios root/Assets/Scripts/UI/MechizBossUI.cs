using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MechizBossUI : MonoBehaviour
{
    public Slider hitpointSlider;

    private MechizMonsterRobot mechizRobot;

    private void Awake()
    {
        mechizRobot = FindObjectOfType<MechizMonsterRobot>();
    }

    private void Update()
    {
        hitpointSlider.value = mechizRobot.Stats.CurrentHitpoint;
        hitpointSlider.maxValue = mechizRobot.Stats.MaxHitpoint.Value;
    }
}
