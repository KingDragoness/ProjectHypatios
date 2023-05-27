using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "OnItemAdded", menuName = "Hypatios/GameEvents/OnItemAdd", order = 1)]
public class gEvent_OnItemAdd : ScriptableObject
{

    //manual event register
    public System.Action<ItemInventory> d_Listeners;


    [Button("Raise")]
    public void Raise(ItemInventory itemClass)
    {
        d_Listeners?.Invoke(itemClass);
    }

}
