using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScript : MonoBehaviour
{
    private void Start()
    {
        OnTriggerEnterEvent enterEvent = GetComponent<OnTriggerEnterEvent>();
        enterEvent.objectToCompare = FindObjectOfType<CharacterScript>().gameObject;
    }

    public void OpenShop()
    {
        MainUI.Instance.ChangeCurrentMode(1);
    }
}
