using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Trigger Block 1", menuName = "Hypatios/Trigger Block", order = 1)]
public class TriggerBlock : ScriptableObject
{

    public Conditioner.ConditionEvent triggerCondition;

}



