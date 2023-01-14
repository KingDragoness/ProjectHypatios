using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingWorkstationTrigger : MonoBehaviour
{
    public enum CameraMode
    {
        Idle,
        WeaponMod,
        Weapon
    }

    public Transform movePlayerHere;
    public CameraMode currentMode;
    public Material defaultMaterial;
    public GameObject previewNormalView;
    public GameObject previewWeaponModView;
    public GameObject previewWeaponView;
    public WeaponModelDisplay displayWeapon_WeaponMod;
    public WeaponModelDisplay displayWeapon_Weapon;

    private bool isWorkbenchOpened = false;



    public void OpenShop()
    {
        var craftingUI = MainGameHUDScript.Instance.craftingUI;
        isWorkbenchOpened = true;
        Hypatios.Player.transform.position = movePlayerHere.transform.position;
        craftingUI.SetShopScript(this);
        MainUI.Instance.ChangeCurrentMode(3);
    }

    public void RefreshWeaponModels()
    {
        var craftingUI = MainGameHUDScript.Instance.craftingUI;

        displayWeapon_WeaponMod.ActivateWeapon();
        displayWeapon_Weapon.ActivateWeapon();

        var meshRenderers = displayWeapon_Weapon.gameObject.GetComponentsInChildren<MeshRenderer>();
        var attachments = displayWeapon_WeaponMod.gameObject.GetComponentsInChildren<WeaponAttachmentVisuals>();

        foreach (var meshRender in meshRenderers)
        {
            meshRender.material = defaultMaterial;
        }

        var currentWeapon = craftingUI.GetCurrentWeaponOnTable();
        if (currentWeapon != null)
        {
            var weaponSave = Hypatios.Game.GetWeaponSave(currentWeapon.weaponName);
            var weapon1 = Hypatios.Assets.GetWeapon(weaponSave.weaponID);
            int order = 0;

            foreach (var attach in attachments)
            {
                Transform[] allTs = attach.GetComponentsInChildren<Transform>(true);

                foreach (var t in allTs)
                    t.gameObject.layer = 0;

                var att11 = weapon1.GetAttachmentWeaponMod(attach.ID); //i dont know what to name this variable
                if (att11 != null)
                {
                    if (order > att11.order)
                        continue;
                }

                if (weaponSave.AttachmentExists(attach.ID))
                {
                    attach.RefreshVisuals(attach.ID);
                    attach.TriggerRequirements(true);

                }
                else
                {
                    attach.RefreshVisuals("");
                    attach.TriggerRequirements(false);

                }
            }
        }
    }

    private void Update()
    {
        if (isWorkbenchOpened)
        {
            if (currentMode == CameraMode.Idle)
            {
                previewNormalView.gameObject.SetActive(true);
                previewWeaponModView.gameObject.SetActive(false);
                previewWeaponView.gameObject.SetActive(false);
                displayWeapon_WeaponMod.gameObject.SetActive(false);
                displayWeapon_Weapon.gameObject.SetActive(false);

            }
            else if (currentMode == CameraMode.WeaponMod)
            {
                previewNormalView.gameObject.SetActive(false);
                previewWeaponModView.gameObject.SetActive(true);
                previewWeaponView.gameObject.SetActive(false);
                displayWeapon_WeaponMod.gameObject.SetActive(true);
                displayWeapon_Weapon.gameObject.SetActive(false);

            }
            else if (currentMode == CameraMode.Weapon)
            {
                previewNormalView.gameObject.SetActive(false);
                previewWeaponModView.gameObject.SetActive(false);
                previewWeaponView.gameObject.SetActive(true);
                displayWeapon_WeaponMod.gameObject.SetActive(false);
                displayWeapon_Weapon.gameObject.SetActive(true);

            }


            if (Hypatios.UI.current_UI != MainUI.UIMode.Crafting)
            {
                CloseShop();
            }
        }
    }


    public void CloseShop()
    {
        currentMode = CameraMode.Idle;
        isWorkbenchOpened = false;
        previewNormalView.gameObject.SetActive(false);
        previewWeaponModView.gameObject.SetActive(false);
        previewWeaponView.gameObject.SetActive(false);
        displayWeapon_WeaponMod.gameObject.SetActive(false);
        displayWeapon_Weapon.gameObject.SetActive(false);
    }
}
