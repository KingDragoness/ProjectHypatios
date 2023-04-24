using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMod_ScatterBullet : MonoBehaviour
{

    public GunScript gunScript;
    [Range(1,20)] public int amount = 3;
    public float multiplierSpread = 10;

    private void Awake()
    {
        if (gunScript == null) return;
        gunScript.OnFireAction += FireScatter;
    }

    public void FireScatter(string param)
    {
        if (gameObject.activeInHierarchy)
            gunScript.FireAdditionalScatterBullets(multiplierSpread, amount);
    }

}
