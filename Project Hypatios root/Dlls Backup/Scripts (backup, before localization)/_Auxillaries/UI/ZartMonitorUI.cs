using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public class ZartMonitorUI : MonoBehaviour
{

    public Chamber_Level6 chamberScript;
    public Zart_CustomerListButton prefab;
    public Transform parentListCustomer;

    private List<Zart_CustomerListButton> allButtons = new List<Zart_CustomerListButton>();
    
    public void RefreshMonitor()
    {
        foreach (var button in allButtons) { Destroy(button.gameObject); }
        allButtons.Clear();

        foreach(var customer in chamberScript.allCustomers)
        {
            var button = Instantiate(prefab, parentListCustomer);
            button.gameObject.SetActive(true);
            button.transform.SetParent(parentListCustomer);
            button.transform.localScale = Vector3.one;
            button.transform.localPosition = Vector3.zero;
            allButtons.Add(button);
            button.RefreshButton(customer);
        }
    }

}
