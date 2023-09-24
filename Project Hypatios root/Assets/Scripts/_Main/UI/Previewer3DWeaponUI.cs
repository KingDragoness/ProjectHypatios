using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Previewer3DWeaponUI : MonoBehaviour
{

    public WeaponModelDisplay weaponModel;
    public Material holographicMaterial;

    private void Start()
    {
        if (Time.timeSinceLevelLoad < 1f)
        {
            gameObject.SetActive(false);
        }
    }

    public void DisplayWeapon(WeaponItem weapon)
    {
        weaponModel.currentWeaponDisplay = weapon;
        weaponModel.ActivateWeapon();
        RefreshWeaponModels();
    }

    public void RefreshWeaponModels()
    {
        var meshRenderers = weaponModel.gameObject.GetComponentsInChildren<MeshRenderer>();


        foreach (var meshRender in meshRenderers)
        {
            meshRender.material = holographicMaterial;
            meshRender.gameObject.layer = LayerMask.NameToLayer("Map");
        }
    }

    }
