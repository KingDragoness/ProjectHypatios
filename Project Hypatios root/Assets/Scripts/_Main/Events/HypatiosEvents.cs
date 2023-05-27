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


    private void Start()
    {
        OnItemAdd.d_Listeners += PlayerReceiveItem;
    }

    private void OnDisable()
    {
        OnItemAdd.d_Listeners = null;
    }

    public void PlayerReceiveItem(ItemInventory itemClass)
    {
        if (itemClass.isTriggerTrivia == true && itemClass.trivia != null)
        {
            Hypatios.Game.TriviaComplete(itemClass.trivia);
        }
    }
}

