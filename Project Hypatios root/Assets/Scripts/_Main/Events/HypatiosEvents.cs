using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Maintaining event systems.
/// </summary>
public class HypatiosEvents : MonoBehaviour
{

    public gEvent_OnItemAdd OnItemAdd;
    public gEvent_OnStatusAilment OnStatusAilment;


    private void Start()
    {
        OnItemAdd.d_Listeners += PlayerReceiveItem;
    }

    private void OnDisable()
    {
        OnItemAdd.d_Listeners = null;
        if (OnStatusAilment.d_Listeners != null)
            OnStatusAilment.d_Listeners = null;
    }

    public void PlayerReceiveItem(ItemInventory itemClass)
    {
        if (itemClass.isTriggerTrivia == true && itemClass.trivia != null)
        {
            Hypatios.Game.TriviaComplete(itemClass.trivia);
        }
    }

    public void InvokeStatusEvent(BaseStatusEffectObject ailment)
    {
        OnStatusAilment?.d_Listeners.Invoke(ailment);
    }
}

