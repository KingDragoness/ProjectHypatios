using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Interact_MysteriousMerchant_Cardbox : MonoBehaviour
{

    public Interact_MysteriousMerchant merchant;
    public Interact_Touchable touchable;
    public ItemInventory item;
    public TextMesh labelPrice;
    public int count = 20;

    public void Refresh()
    {
        labelPrice.text = $"{item.value}";
        touchable.interactDescription = $"{item.GetDisplayText()} [{item.value} souls]";

        if (count <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void BuyItem()
    {
        merchant.Buy(this);
    }
}
