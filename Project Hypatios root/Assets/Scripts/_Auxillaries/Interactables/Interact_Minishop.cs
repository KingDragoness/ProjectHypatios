using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Interact_Minishop : MonoBehaviour
{

    public List<Interact_Minishop_Buyable> itemInCart = new List<Interact_Minishop_Buyable>();
    public Interact_VendingPayAccess vendingPay;
    public Transform cashierTransform;
    public TextMesh label_RegisterPrice;

    [ReadOnly] [ShowInInspector] private List<Interact_Minishop_Buyable> allBuyables = new List<Interact_Minishop_Buyable>();
    private int _totalSoulPrice = 0;

    private void Start()
    {
        ScanItems();
        RefreshUI();
    }

    [Button("Scan items")]
    private void ScanItems()
    {
        allBuyables = GetComponentsInChildren<Interact_Minishop_Buyable>().ToList();
    }

    public void AddToCart(Interact_Minishop_Buyable buyableItem)
    {
        itemInCart.Add(buyableItem);
        Vector3 offset = Vector3.zero;
        offset.x += Random.Range(-0.7f, 0.7f);
        offset.z += Random.Range(-0.7f, 0.7f);
        buyableItem.transform.position = cashierTransform.position + offset;
        CalculatePrices();
        RefreshUI();
    }

    public void ReturnItem(Interact_Minishop_Buyable buyableItem)
    {
        itemInCart.Remove(buyableItem);
        CalculatePrices();
        RefreshUI();
    }

    public void SuccessfulPayment()
    {
        foreach(var item in itemInCart)
        {
            item.addItemScript.ExecuteFunction();
            item.gameObject.SetActive(false);
        }
        itemInCart.Clear();
        CalculatePrices();
        RefreshUI();
    }

    private void RefreshUI()
    {
        label_RegisterPrice.text = $"TOTAL: {_totalSoulPrice} souls";
        vendingPay.soulCost = _totalSoulPrice;
    }

    private void CalculatePrices()
    {
        int price = 0;
        foreach(var item in itemInCart)
        {
            price += item.priceList;
        }

        _totalSoulPrice = price;
    }
}
