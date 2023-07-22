using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_MobiusNetworkPC : MonoBehaviour
{

    public Transform movePlayerHere;
    public GameObject previewNormalView;
    private bool isPCOpened = false;

    public void OpenShop()
    {
        var internetUI = Hypatios.UI.Internet;
        isPCOpened = true;
        Hypatios.Player.transform.position = movePlayerHere.transform.position;
        internetUI.SetShopScript(this);
        MainUI.Instance.ChangeCurrentMode(MainUI.UIMode.MobiusNet);
    }

    private void Update()
    {
        if (isPCOpened)
        {
            previewNormalView.gameObject.SetActive(true);

            if (Hypatios.UI.current_UI != MainUI.UIMode.MobiusNet)
            {
                CloseShop();
            }
        }
    }

    public void CloseShop()
    {
        isPCOpened = false;
        previewNormalView.gameObject.SetActive(false);

    }
}
