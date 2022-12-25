using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScript : MonoBehaviour
{

    public Transform movePlayerHere; //jesus..

    private void Start()
    {
        OnTriggerEnterEvent enterEvent = GetComponent<OnTriggerEnterEvent>();
        enterEvent.objectToCompare = FindObjectOfType<CharacterScript>().gameObject;
    }

    public void OpenShop()
    {
        Hypatios.Player.transform.position = movePlayerHere.transform.position;
        MainUI.Instance.ChangeCurrentMode(1);
    }
}
