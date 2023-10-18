using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingkThanidLabTrigger : MonoBehaviour
{

    public Transform movePlayerHere;
    public GameObject previewNormalView;
    private bool isWorkbenchOpened = false;

    public void OpenShop()
    {
        var kthanidUI = MainGameHUDScript.Instance.kthanidUI;
        isWorkbenchOpened = true;
        Hypatios.Player.transform.position = movePlayerHere.transform.position;
        kthanidUI.SetShopScript(this);
        MainUI.Instance.ChangeCurrentMode(MainUI.UIMode.kThanidLab);
    }

    private void Update()
    {
        if (isWorkbenchOpened)
        {
            previewNormalView.gameObject.SetActive(true);

            if (Hypatios.UI.CurrentUI != MainUI.UIMode.kThanidLab)
            {
                CloseShop();
            }
        }
    }

    public void CloseShop()
    {
        isWorkbenchOpened = false;
        previewNormalView.gameObject.SetActive(false);
      
    }
}
