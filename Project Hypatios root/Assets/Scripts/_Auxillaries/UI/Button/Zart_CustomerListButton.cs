using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Zart_CustomerListButton : MonoBehaviour
{

    public Chamber_Level6 chamberScript;
    public Text label_Order;
    public GameObject servoIcon;
   
    public void RefreshButton(Chamber6_Customer currentCustomer)
    {
        string order = "[";
        int i = 0;
        foreach (var recipe in currentCustomer.order.allRecipes)
        {
            order += recipe.ToString();
            if (i < currentCustomer.order.allRecipes.Count - 1) order += ", ";
            i++;
        }
        order += "]";
        label_Order.text = order;

        bool isServedByBot = chamberScript.IsCustomerBeingServoed(currentCustomer);

        if (isServedByBot)
            servoIcon.gameObject.SetActive(true);
        else
            servoIcon.gameObject.SetActive(false);
    }

}
