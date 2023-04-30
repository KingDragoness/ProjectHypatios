using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Interact_Minishop_Buyable : MonoBehaviour
{

    public Interact_Minishop shopScript;
    public Interact_Touchable touchable;
    public PlayerAddItemScript addItemScript;
    public int priceList = 5;

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position;
        Mode_Buy();


    }

    private void Mode_Buy()
    {
        touchable.interactDescription = $"{addItemScript.item.GetDisplayText()} ({priceList} souls)";
        touchable.OnInteractEvent.RemoveAllListeners();
        touchable.OnInteractEvent.AddListener(AddToCart);
    }

    private void Mode_Cancel()
    {
        touchable.interactDescription = $"Put item back";
        touchable.OnInteractEvent.RemoveAllListeners();
        touchable.OnInteractEvent.AddListener(ResetState);
    }

    public void ResetState()
    {
        transform.position = originalPosition;
        Mode_Buy();
        shopScript.ReturnItem(this);
    }

    public void AddToCart()
    {
        shopScript.AddToCart(this);
        Mode_Cancel();
    }
   
}
