using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "OnStatusAilment", menuName = "Hypatios/GameEvents/OnStatusAilment", order = 1)]
public class gEvent_OnStatusAilment : ScriptableObject
{

    //manual event register
    public System.Action<BaseStatusEffectObject> d_Listeners;


    [Button("Raise")]
    public void Raise(BaseStatusEffectObject ailment)
    {
        d_Listeners?.Invoke(ailment);
    }

}
