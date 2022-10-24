using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class Chamber6_MonitorFood : MonoBehaviour
{

    public Chamber_Level6 chamberScript;
    public Chamber6_Customer currentCustomer;
    public TextMesh label_Order;
    public TextMesh label_Ingredients;

    private void Start()
    {
        label_Order.text = "No Order";
        label_Ingredients.text = "-";
    }

    [ContextMenu("Refresh Monitor")]
    public void RefreshMonitor()
    {
        string recipes = "";

        if (currentCustomer != null)
        {
            foreach (var recipe in currentCustomer.order.allRecipes)
            {
                recipes += recipe.ToString() + "\n";
            }

            label_Order.text = $"Order #{(currentCustomer.tableSeat + 1).ToString("00")}";
            label_Ingredients.text = recipes.ToString();
        }
        else
        {
            label_Order.text = "No Order";
            label_Ingredients.text = "-";
        }
    }

    public void CheckOrderMonitor()
    {
        int index = chamberScript.allCustomers.IndexOf(currentCustomer);

        if (chamberScript.allCustomers.Count == 0)
        {
            label_Order.text = "No Order";
            label_Ingredients.text = "-"; return;
        }

        var customer = chamberScript.allCustomers.NextOf(currentCustomer);
        currentCustomer = customer;
        RefreshMonitor();
    }

    public void RefreshMonitorIfMissing()
    {
        if (currentCustomer == null | chamberScript.allCustomers.Contains(currentCustomer) == false)
        {
            CheckOrderMonitor();
        }
    }

}
